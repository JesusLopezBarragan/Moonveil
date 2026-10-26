using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

// Ventana de la mochila: una cuadrícula de 5x5 que se abre y se cierra con la tecla I (o Tab).
// Al pasar el ratón por encima de un hueco se ve el nombre del objeto en el título.
public class InterfazInventario : MonoBehaviour
{
    public Inventario inventario;

    // static: UsarHerramienta lo mira para no usar herramientas con la mochila abierta
    public static bool abierto = false;

    public float tamRanura = 64f;
    public float separacion = 6f;
    public float margen = 20f;
    public float altoTitulo = 44f;

    private Color colorMarron = new Color32(59, 36, 22, 255);
    private Color colorCrema = new Color32(255, 243, 214, 255);

    private GameObject ventana;
    private Text titulo;
    private Image[] fondos;   // el hueco de color crema
    private Image[] iconos;   // el dibujo del objeto
    private Text[] cantidades;

    private Sprite spriteRanura;
    private Sprite spriteRanuraEncima;

    void Start()
    {
        abierto = false;
        spriteRanura = Resources.Load<Sprite>("UI/ranura");
        spriteRanuraEncima = Resources.Load<Sprite>("UI/ranura_seleccionada");

        Transform canvas = AyudaUI.CrearCanvas("CanvasInventario", transform, 20); // encima de la barra (10)
        CrearVentana(canvas);
        ventana.SetActive(false); // empieza cerrada
    }

    void Update()
    {
        // 1. Abrir / cerrar con I o Tab
        Keyboard teclado = Keyboard.current;
        if (teclado != null && (teclado.iKey.wasPressedThisFrame || teclado.tabKey.wasPressedThisFrame))
        {
            abierto = !abierto;   // si estaba abierto se cierra y al revés
            ventana.SetActive(abierto);
        }

        if (!abierto) return; // si está cerrada no hace falta actualizar nada

        // 2. Pintar cada hueco con lo que tiene la mochila
        string nombreEncima = "";
        for (int i = 0; i < inventario.ranuras.Length; i++)
        {
            Ranura ranura = inventario.ranuras[i];

            if (ranura.EstaVacia())
            {
                iconos[i].enabled = false;
                cantidades[i].text = "";
            }
            else
            {
                iconos[i].enabled = true;
                iconos[i].sprite = Resources.Load<Sprite>("Objetos/" + ranura.nombre.ToLower());
                cantidades[i].text = ranura.cantidad.ToString();
            }

            // 3. Si el ratón está encima del hueco, marco dorado y guardamos el nombre
            if (RatonEncima(fondos[i].rectTransform))
            {
                fondos[i].sprite = spriteRanuraEncima;
                if (!ranura.EstaVacia()) nombreEncima = ranura.nombre + "  x" + ranura.cantidad;
            }
            else
            {
                fondos[i].sprite = spriteRanura;
            }
        }

        // 4. El título enseña el objeto que señala el ratón, o "Mochila" si no señala nada
        if (nombreEncima != "") titulo.text = nombreEncima;
        else titulo.text = "Mochila";
    }

    bool RatonEncima(RectTransform rt)
    {
        if (Mouse.current == null) return false;
        Vector2 posicionRaton = Mouse.current.position.ReadValue();
        return RectTransformUtility.RectangleContainsScreenPoint(rt, posicionRaton);
    }

    void CrearVentana(Transform canvas)
    {
        // Fondo negro semitransparente que oscurece el juego detrás de la mochila
        Image oscuro = AyudaUI.CrearImagen("Oscurecer", canvas, null, 0);
        oscuro.color = new Color(0f, 0f, 0f, 0.4f);
        RectTransform rtOscuro = oscuro.rectTransform;
        rtOscuro.anchorMin = Vector2.zero;
        rtOscuro.anchorMax = Vector2.one;
        rtOscuro.offsetMin = Vector2.zero;
        rtOscuro.offsetMax = Vector2.zero;
        ventana = oscuro.gameObject;

        // Panel de madera en el centro
        float ancho = Inventario.COLUMNAS * tamRanura + (Inventario.COLUMNAS - 1) * separacion + margen * 2;
        float alto = Inventario.FILAS * tamRanura + (Inventario.FILAS - 1) * separacion + margen * 2 + altoTitulo;
        Image panel = AyudaUI.CrearImagen("Mochila", ventana.transform, Resources.Load<Sprite>("UI/panel_madera"), 4);
        AyudaUI.Colocar(panel.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0f, 40f), new Vector2(ancho, alto));

        // Título arriba
        titulo = AyudaUI.CrearTexto("Titulo", panel.transform, "Mochila", 24, colorCrema);
        titulo.alignment = TextAnchor.MiddleCenter;
        titulo.gameObject.AddComponent<Shadow>().effectColor = colorMarron;
        AyudaUI.Colocar(titulo.rectTransform, new Vector2(0.5f, 1f), new Vector2(0f, -12f), new Vector2(ancho - 40f, altoTitulo));

        // Los 25 huecos
        int total = Inventario.FILAS * Inventario.COLUMNAS;
        fondos = new Image[total];
        iconos = new Image[total];
        cantidades = new Text[total];

        for (int fila = 0; fila < Inventario.FILAS; fila++)
        {
            for (int columna = 0; columna < Inventario.COLUMNAS; columna++)
            {
                int i = fila * Inventario.COLUMNAS + columna; // pasar de (fila, columna) a posición del array

                float x = margen + columna * (tamRanura + separacion);
                float y = -(margen + altoTitulo + fila * (tamRanura + separacion)); // hacia abajo es negativo

                Image fondo = AyudaUI.CrearImagen("Hueco" + i, panel.transform, spriteRanura, 3);
                AyudaUI.Colocar(fondo.rectTransform, new Vector2(0f, 1f), new Vector2(x, y), new Vector2(tamRanura, tamRanura));
                fondos[i] = fondo;

                Image icono = AyudaUI.CrearImagen("Icono", fondo.transform, null, 0);
                AyudaUI.Colocar(icono.rectTransform, new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(48f, 48f));
                icono.enabled = false;
                iconos[i] = icono;

                Text cantidad = AyudaUI.CrearTexto("Cantidad", fondo.transform, "", 16, Color.white);
                cantidad.alignment = TextAnchor.LowerRight;
                cantidad.gameObject.AddComponent<Outline>().effectColor = colorMarron;
                AyudaUI.Colocar(cantidad.rectTransform, new Vector2(1f, 0f), new Vector2(-5f, 3f), new Vector2(50f, 22f));
                cantidades[i] = cantidad;
            }
        }
    }
}
