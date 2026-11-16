using UnityEngine;

public class PlayerAnimacion : MonoBehaviour
{
    [SerializeField] private Sprite[] spritesAbajo;
    [SerializeField] private Sprite[] spritesArriba;
    [SerializeField] private Sprite[] spritesLado;

    [SerializeField] private float tiempoCaminar = 0.2f;
    [SerializeField] private float tiempoSprint = 0.1f;

    private SpriteRenderer sr;
    private Rigidbody2D rb;
    private PlayerSprint sprint;

    private Sprite[] spritesActuales;
    private float timer;
    private int frame;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        sprint = GetComponent<PlayerSprint>();
        spritesActuales = spritesAbajo;
    }

    private void Update()
    {
        Vector2 vel = rb.linearVelocity;

        // Parado: primer sprite de la direccion actual
        if (vel == Vector2.zero)
        {
            frame = 0;
            timer = 0f;
            sr.sprite = spritesActuales[0];
            return;
        }

        // Elegir direccion
        if (Mathf.Abs(vel.x) > Mathf.Abs(vel.y))
        {
            spritesActuales = spritesLado;
            sr.flipX = vel.x > 0;   // si se ve al reves, cambia < por >
        }
        else if (vel.y > 0)
        {
            spritesActuales = spritesArriba;
            sr.flipX = false;
        }
        else
        {
            spritesActuales = spritesAbajo;
            sr.flipX = false;
        }

        // Velocidad de la animacion
        float tiempoEntreFrames;
        if (sprint.esprintando)
            tiempoEntreFrames = tiempoSprint;
        else
            tiempoEntreFrames = tiempoCaminar;

        // Cambiar de frame
        timer += Time.deltaTime;
        if (timer >= tiempoEntreFrames)
        {
            timer = 0f;
            if (frame == 1)
                frame = 2;
            else
                frame = 1;
        }

        sr.sprite = spritesActuales[frame];
    }
}