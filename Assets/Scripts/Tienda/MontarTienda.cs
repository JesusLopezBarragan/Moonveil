using UnityEngine;
using UnityEngine.SceneManagement;

// Construye la tienda cuando se carga la granja:
//   - FUERA: el edificio, entre el granero y la plaza, con la puerta mirando al camino.
//   - DENTRO: una sala con estanterías, el mostrador y el vendedor.
// La sala de dentro está lejos del mapa (en x = 200), así que desde fuera no se ve.
// Al cruzar la puerta, el jugador se teletransporta de un sitio al otro.
public static class MontarTienda
{
    static Vector3 baseEdificio = new Vector3(36.5f, 37f, 0f);  // centro de abajo del edificio
    static Vector3 centroSala = new Vector3(200f, 0f, 0f);      // centro de la sala de dentro

    public static void Montar(Scene escena)
    {
        // Quitamos los árboles, rocas y flores que había donde va el edificio
        LimpiarZona(escena, new Vector2(33f, 36.6f), new Vector2(39.9f, 43.6f));

        CrearFuera();
        CrearDentro();
    }

    static void CrearFuera()
    {
        // El edificio, con un collider en la parte de abajo para no atravesar la pared
        GameObject edificio = CrearSprite("Tienda", "Tienda/tienda_exterior", baseEdificio);
        BoxCollider2D pared = edificio.AddComponent<BoxCollider2D>();
        pared.size = new Vector2(5.8f, 4f);
        pared.offset = new Vector2(0f, 2.4f);

        // La puerta de fuera: una zona delante de la puerta. Al pulsar E entras.
        Vector3 esquinaSala = centroSala + new Vector3(-7.5f, -5f, 0f);
        Puerta puerta = CrearPuerta("PuertaFuera", baseEdificio + new Vector3(0f, -0.4f, 0f), new Vector2(1.4f, 0.8f));
        puerta.entrar = true;
        puerta.destino = esquinaSala + new Vector3(7.5f, 1.8f, 0f);  // aparece dentro, junto a la salida
        puerta.centroSala = centroSala;
        puerta.textoAviso = "E: entrar en la tienda";
    }

    static void CrearDentro()
    {
        // Las posiciones de dentro se cuentan desde la esquina de abajo a la izquierda de la sala
        // (la sala mide 15 x 10 casillas)
        Vector3 esquina = centroSala + new Vector3(-7.5f, -5f, 0f);

        // Suelo y paredes (un único dibujo), por debajo de todo
        GameObject sala = CrearSprite("TiendaDentro", "Tienda/tienda_interior", centroSala);
        sala.GetComponent<SpriteRenderer>().sortingOrder = -10;

        // Paredes invisibles para no salirse de la sala
        CrearPared(esquina + new Vector3(7.5f, 8.9f, 0f), 15f, 2.2f);    // pared del fondo
        CrearPared(esquina + new Vector3(0.2f, 5f, 0f), 0.5f, 10f);      // izquierda
        CrearPared(esquina + new Vector3(14.8f, 5f, 0f), 0.5f, 10f);     // derecha
        CrearPared(esquina + new Vector3(3.25f, 0.25f, 0f), 6.5f, 0.5f); // abajo, a la izquierda de la salida
        CrearPared(esquina + new Vector3(11.75f, 0.25f, 0f), 6.5f, 0.5f);// abajo, a la derecha de la salida
        CrearPared(esquina + new Vector3(7.5f, -0.25f, 0f), 2f, 0.5f);   // debajo de la salida

        // Estanterías: tres contra la pared del fondo y dos en el centro
        CrearEstanteria("Tienda/estanteria_1", esquina + new Vector3(2f, 7.4f, 0f));
        CrearEstanteria("Tienda/estanteria_2", esquina + new Vector3(4.5f, 7.4f, 0f));
        CrearEstanteria("Tienda/estanteria_1", esquina + new Vector3(12.8f, 7.4f, 0f));
        CrearEstanteria("Tienda/estanteria_2", esquina + new Vector3(2.5f, 3.5f, 0f));
        CrearEstanteria("Tienda/estanteria_1", esquina + new Vector3(5f, 3.5f, 0f));

        // El mostrador
        GameObject mostrador = CrearSprite("Mostrador", "Tienda/mostrador", esquina + new Vector3(11.5f, 4.6f, 0f));
        BoxCollider2D colMostrador = mostrador.AddComponent<BoxCollider2D>();
        colMostrador.size = new Vector2(2.8f, 0.8f);
        colMostrador.offset = new Vector2(0f, 0.4f);

        // El vendedor, detrás del mostrador. Su zona para hablar está delante del mostrador.
        GameObject vendedor = CrearSprite("Vendedor", "Tienda/vendedor", esquina + new Vector3(11.5f, 5.6f, 0f));
        CircleCollider2D zona = vendedor.AddComponent<CircleCollider2D>();
        zona.isTrigger = true;
        zona.radius = 1.3f;
        zona.offset = new Vector2(0f, -1.3f);
        vendedor.AddComponent<Vendedor>();

        // La puerta de dentro (la salida): al pulsar E vuelves fuera
        Puerta salida = CrearPuerta("PuertaDentro", esquina + new Vector3(7.5f, 0.6f, 0f), new Vector2(2f, 1f));
        salida.entrar = false;
        salida.destino = baseEdificio + new Vector3(0f, -1.4f, 0f); // aparece fuera, delante de la puerta
        salida.textoAviso = "E: salir de la tienda";
    }

