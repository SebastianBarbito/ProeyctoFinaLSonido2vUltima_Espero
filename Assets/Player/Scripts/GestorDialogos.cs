using System.Collections;
using UnityEngine;
using UnityEngine.UI;
// Si usas TextMeshPro, descomenta la siguiente línea y cambia Text por TMP_Text:
// using TMPro;

public class GestorDialogos : MonoBehaviour
{
    public static GestorDialogos Instancia { get; private set; }

    [Header("UI de Subtítulos")]
    public GameObject panelSubtitulo;
    public Text textoSubtitulo; // O usa TMP_Text si trabajas con TextMeshPro

    [Header("Opciones de Cola / Anti-Superposición")]
    [Tooltip("Si está activo, ignora nuevas líneas si ya se está mostrando una (evita que se pisen)")]
    public bool bloquearSiEstaHablando = true;

    private Coroutine rutinaActual;
    private bool estaHablando = false;

    void Awake()
    {
        if (Instancia == null) Instancia = this;
        else Destroy(gameObject);

        if (panelSubtitulo != null)
        {
            panelSubtitulo.SetActive(false);
        }
    }

    public void DecirLinea(string linea, float duracionSegundos, AK.Wwise.Event eventoVoz, GameObject emisor)
    {
        if (string.IsNullOrWhiteSpace(linea)) return;
        if (bloquearSiEstaHablando && estaHablando) return;

        if (rutinaActual != null)
        {
            StopCoroutine(rutinaActual);
        }

        rutinaActual = StartCoroutine(MostrarLineaRoutine(linea, duracionSegundos, eventoVoz, emisor));
    }

    private IEnumerator MostrarLineaRoutine(string linea, float duracionSegundos, AK.Wwise.Event eventoVoz, GameObject emisor)
    {
        estaHablando = true;

        if (eventoVoz != null && eventoVoz.IsValid())
        {
            GameObject target = emisor != null ? emisor : gameObject;
            eventoVoz.Post(target);
        }

        if (textoSubtitulo != null)
        {
            textoSubtitulo.text = linea;
        }

        if (panelSubtitulo != null)
        {
            panelSubtitulo.SetActive(true);
        }

        yield return new WaitForSeconds(duracionSegundos);

        if (panelSubtitulo != null)
        {
            panelSubtitulo.SetActive(false);
        }

        estaHablando = false;
        rutinaActual = null;
    }
}