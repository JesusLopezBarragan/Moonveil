using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float velocidad = 6f;

    private Rigidbody2D rb;
    private Vector2 inputMovimiento;
    private InputAction accionMovimiento;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        accionMovimiento = new InputAction("Movimiento", InputActionType.Value);
        accionMovimiento.AddCompositeBinding("2DVector")
            .With("Up", "<Keyboard>/w")
            .With("Down", "<Keyboard>/s")
            .With("Left", "<Keyboard>/a")
            .With("Right", "<Keyboard>/d");
        accionMovimiento.AddCompositeBinding("2DVector")
            .With("Up", "<Keyboard>/upArrow")
            .With("Down", "<Keyboard>/downArrow")
            .With("Left", "<Keyboard>/leftArrow")
            .With("Right", "<Keyboard>/rightArrow");
    }

    private void OnEnable() => accionMovimiento.Enable();
    private void OnDisable() => accionMovimiento.Disable();

    private void Start()
    {
        aplicarAspectoSeleccionado();
    }

    private void Update()
    {
        inputMovimiento = accionMovimiento.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = inputMovimiento.normalized * velocidad;
    }

    private void aplicarAspectoSeleccionado()
    {
        // El sprite visual es opcional; sin SpriteRenderer no hay nada que tintar.
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        if (sprite == null)
        {
            return;
        }

        // El orden debe coincidir con Natural, Bosque y Luna en el menu de creacion.
        Color[] coloresAspecto =
        {
            Color.white,
            new Color(0.75f, 0.92f, 0.75f),
            new Color(0.76f, 0.8f, 1f)
        };
        // Natural es el valor por defecto; Clamp protege ante indices guardados invalidos.
        int indiceAspecto = Mathf.Clamp(
            PlayerPrefs.GetInt("moonveil_personaje_aspecto", 0),
            0,
            coloresAspecto.Length - 1);
        sprite.color = coloresAspecto[indiceAspecto];
    }

    public float Velocidad
    {
        get => velocidad;
        set => velocidad = value;
    }
}