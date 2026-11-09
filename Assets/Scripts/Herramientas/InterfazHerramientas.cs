using UnityEngine;
using UnityEngine.UI;

// Dibuja la barra de herramientas (como la de Stardew Valley) en la parte de abajo de la pantalla.
// Toda la interfaz se crea por código en Start() (con ayuda de AyudaUI), así no hace falta montarla a mano en la escena.
public class InterfazHerramientas : MonoBehaviour
{
    public InventarioHerramientas inventario;

    [Header("Tamaños (en píxeles de pantalla)")]
    public float tamRanura = 72f;
    public float separacion = 8f;
    public float margen = 16f;

    // Colores sacados de la paleta del mapa (madera y crema)
    private Color colorMarron = new Color32(59, 36, 22, 255);
    private Color colorCrema = new Color32(255, 243, 214, 255);

    private Sprite spritePanel;
    private Sprite spriteRanura;
    private Sprite spriteRanuraSeleccionada;

    private Image[] ranuras;
    private RectTransform[] iconos;
    private Text textoSemillas;
    private CanvasGroup cartel;
    private Text textoCartel;

    private int ultimaSeleccion = -1;
    private float tiempoCartel = 0f;

    void Start()
    {
        spritePanel = Resources.Load<Sprite>("UI/panel_madera");
        spriteRanura = Resources.Load<Sprite>("UI/ranura");
        spriteRanuraSeleccionada = Resources.Load<Sprite>("UI/ranura_seleccionada");

        Transform canvas = AyudaUI.CrearCanvas("CanvasHerramientas", transform, 10); // por encima del Canvas que ya había
        RectTransform barra = CrearBarra(canvas);
        CrearCartel(canvas, barra);
    }

    void Update()
    {
        int sel = inventario.seleccionada;

        // 1. Si ha cambiado la herramienta, cambiamos los marcos y enseñamos el cartel con el nombre
        if (sel != ultimaSeleccion)
        {
            for (int i = 0; i < ranuras.Length; i++)
            {
                if (i == sel) ranuras[i].sprite = spriteRanuraSeleccionada;
                else ranuras[i].sprite = spriteRanura;
            }
            textoCartel.text = inventario.HerramientaActual().nombre;
            tiempoCartel = 2f;
            ultimaSeleccion = sel;
        }

        // 2. Animación: la casilla elegida sube un poco y su icono crece y "flota"
        for (int i = 0; i < ranuras.Length; i++)
        {
            RectTransform ranura = ranuras[i].rectTransform;
            float alturaObjetivo = (i == sel) ? margen + 6f : margen;
            Vector2 pos = ranura.anchoredPosition;
            pos.y = Mathf.Lerp(pos.y, alturaObjetivo, 15f * Time.deltaTime);
            ranura.anchoredPosition = pos;

            float escalaObjetivo = (i == sel) ? 1.15f : 1f;
            float escala = Mathf.Lerp(iconos[i].localScale.x, escalaObjetivo, 15f * Time.deltaTime);
            iconos[i].localScale = new Vector3(escala, escala, 1f);

            float flotar = (i == sel) ? Mathf.Sin(Time.time * 4f) * 2f : 0f;
            iconos[i].anchoredPosition = new Vector2(0f, flotar);
        }

        // 3. Número de semillas que quedan
        textoSemillas.text = "x" + ControladorJuego.semillas;

        // 4. El cartel se ve 2 segundos y en el último medio segundo se va desvaneciendo
        tiempoCartel -= Time.deltaTime;
        cartel.alpha = Mathf.Clamp01(tiempoCartel / 0.5f);
    }

    // ---------------- Construcción de la interfaz ----------------

