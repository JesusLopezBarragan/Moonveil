using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerMovement))]
public class PlayerSprint : MonoBehaviour
{
    [SerializeField] private float multiplicadorSprint = 1.6f;
    [SerializeField] private float duracionSprint = 5f;
    [SerializeField] private float cooldownSprint = 5f;

    private PlayerMovement movimiento;
    private InputAction accionSprint;

    private float velocidadBase;
    private float timerSprint;
    private float timerCooldown;
    public bool esprintando = false;

    private void Awake()
    {
        movimiento = GetComponent<PlayerMovement>();

        accionSprint = new InputAction("Sprint", InputActionType.Button);
        accionSprint.AddBinding("<Keyboard>/leftShift");
    }

    private void OnEnable() => accionSprint.Enable();
    private void OnDisable() => accionSprint.Disable();

    private void Start()
    {
        velocidadBase = movimiento.Velocidad;
    }

    private void Update()
    {
        if (esprintando)
        {
            timerSprint -= Time.deltaTime;
            if (timerSprint <= 0f)
            {
                PararSprint();
            }
        }
        else
        {
            if (timerCooldown > 0f) timerCooldown -= Time.deltaTime;
            if (accionSprint.WasPressedThisFrame() && timerCooldown <= 0f) EmpezarSprint();
        }
    }

    private void EmpezarSprint()
    {
        esprintando = true;
        timerSprint = duracionSprint;
        movimiento.Velocidad = velocidadBase * multiplicadorSprint;
    }

    private void PararSprint()
    {
        esprintando = false;
        timerCooldown = cooldownSprint;
        movimiento.Velocidad = velocidadBase;
    }
}