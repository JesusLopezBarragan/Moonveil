using UnityEngine;
using UnityEngine.UI;

public class BarraEstamina : MonoBehaviour
{
    [SerializeField] private PlayerSprint jugador;
    [SerializeField] private Image relleno;
    [SerializeField] private Vector3 offset = new Vector3(0f, 1f, 0f);

    // Píxeles de pantalla que sube la barra por encima de la cabeza, para quedar sobre el nombre
    [SerializeField] private float pixelesSobreNombre = 130f;

    private CanvasGroup grupo;
    private Camera cam;
    private SpriteRenderer spriteJugador;

    private void Awake()
    {
        grupo = GetComponent<CanvasGroup>();
        cam = Camera.main;
        spriteJugador = jugador.GetComponentInChildren<SpriteRenderer>();
    }

    private void LateUpdate()
    {
        // Rellenar la barra
        relleno.fillAmount = jugador.PorcentajeEstamina();

        // Colocarla encima del nombre: partimos de la cabeza del jugador (parte superior
        // de su sprite, donde se dibuja el nombre) y subimos unos píxeles de pantalla
        float yCabeza = spriteJugador != null
            ? spriteJugador.bounds.max.y
            : jugador.transform.position.y + offset.y;
        Vector3 posicionMundo = new Vector3(jugador.transform.position.x, yCabeza, jugador.transform.position.z);
        Vector3 posicionPantalla = cam.WorldToScreenPoint(posicionMundo);
        posicionPantalla.y += pixelesSobreNombre;
        transform.position = posicionPantalla;

        // Mostrar solo al esprintar
        if (jugador.esprintando)
            grupo.alpha = 1f;
        else
            grupo.alpha = 0f;
    }
}