using UnityEngine;
using UnityEngine.UI;
// Si usas TextMeshPro, descomenta la siguiente línea y cambia Text por TMP_Text:
// using TMPro;

public class DoorController : MonoBehaviour
{
    [Header("Configuración de Apertura")]
    public float openAngle = 90f;

    [Tooltip("Define qué eje rota. Por defecto es Y (0,1,0). Si rota mal, prueba cambiando los valores a 1 en X o Z.")]
    public Vector3 rotationAxis = new Vector3(0, 1, 0);

    public float smooth = 2f;

    [Header("Configuración de Wwise (Audio)")]
    public AK.Wwise.Event eventoAbrirPuerta;
    public AK.Wwise.Event eventoCerrarPuerta;

    [Header("UI de Interacción")]
    [Tooltip("Panel que aparece al acercarse a la puerta")]
    public GameObject panelInteractuar;
    [Tooltip("Texto opcional dentro del panel para asegurar el mensaje")]
    public Text textoInteractuar; // O usa TMP_Text si trabajas con TextMeshPro
    public string mensajeInteractuar = "Presiona E para interactuar";

    private Quaternion defaultRot;
    private Quaternion targetRot;
    private bool jugadorEnZona = false;
    private bool estaAbierta = false;

    void Awake()
    {
        if (panelInteractuar != null)
        {
            panelInteractuar.SetActive(false);
        }
    }

    void Start()
    {
        defaultRot = transform.localRotation;
        targetRot = defaultRot;
    }

    void Update()
    {
        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRot, Time.deltaTime * smooth);

        if (!jugadorEnZona) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!estaAbierta)
            {
                OpenDoor();
            }
            else
            {
                CloseDoor();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        jugadorEnZona = true;

        if (textoInteractuar != null)
        {
            textoInteractuar.text = mensajeInteractuar;
        }

        if (panelInteractuar != null)
        {
            panelInteractuar.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        jugadorEnZona = false;

        if (panelInteractuar != null)
        {
            panelInteractuar.SetActive(false);
        }
    }

    private void OpenDoor()
    {
        targetRot = defaultRot * Quaternion.Euler(rotationAxis * openAngle);

        if (eventoAbrirPuerta != null && eventoAbrirPuerta.IsValid())
        {
            eventoAbrirPuerta.Post(gameObject);
        }

        estaAbierta = true;
    }

    private void CloseDoor()
    {
        targetRot = defaultRot;

        if (eventoCerrarPuerta != null && eventoCerrarPuerta.IsValid())
        {
            eventoCerrarPuerta.Post(gameObject);
        }

        estaAbierta = false;
    }
}