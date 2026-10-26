using UnityEngine;
using UnityEngine.InputSystem;

// Una zanahoria que crece sola y se cosecha pulsando E cuando el jugador está al lado.
public class Cultivo : MonoBehaviour
{
    public Sprite[] fases;          // 0 semilla, 1 brote, 2 creciendo, 3 madura
    public int faseActual = 0;
    public float tiempoPorFase = 5f;

    private SpriteRenderer sr;
    private float contador = 0f;
    private bool jugadorCerca = false;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.sprite = fases[faseActual];
    }

    void Update()
    {
        // Crecer: cada cierto tiempo pasa a la siguiente fase
        if (faseActual < fases.Length - 1)
        {
            contador += Time.deltaTime;
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
