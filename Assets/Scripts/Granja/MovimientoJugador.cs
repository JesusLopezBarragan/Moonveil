using UnityEngine;
using UnityEngine.InputSystem;

// Mueve al jugador con WASD o las flechas y cambia su sprite para animarlo.
public class MovimientoJugador : MonoBehaviour
{
    public float velocidad = 4f;

    // Sprites de cada dirección: [0] = quieto, [1] y [2] = pasos
    public Sprite[] spritesAbajo;
    public Sprite[] spritesArriba;
    public Sprite[] spritesLado;   // mirando a la izquierda

    public float tiempoEntrePasos = 0.15f;

    // NUEVO (herramientas): hacia dónde mira el jugador y si puede moverse
    public Vector2 direccion = Vector2.down;
    public bool bloqueado = false;   // true mientras usa una herramienta

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Vector2 movimiento;
    private Sprite[] spritesActuales;
    private float contador = 0f;
    private int frame = 0;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        spritesActuales = spritesAbajo;
    }

    void Update()
    {
        // 1. Leer el teclado (-1, 0 o 1 en cada eje)
        Keyboard teclado = Keyboard.current;
        movimiento = Vector2.zero;
        if (teclado != null)
        {
            if (teclado.dKey.isPressed || teclado.rightArrowKey.isPressed) movimiento.x += 1;
            if (teclado.aKey.isPressed || teclado.leftArrowKey.isPressed) movimiento.x -= 1;
            if (teclado.wKey.isPressed || teclado.upArrowKey.isPressed) movimiento.y += 1;
            if (teclado.sKey.isPressed || teclado.downArrowKey.isPressed) movimiento.y -= 1;
        }
        movimiento = movimiento.normalized; // para no ir más rápido en diagonal
        if (bloqueado) movimiento = Vector2.zero; // NUEVO: quieto mientras usa una herramienta

        // 2. Elegir los sprites según la dirección
        if (movimiento.x > 0)
        {
            spritesActuales = spritesLado;
            sr.flipX = true; // el sprite mira a la izquierda, lo damos la vuelta
            direccion = Vector2.right;
        }
        else if (movimiento.x < 0)
        {
            spritesActuales = spritesLado;
            sr.flipX = false;
            direccion = Vector2.left;
        }
        else if (movimiento.y > 0)
        {
            spritesActuales = spritesArriba;
            sr.flipX = false;
            direccion = Vector2.up;
        }
        else if (movimiento.y < 0)
        {
            spritesActuales = spritesAbajo;
            sr.flipX = false;
            direccion = Vector2.down;
        }

        // 3. Animación: si se mueve, alterna entre el paso 1 y el paso 2
        if (movimiento != Vector2.zero)
        {
            contador += Time.deltaTime;
            if (contador >= tiempoEntrePasos)
            {
                contador = 0f;
                if (frame == 1) frame = 2;
                else frame = 1;
            }
        }
        else
        {
            frame = 0; // quieto
        }

        sr.sprite = spritesActuales[frame];
    }

    void FixedUpdate()
    {
        // 4. Mover usando la física para que choque con las colisiones
        rb.linearVelocity = movimiento * velocidad;
    }
}
