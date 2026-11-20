using System.Collections;
using UnityEngine;
using UnityEngine.UI;
// Si usas TextMeshPro, descomenta la siguiente línea y cambia Text por TMP_Text:
// using TMPro;

public class LlaveMisteriosa : MonoBehaviour
{
    public static bool TieneLlaveMisteriosa { get; private set; } = false;

    [Header("Audio de Wwise")]
    public AK.Wwise.Event eventoRecogerLlave;

    [Header("Diálogo del Jugador (ControladorVozPlayer)")]
    [Tooltip("ID del diálogo al recoger la llave misteriosa")]
    public string idDialogoLlave = "llave";

    [Header("UI de Interacción y Mensaje")]
    [Tooltip("Aviso opcional al acercarse (ej. 'Presiona E para recoger')")]
    public GameObject panelInteractuar;
    public GameObject panelMensajeLlave;
    public Text textoMensajeLlave; // O usa TMP_Text si trabajas con TextMeshPro
    public string mensaje = "Has encontrado una llave misteriosa";
    public float duracionMensaje = 3.5f;

    private bool jugadorEnZona = false;
    private bool recogida = false;
    private GameObject ultimoPlayer;
    private Collider col;
    private MeshRenderer[] renderers;

    void Awake()
    {
        col = GetComponent<Collider>();
        renderers = GetComponentsInChildren<MeshRenderer>();

        if (panelInteractuar != null)
        {
            panelInteractuar.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (recogida || !other.CompareTag("Player")) return;

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
        if (panelInteractuar != null)
        {
            panelInteractuar.SetActive(false);
        }
    }

    void Update()
    {
        if (recogida || !jugadorEnZona) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            RecogerLlave();
        }
    }

    private void RecogerLlave()
    {
        recogida = true;
        jugadorEnZona = false;
        TieneLlaveMisteriosa = true;

        if (panelInteractuar != null)
        {
            panelInteractuar.SetActive(false);
        }

        if (eventoRecogerLlave != null && eventoRecogerLlave.IsValid())
        {
            eventoRecogerLlave.Post(gameObject);
        }

        if (!string.IsNullOrWhiteSpace(idDialogoLlave) && ultimoPlayer != null)
        {
            ultimoPlayer.GetComponentInParent<ControladorVozPlayer>()?.ReproducirDialogo(idDialogoLlave);
        }

        if (col != null) col.enabled = false;
        foreach (MeshRenderer r in renderers)
        {
            r.enabled = false;
        }

        StartCoroutine(MostrarMensajeRoutine());
    }

    IEnumerator MostrarMensajeRoutine()
    {
        if (textoMensajeLlave != null)
        {
            textoMensajeLlave.text = mensaje;
        }

        if (panelMensajeLlave != null)
        {
            panelMensajeLlave.SetActive(true);
        }

        yield return new WaitForSeconds(duracionMensaje);

        if (panelMensajeLlave != null)
        {
            panelMensajeLlave.SetActive(false);
        }

        gameObject.SetActive(false);
    }
}