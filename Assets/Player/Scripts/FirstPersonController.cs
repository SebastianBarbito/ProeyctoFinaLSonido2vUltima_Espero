using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class FirstPersonController : MonoBehaviour
{
    [Header("Movimiento")]
    public float moveSpeed = 5.0f;
    public float gravity = -9.81f;

    [Header("Cámara y Mouse")]
    public Transform playerCamera;
    public float mouseSensitivity = 2.0f;
    public float maxLookAngle = 80.0f;

    [Header("Configuración de Wwise (Pasos)")]
    public AK.Wwise.Event eventoPasos;
    [Tooltip("Distancia que debe recorrer el jugador para que suene el siguiente paso")]
    public float distanciaEntrePasos = 1.8f;

    private CharacterController controller;
    private Vector3 velocity;
    private float cameraPitch = 0.0f;

    // Variables para el control de distancia de los pasos
    private float distanciaRecorrida = 0f;
    private Vector3 ultimaPosicion;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        if (playerCamera == null)
        {
            if (Camera.main != null)
            {
                playerCamera = Camera.main.transform;
            }
        }

        if (playerCamera != null)
        {
            playerCamera.localPosition = new Vector3(0, 0.6f, 0);
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        ultimaPosicion = transform.position;
    }

    void Update()
    {
        HandleMouseLook();
        HandleMovement();
        GestionarPasos();
    }

    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        cameraPitch -= mouseY;
        cameraPitch = Mathf.Clamp(cameraPitch, -maxLookAngle, maxLookAngle);

        if (playerCamera != null)
        {
            playerCamera.localEulerAngles = Vector3.right * cameraPitch;
        }

        transform.Rotate(Vector3.up * mouseX);
    }

    void HandleMovement()
    {
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        controller.Move(move * moveSpeed * Time.deltaTime);

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void GestionarPasos()
    {
        // Verificamos si el jugador está en el suelo y presionando alguna tecla de movimiento (WASD)
        bool estaEnSuelo = controller.isGrounded;
        bool estaMoviendoTeclas = Mathf.Abs(Input.GetAxis("Horizontal")) > 0.1f || Mathf.Abs(Input.GetAxis("Vertical")) > 0.1f;

        if (estaEnSuelo && estaMoviendoTeclas)
        {
            // Calculamos la distancia recorrida en el plano horizontal (X, Z) desde el último frame
            Vector3 posicionActual = transform.position;
            float distanciaFrame = Vector3.Distance(new Vector3(posicionActual.x, 0, posicionActual.z), new Vector3(ultimaPosicion.x, 0, ultimaPosicion.z));

            distanciaRecorrida += distanciaFrame;

            // Si acumulamos la distancia necesaria, disparamos el paso
            if (distanciaRecorrida >= distanciaEntrePasos)
            {
                ReproducirPaso();
                distanciaRecorrida = 0f; // Reiniciamos el acumulador
            }
        }
        else
        {
            // Si se detiene o salta, puedes reiniciar la distancia para que al volver a caminar suene inmediato
            distanciaRecorrida = 0f;
        }

        ultimaPosicion = transform.position;
    }

    void ReproducirPaso()
    {
        if (eventoPasos != null && eventoPasos.IsValid())
        {
            eventoPasos.Post(gameObject);
        }
    }
}