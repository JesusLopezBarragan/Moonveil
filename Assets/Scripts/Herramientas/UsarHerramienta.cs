using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

// Hace que el jugador use la herramienta seleccionada sobre la casilla que tiene delante.
// Se usa con ESPACIO o con el CLIC IZQUIERDO del ratón.
public class UsarHerramienta : MonoBehaviour
{
    [Header("Mapa")]
    public Tilemap suelo;            // el tilemap "Suelo" (césped, caminos, tierra...)
    public TileBase tileTierra;      // el tile de tierra arada

    [Header("Ajustes")]
    public float tiempoUso = 0.3f;       // lo que dura la animación de usar la herramienta
    public float duracionRiego = 30f;    // segundos que la tierra se queda mojada
    public Color colorMojado = new Color(0.65f, 0.55f, 0.5f);

    private InventarioHerramientas inventario;
    private MovimientoJugador movimiento;
    private Collider2D colliderJugador;
    private SpriteRenderer srJugador;

    private SpriteRenderer selector;          // cuadrado que marca la casilla objetivo
    private SpriteRenderer herramientaEnMano; // dibujo de la herramienta al usarla
    private bool usando = false;

    // Casillas mojadas y los segundos que les quedan para secarse
    private Dictionary<Vector3Int, float> casillasMojadas = new Dictionary<Vector3Int, float>();

    void Start()
    {
        inventario = GetComponent<InventarioHerramientas>();
        movimiento = GetComponent<MovimientoJugador>();
        colliderJugador = GetComponent<Collider2D>();
        srJugador = GetComponent<SpriteRenderer>();

        // Creamos el selector de casilla
        GameObject objSelector = new GameObject("SelectorCasilla");
        selector = objSelector.AddComponent<SpriteRenderer>();
        selector.sprite = Resources.Load<Sprite>("UI/selector_casilla");
        selector.sortingOrder = -5; // encima del suelo pero debajo de los objetos

        // Creamos el objeto que enseña la herramienta en la mano (empieza escondido)
        GameObject objHerramienta = new GameObject("HerramientaEnMano");
        objHerramienta.transform.SetParent(transform);
        herramientaEnMano = objHerramienta.AddComponent<SpriteRenderer>();
        herramientaEnMano.enabled = false;
    }

