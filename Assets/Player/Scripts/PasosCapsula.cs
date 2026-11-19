using UnityEngine;

public class PasosCapsula : MonoBehaviour
{
    [Header("Wwise Event para Pasos")]
    public AK.Wwise.Event eventoPasos;

    [Header("Configuración de Distancia")]
    [Tooltip("Distancia que debe recorrer la cápsula para que suene el siguiente paso")]
    public float distanciaEntrePasos = 1.8f;
    [Tooltip("Velocidad mínima para considerar que se está moviendo")]
    public float umbralVelocidad = 0.1f;

    private CharacterController controladorCC;
    private Rigidbody rb3D;
    private float distanciaRecorrida = 0f;
    private Vector3 ultimaPosicion;

    void Start()
    {
        // Detecta automáticamente si usas CharacterController o Rigidbody
        controladorCC = GetComponent<CharacterController>();
        rb3D = GetComponent<Rigidbody>();

        ultimaPosicion = transform.position;
    }

    void Update()
    {
        // Calculamos la distancia real movida en el plano horizontal (X e Z)
        Vector3 posicionActual = transform.position;
        Vector3 desplazamiento = new Vector3(posicionActual.x - ultimaPosicion.x, 0f, posicionActual.z - ultimaPosicion.z);
        float distanciaFrame = desplazamiento.magnitude;

        bool seEstaMoviendo = false;

        // Verificamos el movimiento según el componente físico que use tu cápsula
        if (controladorCC != null)
        {
            seEstaMoviendo = controladorCC.isGrounded && controladorCC.velocity.magnitude > umbralVelocidad;
        }
        else if (rb3D != null)
        {
            // CORREGIDO: Se usa 'velocity' en lugar de 'linearVelocity' para Unity 2022 LTS
            Vector3 velocidadPlana = new Vector3(rb3D.velocity.x, 0f, rb3D.velocity.z);
            seEstaMoviendo = velocidadPlana.magnitude > umbralVelocidad;
        }
        else
        {
            // Método genérico por posición si no usas físicas complejas
            seEstaMoviendo = distanciaFrame > 0.001f;
        }

        if (seEstaMoviendo)
        {
            distanciaRecorrida += distanciaFrame;

            // Si acumulamos la distancia necesaria, suena el paso
            if (distanciaRecorrida >= distanciaEntrePasos)
            {
                ReproducirPaso();
                distanciaRecorrida = 0f; // Reiniciamos el acumulador
            }
        }
        else
        {
            distanciaRecorrida = 0f;
        }

        ultimaPosicion = posicionActual;
    }

    
    void ReproducirPaso()
    {
        Debug.Log("¡Intentando reproducir paso!"); // Mira si esto aparece en la consola de Unity

        if (eventoPasos != null && eventoPasos.IsValid())
        {
            eventoPasos.Post(gameObject);
        }
        else
        {
            Debug.LogWarning("El evento de Wwise es nulo o no es válido.");
        }
    }
}