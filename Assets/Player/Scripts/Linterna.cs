using UnityEngine;

public class Linterna : MonoBehaviour
{
    [Header("Referencias de Unity")]
    public Light luzLinterna;

    [Header("Configuración de Wwise (Audio)")]
    [Tooltip("Evento que suena al encender la linterna")]
    public AK.Wwise.Event eventoEncender;

    [Tooltip("Evento que suena al apagar la linterna")]
    public AK.Wwise.Event eventoApagar;

    [Header("Estado (Solo lectura)")]
    [SerializeField] private bool estaEncendida = false;

    void Start()
    {
        if (luzLinterna != null)
        {
            luzLinterna.enabled = estaEncendida;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            AlternarLinterna();
        }
    }

    void AlternarLinterna()
    {
        estaEncendida = !estaEncendida;

        // 1. Encender o apagar la luz física
        if (luzLinterna != null)
        {
            luzLinterna.enabled = estaEncendida;
        }

        // 2. Reproducir el sonido correspondiente en Wwise
        if (estaEncendida)
        {
            if (eventoEncender != null && eventoEncender.IsValid())
            {
                eventoEncender.Post(gameObject);
            }
        }
        else
        {
            if (eventoApagar != null && eventoApagar.IsValid())
            {
                eventoApagar.Post(gameObject);
            }
        }
    }
}