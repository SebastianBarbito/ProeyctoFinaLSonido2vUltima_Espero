using System.Collections;
using UnityEngine;

public class AscensorFalla : MonoBehaviour
{
    [Header("Eventos de Wwise")]
    public AK.Wwise.Event eventoLlamarAscensor;
    public AK.Wwise.Event eventoFallaAscensor;
    public AK.Wwise.Event eventoAdvertencia;

    [Header("Tiempos de la Secuencia")]
    public float tiempoLlamada = 1.5f;
    public float retrasoAdvertenciaYNota = 1.0f;
    public float tiempoMostrarNota = 5.0f;

    [Header("Configuración de UI")]
    public GameObject panelInteractuar;
    public GameObject panelNota;

    [Header("Recompensa / Escape")]
    [Tooltip("Arrastra aquí el objeto de la llave (empieza desactivado y se activa al terminar la falla)")]
    public GameObject llaveObjeto;

    [Header("Configuración de la Cámara (Efecto Temblor)")]
    public Transform camaraPlayer;
    public float duracionTemblor = 1.5f;
    public float magnitudTemblor = 0.1f;

    private bool secuenciaIniciada = false;
    private bool jugadorEnZona = false;

    void Start()
    {
        if (llaveObjeto != null)
        {
            llaveObjeto.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (secuenciaIniciada) return;

        if (other.CompareTag("Player"))
        {
            jugadorEnZona = true;
            if (panelInteractuar != null)
            {
                panelInteractuar.SetActive(true);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (secuenciaIniciada || !jugadorEnZona) return;

        if (other.CompareTag("Player") && Input.GetKeyDown(KeyCode.E))
        {
            secuenciaIniciada = true;
            jugadorEnZona = false;

            if (panelInteractuar != null)
            {
                panelInteractuar.SetActive(false);
            }

            StartCoroutine(SecuenciaFallaAscensor());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorEnZona = false;
            if (panelInteractuar != null)
            {
                panelInteractuar.SetActive(false);
            }
        }
    }

    IEnumerator SecuenciaFallaAscensor()
    {
        if (eventoLlamarAscensor != null && eventoLlamarAscensor.IsValid())
        {
            eventoLlamarAscensor.Post(gameObject);
        }

        if (tiempoLlamada > 0f)
        {
            yield return new WaitForSeconds(tiempoLlamada);
        }

        if (eventoFallaAscensor != null && eventoFallaAscensor.IsValid())
        {
            eventoFallaAscensor.Post(gameObject);
        }

        if (camaraPlayer != null)
        {
            StartCoroutine(TemblorCamaraRoutine());
        }

        if (retrasoAdvertenciaYNota > 0f)
        {
            yield return new WaitForSeconds(retrasoAdvertenciaYNota);
        }

        if (eventoAdvertencia != null && eventoAdvertencia.IsValid())
        {
            eventoAdvertencia.Post(gameObject);
        }

        if (panelNota != null)
        {
            panelNota.SetActive(true);
        }

        yield return new WaitForSeconds(tiempoMostrarNota);

        if (panelNota != null)
        {
            panelNota.SetActive(false);
        }

        // Habilita la llave una vez que terminó toda la secuencia de falla
        if (llaveObjeto != null)
        {
            llaveObjeto.SetActive(true);
        }
    }

    IEnumerator TemblorCamaraRoutine()
    {
        Vector3 posicionOriginal = camaraPlayer.localPosition;
        float tiempoTranscurrido = 0f;

        while (tiempoTranscurrido < duracionTemblor)
        {
            float x = Random.Range(-1f, 1f) * magnitudTemblor;
            float y = Random.Range(-1f, 1f) * magnitudTemblor;

            camaraPlayer.localPosition = new Vector3(posicionOriginal.x + x, posicionOriginal.y + y, posicionOriginal.z);

            tiempoTranscurrido += Time.deltaTime;
            yield return null;
        }

        camaraPlayer.localPosition = posicionOriginal;
    }
}