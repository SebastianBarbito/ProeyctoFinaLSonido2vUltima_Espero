using System.Collections;
using UnityEngine;
using UnityEngine.UI;
// Si usas TextMeshPro, descomenta la siguiente línea y cambia Text por TMP_Text:
// using TMPro;

public class SalidaEscape : MonoBehaviour
{
    [Header("Eventos de Wwise (Opcional)")]
    public AK.Wwise.Event eventoEscapeExitoso;
    public AK.Wwise.Event eventoPuertaBloqueada;

    [Header("Diálogos del Jugador (ControladorVozPlayer)")]
    [Tooltip("ID del diálogo cuando presiona E sin tener la llave misteriosa")]
    public string idDialogoSinLlave = "puerta_sin_llave";

    [Tooltip("ID del diálogo cuando escapa exitosamente con la llave")]
    public string idDialogoEscape = "escape";

    [Header("UI de Interacción y Finalización")]
    [Tooltip("Aviso al acercarse (ej. 'Presiona E para escapar')")]
    public GameObject panelInteractuar;
    [Tooltip("Aviso si intentas escapar sin la llave misteriosa")]
    public GameObject panelSinLlave;
    [Tooltip("Panel final con el mensaje de victoria")]
    public GameObject panelVictoria;
    public Text textoVictoria; // O usa TMP_Text si trabajas con TextMeshPro
    public string mensajeVictoria = "Felicidades lograste escapar";
    public float segundosAntesDeSalir = 3.0f;

    private bool jugadorEnZona = false;
    private bool escapeActivado = false;
    private GameObject ultimoPlayer;

    void Awake()
    {
        if (panelInteractuar != null) panelInteractuar.SetActive(false);
        if (panelSinLlave != null) panelSinLlave.SetActive(false);
        if (panelVictoria != null) panelVictoria.SetActive(false);
    }

    void Update()
    {
        if (escapeActivado || !jugadorEnZona) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            IntentarEscapar();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (escapeActivado || !other.CompareTag("Player")) return;

        jugadorEnZona = true;
        ultimoPlayer = other.gameObject;

        if (panelInteractuar != null)
        {
            panelInteractuar.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        jugadorEnZona = false;
        if (panelInteractuar != null) panelInteractuar.SetActive(false);
        if (panelSinLlave != null) panelSinLlave.SetActive(false);
    }

    private void IntentarEscapar()
    {
        if (LlaveMisteriosa.TieneLlaveMisteriosa)
        {
            escapeActivado = true;
            jugadorEnZona = false;

            if (panelInteractuar != null) panelInteractuar.SetActive(false);
            if (panelSinLlave != null) panelSinLlave.SetActive(false);

            if (eventoEscapeExitoso != null && eventoEscapeExitoso.IsValid())
            {
                eventoEscapeExitoso.Post(gameObject);
            }

            if (!string.IsNullOrWhiteSpace(idDialogoEscape) && ultimoPlayer != null)
            {
                ultimoPlayer.GetComponentInParent<ControladorVozPlayer>()?.ReproducirDialogo(idDialogoEscape);
            }

            StartCoroutine(FinalizarJuegoRoutine());
        }
        else
        {
            if (panelSinLlave != null)
            {
                panelSinLlave.SetActive(true);
            }

            if (eventoPuertaBloqueada != null && eventoPuertaBloqueada.IsValid())
            {
                eventoPuertaBloqueada.Post(gameObject);
            }

            if (!string.IsNullOrWhiteSpace(idDialogoSinLlave) && ultimoPlayer != null)
            {
                ultimoPlayer.GetComponentInParent<ControladorVozPlayer>()?.ReproducirDialogo(idDialogoSinLlave);
            }
        }
    }

    private IEnumerator FinalizarJuegoRoutine()
    {
        if (textoVictoria != null)
        {
            textoVictoria.text = mensajeVictoria;
        }

        if (panelVictoria != null)
        {
            panelVictoria.SetActive(true);
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        yield return new WaitForSeconds(segundosAntesDeSalir);

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}