    void Update()
    {
        Vector3Int casilla = CasillaObjetivo();

        // 1. Mover el selector a la casilla y pintarlo blanco (se puede) o rojo (no se puede)
        selector.transform.position = suelo.GetCellCenterWorld(casilla);
        if (PuedeUsar(casilla)) selector.color = new Color(1f, 1f, 1f, 0.9f);
        else selector.color = new Color(1f, 0.4f, 0.4f, 0.5f);

        // 2. Usar la herramienta al pulsar Espacio o el clic izquierdo
        bool pulsado = false;
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame) pulsado = true;
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame) pulsado = true;

        if (pulsado && !usando && !InterfazInventario.abierto) // con la mochila abierta no se usan herramientas
        {
            StartCoroutine(Usar(casilla));
        }

        // 3. Ir secando la tierra mojada
        Secar();
    }

    // La casilla que está justo delante del jugador, según hacia dónde mira
    Vector3Int CasillaObjetivo()
    {
        Vector3 pies = colliderJugador.bounds.center;
        Vector3Int casillaJugador = suelo.WorldToCell(pies);
        Vector2 dir = movimiento.direccion;
        return casillaJugador + new Vector3Int(Mathf.RoundToInt(dir.x), Mathf.RoundToInt(dir.y), 0);
    }

    // ¿Tendría efecto usar la herramienta actual en esta casilla?
    bool PuedeUsar(Vector3Int casilla)
    {
        switch (inventario.HerramientaActual().tipo)
        {
            case TipoHerramienta.Azada:
                return EsCesped(casilla) && !HayObstaculo(casilla);
            case TipoHerramienta.Regadera:
                return EsTierra(casilla) && !casillasMojadas.ContainsKey(casilla);
            case TipoHerramienta.Pico:
                return BuscarRompible(casilla, TipoHerramienta.Pico) != null;
            case TipoHerramienta.Hacha:
                return BuscarRompible(casilla, TipoHerramienta.Hacha) != null;
            case TipoHerramienta.Semillas:
                return EsTierra(casilla) && BuscarCultivo(casilla) == null && ControladorJuego.semillas > 0;
        }
        return false;
    }

    IEnumerator Usar(Vector3Int casilla)
    {
        usando = true;
        movimiento.bloqueado = true; // el jugador se queda quieto mientras trabaja

        Herramienta herramienta = inventario.HerramientaActual();
        bool sePuede = PuedeUsar(casilla);

        // Animación: la herramienta aparece en la mano y gira
        yield return StartCoroutine(Animar(herramienta));

        // Al terminar el golpe aplicamos el efecto (solo si se podía)
        if (sePuede)
        {
            Aplicar(herramienta.tipo, casilla);
        }

        movimiento.bloqueado = false;
        usando = false;
    }

    void Aplicar(TipoHerramienta tipo, Vector3Int casilla)
    {
        switch (tipo)
        {
            case TipoHerramienta.Azada:
                suelo.SetTile(casilla, tileTierra);
                break;

            case TipoHerramienta.Regadera:
                Mojar(casilla);
                Cultivo cultivo = BuscarCultivo(casilla);
                if (cultivo != null) cultivo.Regar(duracionRiego);
                break;

            case TipoHerramienta.Pico:
                BuscarRompible(casilla, TipoHerramienta.Pico).Golpear();
                break;

            case TipoHerramienta.Hacha:
                BuscarRompible(casilla, TipoHerramienta.Hacha).Golpear();
                break;

            case TipoHerramienta.Semillas:
                Plantar(casilla);
                break;
        }
    }

    // ---------------- Comprobaciones del mapa ----------------

    bool EsCesped(Vector3Int casilla)
    {
        TileBase tile = suelo.GetTile(casilla);
        return tile != null && tile.name.StartsWith("cesped");
    }

    bool EsTierra(Vector3Int casilla)
    {
        return suelo.GetTile(casilla) == tileTierra;
    }

    // ¿Hay algo encima de la casilla? (árboles, rocas, vallas, agua, flores...)
    bool HayObstaculo(Vector3Int casilla)
    {
        Vector2 centro = suelo.GetCellCenterWorld(casilla);
        Collider2D[] choques = Physics2D.OverlapBoxAll(centro, new Vector2(0.8f, 0.8f), 0f);
        foreach (Collider2D c in choques)
        {
            if (c != colliderJugador) return true;
        }
        return false;
    }

    ObjetoRompible BuscarRompible(Vector3Int casilla, TipoHerramienta tipo)
    {
        Vector2 centro = suelo.GetCellCenterWorld(casilla);
        Collider2D[] choques = Physics2D.OverlapBoxAll(centro, new Vector2(0.9f, 0.9f), 0f);
        foreach (Collider2D c in choques)
        {
            ObjetoRompible rompible = c.GetComponentInParent<ObjetoRompible>();
            if (rompible != null && rompible.herramientaNecesaria == tipo) return rompible;
        }
        return null;
    }

    Cultivo BuscarCultivo(Vector3Int casilla)
    {
        foreach (Cultivo c in Cultivo.todos)
        {
            if (suelo.WorldToCell(c.transform.position) == casilla) return c;
        }
        return null;
    }

    // ---------------- Acciones ----------------

    void Plantar(Vector3Int casilla)
    {
        // Copiamos una zanahoria que ya existe en el huerto (así tiene sus sprites y su collider)
        if (Cultivo.todos.Count == 0)
        {
            Debug.LogWarning("No hay ninguna zanahoria en la escena para usarla de plantilla");
            return;
        }
        Cultivo plantilla = Cultivo.todos[0];

        // Mantenemos la misma separación con el centro de la casilla que tiene la plantilla
        Vector3 centroPlantilla = suelo.GetCellCenterWorld(suelo.WorldToCell(plantilla.transform.position));
        Vector3 desfase = plantilla.transform.position - centroPlantilla;
        Vector3 posicion = suelo.GetCellCenterWorld(casilla) + desfase;

        Cultivo nuevo = Instantiate(plantilla, posicion, Quaternion.identity, plantilla.transform.parent);
        nuevo.name = "Zanahoria";
        nuevo.faseActual = 0; // empieza siendo una semilla

        ControladorJuego.semillas--;

        // Si la tierra ya estaba mojada, la semilla empieza regada
        if (casillasMojadas.ContainsKey(casilla))
        {
            nuevo.Regar(casillasMojadas[casilla]);
        }
    }

    void Mojar(Vector3Int casilla)
    {
        casillasMojadas[casilla] = duracionRiego;
        suelo.SetTileFlags(casilla, TileFlags.None); // sin esto el tilemap no deja cambiar el color
        suelo.SetColor(casilla, colorMojado);
    }

    void Secar()
    {
        // Recorremos una copia de las claves porque vamos a borrar elementos del diccionario
        List<Vector3Int> casillas = new List<Vector3Int>(casillasMojadas.Keys);
        foreach (Vector3Int casilla in casillas)
        {
            casillasMojadas[casilla] -= Time.deltaTime;
            if (casillasMojadas[casilla] <= 0f)
            {
                casillasMojadas.Remove(casilla);
                suelo.SetColor(casilla, Color.white); // vuelve a su color normal
            }
        }
    }

    // ---------------- Animación ----------------

    IEnumerator Animar(Herramienta herramienta)
    {
        Vector2 dir = movimiento.direccion;
        bool izquierda = dir.x < 0;

        herramientaEnMano.sprite = herramienta.icono;
        herramientaEnMano.flipX = izquierda;
        herramientaEnMano.enabled = true;

        // Si mira hacia arriba la herramienta va detrás del jugador; si no, delante
        if (dir.y > 0) herramientaEnMano.sortingOrder = srJugador.sortingOrder - 1;
        else herramientaEnMano.sortingOrder = srJugador.sortingOrder + 1;

        // Ángulos de inicio y fin del giro (en grados)
        float anguloInicio = 45f;
        float anguloFin = -90f;
        if (herramienta.tipo == TipoHerramienta.Regadera)
        {
            anguloInicio = 0f;   // la regadera no se balancea, se inclina
            anguloFin = -35f;
        }
        else if (herramienta.tipo == TipoHerramienta.Semillas)
        {
            anguloInicio = 0f;   // las semillas tampoco giran
            anguloFin = 0f;
        }
        if (izquierda)
        {
            anguloInicio = -anguloInicio;
            anguloFin = -anguloFin;
        }

        float lado = izquierda ? -0.25f : 0.25f;
        Vector3 posicionMano = new Vector3(lado, 0.55f, 0f);

        float tiempo = 0f;
        while (tiempo < tiempoUso)
        {
            tiempo += Time.deltaTime;
            float t = tiempo / tiempoUso; // va de 0 a 1

            float angulo = Mathf.Lerp(anguloInicio, anguloFin, t);
            herramientaEnMano.transform.localRotation = Quaternion.Euler(0f, 0f, angulo);

            // Un pequeño salto hacia arriba (sirve para que las semillas "se lancen")
            float salto = Mathf.Sin(t * Mathf.PI) * 0.15f;
            herramientaEnMano.transform.localPosition = posicionMano + new Vector3(0f, salto, 0f);

            yield return null;
        }

        herramientaEnMano.enabled = false;
    }
}
