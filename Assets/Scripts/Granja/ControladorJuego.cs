using UnityEngine;
using UnityEngine.UI;

// Guarda cuántas zanahorias llevamos y lo muestra en pantalla.
public class ControladorJuego : MonoBehaviour
{
    public static int zanahorias = 0;   // static: se puede usar desde cualquier script
    public static int semillas = 10;    // NUEVO: semillas de zanahoria para plantar con la herramienta
    public static int monedas = 0;      // NUEVO (tienda): monedas que da el vendedor
    public Text textoZanahorias;

    void Start()
    {
        zanahorias = 0;
        semillas = 10;
        monedas = 0;
        textoZanahorias.verticalOverflow = VerticalWrapMode.Overflow; // NUEVO: para que quepan las dos líneas
    }

    void Update()
    {
        textoZanahorias.text = "Zanahorias: " + zanahorias + "\nMonedas: " + monedas;
    }
}
