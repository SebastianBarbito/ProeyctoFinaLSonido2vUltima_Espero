using UnityEngine;

[RequireComponent(typeof(Collider))]
public class SonidoEnCollider : MonoBehaviour
{
    [Header("Evento de Wwise")]
    [Tooltip("Evento en loop del caloventor o foco LED")]
    public AK.Wwise.Event eventoSonido;

    [Header("Control de Salida")]
    [Tooltip("Tiempo de fade-out en milisegundos al salir del collider (0 = corte inmediato)")]
    public int fadeOutMs = 150;

    [Tooltip("Si está activo, evita que se encime si ya está sonando dentro del collider")]
    public bool evitarSuperposicion = true;

    private bool jugadorDentro = false;
    private bool estaSonando = false;

    private void Reset()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        jugadorDentro = true;

        if (evitarSuperposicion && estaSonando) return;

        if (eventoSonido != null && eventoSonido.IsValid())
        {
            eventoSonido.Post(gameObject);
            estaSonando = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        jugadorDentro = false;

        if (eventoSonido != null && eventoSonido.IsValid())
        {
            eventoSonido.Stop(gameObject, fadeOutMs);
        }
        estaSonando = false;
    }

    private void OnDisable()
    {
        if (estaSonando && eventoSonido != null && eventoSonido.IsValid())
        {
            eventoSonido.Stop(gameObject);
        }
        jugadorDentro = false;
        estaSonando = false;
    }
}