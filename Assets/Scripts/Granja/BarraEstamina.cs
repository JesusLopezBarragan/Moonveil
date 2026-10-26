using UnityEngine;
using UnityEngine.UI;

public class BarraEstamina : MonoBehaviour
{
    [SerializeField] private PlayerSprint jugador;
    [SerializeField] private Image relleno;
    [SerializeField] private Vector3 offset = new Vector3(0f, 1f, 0f);

    private CanvasGroup grupo;
    private Camera cam;

    private void Awake()
    {
        grupo = GetComponent<CanvasGroup>();
        cam = Camera.main;
    }

    private void LateUpdate()
    {
        // Rellenar la barra
        relleno.fillAmount = jugador.PorcentajeEstamina();

        // Colocarla encima del jugador
        transform.position = cam.WorldToScreenPoint(jugador.transform.position + offset);

        // Mostrar solo al esprintar
        if (jugador.esprintando)
            grupo.alpha = 1f;
        else
            grupo.alpha = 0f;
    }
}