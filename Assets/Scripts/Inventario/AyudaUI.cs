using UnityEngine;
using UnityEngine.UI;

// Métodos para crear interfaz por código sin repetir siempre lo mismo.
// Los usan la barra de herramientas y la mochila.
public static class AyudaUI
{
    private static Font fuente;

    public static Transform CrearCanvas(string nombre, Transform padre, int orden)
    {
        GameObject obj = new GameObject(nombre);
        obj.transform.SetParent(padre);

        Canvas canvas = obj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = orden; // los Canvas con número más alto se dibujan encima

        // Escala la interfaz según el tamaño de la ventana (igual que el Canvas de la escena)
        CanvasScaler escalador = obj.AddComponent<CanvasScaler>();
        escalador.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        escalador.referenceResolution = new Vector2(1280, 720);
        escalador.matchWidthOrHeight = 0.5f;

        return obj.transform;
    }

    // escalaPixel: cuántas veces se agranda cada píxel del borde (0 = imagen normal, sin 9-slice)
    public static Image CrearImagen(string nombre, Transform padre, Sprite sprite, int escalaPixel)
    {
        GameObject obj = new GameObject(nombre);
        obj.transform.SetParent(padre, false);
        Image imagen = obj.AddComponent<Image>();
        imagen.sprite = sprite;
        imagen.raycastTarget = false;

        if (escalaPixel > 0)
        {
            // "Sliced" (9-slice): las esquinas no se estiran, solo el centro. Así la madera no se deforma.
            imagen.type = Image.Type.Sliced;
            // Los sprites son de 16 píxeles por unidad y el Canvas usa 100, de ahí esta cuenta
            imagen.pixelsPerUnitMultiplier = 100f / (16f * escalaPixel);
        }
        return imagen;
    }

    public static Text CrearTexto(string nombre, Transform padre, string contenido, int tamano, Color color)
    {
        if (fuente == null) fuente = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        GameObject obj = new GameObject(nombre);
        obj.transform.SetParent(padre, false);
        Text texto = obj.AddComponent<Text>();
        texto.font = fuente;
        texto.text = contenido;
        texto.fontSize = tamano;
        texto.fontStyle = FontStyle.Bold;
        texto.color = color;
        texto.raycastTarget = false;
        return texto;
    }

    // Coloca un elemento: "ancla" es el punto del padre al que se engancha
    // (0,0 = abajo izquierda, 0.5,0.5 = centro, 1,1 = arriba derecha)
    public static void Colocar(RectTransform rt, Vector2 ancla, Vector2 posicion, Vector2 tamano)
    {
        rt.anchorMin = ancla;
        rt.anchorMax = ancla;
        rt.pivot = ancla;
        rt.anchoredPosition = posicion;
        rt.sizeDelta = tamano;
    }
}
