using System.Collections;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    [Header("Configuración de Apertura")]
    public float openAngle = 90f;

    [Tooltip("Define qué eje rota. Por defecto es Y (0,1,0). Si rota mal, prueba cambiando los valores a 1 en X o Z.")]
    public Vector3 rotationAxis = new Vector3(0, 1, 0);

    public float smooth = 2f;
    public float closeDelay = 3f;

    private Quaternion defaultRot;
    private Quaternion targetRot;
    private Coroutine closeCoroutine;

    void Start()
    {
        // Guardamos la rotación local inicial de la puerta
        defaultRot = transform.localRotation;
        targetRot = defaultRot;
    }

    void Update()
    {
        // Rota suavemente la puerta usando rotación local
        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRot, Time.deltaTime * smooth);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("player") || other.CompareTag("Player"))
        {
            OpenDoor();
        }
    }

    void OpenDoor()
    {
        // Aplica la rotación multiplicando por el eje configurado
        targetRot = defaultRot * Quaternion.Euler(rotationAxis * openAngle);

        if (closeCoroutine != null)
        {
            StopCoroutine(closeCoroutine);
        }
        closeCoroutine = StartCoroutine(CloseDoorRoutine());
    }

    IEnumerator CloseDoorRoutine()
    {
        yield return new WaitForSeconds(closeDelay);
        targetRot = defaultRot;
    }
}