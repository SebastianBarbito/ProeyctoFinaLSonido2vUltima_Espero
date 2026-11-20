using UnityEngine;

public class ProximidadAudioWwise : MonoBehaviour
{
    [Header("Evento y RTPC de Wwise")]
    [Tooltip("Evento en loop del caloventor o foco LED")]
    public AK.Wwise.Event eventoLoopWwise;

    [Tooltip("Nombre exacto del RTPC en Wwise (ej. RTPC_VolumenProximidad)")]
    public string nombreRTPC = "RTPC_VolumenProximidad";

    [Header("Distancias en Metros")]
    [Tooltip("Distancia a la que alcanza el volumen máximo configurado")]
    public float distanciaMinima = 1.0f;

    [Tooltip("Distancia a partir de la cual empieza a sonar al acercarte")]
    public float distanciaMaxima = 6.0f;

    [Tooltip("Margen extra para apagar al alejarse y evitar cortes rápidos en el borde")]
    public float margenApagado = 0.75f;

    [Header("Control de Nivel (Evita Saturación)")]
    [Range(0f, 100f)]
    [Tooltip("Tope máximo del RTPC al estar pegado al objeto (ej. 65-75 para no saturar)")]
    public float volumenMaximoRTPC = 70f;

    private Transform playerTransform;
    private bool estaSonando = false;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }

        AkSoundEngine.SetRTPCValue(nombreRTPC, 0f, gameObject);
        estaSonando = false;
    }

    void Update()
    {
        if (playerTransform == null) return;

        float distancia = Vector3.Distance(transform.position, playerTransform.position);
        float distanciaCorte = distanciaMaxima + margenApagado;

        if (!estaSonando && distancia <= distanciaMaxima)
        {
            AkSoundEngine.SetRTPCValue(nombreRTPC, 0f, gameObject);
            if (eventoLoopWwise != null && eventoLoopWwise.IsValid())
            {
                eventoLoopWwise.Post(gameObject);
                estaSonando = true;
            }
        }
        else if (estaSonando && distancia > distanciaCorte)
        {
            AkSoundEngine.SetRTPCValue(nombreRTPC, 0f, gameObject);
            if (eventoLoopWwise != null && eventoLoopWwise.IsValid())
            {
                eventoLoopWwise.Stop(gameObject, 150);
            }
            estaSonando = false;
        }

        if (estaSonando)
        {
            float factor = 1f - Mathf.InverseLerp(distanciaMinima, distanciaMaxima, distancia);
            float valorRTPC = (factor * factor) * volumenMaximoRTPC;
            AkSoundEngine.SetRTPCValue(nombreRTPC, valorRTPC, gameObject);
        }
    }

    private void OnDisable()
    {
        if (estaSonando && eventoLoopWwise != null && eventoLoopWwise.IsValid())
        {
            AkSoundEngine.SetRTPCValue(nombreRTPC, 0f, gameObject);
            eventoLoopWwise.Stop(gameObject);
        }
        estaSonando = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, distanciaMinima);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, distanciaMaxima);
    }
}