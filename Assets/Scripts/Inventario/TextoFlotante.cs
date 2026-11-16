using UnityEngine;

// Un texto que aparece en el mapa (por ejemplo "+3 Madera"), sube un poco y desaparece.
public class TextoFlotante : MonoBehaviour
{
    private TextMesh texto;
    private TextMesh sombra;
    private float tiempo = 0f;
    private float duracion = 1f;

    public static void Crear(string contenido, Vector3 posicion)
    {
        GameObject obj = new GameObject("TextoFlotante");
        obj.transform.position = posicion;
        TextoFlotante flotante = obj.AddComponent<TextoFlotante>();

        // Primero la sombra (marrón, un poco desplazada) y encima el texto blanco
        flotante.sombra = CrearTexto(obj.transform, contenido, new Color32(59, 36, 22, 255), new Vector3(0.03f, -0.03f, 0f), 99);
        flotante.texto = CrearTexto(obj.transform, contenido, Color.white, Vector3.zero, 100);
    }

    // TextMesh es un texto que se dibuja en el mundo (no en el Canvas)
    static TextMesh CrearTexto(Transform padre, string contenido, Color color, Vector3 desplazamiento, int orden)
    {
        GameObject obj = new GameObject("Texto");
        obj.transform.SetParent(padre, false);
        obj.transform.localPosition = desplazamiento;

        Font fuente = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        TextMesh tm = obj.AddComponent<TextMesh>();
        tm.font = fuente;
        tm.text = contenido;
        tm.fontSize = 64;
        tm.characterSize = 0.05f;
        tm.fontStyle = FontStyle.Bold;
        tm.anchor = TextAnchor.MiddleCenter;
        tm.color = color;

        MeshRenderer mr = obj.GetComponent<MeshRenderer>();
        mr.material = fuente.material;
        mr.sortingOrder = orden; // encima de todo el mapa
        return tm;
    }

    void Update()
    {
        tiempo += Time.deltaTime;

        // Sube poco a poco
        transform.position += new Vector3(0f, 0.8f * Time.deltaTime, 0f);

        // Se va haciendo transparente
        float alfa = 1f - tiempo / duracion;
        Color c = texto.color;
        c.a = alfa;
        texto.color = c;
        Color cs = sombra.color;
        cs.a = alfa;
        sombra.color = cs;

        if (tiempo >= duracion) Destroy(gameObject);
    }
}
