using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// Una zanahoria que crece sola y se cosecha pulsando E cuando el jugador está al lado.
public class Cultivo : MonoBehaviour
{
    // NUEVO: lista con todas las zanahorias de la escena (la usan las herramientas para encontrarlas)
    public static List<Cultivo> todos = new List<Cultivo>();

    public Sprite[] fases;          // 0 semilla, 1 brote, 2 creciendo, 3 madura
    public int faseActual = 0;
    public float tiempoPorFase = 5f;

    private SpriteRenderer sr;
    private float contador = 0f;
    private bool jugadorCerca = false;
    private float tiempoRegado = 0f;  // NUEVO: segundos que le quedan regada

    void OnEnable()
    {
        todos.Add(this);
    }

    void OnDisable()
    {
        todos.Remove(this);
    }

    // NUEVO: la regadera llama a este método. Mientras está regada crece el doble de rápido.
    public void Regar(float segundos)
    {
        tiempoRegado = segundos;
    }

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.sprite = fases[faseActual];
    }

    void Update()
    {
        // Crecer: cada cierto tiempo pasa a la siguiente fase
        if (tiempoRegado > 0f) tiempoRegado -= Time.deltaTime;

        if (faseActual < fases.Length - 1)
        {
            if (tiempoRegado > 0f) contador += Time.deltaTime * 2f; // NUEVO: regada crece x2
            else contador += Time.deltaTime;
            if (contador >= tiempoPorFase)
            {
                contador = 0f;
                faseActual++;
                sr.sprite = fases[faseActual];
            }
        }

        // Cosechar: solo si está madura y el jugador está cerca
        if (jugadorCerca && faseActual == fases.Length - 1 && Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            ControladorJuego.zanahorias++;
            ControladorJuego.semillas++;  // NUEVO: al cosechar recuperamos una semilla
            if (Inventario.instancia != null) Inventario.instancia.Guardar("Zanahoria", 1); // NUEVO: va a la mochila
            faseActual = 0;           // se vuelve a plantar
            sr.sprite = fases[0];
        }
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
