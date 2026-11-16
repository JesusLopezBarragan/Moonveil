using UnityEngine;
using UnityEngine.UI;

// Marcador de arriba a la izquierda con las zanahorias y las monedas.
// Cada fila tiene un icono, un nombre pequeño y el número grande.
// Cuando el número cambia, da un "salto" y se pone dorado un momento.
public class InterfazContadores : MonoBehaviour
{
    private Color crema = new Color32(255, 243, 214, 255);
    private Color marron = new Color32(59, 36, 22, 255);
    private Color dorado = new Color32(255, 214, 90, 255);

    private Text numeroZanahorias;
    private Text numeroMonedas;
    private int zanahoriasAntes = 0;
    private int monedasAntes = 0;
    private float saltoZanahorias = 0f;   // de 1 a 0: lo que queda de la animación
    private float saltoMonedas = 0f;

    void Start()
    {
        // Escondemos el texto antiguo de la escena ("Zanahorias: 0")
        GameObject textoViejo = GameObject.Find("TextoZanahorias");
        if (textoViejo != null) textoViejo.SetActive(false);

        Transform canvas = AyudaUI.CrearCanvas("CanvasContadores", transform, 10);

        Image panel = AyudaUI.CrearImagen("PanelContadores", canvas, Resources.Load<Sprite>("UI/panel_liso"), 3);
        AyudaUI.Colocar(panel.rectTransform, new Vector2(0f, 1f), new Vector2(16f, -16f), new Vector2(230f, 152f));

        numeroZanahorias = CrearFila(panel.transform, "Zanahorias", "Objetos/zanahoria", -18f);
        numeroMonedas = CrearFila(panel.transform, "Monedas", "Objetos/moneda", -78f);
    }

    void Update()
    {
        // Las zanahorias son las que hay en la mochila; las monedas, las de ControladorJuego
        int zanahorias = 0;
        if (Inventario.instancia != null) zanahorias = Inventario.instancia.Contar("Zanahoria");
        int monedas = ControladorJuego.monedas;

        // Si ha cambiado algún número, empieza su animación
        if (zanahorias != zanahoriasAntes) saltoZanahorias = 1f;
        if (monedas != monedasAntes) saltoMonedas = 1f;
        zanahoriasAntes = zanahorias;
        monedasAntes = monedas;

        numeroZanahorias.text = zanahorias.ToString();
        numeroMonedas.text = monedas.ToString();

        saltoZanahorias = Animar(numeroZanahorias, saltoZanahorias);
        saltoMonedas = Animar(numeroMonedas, saltoMonedas);
    }

    // Hace el salto: el número se agranda y se pone dorado, y poco a poco vuelve a la normalidad
    float Animar(Text numero, float salto)
    {
        salto -= Time.deltaTime * 3f;   // dura un tercio de segundo
        if (salto < 0f) salto = 0f;

        numero.transform.localScale = Vector3.one * (1f + salto * 0.5f);
        numero.color = Color.Lerp(Color.white, dorado, salto);
        return salto;
    }

    // Crea una fila: hueco con el icono + nombre pequeño + número grande. Devuelve el texto del número.
    Text CrearFila(Transform panel, string nombre, string icono, float y)
    {
        Image hueco = AyudaUI.CrearImagen("Hueco" + nombre, panel, Resources.Load<Sprite>("UI/ranura"), 3);
        AyudaUI.Colocar(hueco.rectTransform, new Vector2(0f, 1f), new Vector2(18f, y), new Vector2(56f, 56f));

        Image imagen = AyudaUI.CrearImagen("Icono", hueco.transform, Resources.Load<Sprite>(icono), 0);
        AyudaUI.Colocar(imagen.rectTransform, new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(40f, 40f));

        Text etiqueta = AyudaUI.CrearTexto("Nombre", panel, nombre, 14, crema);
        AyudaUI.Colocar(etiqueta.rectTransform, new Vector2(0f, 1f), new Vector2(86f, y - 2f), new Vector2(130f, 20f));

        Text numero = AyudaUI.CrearTexto("Numero", panel, "0", 30, Color.white);
        numero.alignment = TextAnchor.MiddleLeft;
        numero.gameObject.AddComponent<Outline>().effectColor = marron;
        // El pivote a la izquierda hace que el número crezca hacia la derecha al saltar
        AyudaUI.Colocar(numero.rectTransform, new Vector2(0f, 1f), new Vector2(86f, y - 20f), new Vector2(130f, 36f));
        numero.rectTransform.pivot = new Vector2(0f, 0.5f);
        numero.rectTransform.anchoredPosition = new Vector2(86f, y - 38f);
        return numero;
    }
}
