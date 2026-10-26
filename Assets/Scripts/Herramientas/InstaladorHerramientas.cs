using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

// Monta el sistema de herramientas automáticamente cuando se carga la escena "Granja".
// Así no hay que tocar la escena a mano: al darle a Play ya está todo funcionando.
public static class InstaladorHerramientas
{
    // [RuntimeInitializeOnLoadMethod] hace que Unity llame a este método solo al arrancar el juego
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Registrar()
    {
        SceneManager.sceneLoaded -= AlCargarEscena; // por si acaso, para no apuntarnos dos veces
        SceneManager.sceneLoaded += AlCargarEscena;
    }

    static void AlCargarEscena(Scene escena, LoadSceneMode modo)
    {
        if (escena.name == "Granja")
        {
            Montar(escena);
        }
    }

    static void Montar(Scene escena)
    {
        GameObject jugador = GameObject.FindWithTag("Player");
        GameObject objSuelo = GameObject.Find("Suelo");
        if (jugador == null || objSuelo == null)
        {
            Debug.LogWarning("InstaladorHerramientas: no encuentro al Jugador o el tilemap Suelo");
            return;
        }

        // Si alguien ya ha puesto los scripts a mano en la escena, no hacemos nada
        if (jugador.GetComponent<InventarioHerramientas>() != null) return;

        Tilemap suelo = objSuelo.GetComponent<Tilemap>();

        // 1. Las herramientas del jugador (los iconos están en Assets/Resources/Herramientas)
        InventarioHerramientas inventario = jugador.AddComponent<InventarioHerramientas>();
        inventario.herramientas.Add(new Herramienta("Azada", TipoHerramienta.Azada, Resources.Load<Sprite>("Herramientas/azada")));
        inventario.herramientas.Add(new Herramienta("Regadera", TipoHerramienta.Regadera, Resources.Load<Sprite>("Herramientas/regadera")));
        inventario.herramientas.Add(new Herramienta("Pico", TipoHerramienta.Pico, Resources.Load<Sprite>("Herramientas/pico")));
        inventario.herramientas.Add(new Herramienta("Hacha", TipoHerramienta.Hacha, Resources.Load<Sprite>("Herramientas/hacha")));
        inventario.herramientas.Add(new Herramienta("Semillas de zanahoria", TipoHerramienta.Semillas, Resources.Load<Sprite>("Herramientas/semillas")));

        // 2. El script que usa las herramientas
        UsarHerramienta usar = jugador.AddComponent<UsarHerramienta>();
        usar.suelo = suelo;
        usar.tileTierra = BuscarTile(suelo, "tierra_arada");

        // 3. La interfaz (barra de abajo)
        GameObject objInterfaz = new GameObject("InterfazHerramientas");
        InterfazHerramientas interfaz = objInterfaz.AddComponent<InterfazHerramientas>();
        interfaz.inventario = inventario;

        // 4. La mochila (inventario de 5x5) y su ventana
        Inventario mochila = jugador.AddComponent<Inventario>();
        GameObject objMochila = new GameObject("InterfazInventario");
        InterfazInventario interfazMochila = objMochila.AddComponent<InterfazInventario>();
        interfazMochila.inventario = mochila;

        // 5. Decimos qué objetos del mapa se pueden romper, con qué herramienta y qué dan
        MarcarRompibles(escena);

        // 6. Añadimos los nuevos controles al texto de ayuda que ya había
        GameObject objControles = GameObject.Find("TextoControles");
        if (objControles != null)
        {
            Text texto = objControles.GetComponent<Text>();
            texto.text += "\nEspacio / Clic: usar herramienta     1-5 / Rueda: cambiar herramienta     I: mochila";
            texto.verticalOverflow = VerticalWrapMode.Overflow; // la caja era de una línea, así no se corta la segunda
        }
    }

    // Busca en el tilemap un tile por su nombre (lo usamos para sacar el tile de tierra arada)
    static TileBase BuscarTile(Tilemap tilemap, string nombre)
    {
        foreach (Vector3Int casilla in tilemap.cellBounds.allPositionsWithin)
        {
            TileBase tile = tilemap.GetTile(casilla);
            if (tile != null && tile.name == nombre) return tile;
        }
        Debug.LogWarning("InstaladorHerramientas: no hay ningún tile llamado " + nombre);
        return null;
    }

    static void MarcarRompibles(Scene escena)
    {
        foreach (GameObject raiz in escena.GetRootGameObjects())
        {
            foreach (SpriteRenderer sr in raiz.GetComponentsInChildren<SpriteRenderer>(true))
            {
                GameObject obj = sr.gameObject;
                string nombre = obj.name;

                // Ojo: "RocaGrande" tiene que ir antes que "Roca", porque también empieza por "Roca"
                if (nombre.StartsWith("RocaGrande")) HacerRompible(obj, TipoHerramienta.Pico, 4, "Piedra", 3);
                else if (nombre.StartsWith("Roca")) HacerRompible(obj, TipoHerramienta.Pico, 2, "Piedra", 1);
                else if (nombre.StartsWith("Tocon")) HacerRompible(obj, TipoHerramienta.Hacha, 3, "Madera", 2);
                else if (nombre.StartsWith("ArbustoBayas")) { } // los arbustos con bayas se quedan
                else if (nombre.StartsWith("Arbusto")) HacerRompible(obj, TipoHerramienta.Hacha, 2, "Fibra", 2);
                else if (nombre.StartsWith("Pino") || nombre.StartsWith("Roble") || nombre.StartsWith("Arce") || nombre.StartsWith("Manzano"))
                {
                    // Árbol: 4 golpes para talarlo (se queda en tronco y da 3 de madera)
                    // y 2 golpes más para quitar el tronco (da 2 de madera más)
                    ObjetoRompible arbol = HacerRompible(obj, TipoHerramienta.Hacha, 4, "Madera", 3);
                    arbol.spriteTronco = Resources.Load<Sprite>("Objetos/tronco_cortado");
                    arbol.golpesTronco = 2;
                }
                else if (nombre.StartsWith("HierbaAlta") || nombre.StartsWith("Flores") || nombre.StartsWith("Setas"))
                {
                    // Las plantas pequeñas no tenían collider. Les ponemos uno "trigger" (no choca, solo detecta)
                    // para que la azada sepa que la casilla está ocupada y el hacha las pueda cortar.
                    if (obj.GetComponent<Collider2D>() == null)
                    {
                        obj.AddComponent<BoxCollider2D>().isTrigger = true;
                    }
                    HacerRompible(obj, TipoHerramienta.Hacha, 1, "Fibra", 1);
                }
            }
        }
    }

    static ObjetoRompible HacerRompible(GameObject obj, TipoHerramienta herramienta, int golpes, string objeto, int cantidad)
    {
        ObjetoRompible rompible = obj.AddComponent<ObjetoRompible>();
        rompible.herramientaNecesaria = herramienta;
        rompible.golpes = golpes;
        rompible.objetoQueDa = objeto;
        rompible.cantidadQueDa = cantidad;
        return rompible;
    }
}
