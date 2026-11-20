using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
// Si usas TextMeshPro, descomenta:
// using TMPro;

public class ControladorVozPlayer : MonoBehaviour
{
    [System.Serializable]
    public class EntradaDialogo
    {
        public string id; // Ej: "camilla", "ascensor_falla", "llave", "escape", "pasillo_1"
        [TextArea] public string subtitulo;
        public AK.Wwise.Event eventoWwise;
        public float duracion = 3.0f;
    }

    [Header("Lista de Todos los Diálogos")]
    public List<EntradaDialogo> dialogos = new List<EntradaDialogo>();

    [Header("UI de Subtítulos")]
    public GameObject panelSubtitulo;
    public Text textoSubtitulo; // O TMP_Text

    private Coroutine rutinaActual;

    void Awake()
    {
        if (panelSubtitulo != null) panelSubtitulo.SetActive(false);
    }

    public void ReproducirDialogo(string idDialogo)
    {
        EntradaDialogo encontrado = dialogos.Find(d => d.id == idDialogo);
        if (encontrado == null)
        {
            Debug.LogWarning("No se encontró el diálogo con ID: " + idDialogo);
            return;
        }

        if (rutinaActual != null) StopCoroutine(rutinaActual);
        rutinaActual = StartCoroutine(RutinaMostrar(encontrado));
    }

    private IEnumerator RutinaMostrar(EntradaDialogo d)
    {
        // Dispara el evento directo en el Player
        if (d.eventoWwise != null && d.eventoWwise.IsValid())
        {
            d.eventoWwise.Post(gameObject);
        }

        if (textoSubtitulo != null) textoSubtitulo.text = d.subtitulo;
        if (panelSubtitulo != null) panelSubtitulo.SetActive(true);

        yield return new WaitForSeconds(d.duracion);

        if (panelSubtitulo != null) panelSubtitulo.SetActive(false);
        rutinaActual = null;
    }
}