    RectTransform CrearBarra(Transform canvas)
    {
        int cantidad = inventario.herramientas.Count;
        float ancho = cantidad * tamRanura + (cantidad - 1) * separacion + margen * 2;
        float alto = tamRanura + margen * 2;

        // Fondo de madera, anclado abajo en el centro
        Image fondo = AyudaUI.CrearImagen("BarraHerramientas", canvas, spritePanel, 4);
        RectTransform rtFondo = fondo.rectTransform;
        rtFondo.anchorMin = new Vector2(0.5f, 0f);
        rtFondo.anchorMax = new Vector2(0.5f, 0f);
        rtFondo.pivot = new Vector2(0.5f, 0f);
        rtFondo.anchoredPosition = new Vector2(0f, 12f);
        rtFondo.sizeDelta = new Vector2(ancho, alto);

        ranuras = new Image[cantidad];
        iconos = new RectTransform[cantidad];

        for (int i = 0; i < cantidad; i++)
        {
            Herramienta herramienta = inventario.herramientas[i];

            // Ranura (el hueco de color crema)
            Image ranura = AyudaUI.CrearImagen("Ranura" + (i + 1), rtFondo, spriteRanura, 4);
            RectTransform rt = ranura.rectTransform;
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.zero;
            rt.pivot = Vector2.zero;
            rt.anchoredPosition = new Vector2(margen + i * (tamRanura + separacion), margen);
            rt.sizeDelta = new Vector2(tamRanura, tamRanura);
            ranuras[i] = ranura;

            // Icono de la herramienta (16x16 píxeles dibujado a 3x = 48)
            Image icono = AyudaUI.CrearImagen("Icono", rt, herramienta.icono, 0);
            icono.preserveAspect = true;
            RectTransform rtIcono = icono.rectTransform;
            rtIcono.anchorMin = new Vector2(0.5f, 0.5f);
            rtIcono.anchorMax = new Vector2(0.5f, 0.5f);
            rtIcono.pivot = new Vector2(0.5f, 0.5f);
            rtIcono.sizeDelta = new Vector2(48f, 48f);
            iconos[i] = rtIcono;

            // Número de la tecla en la esquina de arriba a la izquierda
            Text numero = AyudaUI.CrearTexto("Tecla", rt, (i + 1).ToString(), 16, colorMarron);
            RectTransform rtNumero = numero.rectTransform;
            rtNumero.anchorMin = new Vector2(0f, 1f);
            rtNumero.anchorMax = new Vector2(0f, 1f);
            rtNumero.pivot = new Vector2(0f, 1f);
            rtNumero.anchoredPosition = new Vector2(8f, -4f);
            rtNumero.sizeDelta = new Vector2(20f, 20f);

            // Contador de semillas en la esquina de abajo a la derecha
            if (herramienta.tipo == TipoHerramienta.Semillas)
            {
                textoSemillas = AyudaUI.CrearTexto("Cantidad", rt, "", 18, Color.white);
                textoSemillas.alignment = TextAnchor.LowerRight;
                textoSemillas.gameObject.AddComponent<Outline>().effectColor = colorMarron;
                RectTransform rtCantidad = textoSemillas.rectTransform;
                rtCantidad.anchorMin = new Vector2(1f, 0f);
                rtCantidad.anchorMax = new Vector2(1f, 0f);
                rtCantidad.pivot = new Vector2(1f, 0f);
                rtCantidad.anchoredPosition = new Vector2(-6f, 4f);
                rtCantidad.sizeDelta = new Vector2(50f, 24f);
            }
        }

        return rtFondo;
    }

    void CrearCartel(Transform canvas, RectTransform barra)
    {
        // Cartel de madera con el nombre de la herramienta, encima de la barra
        Image fondo = AyudaUI.CrearImagen("CartelNombre", canvas, spritePanel, 3);
        RectTransform rt = fondo.rectTransform;
        rt.anchorMin = new Vector2(0.5f, 0f);
        rt.anchorMax = new Vector2(0.5f, 0f);
        rt.pivot = new Vector2(0.5f, 0f);
        rt.anchoredPosition = new Vector2(0f, barra.anchoredPosition.y + barra.sizeDelta.y + 10f);
        rt.sizeDelta = new Vector2(240f, 50f);

        // Un CanvasGroup permite cambiar la transparencia del cartel y de todo lo que tiene dentro
        cartel = fondo.gameObject.AddComponent<CanvasGroup>();

        textoCartel = AyudaUI.CrearTexto("Nombre", rt, "", 22, colorCrema);
        textoCartel.alignment = TextAnchor.MiddleCenter;
        textoCartel.gameObject.AddComponent<Shadow>().effectColor = colorMarron;
        RectTransform rtTexto = textoCartel.rectTransform;
        rtTexto.anchorMin = Vector2.zero;
        rtTexto.anchorMax = Vector2.one;
        rtTexto.offsetMin = Vector2.zero;
        rtTexto.offsetMax = Vector2.zero;
    }
}
