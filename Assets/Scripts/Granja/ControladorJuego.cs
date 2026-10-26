using UnityEngine;
using UnityEngine.UI;

// Guarda cuántas zanahorias llevamos y lo muestra en pantalla.
public class ControladorJuego : MonoBehaviour
{
    public static int zanahorias = 0;   // static: se puede usar desde cualquier script
    public Text textoZanahorias;

    void Start()
    {
        zanahorias = 0;
    }

    void Update()
    {
        textoZanahorias.text = "Zanahorias: " + zanahorias;
    }
}