    // ---------------- Métodos de ayuda (públicos: MontarMina también los usa) ----------------

    // Crea un objeto con un dibujo
    public static GameObject CrearSprite(string nombre, string rutaSprite, Vector3 posicion)
    {
        GameObject obj = new GameObject(nombre);
        obj.transform.position = posicion;
        obj.AddComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(rutaSprite);
        return obj;
    }

    static void CrearEstanteria(string rutaSprite, Vector3 posicion)
    {
        GameObject estanteria = CrearSprite("Estanteria", rutaSprite, posicion);
        BoxCollider2D col = estanteria.AddComponent<BoxCollider2D>();
        col.size = new Vector2(1.8f, 0.6f);   // solo la base, así el jugador puede pasar por delante
        col.offset = new Vector2(0f, 0.3f);
    }

    // Una pared invisible: solo un collider
    public static void CrearPared(Vector3 centro, float ancho, float alto)
    {
        GameObject pared = new GameObject("Pared");
        pared.transform.position = centro;
        pared.AddComponent<BoxCollider2D>().size = new Vector2(ancho, alto);
    }

    // Una zona (trigger) con el script Puerta
    public static Puerta CrearPuerta(string nombre, Vector3 centro, Vector2 tamano)
    {
        GameObject obj = new GameObject(nombre);
        obj.transform.position = centro;
        BoxCollider2D zona = obj.AddComponent<BoxCollider2D>();
        zona.isTrigger = true;
        zona.size = tamano;
        return obj.AddComponent<Puerta>();
    }

    // Borra los objetos del mapa (árboles, flores, rocas...) que estén dentro de un rectángulo
    public static void LimpiarZona(Scene escena, Vector2 minimo, Vector2 maximo)
    {
        foreach (GameObject raiz in escena.GetRootGameObjects())
        {
            foreach (SpriteRenderer sr in raiz.GetComponentsInChildren<SpriteRenderer>())
            {
                Vector3 p = sr.transform.position;
                bool dentro = p.x > minimo.x && p.x < maximo.x && p.y > minimo.y && p.y < maximo.y;
                if (dentro && !sr.CompareTag("Player"))
                {
                    Object.Destroy(sr.gameObject);
                }
            }
        }
    }
}
