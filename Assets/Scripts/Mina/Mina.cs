using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

// La mina por dentro. Cada nivel tiene rocas colocadas al azar que se rompen con el pico.
// Una de las rocas esconde la escalera para bajar al siguiente nivel.
// Cuanto más abajo, más rocas y mejores minerales (cobre, hierro y oro).
public class Mina : MonoBehaviour
{
    public Vector3 centroSala;        // centro de la sala de la mina
    public SpriteRenderer suelo;      // el dibujo de la sala (lo oscurecemos en cada nivel)
    public int nivel = 1;

    private Vector3 esquina;          // esquina de abajo a la izquierda de la sala
    private Transform jugador;

    private List<GameObject> rocas = new List<GameObject>();
    private GameObject rocaConEscalera;  // la roca que tapa la escalera
    private GameObject escalera;         // el agujero para bajar

    private GameObject cartelNivel;
    private Text textoNivel;
    private GameObject aviso;

    void Start()
    {
        esquina = centroSala + new Vector3(-7.5f, -5f, 0f);
        jugador = GameObject.FindWithTag("Player").transform;

        // La escalera para bajar: empieza escondida
        escalera = new GameObject("EscaleraBajar");
        SpriteRenderer sr = escalera.AddComponent<SpriteRenderer>();
        sr.sprite = Resources.Load<Sprite>("Mina/escalera");
        sr.sortingOrder = -5; // encima del suelo, debajo del jugador
        escalera.SetActive(false);

        CrearInterfaz();
        GenerarNivel();
    }

    void Update()
    {
        // El cartel con el nivel solo se ve si el jugador está dentro de la mina
        bool dentro = Vector3.Distance(jugador.position, centroSala) < 12f;
        cartelNivel.SetActive(dentro);

        // Si la roca que tapaba la escalera ya no existe (se ha roto), sale la escalera
        if (rocaConEscalera == null && !escalera.activeSelf)
        {
            escalera.SetActive(true);
        }

        // Si estás encima de la escalera y pulsas E, bajas un nivel
        bool encima = escalera.activeSelf && Vector3.Distance(jugador.position, escalera.transform.position) < 0.8f;
        aviso.SetActive(encima);
        if (encima && Keyboard.current.eKey.wasPressedThisFrame)
        {
            nivel++;
            GenerarNivel();
            jugador.position = esquina + new Vector3(7.5f, 6f, 0f); // apareces al pie de la escalera de subida
        }
    }

    void GenerarNivel()
    {
        // 1. Borramos las rocas que quedaban del nivel anterior
        foreach (GameObject roca in rocas)
        {
            if (roca != null) Destroy(roca);
        }
        rocas.Clear();
        escalera.SetActive(false);

        // 2. Ponemos rocas nuevas en casillas al azar (más rocas cuanto más hondo, como mucho 30)
        int cantidad = Mathf.Min(8 + nivel * 2, 30);

        List<Vector2Int> ocupadas = new List<Vector2Int>();
        // dejamos libre el sitio donde aparece el jugador
        ocupadas.Add(new Vector2Int(7, 6));
        ocupadas.Add(new Vector2Int(6, 6));
        ocupadas.Add(new Vector2Int(8, 6));
        ocupadas.Add(new Vector2Int(7, 5));

        while (rocas.Count < cantidad)
        {
            // la sala tiene casillas libres de x = 1 a 13 y de y = 1 a 6
            Vector2Int casilla = new Vector2Int(Random.Range(1, 14), Random.Range(1, 7));
            if (ocupadas.Contains(casilla)) continue; // ya hay algo ahí, probamos otra

            ocupadas.Add(casilla);
            rocas.Add(CrearRoca(casilla, ElegirMineral()));
        }

        // 3. Una de las rocas esconde la escalera
        rocaConEscalera = rocas[Random.Range(0, rocas.Count)];
        escalera.transform.position = rocaConEscalera.transform.position;

        // 4. Cuanto más hondo, más oscuro el suelo
        suelo.color = Color.Lerp(Color.white, new Color(0.45f, 0.45f, 0.55f), (nivel - 1) / 10f);
        textoNivel.text = "Mina - Nivel " + nivel;
    }

    // Elige qué tiene dentro una roca. Los minerales buenos solo salen en niveles hondos.
    string ElegirMineral()
    {
        int azar = Random.Range(0, 100); // número de 0 a 99

        if (nivel >= 5 && azar < 10) return "Oro";
        if (nivel >= 3 && azar < 25) return "Hierro";
        if (azar < 40) return "Cobre";
        return "Piedra";
    }

    GameObject CrearRoca(Vector2Int casilla, string mineral)
    {
        GameObject roca = new GameObject("Roca" + mineral);
        roca.transform.position = esquina + new Vector3(casilla.x + 0.5f, casilla.y + 0.5f, 0f); // centro de la casilla
        roca.AddComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Mina/roca_" + mineral.ToLower());
        roca.AddComponent<BoxCollider2D>().size = new Vector2(0.8f, 0.6f);

        // Reutilizamos ObjetoRompible: se rompe con el pico y da el mineral
        ObjetoRompible rompible = roca.AddComponent<ObjetoRompible>();
        rompible.herramientaNecesaria = TipoHerramienta.Pico;
        rompible.objetoQueDa = mineral;
        rompible.cantidadQueDa = 1;

        // Los minerales mejores son más duros
        if (mineral == "Piedra") rompible.golpes = 2;
        else if (mineral == "Cobre") rompible.golpes = 3;
        else if (mineral == "Hierro") rompible.golpes = 4;
        else rompible.golpes = 5;

        return roca;
    }

    void CrearInterfaz()
    {
        Color crema = new Color32(255, 243, 214, 255);
        Transform canvas = AyudaUI.CrearCanvas("CanvasMina", transform, 15);

        // Cartel arriba en el centro con el nivel
        Image fondo = AyudaUI.CrearImagen("CartelNivel", canvas, Resources.Load<Sprite>("UI/panel_liso"), 3);
        AyudaUI.Colocar(fondo.rectTransform, new Vector2(0.5f, 1f), new Vector2(0f, -15f), new Vector2(240f, 50f));
        textoNivel = AyudaUI.CrearTexto("Texto", fondo.transform, "", 20, crema);
        textoNivel.alignment = TextAnchor.MiddleCenter;
        AyudaUI.Colocar(textoNivel.rectTransform, new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(240f, 50f));
        cartelNivel = fondo.gameObject;

        // Aviso "E: bajar" abajo
        Image fondoAviso = AyudaUI.CrearImagen("Aviso", canvas, Resources.Load<Sprite>("UI/panel_liso"), 3);
        AyudaUI.Colocar(fondoAviso.rectTransform, new Vector2(0.5f, 0f), new Vector2(0f, 120f), new Vector2(240f, 50f));
        Text textoAviso = AyudaUI.CrearTexto("Texto", fondoAviso.transform, "E: bajar al siguiente nivel", 18, crema);
        textoAviso.alignment = TextAnchor.MiddleCenter;
        AyudaUI.Colocar(textoAviso.rectTransform, new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(240f, 50f));
        aviso = fondoAviso.gameObject;
        aviso.SetActive(false);
    }
}
