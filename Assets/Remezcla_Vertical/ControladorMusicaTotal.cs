using UnityEngine;

public class ControladorMusicaTotal : MonoBehaviour
{
    public static ControladorMusicaTotal Instancia;

    [Header("Wwise Events & States (Ambiente / Zona)")]
    public AK.Wwise.Event eventoPasilloMaquinas;
    public AK.Wwise.Event eventoMorgue;

    public AK.Wwise.State estadoPasillo;
    public AK.Wwise.State estadoSalaMaquinas;
    public AK.Wwise.State estadoPasilloSuspenso;

    [Header("Wwise RTPC (Capas de Intensidad)")]
    public AK.Wwise.RTPC rtpcMusica;
    public float[] valoresPorCapa = new float[] { 0f, 35f, 70f, 100f };

    private bool sonandoPasilloMaquinas = false;
    private bool sonandoMorgue = false;
    private string estadoActual = "";
    private float tiempoUltimoCambio = -10f;

    void Awake()
    {
        if (Instancia == null) Instancia = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        // Arranca por defecto en el Pasillo con 1 capa
        CambiarZona(ZonaAudioTrigger.TipoZona.Pasillo, 1);
    }

    public void CambiarZona(ZonaAudioTrigger.TipoZona zona, int cantidadCapas)
    {
        // Cooldown de 0.5s para evitar rebotes en los bordes de los colliders
        if (Time.time - tiempoUltimoCambio < 0.5f) return;

        // 1. GESTIÓN DE EVENTOS Y STATES (Morgue vs Pasillos)
        if (zona == ZonaAudioTrigger.TipoZona.Morgue)
        {
            // Si estábamos en pasillo/maquinarias, lo apagamos con fade out (1000ms)
            if (sonandoPasilloMaquinas && eventoPasilloMaquinas != null)
            {
                eventoPasilloMaquinas.Stop(gameObject, 1000);
                sonandoPasilloMaquinas = false;
                estadoActual = "";
            }

            // Encendemos la morgue si no está sonando ya
            if (!sonandoMorgue && eventoMorgue != null)
            {
                eventoMorgue.Post(gameObject);
                sonandoMorgue = true;
            }
        }
        else // Es Pasillo, Sala de Máquinas o Pasillo Suspenso
        {
            // Si sonaba la morgue, la apagamos con fade out
            if (sonandoMorgue && eventoMorgue != null)
            {
                eventoMorgue.Stop(gameObject, 1000);
                sonandoMorgue = false;
            }

            // Seleccionamos el State correcto según la zona
            AK.Wwise.State estadoObjetivo = null;
            switch (zona)
            {
                case ZonaAudioTrigger.TipoZona.Pasillo:
                    estadoObjetivo = estadoPasillo;
                    break;
                case ZonaAudioTrigger.TipoZona.SalaMaquinas:
                    estadoObjetivo = estadoSalaMaquinas;
                    break;
                case ZonaAudioTrigger.TipoZona.PasilloSuspenso:
                    estadoObjetivo = estadoPasilloSuspenso;
                    break;
            }

            // Aplicamos el State si es válido y cambió
            if (estadoObjetivo != null && estadoObjetivo.IsValid() && estadoActual != estadoObjetivo.Name)
            {
                estadoObjetivo.SetValue();
                estadoActual = estadoObjetivo.Name;
            }

            // Aseguramos que el evento principal esté sonando
            if (!sonandoPasilloMaquinas && eventoPasilloMaquinas != null)
            {
                eventoPasilloMaquinas.Post(gameObject);
                sonandoPasilloMaquinas = true;
            }
        }

        // 2. GESTIÓN DEL RTPC (Capas de música)
        if (rtpcMusica != null)
        {
            int indice = Mathf.Clamp(cantidadCapas - 1, 0, valoresPorCapa.Length - 1);
            rtpcMusica.SetGlobalValue(valoresPorCapa[indice]);
        }

        tiempoUltimoCambio = Time.time;
    }
}