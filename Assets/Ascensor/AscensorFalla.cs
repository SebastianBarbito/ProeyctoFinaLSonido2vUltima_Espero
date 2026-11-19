using System.Collections;
using UnityEngine;

public class AscensorFalla : MonoBehaviour
{
    [Header("Eventos de Wwise")]
    [Tooltip("Sonido al pulsar E / llamar al ascensor")]
    public AK.Wwise.Event eventoLlamarAscensor;

    [Tooltip("Sonido de falla del ascensor (falla_elevador)")]
    public AK.Wwise.Event eventoFallaAscensor;

    [Tooltip("Sonido de advertencia o alarma")]
    public AK.Wwise.Event eventoAdvertencia;

    [Header("Tiempos de la Secuencia")]
    [Tooltip("Segundos que espera tras llamar antes de que falle el ascensor")]
    public float tiempoLlamada = 1.5f;

    [Tooltip("Retraso tras el sonido de falla antes de lanzar la advertencia y mostrar la nota")]
    public float retrasoAdvertenciaYNota = 1.0f;

    [Tooltip("Segundos que se muestra la nota en pantalla")]
    public float tiempoMostrarNota = 5.0f;

    [Header("Configuración de UI")]
    [Tooltip("Aviso opcional al acercarse (ej. 'Presiona E')")]
    public GameObject panelInteractuar;

    [Tooltip("Panel o texto con el mensaje de fallo del ascensor")]
    public GameObject panelNota;

    [Header("Configuración de la Cámara (Efecto Temblor)")]
    public Transform camaraPlayer;
    public float duracionTemblor = 1.5f;
    public float magnitudTemblor = 0.1f;

    private bool secuenciaIniciada = false;
    private bool jugadorEnZona = false;

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
        // 1. Sonido al llamar al ascensor
        if (eventoLlamarAscensor != null && eventoLlamarAscensor.IsValid())
        {
            eventoLlamarAscensor.Post(gameObject);
        }

        if (tiempoLlamada > 0f)
        {
            yield return new WaitForSeconds(tiempoLlamada);
        }

        // 2. Sonido de falla del ascensor + temblor de cámara
        if (eventoFallaAscensor != null && eventoFallaAscensor.IsValid())
        {
            eventoFallaAscensor.Post(gameObject);
        }

        if (camaraPlayer != null)
        {
            StartCoroutine(TemblorCamaraRoutine());
        }

        // 3. Esperamos 1 segundo (retrasoAdvertenciaYNota) tras la falla
        if (retrasoAdvertenciaYNota > 0f)
        {
            yield return new WaitForSeconds(retrasoAdvertenciaYNota);
        }

        // 4. Suena la advertencia y aparece la nota al mismo tiempo
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