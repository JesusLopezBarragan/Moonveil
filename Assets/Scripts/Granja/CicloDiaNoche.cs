using UnityEngine;
using UnityEngine.Rendering.Universal;
using TMPro;

public class CicloDiaNoche : MonoBehaviour
{
    [SerializeField] private Light2D luzGlobal;
    [SerializeField] private TextMeshProUGUI textoReloj;
    [SerializeField] private Gradient colorDelDia;
    [SerializeField] private float segundosPorDia = 120f;
    [SerializeField] private float horaInicial = 8f;

    private float hora;
    private int dia = 1;

    public float Hora => hora;
    public int Dia => dia;

    private void Start()
    {
        hora = horaInicial;
    }

    private void Update()
    {
        // Avanzar la hora
        hora += (24f / segundosPorDia) * Time.deltaTime;

        if (hora >= 24f)
        {
            hora -= 24f;
            dia++;
        }

        // Cambiar el color de la luz
        luzGlobal.color = colorDelDia.Evaluate(hora / 24f);

        // Mostrar el reloj
        int h = (int)hora;
        int m = (int)((hora - h) * 60f);
        textoReloj.text = "Día " + dia + "  " + h.ToString("00") + ":" + m.ToString("00");
    }
}