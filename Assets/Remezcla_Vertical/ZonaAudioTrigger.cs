using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ZonaAudioTrigger : MonoBehaviour
{
    public enum TipoZona { Pasillo, SalaMaquinas, PasilloSuspenso, Morgue }

    [Header("Configuración de la Zona")]
    public TipoZona zona;

    [Header("Capas de música para esta zona (1 a 4)")]
    [Range(1, 4)]
    public int cantidadDeCapas = 1;

    private void Reset()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Evita triggers secundarios y verifica que sea el Jugador
        if (other.isTrigger || !other.CompareTag("Player")) return;
        if (ControladorMusicaTotal.Instancia == null) return;

        // Envía ambos datos de golpe al controlador maestro
        ControladorMusicaTotal.Instancia.CambiarZona(zona, cantidadDeCapas);
    }
}