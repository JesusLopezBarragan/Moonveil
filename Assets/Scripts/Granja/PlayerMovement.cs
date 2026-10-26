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

    private void Update()
    {
        inputMovimiento = accionMovimiento.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = inputMovimiento.normalized * velocidad;
    }

    public float Velocidad
    {
        get => velocidad;
        set => velocidad = value;
    }
}