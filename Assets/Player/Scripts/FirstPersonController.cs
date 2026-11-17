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
    public float maxLookAngle = 80.0f; // Límite para no doblar el cuello por completo hacia atrás

    private CharacterController controller;
    private Vector3 velocity;
    private float cameraPitch = 0.0f;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        // Si no asignaste la cámara desde el inspector, busca la cámara principal automáticamente
        if (playerCamera == null)
        {
            if (Camera.main != null)
            {
                playerCamera = Camera.main.transform;
            }
        }

        // Posiciona la cámara correctamente a la altura de los ojos (cerca de la cabeza)
        if (playerCamera != null)
        {
            playerCamera.localPosition = new Vector3(0, 0.6f, 0);
        }

        // Bloquea el cursor en el centro de la pantalla y lo oculta
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        HandleMouseLook();
        HandleMovement();
    }

    void HandleMouseLook()
    {
        // Obtiene el movimiento del mouse
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Rota la cámara verticalmente y la limita
        cameraPitch -= mouseY;
        cameraPitch = Mathf.Clamp(cameraPitch, -maxLookAngle, maxLookAngle);

        if (playerCamera != null)
        {
            playerCamera.localEulerAngles = Vector3.right * cameraPitch;
        }

        // Rota el cuerpo del personaje horizontalmente
        transform.Rotate(Vector3.up * mouseX);
    }

    void HandleMovement()
    {
        // Comprueba si está en el suelo para resetear la gravedad leve acumulada
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // Captura las entradas de movimiento (WASD / Flechas)
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        // Calcula la dirección del movimiento en base a hacia dónde mira el personaje
        Vector3 move = transform.right * moveX + transform.forward * moveZ;

        // Mueve al personaje
        controller.Move(move * moveSpeed * Time.deltaTime);

        // Aplica la gravedad constante
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}