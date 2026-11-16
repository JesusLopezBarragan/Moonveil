using UnityEngine;
using UnityEngine.SceneManagement;

// Construye la mina cuando se carga la granja:
//   - FUERA: la entrada de la mina al final del camino de arriba.
//   - DENTRO: la sala de la mina, lejos del mapa (en x = 300), con el script Mina que pone las rocas.
// Funciona igual que la tienda: se entra y se sale con la clase Puerta.
public static class MontarMina
{
    static Vector3 baseEntrada = new Vector3(47f, 65.5f, 0f);   // centro de abajo de la entrada
    // El centro acaba en .5 para que las casillas de dentro cuadren con las del mapa
    static Vector3 centroSala = new Vector3(300.5f, 0f, 0f);

    public static void Montar(Scene escena)
    {
        // Quitamos los árboles y plantas que había donde va la entrada
        MontarTienda.LimpiarZona(escena, new Vector2(43f, 64f), new Vector2(51f, 71f));

        Vector3 esquina = centroSala + new Vector3(-7.5f, -5f, 0f); // esquina de abajo a la izquierda de la sala

        // ---------- FUERA ----------
        GameObject entrada = MontarTienda.CrearSprite("Mina", "Mina/mina_entrada", baseEntrada);
        BoxCollider2D roca = entrada.AddComponent<BoxCollider2D>();
        roca.size = new Vector2(5.6f, 3f);
        roca.offset = new Vector2(0f, 2f);

        Puerta puerta = MontarTienda.CrearPuerta("PuertaMina", baseEntrada + new Vector3(0f, -0.4f, 0f), new Vector2(1.4f, 0.8f));
        puerta.entrar = true;
        puerta.destino = esquina + new Vector3(7.5f, 6f, 0f);  // al pie de la escalera de dentro
        puerta.textoAviso = "E: entrar en la mina";

        // ---------- DENTRO ----------
        GameObject sala = MontarTienda.CrearSprite("MinaDentro", "Mina/mina_interior", centroSala);
        SpriteRenderer suelo = sala.GetComponent<SpriteRenderer>();
        suelo.sortingOrder = -10; // por debajo de todo

        // Paredes invisibles
        MontarTienda.CrearPared(esquina + new Vector3(7.5f, 8.9f, 0f), 15f, 2.2f);   // fondo
        MontarTienda.CrearPared(esquina + new Vector3(0.2f, 5f, 0f), 0.5f, 10f);     // izquierda
        MontarTienda.CrearPared(esquina + new Vector3(14.8f, 5f, 0f), 0.5f, 10f);    // derecha
        MontarTienda.CrearPared(esquina + new Vector3(7.5f, 0.25f, 0f), 15f, 0.5f);  // abajo

        // La escalera de subida (dibujada en la pared del fondo) sirve para salir
        Puerta salida = MontarTienda.CrearPuerta("SalidaMina", esquina + new Vector3(7.5f, 7f, 0f), new Vector2(1.4f, 0.8f));
        salida.entrar = false;
        salida.destino = baseEntrada + new Vector3(0f, -1.4f, 0f); // aparece fuera, delante de la entrada
        salida.textoAviso = "E: salir de la mina";

        // El script que pone las rocas y controla los niveles
        Mina mina = new GameObject("ControlMina").AddComponent<Mina>();
        mina.centroSala = centroSala;
        mina.suelo = suelo;
    }
}
