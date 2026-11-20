using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DisparadorDialogo : MonoBehaviour
{
    public string idDialogo = "pasillo_1";
    public bool soloUnaVez = true;
    private bool usado = false;

    private void Reset()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (usado && soloUnaVez) return;
        if (!other.CompareTag("Player")) return;

        ControladorVozPlayer voz = other.GetComponentInParent<ControladorVozPlayer>();
        if (voz != null)
        {
            usado = true;
            voz.ReproducirDialogo(idDialogo);
        }
    }
}