using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

// Una puerta de la tienda. Si el jugador está delante y pulsa E, pasa al otro lado.
// Se usa dos veces: la puerta de fuera (para entrar) y la de dentro (para salir).
public class Puerta : MonoBehaviour
{
    public Vector3 destino;           // dónde aparece el jugador al cruzar
    public bool entrar = true;        // true = entra en la tienda, false = sale
    public Vector3 centroSala;        // dentro de la tienda la cámara se queda quieta aquí
    public string textoAviso = "E: entrar";

    // static: las dos puertas comparten estos datos (cómo estaba la cámara fuera)
    private static float minX, maxX, minY, maxY;
    private static Color colorFondo;

    private Transform jugador;
    private bool jugadorCerca = false;
    private GameObject aviso;

    void Start()
    {
        jugador = GameObject.FindWithTag("Player").transform;

        // Cartel "E: entrar" abajo en la pantalla (empieza escondido)
        Color crema = new Color32(255, 243, 214, 255);
        Transform canvas = AyudaUI.CrearCanvas("CanvasPuerta", transform, 15);
        Image fondo = AyudaUI.CrearImagen("Aviso", canvas, Resources.Load<Sprite>("UI/panel_liso"), 3);
        AyudaUI.Colocar(fondo.rectTransform, new Vector2(0.5f, 0f), new Vector2(0f, 120f), new Vector2(220f, 50f));
        Text texto = AyudaUI.CrearTexto("Texto", fondo.transform, textoAviso, 18, crema);
        texto.alignment = TextAnchor.MiddleCenter;
        AyudaUI.Colocar(texto.rectTransform, new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(220f, 50f));
        aviso = fondo.gameObject;
        aviso.SetActive(false);
    }

    void Update()
    {
        aviso.SetActive(jugadorCerca);

        if (jugadorCerca && Keyboard.current.eKey.wasPressedThisFrame)
        {
            Cruzar();
        }
    }

    void Cruzar()
    {
        jugadorCerca = false;
        jugador.position = destino;   // movemos al jugador al otro lado

        Camera camara = Camera.main;
        SeguirCamara seguir = camara.GetComponent<SeguirCamara>();

        if (entrar)
        {
            // Guardamos cómo era la cámara fuera, para dejarla igual al salir
            minX = seguir.minX;
            maxX = seguir.maxX;
            minY = seguir.minY;
            maxY = seguir.maxY;
            colorFondo = camara.backgroundColor;

            // Dentro: si el mínimo y el máximo son iguales, la cámara no se puede mover del centro de la sala
            seguir.minX = centroSala.x;
            seguir.maxX = centroSala.x;
            seguir.minY = centroSala.y;
            seguir.maxY = centroSala.y;
            camara.backgroundColor = Color.black; // lo que hay alrededor de la sala se ve negro
        }
        else
        {
            // Fuera: devolvemos los límites de antes
            seguir.minX = minX;
            seguir.maxX = maxX;
            seguir.minY = minY;
            seguir.maxY = maxY;
            camara.backgroundColor = colorFondo;
        }

        // Ponemos la cámara en su sitio de golpe (si no, se iría deslizando por todo el mapa)
        float x = Mathf.Clamp(destino.x, seguir.minX, seguir.maxX);
        float y = Mathf.Clamp(destino.y, seguir.minY, seguir.maxY);
        camara.transform.position = new Vector3(x, y, -10f);
    }

    void OnTriggerEnter2D(Collider2D otro)
    {
        if (otro.CompareTag("Player")) jugadorCerca = true;
    }

    void OnTriggerExit2D(Collider2D otro)
    {
        if (otro.CompareTag("Player")) jugadorCerca = false;
    }
}
