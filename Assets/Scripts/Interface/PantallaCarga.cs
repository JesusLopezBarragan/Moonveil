using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Pantalla de carga que sale al pulsar JUGAR en el menú.
// Tiene un fondo, una barra de carga y un panel con consejos de los controles.
public class PantallaCarga : MonoBehaviour
{
    public string escena = "Granja";
    public float tiempoCarga = 5f;        // segundos que tarda en llenarse la barra
    public float tiempoPorConsejo = 3f;   // segundos que se ve cada consejo

    private string[] consejos =
    {
        "W A S D o las flechas:\nmover al personaje",
        "Teclas 1, 2, 3, 4 y 5:\nelegir herramienta",
        "Rueda del ratón:\ncambiar de herramienta",
        "Espacio o clic izquierdo:\nusar la herramienta",
        "Tecla E:\nrecoger una zanahoria madura",
        "Tecla I o Tab:\nabrir y cerrar la mochila",
        "Tecla P:\nabrir la ayuda con los controles"
    };

    private Image relleno;        // la parte verde de la barra
    private Text textoBarra;      // "Cargando... 50%"
    private Text textoConsejo;    // el consejo que se ve ahora

    private int consejoActual = 0;
    private float contadorConsejo = 0f;

    // Se llama desde el botón JUGAR del menú
    public static void Empezar(string escena)
    {
        GameObject obj = new GameObject("PantallaCarga");
        PantallaCarga pantalla = obj.AddComponent<PantallaCarga>();
        pantalla.escena = escena;
    }

    void Start()
    {
        CrearInterfaz();
        textoConsejo.text = consejos[0];
        StartCoroutine(Cargar());
    }

    void Update()
    {
        // Cada 3 segundos pasamos al siguiente consejo
        contadorConsejo += Time.deltaTime;
        if (contadorConsejo >= tiempoPorConsejo)
        {
            contadorConsejo = 0f;
            consejoActual++;
            if (consejoActual >= consejos.Length) consejoActual = 0; // vuelve al primero
            textoConsejo.text = consejos[consejoActual];
        }
    }

    IEnumerator Cargar()
    {
        // Empezamos a cargar la escena por detrás, pero sin enseñarla todavía
        AsyncOperation carga = SceneManager.LoadSceneAsync(escena);
        carga.allowSceneActivation = false;

        // Llenamos la barra poco a poco
        float tiempo = 0f;
        while (tiempo < tiempoCarga)
        {
            tiempo += Time.deltaTime;
            float progreso = tiempo / tiempoCarga;   // va de 0 a 1
            relleno.fillAmount = progreso;
            textoBarra.text = "Cargando... " + (int)(progreso * 100) + "%";
            yield return null; // esperar al siguiente frame
        }

        // Esperamos a que el jugador pulse una tecla
        textoBarra.text = "Pulsa cualquier tecla para empezar";
        while (!Keyboard.current.anyKey.wasPressedThisFrame)
        {
            yield return null;
        }

        // Enseñamos la escena del juego (esta pantalla se borra sola al cambiar de escena)
        carga.allowSceneActivation = true;
    }

    // ---------------- Crear la interfaz ----------------

    void CrearInterfaz()
    {
        Color marron = new Color32(59, 36, 22, 255);
        Color crema = new Color32(255, 243, 214, 255);

        Transform canvas = AyudaUI.CrearCanvas("CanvasCarga", transform, 100);

        // 1. Fondo que ocupa toda la pantalla
        Image fondo = AyudaUI.CrearImagen("Fondo", canvas, Resources.Load<Sprite>("Carga/fondo_carga"), 0);
        fondo.rectTransform.anchorMin = Vector2.zero;
        fondo.rectTransform.anchorMax = Vector2.one;
        fondo.rectTransform.offsetMin = Vector2.zero;
        fondo.rectTransform.offsetMax = Vector2.zero;

        // 2. Panel de consejos a la derecha
        Image panel = AyudaUI.CrearImagen("PanelConsejos", canvas, Resources.Load<Sprite>("UI/panel_liso"), 4);
        AyudaUI.Colocar(panel.rectTransform, new Vector2(1f, 0.5f), new Vector2(-40f, 0f), new Vector2(360f, 200f));

        Text titulo = AyudaUI.CrearTexto("Titulo", panel.transform, "Consejo", 28, crema);
        titulo.alignment = TextAnchor.MiddleCenter;
        AyudaUI.Colocar(titulo.rectTransform, new Vector2(0.5f, 1f), new Vector2(0f, -20f), new Vector2(300f, 40f));

        textoConsejo = AyudaUI.CrearTexto("Consejo", panel.transform, "", 22, crema);
        textoConsejo.alignment = TextAnchor.MiddleCenter;
        AyudaUI.Colocar(textoConsejo.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0f, -15f), new Vector2(300f, 100f));

        // 3. Barra de carga abajo: un marco de madera, un fondo oscuro y el relleno verde
        Image marco = AyudaUI.CrearImagen("Barra", canvas, Resources.Load<Sprite>("UI/panel_madera"), 3);
        AyudaUI.Colocar(marco.rectTransform, new Vector2(0.5f, 0f), new Vector2(0f, 40f), new Vector2(760f, 60f));

        Image interior = AyudaUI.CrearImagen("Interior", marco.transform, null, 0);
        interior.color = marron;
        AyudaUI.Colocar(interior.rectTransform, new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(732f, 32f));

        relleno = AyudaUI.CrearImagen("Relleno", interior.transform, Resources.Load<Sprite>("Carga/barra_relleno"), 0);
        relleno.type = Image.Type.Filled;                    // se recorta según fillAmount
        relleno.fillMethod = Image.FillMethod.Horizontal;    // de izquierda a derecha
        relleno.fillAmount = 0f;
        AyudaUI.Colocar(relleno.rectTransform, new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(732f, 32f));

        textoBarra = AyudaUI.CrearTexto("Texto", marco.transform, "Cargando... 0%", 18, Color.white);
        textoBarra.alignment = TextAnchor.MiddleCenter;
        AyudaUI.Colocar(textoBarra.rectTransform, new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(760f, 60f));
    }
}
