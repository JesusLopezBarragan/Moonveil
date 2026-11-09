using System.Collections;
using UnityEngine;

// Un objeto que sale al romper algo o al cosechar (madera, piedra, zanahoria...).
// Primero salta y cae al suelo, luego vuela hacia el jugador
// y cuando llega se guarda en la mochila.
public class ObjetoRecogible : MonoBehaviour
{
    public string nombre;
    public string textoAlCoger = "";  // si no está vacío, sale flotando al recogerlo

    private Transform jugador;

    // Suelta "cantidad" objetos en una posición (un dibujo por cada unidad)
    public static void Soltar(string nombre, int cantidad, Vector3 posicion)
    {
        for (int i = 0; i < cantidad; i++)
        {
            GameObject obj = new GameObject("Recogible" + nombre);
            obj.transform.position = posicion;

            SpriteRenderer sr = obj.AddComponent<SpriteRenderer>();
            sr.sprite = Resources.Load<Sprite>("Objetos/" + nombre.ToLower());
            sr.sortingOrder = 5; // por encima de los árboles y rocas

            ObjetoRecogible recogible = obj.AddComponent<ObjetoRecogible>();
            recogible.nombre = nombre;
            // Solo el último enseña el texto, así sale "+3 Madera" una vez y no tres veces "+1"
            if (i == cantidad - 1) recogible.textoAlCoger = "+" + cantidad + " " + nombre;
        }
    }

    void Start()
    {
        jugador = GameObject.FindWithTag("Player").transform;
        StartCoroutine(Animar());
    }

    IEnumerator Animar()
    {
        // 1. Sale disparado y cae al suelo cerca, haciendo un arco
        Vector3 inicio = transform.position;
        Vector3 suelo = inicio + new Vector3(Random.Range(-0.7f, 0.7f), Random.Range(-0.6f, -0.1f), 0f);
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / 0.4f;               // tarda 0.4 segundos
            Vector3 posicion = Vector3.Lerp(inicio, suelo, t);
            posicion.y += Mathf.Sin(t * Mathf.PI) * 0.8f;  // sube y baja (como un salto)
            transform.position = posicion;
            yield return null;
        }

        // 2. Se queda un momento en el suelo
        yield return new WaitForSeconds(Random.Range(0.25f, 0.45f));

        // 3. Vuela hacia el jugador, cada vez más rápido, y se hace pequeño al llegar
        float velocidad = 2f;
        float distancia = 99f;
        while (distancia > 0.1f)
        {
            Vector3 destino = jugador.position + new Vector3(0f, 0.6f, 0f); // hacia el pecho del jugador
            velocidad += 15f * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, destino, velocidad * Time.deltaTime);

            distancia = Vector3.Distance(transform.position, destino);
            if (distancia < 1f) transform.localScale = Vector3.one * distancia;
            yield return null;
        }

        // 4. Llega: se guarda en la mochila y sale el texto
        if (Inventario.instancia != null) Inventario.instancia.Guardar(nombre, 1);
        if (textoAlCoger != "") TextoFlotante.Crear(textoAlCoger, jugador.position + new Vector3(0f, 1.7f, 0f));

        Destroy(gameObject);
    }
}
