using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class PauseMenuController : MonoBehaviour
{
    [SerializeField] private PanelSettings panelSettings;
    [SerializeField] private VisualTreeAsset pauseMenu;
    [SerializeField] private VisualTreeAsset menuConfiguracion;
    [SerializeField] private string escenaMenuPrincipal = "MenuScene";

    private static readonly Vector2Int[] resoluciones =
    {
        new Vector2Int(1280, 720),
        new Vector2Int(1600, 900),
        new Vector2Int(1920, 1080)
    };

    private static readonly int[] limitesFps = { 30, 60, 120, -1 };
    private const string CLAVE_VOLUMEN = "configuracion_volumen";
    private const string CLAVE_RESOLUCION = "configuracion_resolucion";
    private const string CLAVE_PANTALLA_COMPLETA = "configuracion_pantalla_completa";
    private const string CLAVE_FPS = "configuracion_fps";
    private const string CLAVE_NOMBRE_PERSONAJE = "moonveil_personaje_nombre";

    private InputAction accionPausa;
    private UIDocument documento;
    private VisualElement raizPausa;
    private VisualElement arbolMenuPausa;
    private VisualElement superposicionPausa;
    private VisualElement vistaPausa;
    private Label etiquetaNombre;
    private Button botonContinuar;
    private SpriteRenderer spriteJugador;
    private bool estaPausado;
    private bool enConfiguracion;
    private bool cursorVisibleAnterior;
    private CursorLockMode bloqueoCursorAnterior;

    private void Awake()
    {
        accionPausa = new InputAction("Pausa", InputActionType.Button);
        accionPausa.AddBinding("<Keyboard>/escape");

        if (panelSettings == null || pauseMenu == null || menuConfiguracion == null)
        {
            Debug.LogError("Asigna PanelSettings, PauseMenu.uxml y MenuConfigurer.uxml al PauseMenuController.", this);
            enabled = false;
            return;
        }

        GameObject objetoInterfaz = new GameObject("Interfaz de pausa");
        documento = objetoInterfaz.AddComponent<UIDocument>();
        documento.panelSettings = panelSettings;
        documento.sortingOrder = 10;

        raizPausa = documento.rootVisualElement;
        arbolMenuPausa = pauseMenu.Instantiate();
        raizPausa.Add(arbolMenuPausa);
        superposicionPausa = arbolMenuPausa.Q<VisualElement>("pause-overlay");
        vistaPausa = arbolMenuPausa.Q<VisualElement>("pause-panel");
        etiquetaNombre = arbolMenuPausa.Q<Label>("player-name-tag");
        botonContinuar = arbolMenuPausa.Q<Button>("btn-resume");
        Button botonConfiguracion = arbolMenuPausa.Q<Button>("btn-pause-settings");
        Button botonSalir = arbolMenuPausa.Q<Button>("btn-main-menu");

        if (superposicionPausa == null || vistaPausa == null ||
            etiquetaNombre == null || botonContinuar == null ||
            botonConfiguracion == null || botonSalir == null)
        {
            Debug.LogError("Faltan elementos necesarios en PauseMenu.uxml.", this);
            enabled = false;
            Destroy(objetoInterfaz);
            return;
        }

        etiquetaNombre.text = PlayerPrefs.GetString(CLAVE_NOMBRE_PERSONAJE, "Jugador");
        spriteJugador = GetComponent<SpriteRenderer>();
        raizPausa.RegisterCallback<GeometryChangedEvent>(centrarVentanaPausa);
        superposicionPausa.RegisterCallback<GeometryChangedEvent>(_ => centrarVentanaPausa());
        vistaPausa.RegisterCallback<GeometryChangedEvent>(_ => centrarVentanaPausa());
        botonContinuar.clicked += reanudarJuego;
        botonConfiguracion.clicked += mostrarConfiguracion;
        botonSalir.clicked += volverAlMenuPrincipal;
    }

    private void OnEnable()
    {
        accionPausa?.Enable();
    }

    private void OnDisable()
    {
        accionPausa?.Disable();
    }

    private void Update()
    {
        if (accionPausa != null && accionPausa.WasPressedThisFrame())
        {
            alternarPausa();
        }
    }

    private void LateUpdate()
    {
        if (etiquetaNombre == null || raizPausa?.panel == null)
        {
            return;
        }

        Camera camara = Camera.main;
        if (camara == null)
        {
            return;
        }

        float parteSuperiorSprite = spriteJugador != null
            ? spriteJugador.bounds.max.y
            : transform.position.y + 1.25f;
        Vector3 posicionPantalla = camara.WorldToScreenPoint(
            new Vector3(transform.position.x, parteSuperiorSprite, transform.position.z));
        bool visible = posicionPantalla.z > 0;
        etiquetaNombre.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
        if (!visible)
        {
            return;
        }

        Vector2 tamañoPanel = raizPausa.layout.size;
        Vector2 posicionPanel = new Vector2(
            posicionPantalla.x / Screen.width * tamañoPanel.x,
            (Screen.height - posicionPantalla.y) / Screen.height * tamañoPanel.y);
        float altoEtiqueta = etiquetaNombre.resolvedStyle.height;
        etiquetaNombre.style.left = posicionPanel.x - etiquetaNombre.resolvedStyle.width / 2f;
        etiquetaNombre.style.top = posicionPanel.y - altoEtiqueta - 8f;
    }

    private void alternarPausa()
    {
        if (!estaPausado)
        {
            pausarJuego();
        }
        else if (enConfiguracion)
        {
            mostrarMenuPausa();
        }
        else
        {
            reanudarJuego();
        }
    }

    private void pausarJuego()
    {
        estaPausado = true;
        Time.timeScale = 0f;
        cursorVisibleAnterior = UnityEngine.Cursor.visible;
        bloqueoCursorAnterior = UnityEngine.Cursor.lockState;
        UnityEngine.Cursor.visible = true;
        UnityEngine.Cursor.lockState = CursorLockMode.None;
        mostrarMenuPausa();
    }

    private void mostrarMenuPausa()
    {
        raizPausa.Clear();
        raizPausa.Add(arbolMenuPausa);
        superposicionPausa.style.position = Position.Absolute;
        superposicionPausa.style.left = 0;
        superposicionPausa.style.top = 0;
        superposicionPausa.style.width = raizPausa.layout.width;
        superposicionPausa.style.height = raizPausa.layout.height;
        superposicionPausa.style.backgroundColor = new StyleColor(new Color(10f / 255f, 12f / 255f, 28f / 255f, 0.82f));
        vistaPausa.style.position = Position.Absolute;
        superposicionPausa.style.display = DisplayStyle.Flex;
        enConfiguracion = false;
        botonContinuar.Focus();
        centrarVentanaPausa();
    }

    private void centrarVentanaPausa(GeometryChangedEvent evt)
    {
        centrarVentanaPausa();
    }

    private void centrarVentanaPausa()
    {
        if (superposicionPausa == null || vistaPausa == null ||
            superposicionPausa.layout.width <= 0f || superposicionPausa.layout.height <= 0f ||
            vistaPausa.layout.width <= 0f || vistaPausa.layout.height <= 0f)
        {
            return;
        }

        vistaPausa.style.left = (superposicionPausa.layout.width - vistaPausa.layout.width) / 2f;
        vistaPausa.style.top = (superposicionPausa.layout.height - vistaPausa.layout.height) / 2f;
    }

    private void reanudarJuego()
    {
        estaPausado = false;
        enConfiguracion = false;
        Time.timeScale = 1f;
        UnityEngine.Cursor.visible = cursorVisibleAnterior;
        UnityEngine.Cursor.lockState = bloqueoCursorAnterior;
        superposicionPausa.style.display = DisplayStyle.None;
    }

    private void mostrarConfiguracion()
    {
        VisualElement vistaConfiguracion = menuConfiguracion.Instantiate();
        VisualElement raizConfiguracion = vistaConfiguracion.Q<VisualElement>("root");
        VisualElement panelConfiguracion = vistaConfiguracion.Q<VisualElement>("settings-panel");
        Button botonVolver = vistaConfiguracion.Q<Button>("btn-back");

        if (raizConfiguracion == null || panelConfiguracion == null || botonVolver == null)
        {
            Debug.LogError("Faltan elementos necesarios en MenuConfigurer.uxml.", this);
            return;
        }

        configurarOpciones(vistaConfiguracion);
        vistaConfiguracion.style.flexGrow = 1;
        panelConfiguracion.style.display = DisplayStyle.Flex;
        raizConfiguracion.AddToClassList("root--visible");
        botonVolver.clicked += mostrarMenuPausa;

        raizPausa.Clear();
        raizPausa.Add(vistaConfiguracion);
        enConfiguracion = true;
    }

    private void configurarOpciones(VisualElement vistaConfiguracion)
    {
        Slider controlVolumen = vistaConfiguracion.Q<Slider>("slider-volume");
        Toggle controlPantallaCompleta = vistaConfiguracion.Q<Toggle>("toggle-fullscreen");
        DropdownField controlResolucion = vistaConfiguracion.Q<DropdownField>("dropdown-resolution");
        DropdownField controlFps = vistaConfiguracion.Q<DropdownField>("dropdown-fps");

        if (controlVolumen == null || controlPantallaCompleta == null ||
            controlResolucion == null || controlFps == null)
        {
            Debug.LogError("Faltan controles de configuración en MenuConfigurer.uxml.", this);
            return;
        }

        List<string> opcionesResolucion = new List<string>();
        foreach (Vector2Int resolucion in resoluciones)
        {
            opcionesResolucion.Add(resolucion.x + " x " + resolucion.y);
        }

        controlResolucion.choices = opcionesResolucion;
        controlFps.choices = new List<string> { "30 FPS", "60 FPS", "120 FPS", "Sin límite" };

        int indiceResolucion = Mathf.Clamp(
            PlayerPrefs.GetInt(CLAVE_RESOLUCION, obtenerResolucionActual()),
            0,
            resoluciones.Length - 1);
        int indiceFps = Mathf.Clamp(PlayerPrefs.GetInt(CLAVE_FPS, 1), 0, limitesFps.Length - 1);
        float volumen = Mathf.Clamp01(PlayerPrefs.GetFloat(CLAVE_VOLUMEN, AudioListener.volume));
        bool pantallaCompleta = PlayerPrefs.GetInt(
            CLAVE_PANTALLA_COMPLETA,
            Screen.fullScreen ? 1 : 0) == 1;

        controlVolumen.SetValueWithoutNotify(volumen);
        controlResolucion.SetValueWithoutNotify(opcionesResolucion[indiceResolucion]);
        controlFps.SetValueWithoutNotify(controlFps.choices[indiceFps]);
        controlPantallaCompleta.SetValueWithoutNotify(pantallaCompleta);

        controlVolumen.RegisterValueChangedCallback(evt => aplicarVolumen(evt.newValue));
        controlResolucion.RegisterValueChangedCallback(evt =>
        {
            int nuevoIndice = controlResolucion.choices.IndexOf(evt.newValue);
            if (nuevoIndice >= 0)
            {
                aplicarResolucion(nuevoIndice, controlPantallaCompleta.value);
            }
        });
        controlPantallaCompleta.RegisterValueChangedCallback(evt =>
            aplicarResolucion(controlResolucion.choices.IndexOf(controlResolucion.value), evt.newValue));
        controlFps.RegisterValueChangedCallback(evt =>
        {
            int nuevoIndice = controlFps.choices.IndexOf(evt.newValue);
            if (nuevoIndice >= 0)
            {
                aplicarFps(nuevoIndice);
            }
        });
    }

    private void aplicarVolumen(float volumen)
    {
        AudioListener.volume = Mathf.Clamp01(volumen);
        PlayerPrefs.SetFloat(CLAVE_VOLUMEN, AudioListener.volume);
    }

    private void aplicarResolucion(int indice, bool pantallaCompleta)
    {
        indice = Mathf.Clamp(indice, 0, resoluciones.Length - 1);
        Vector2Int resolucion = resoluciones[indice];
        FullScreenMode modo = pantallaCompleta
            ? FullScreenMode.FullScreenWindow
            : FullScreenMode.Windowed;

        Screen.SetResolution(resolucion.x, resolucion.y, modo);
        PlayerPrefs.SetInt(CLAVE_RESOLUCION, indice);
        PlayerPrefs.SetInt(CLAVE_PANTALLA_COMPLETA, pantallaCompleta ? 1 : 0);
    }

    private void aplicarFps(int indice)
    {
        indice = Mathf.Clamp(indice, 0, limitesFps.Length - 1);
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = limitesFps[indice];
        PlayerPrefs.SetInt(CLAVE_FPS, indice);
    }

    private int obtenerResolucionActual()
    {
        int indiceMasCercano = 0;
        int menorDiferencia = int.MaxValue;
        for (int i = 0; i < resoluciones.Length; i++)
        {
            int diferencia = Mathf.Abs(Screen.width - resoluciones[i].x) +
                             Mathf.Abs(Screen.height - resoluciones[i].y);
            if (diferencia < menorDiferencia)
            {
                menorDiferencia = diferencia;
                indiceMasCercano = i;
            }
        }

        return indiceMasCercano;
    }

    private void volverAlMenuPrincipal()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(escenaMenuPrincipal);
    }

    private void OnDestroy()
    {
        if (estaPausado)
        {
            Time.timeScale = 1f;
            UnityEngine.Cursor.visible = cursorVisibleAnterior;
            UnityEngine.Cursor.lockState = bloqueoCursorAnterior;
        }

        if (documento != null)
        {
            Destroy(documento.gameObject);
        }

        accionPausa?.Dispose();
    }
}
