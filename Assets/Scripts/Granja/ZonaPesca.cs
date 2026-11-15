using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

// Sistema de pesca. Se añade al objeto "Agua" (el Tilemap del lago).
//  1. El jugador se pone junto al agua mirando hacia ella -> sale el aviso "E: pescar".
//  2. Pulsa E -> lanza la caña y espera un tiempo al azar (el jugador se queda quieto).
//  3. Pica un pez -> sale "¡PICA!" y hay poco tiempo para pulsar E otra vez.
//  4. Si llega a tiempo el pez sale disparado hacia el jugador y se guarda en la mochila
//     (se reutiliza ObjetoRecogible, igual que al romper rocas o cosechar).
// No usa colliders: mira si la casilla que hay delante del jugador es agua.
[RequireComponent(typeof(Tilemap))]
public class ZonaPesca : MonoBehaviour
{
    public string nombrePez = "Pez";        // debe coincidir con Resources/Objetos/pez.png
    public float alcance = 1f;              // a qué distancia delante del jugador se busca el agua
    public float esperaMinima = 2f;         // segundos mínimos hasta que pica
    public float esperaMaxima = 5f;         // segundos máximos hasta que pica
    public float tiempoParaPulsar = 1f;     // margen que tienes para pulsar E cuando pica

    private Tilemap agua;
    private bool jugadorCerca = false;      // true si el jugador mira al agua
    private bool pescando = false;          // true mientras dura todo el proceso de pescar
    private PlayerMovement jugador;

    private GameObject panelAviso;
    private Text textoAviso;

    void Start()
    {
        agua = GetComponent<Tilemap>();
        jugador = FindFirstObjectByType<PlayerMovement>();
        CrearInterfaz();
    }

    void Update()
    {
        if (Time.timeScale == 0f) return; // el juego está en pausa

        // Si estamos pescando, la corrutina se encarga de todo
        if (pescando || jugador == null) return;

        // ¿La casilla que hay delante del jugador es agua?
        Vector3 delante = jugador.transform.position + (Vector3)(jugador.direccion * alcance);
        jugadorCerca = agua.HasTile(agua.WorldToCell(delante));
        panelAviso.SetActive(jugadorCerca);

        if (jugadorCerca && Keyboard.current.eKey.wasPressedThisFrame)
        {
            StartCoroutine(Pescar());
        }
    }

    IEnumerator Pescar()
    {
        pescando = true;
        jugador.bloqueado = true; // el jugador no se puede mover mientras pesca

        // 1. Esperar a que pique
        MostrarAviso("Pescando...");
        yield return new WaitForSeconds(Random.Range(esperaMinima, esperaMaxima));

        // 2. Ha picado: hay que pulsar E antes de que se acabe el tiempo
        MostrarAviso("¡PICA! ¡Pulsa E!");
        bool pescado = false;
        float t = 0f;
        // Se espera un frame para no contar la pulsación de E que lanzó la caña
        yield return null;
        while (t < tiempoParaPulsar)
        {
            if (Keyboard.current.eKey.wasPressedThisFrame)
            {
                pescado = true;
                break;
            }
            t += Time.deltaTime;
            yield return null;
        }

        // 3. Resultado
        if (pescado)
        {
            // El pez salta desde el jugador y vuela a la mochila ("+1 Pez")
            Vector3 posicion = jugador.transform.position + (Vector3)(jugador.direccion * 0.5f);
            ObjetoRecogible.Soltar(nombrePez, 1, posicion);
            MostrarAviso("¡Has pescado un pez!");
        }
        else
        {
            MostrarAviso("Se ha escapado...");
        }

        yield return new WaitForSeconds(1.2f);

        // 4. Volvemos a la normalidad
        jugador.bloqueado = false;
        pescando = false;
        textoAviso.text = "E: pescar";
    }

    void MostrarAviso(string mensaje)
    {
        textoAviso.text = mensaje;
        panelAviso.SetActive(true);
    }

    // Crea el cartel de aviso por código (mismo estilo que el del vendedor)
    void CrearInterfaz()
    {
        Color crema = new Color32(255, 243, 214, 255);

        Transform canvas = AyudaUI.CrearCanvas("CanvasPesca", transform, 15);

        Image fondo = AyudaUI.CrearImagen("Aviso", canvas, Resources.Load<Sprite>("UI/panel_liso"), 3);
        AyudaUI.Colocar(fondo.rectTransform, new Vector2(0.5f, 0f), new Vector2(0f, 120f), new Vector2(260f, 50f));
        textoAviso = AyudaUI.CrearTexto("Texto", fondo.transform, "E: pescar", 18, crema);
        textoAviso.alignment = TextAnchor.MiddleCenter;
        AyudaUI.Colocar(textoAviso.rectTransform, new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(260f, 50f));

        panelAviso = fondo.gameObject;
        panelAviso.SetActive(false);
    }
}
