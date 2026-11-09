using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

// Panel de ayuda con los controles del juego. Se abre y se cierra con la tecla P.
public class InterfazAyuda : MonoBehaviour
{
    private string controles =
        "W A S D o flechas:  moverse\n" +
        "1, 2, 3, 4, 5:  elegir herramienta\n" +
        "Rueda del ratón:  cambiar herramienta\n" +
        "Espacio o clic izquierdo:  usar herramienta\n" +
        "E:  recoger zanahoria madura\n" +
        "E:  entrar en la tienda y hablar con el vendedor\n" +
        "E:  entrar en la mina y bajar de nivel\n" +
        "I o Tab:  abrir la mochila\n" +
        "P:  abrir y cerrar esta ayuda";

    private GameObject panel;

    void Start()
    {
        Color crema = new Color32(255, 243, 214, 255);

        Transform canvas = AyudaUI.CrearCanvas("CanvasAyuda", transform, 30); // encima de la mochila (20)

        // Panel de madera en el centro de la pantalla
        Image fondo = AyudaUI.CrearImagen("PanelAyuda", canvas, Resources.Load<Sprite>("UI/panel_liso"), 4);
        AyudaUI.Colocar(fondo.rectTransform, new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(560f, 450f));
        panel = fondo.gameObject;

        Text titulo = AyudaUI.CrearTexto("Titulo", panel.transform, "Controles", 30, crema);
        titulo.alignment = TextAnchor.MiddleCenter;
        AyudaUI.Colocar(titulo.rectTransform, new Vector2(0.5f, 1f), new Vector2(0f, -20f), new Vector2(400f, 44f));

        Text texto = AyudaUI.CrearTexto("Texto", panel.transform, controles, 20, crema);
        texto.alignment = TextAnchor.MiddleCenter;
        texto.lineSpacing = 1.3f;
        AyudaUI.Colocar(texto.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0f, -20f), new Vector2(480f, 330f));

        panel.SetActive(false); // empieza cerrado
    }

    void Update()
    {
        // Al pulsar P: si está cerrado se abre y si está abierto se cierra
        if (Keyboard.current.pKey.wasPressedThisFrame)
        {
            panel.SetActive(!panel.activeSelf);
        }
    }
}
