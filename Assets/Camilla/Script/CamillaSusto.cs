using System.Collections;
using UnityEngine;

public class CamillaSusto : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    [Tooltip("Cuánto se desplazará la camilla en ejes locales (X, Y, Z)")]
    public Vector3 desplazamientoLocal = new Vector3(1.5f, 0f, 0f);

    [Tooltip("Rotación extra en grados (opcional, ej. un pequeño giro brusco al moverse)")]
    public Vector3 rotacionExtra = new Vector3(0f, 15f, 0f);

    [Tooltip("Duración del movimiento en segundos (valores bajos = susto más rápido)")]
    public float duracionMovimiento = 0.6f;

    [Tooltip("Retraso opcional antes de moverse tras detectar al Player")]
    public float retrasoActivacion = 0f;

    [Header("Audio de Wwise")]
    [Tooltip("Evento de Wwise con el sonido de las ruedas/metal de la camilla")]
    public AK.Wwise.Event eventoMoverCamilla;

    [Header("Opciones de Activación")]
    [Tooltip("Si está activo, solo ocurre una vez por partida")]
    public bool soloUnaVez = true;

    private Vector3 posicionInicial;
    private Quaternion rotacionInicial;
    private bool yaSeActivo = false;

    void Start()
    {
        posicionInicial = transform.localPosition;
        rotacionInicial = transform.localRotation;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (yaSeActivo && soloUnaVez) return;
        if (!other.CompareTag("Player")) return;

        yaSeActivo = true;
        StartCoroutine(MoverCamillaRoutine());
    }

    private IEnumerator MoverCamillaRoutine()
    {
        if (retrasoActivacion > 0f)
        {
            yield return new WaitForSeconds(retrasoActivacion);
        }

        // Disparar sonido espacializado en la camilla
        if (eventoMoverCamilla != null && eventoMoverCamilla.IsValid())
        {
            eventoMoverCamilla.Post(gameObject);
        }

        Vector3 posicionObjetivo = posicionInicial + transform.TransformDirection(desplazamientoLocal);
        // Si es hija de otro objeto rotado, usamos localPosition con rotación local:
        Vector3 posLocalObjetivo = posicionInicial + (rotacionInicial * desplazamientoLocal);
        Quaternion rotLocalObjetivo = rotacionInicial * Quaternion.Euler(rotacionExtra);

        float tiempo = 0f;

        while (tiempo < duracionMovimiento)
        {
            tiempo += Time.deltaTime;
            float t = Mathf.Clamp01(tiempo / duracionMovimiento);
            // Curva suave al frenar (Ease Out) para que parezca que rueda y frena de golpe
            float curva = 1f - Mathf.Pow(1f - t, 3f);

            transform.localPosition = Vector3.Lerp(posicionInicial, posLocalObjetivo, curva);
            transform.localRotation = Quaternion.Slerp(rotacionInicial, rotLocalObjetivo, curva);

            yield return null;
        }

        transform.localPosition = posLocalObjetivo;
        transform.localRotation = rotLocalObjetivo;
    }
}