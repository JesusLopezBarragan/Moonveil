using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

// El vendedor de la tienda. Si te acercas y pulsas E te habla,
// y si vuelves a pulsar E te compra las zanahorias, la piedra y los minerales de la mochila.
public class Vendedor : MonoBehaviour
{
    public string nombre = "Tomás";
    // Lo que compra y cuántas monedas da por cada uno (misma posición en los dos arrays)
    private string[] productos = { "Zanahoria", "Piedra", "Cobre", "Hierro", "Oro" };
    private int[] precios = { 5, 1, 5, 10, 25 };

    private bool jugadorCerca = false;
    private bool hablando = false;

    private GameObject aviso;     // cartel "E: hablar"
    private GameObject ventana;   // cuadro de diálogo
    private Text textoDialogo;

    void Start()
    {
        CrearInterfaz();
    }

    void Update()
    {
        // El aviso solo se ve si estás cerca y todavía no estás hablando
        aviso.SetActive(jugadorCerca && !hablando);

        if (!jugadorCerca) return;

        // Tecla E: la primera vez saluda, la segunda vende
        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            if (!hablando) Saludar();
            else Vender();
        }

        // Tecla Escape: dejar de hablar
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Cerrar();
        }
    }

    void Saludar()
    {
        hablando = true;
        ventana.SetActive(true);

        // Montamos la lista de precios y calculamos cuánto vale lo que llevamos
        string listaPrecios = "";
        int valorMochila = 0;
        for (int i = 0; i < productos.Length; i++)
        {
            listaPrecios += productos[i] + " " + precios[i] + "     ";
            valorMochila += Inventario.instancia.Contar(productos[i]) * precios[i];
        }

        textoDialogo.text = "¡Hola, granjero! Te compro esto (monedas por unidad):\n" +
                            listaPrecios + "\n" +
                            "Lo que llevas en la mochila vale " + valorMochila + " monedas.\n\n" +
                            "E: vender todo          Esc: salir";
    }

    void Vender()
    {
        int vendidos = 0;
        int ganadas = 0;

        // Recorremos todos los productos: los sacamos de la mochila y sumamos su precio
        for (int i = 0; i < productos.Length; i++)
        {
            int cantidad = Inventario.instancia.QuitarTodos(productos[i]);
            vendidos += cantidad;
            ganadas += cantidad * precios[i];

            if (productos[i] == "Zanahoria") ControladorJuego.zanahorias -= cantidad;
        }

        if (vendidos == 0)
        {
            textoDialogo.text = "No llevas nada que me interese...\n¡Vuelve con zanahorias o con minerales de la mina!\n\nEsc: salir";
            return;
        }

        ControladorJuego.monedas += ganadas;
        textoDialogo.text = "¡Gracias! Por tus " + vendidos + " cosas te doy " + ganadas + " monedas.\n" +
                            "Ahora tienes " + ControladorJuego.monedas + " monedas.\n\nEsc: salir";
    }

    void Cerrar()
    {
        hablando = false;
        ventana.SetActive(false);
    }

    // El círculo grande del vendedor es un "trigger": avisa cuando el jugador entra o sale
    void OnTriggerEnter2D(Collider2D otro)
    {
        if (otro.CompareTag("Player")) jugadorCerca = true;
    }

    void OnTriggerExit2D(Collider2D otro)
    {
        if (otro.CompareTag("Player"))
        {
            jugadorCerca = false;
            Cerrar(); // si te vas, deja de hablar
        }
    }

    // ---------------- Interfaz ----------------

    void CrearInterfaz()
    {
        Color crema = new Color32(255, 243, 214, 255);
        Color marron = new Color32(59, 36, 22, 255);

        Transform canvas = AyudaUI.CrearCanvas("CanvasVendedor", transform, 15);

        // Aviso pequeño encima de la barra de herramientas
        Image fondoAviso = AyudaUI.CrearImagen("Aviso", canvas, Resources.Load<Sprite>("UI/panel_liso"), 3);
        AyudaUI.Colocar(fondoAviso.rectTransform, new Vector2(0.5f, 0f), new Vector2(0f, 120f), new Vector2(220f, 50f));
        Text textoAviso = AyudaUI.CrearTexto("Texto", fondoAviso.transform, "E: hablar con " + nombre, 18, crema);
        textoAviso.alignment = TextAnchor.MiddleCenter;
        AyudaUI.Colocar(textoAviso.rectTransform, new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(220f, 50f));
        aviso = fondoAviso.gameObject;

        // Cuadro de diálogo abajo, con la cara del vendedor a la izquierda
        Image fondo = AyudaUI.CrearImagen("Dialogo", canvas, Resources.Load<Sprite>("UI/panel_liso"), 4);
        AyudaUI.Colocar(fondo.rectTransform, new Vector2(0.5f, 0f), new Vector2(0f, 110f), new Vector2(760f, 190f));
        ventana = fondo.gameObject;

        Image hueco = AyudaUI.CrearImagen("Hueco", ventana.transform, Resources.Load<Sprite>("UI/ranura"), 3);
        AyudaUI.Colocar(hueco.rectTransform, new Vector2(0f, 0.5f), new Vector2(30f, 10f), new Vector2(110f, 110f));
        Image cara = AyudaUI.CrearImagen("Cara", hueco.transform, Resources.Load<Sprite>("Tienda/vendedor"), 0);
        cara.preserveAspect = true;
        AyudaUI.Colocar(cara.rectTransform, new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(80f, 90f));

        Text textoNombre = AyudaUI.CrearTexto("Nombre", ventana.transform, nombre, 20, crema);
        textoNombre.alignment = TextAnchor.MiddleCenter;
        textoNombre.gameObject.AddComponent<Shadow>().effectColor = marron;
        AyudaUI.Colocar(textoNombre.rectTransform, new Vector2(0f, 0f), new Vector2(30f, 18f), new Vector2(110f, 30f));

        textoDialogo = AyudaUI.CrearTexto("Texto", ventana.transform, "", 20, crema);
        textoDialogo.alignment = TextAnchor.MiddleLeft;
        textoDialogo.gameObject.AddComponent<Shadow>().effectColor = marron;
        AyudaUI.Colocar(textoDialogo.rectTransform, new Vector2(0f, 0.5f), new Vector2(165f, 0f), new Vector2(560f, 150f));

        aviso.SetActive(false);
        ventana.SetActive(false);
    }
}
