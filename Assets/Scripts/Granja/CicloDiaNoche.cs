using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

// Ciclo de día y noche:
//  - Hace avanzar la hora del juego (0 a 24).
//  - Cambia el color de la luz global según la hora.
//  - Muestra un reloj en pantalla ("Día 1  08:30").
//  - Avisa a otros scripts cuando empieza un día nuevo (evento OnNuevoDia).
public class CicloDiaNoche : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Light2D luzGlobal;   // La "Global Light 2D" de la escena
    [SerializeField] private Text textoReloj;     // Opcional: si se deja vacío se crea solo

    [Header("Tiempo")]
    [SerializeField] private float segundosPorDia = 120f; // Duración real de un día completo
    [SerializeField] private float horaInicial = 8f;

    [Header("Color de la luz")]
    [SerializeField] private Color colorNoche = new Color(0.15f, 0.2f, 0.45f);
    [SerializeField] private Color colorAmanecer = new Color(1f, 0.7f, 0.5f);
    [SerializeField] private Color colorDia = Color.white;
    [SerializeField] private Color colorAtardecer = new Color(1f, 0.55f, 0.35f);

    private float hora;
    private int dia = 1;

    // Para que otros scripts puedan consultar la hora o el día
    public float Hora => hora;
    public int Dia => dia;
    public bool EsDeNoche => hora < 6f || hora >= 20f;

    // Se lanza cada vez que pasa la medianoche. Uso: ciclo.OnNuevoDia += MiMetodo;
    public event System.Action<int> OnNuevoDia;

    private void Start()
    {
        hora = horaInicial;

        // Si no hay texto asignado, lo creamos dentro del Canvas de la escena
        if (textoReloj == null)
            textoReloj = CrearTextoReloj();

        ActualizarLuz();
        ActualizarReloj();
    }

    private void Update()
    {
        // Avanzar la hora (24 horas repartidas en segundosPorDia segundos reales).
        // Time.deltaTime vale 0 con el juego en pausa, así que el reloj se para solo.
        hora += (24f / segundosPorDia) * Time.deltaTime;

        // Pasada la medianoche empieza un día nuevo
        if (hora >= 24f)
        {
            hora -= 24f;
            dia++;
            OnNuevoDia?.Invoke(dia);
        }

        ActualizarLuz();
        ActualizarReloj();
    }

    // Elige el color de la luz según la hora, mezclando entre las franjas del día
    private void ActualizarLuz()
    {
        if (luzGlobal == null) return;

        Color color;
        if (hora < 5f)        color = colorNoche;                                          // 00:00 - 05:00 noche
        else if (hora < 7f)   color = Color.Lerp(colorNoche, colorAmanecer, (hora - 5f) / 2f);   // amanece
        else if (hora < 9f)   color = Color.Lerp(colorAmanecer, colorDia, (hora - 7f) / 2f);     // ya es de día
        else if (hora < 17f)  color = colorDia;                                            // 09:00 - 17:00 día
        else if (hora < 19f)  color = Color.Lerp(colorDia, colorAtardecer, (hora - 17f) / 2f);   // atardece
        else if (hora < 21f)  color = Color.Lerp(colorAtardecer, colorNoche, (hora - 19f) / 2f); // anochece
        else                  color = colorNoche;                                          // 21:00 - 24:00 noche

        luzGlobal.color = color;
    }

    // Escribe la hora en pantalla con formato "Día 1  08:30"
    private void ActualizarReloj()
    {
        if (textoReloj == null) return;

        int h = (int)hora;
        int m = (int)((hora - h) * 60f);
        textoReloj.text = "Día " + dia + "  " + h.ToString("00") + ":" + m.ToString("00");
    }

    // Crea un texto en la esquina superior derecha del Canvas
    private Text CrearTextoReloj()
    {
        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null) return null;

        GameObject obj = new GameObject("TextoReloj");
        obj.transform.SetParent(canvas.transform, false);

        Text texto = obj.AddComponent<Text>();
        texto.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        texto.fontSize = 28;
        texto.alignment = TextAnchor.UpperRight;
        texto.color = Color.white;
        texto.raycastTarget = false;

        // Anclar arriba a la derecha
        RectTransform rt = texto.rectTransform;
        rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(1f, 1f);
        rt.anchoredPosition = new Vector2(-20f, -20f);
        rt.sizeDelta = new Vector2(300f, 40f);

        return texto;
    }
}
