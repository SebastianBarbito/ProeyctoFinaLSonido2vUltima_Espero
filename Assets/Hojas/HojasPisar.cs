using UnityEngine;

[RequireComponent(typeof(Collider))]
public class HojasPisar : MonoBehaviour
{
    [Header("Evento de Wwise (Container de Hojas)")]
    [Tooltip("Evento que dispara el Random/Blend Container de hojas")]
    public AK.Wwise.Event eventoPisarHojas;

    [Header("Control de Repetición")]
    [Tooltip("Tiempo mínimo en segundos antes de volver a sonar si el Player sigue moviéndose encima")]
    public float cooldownSonido = 0.45f;

    [Tooltip("Si está activo, también suena mientras caminas encima (no solo al entrar)")]
    public bool sonarAlCaminarEncima = true;

    private float tiempoSiguienteSonido = 0f;
    private Vector3 ultimaPosicionPlayer;

    private void Reset()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        ultimaPosicionPlayer = other.transform.position;
        DispararSonidoHojas();
    }

    private void OnTriggerStay(Collider other)
    {
        if (!sonarAlCaminarEncima || !other.CompareTag("Player")) return;

        // Solo dispara si el jugador realmente se está desplazando sobre las hojas
        float distanciaMovida = Vector3.Distance(other.transform.position, ultimaPosicionPlayer);
        if (distanciaMovida > 0.35f && Time.time >= tiempoSiguienteSonido)
        {
            ultimaPosicionPlayer = other.transform.position;
            DispararSonidoHojas();
        }
    }

    private void DispararSonidoHojas()
    {
        if (Time.time < tiempoSiguienteSonido) return;

        if (eventoPisarHojas != null && eventoPisarHojas.IsValid())
        {
            eventoPisarHojas.Post(gameObject);
            tiempoSiguienteSonido = Time.time + cooldownSonido;
        }
    }
}