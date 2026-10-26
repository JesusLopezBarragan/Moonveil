using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerMovement))]
public class PlayerSprint : MonoBehaviour
{
    [SerializeField] private float multiplicadorSprint = 1.6f;
    [SerializeField] private float estaminaMaxima = 30f;
    [SerializeField] private float velocidadGasto = 10f;    // estamina por segundo gastando
    [SerializeField] private float tiempoEsperaRecarga = 5f;
    [SerializeField] private float velocidadRecarga = 10f;  // estamina por segundo recargando

    private PlayerMovement movimiento;
    private InputAction accionSprint;

    private float velocidadBase;
    private float estaminaActual;
    private float timerEspera;
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
        estaminaActual = estaminaMaxima;
    }

    private void Update()
    {
        bool shiftPulsado = accionSprint.IsPressed();

        if (shiftPulsado && estaminaActual > 0f)
        {
            // Esprintar
            esprintando = true;
            estaminaActual -= velocidadGasto * Time.deltaTime;
            timerEspera = tiempoEsperaRecarga;
            movimiento.Velocidad = velocidadBase * multiplicadorSprint;

            if (estaminaActual < 0f) estaminaActual = 0f;
        }
        else
        {
            // Parar sprint
            esprintando = false;
            movimiento.Velocidad = velocidadBase;

            // Esperar antes de recargar
            if (timerEspera > 0f)
            {
                timerEspera -= Time.deltaTime;
            }
            else
            {
                // Recargar estamina
                estaminaActual += velocidadRecarga * Time.deltaTime;
                if (estaminaActual > estaminaMaxima) estaminaActual = estaminaMaxima;
            }
        }
    }
}