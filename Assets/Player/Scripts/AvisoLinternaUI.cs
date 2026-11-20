using System.Collections;
using UnityEngine;
using UnityEngine.UI;
// Si usas TextMeshPro, descomenta la siguiente línea y cambia Text por TMP_Text:
// using TMPro;

public class AvisoLinternaUI : MonoBehaviour
{
    [Header("Referencias de UI")]
    [Tooltip("Panel del Canvas con el aviso de la linterna")]
    public GameObject panelAvisoLinterna;
    [Tooltip("Texto opcional dentro del panel")]
    public Text textoAviso; // O TMP_Text si usas TextMeshPro
    public string mensaje = "Presiona F para prender la linterna";

    [Header("Opciones de Desaparición")]
    [Tooltip("Si está activo, el cartel se oculta apenas el jugador presiona F por primera vez")]
    public bool ocultarAlPresionarF = true;

    [Tooltip("Tiempo máximo en segundos antes de ocultarse solo (0 = no se oculta por tiempo)")]
    public float tiempoMaximoVisible = 0f;

    private bool yaSeOculto = false;

    void Start()
    {
        if (textoAviso != null)
        {
            textoAviso.text = mensaje;
        }

        if (panelAvisoLinterna != null)
        {
            panelAvisoLinterna.SetActive(true);
        }

        if (tiempoMaximoVisible > 0f)
        {
            StartCoroutine(OcultarPorTiempoRoutine());
        }
    }

    void Update()
    {
        if (yaSeOculto) return;

        if (ocultarAlPresionarF && Input.GetKeyDown(KeyCode.F))
        {
            OcultarAviso();
        }
    }

    private IEnumerator OcultarPorTiempoRoutine()
    {
        yield return new WaitForSeconds(tiempoMaximoVisible);
        OcultarAviso();
    }

    public void OcultarAviso()
    {
        if (yaSeOculto) return;
        yaSeOculto = true;

        if (panelAvisoLinterna != null)
        {
            panelAvisoLinterna.SetActive(false);
        }
    }
}