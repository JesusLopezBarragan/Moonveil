using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

[RequireComponent(typeof(PanelRenderer))]
public class FuncionalidadBotonesInterface : MonoBehaviour
{
    [SerializeField] private string SceneVideojuego;
    [SerializeField] private VisualTreeAsset menuConfiguracion;
    private VisualElement vistaMenuPrincipal; // La vista de el archivo uxml de menuprincipal
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

    private void Awake() //Hace que se ejecute solo una vez cuando se le llama.
    {
        aplicarPreferenciasGuardadas();
        GetComponent<PanelRenderer>().RegisterUIReloadCallback(cargaArchivo); //Hace que el panel se carga junto a los botones.
    }

    private void OnDestroy()
    {
        //Cuando se cambie de escena Unity de por si destruira este GameObject.
        GetComponent<PanelRenderer>().UnregisterUIReloadCallback(cargaArchivo); //Hace que no se pueda volver a llamada al metodo cargaArchivo();
    }
    
    private void cargaArchivo(PanelRenderer panelRenderer, VisualElement root)
    {
        vistaMenuPrincipal = root.Q<VisualElement>("root");// Se guarda el menú original antes del cambio (En este momento Opacity = 0)
        if (vistaMenuPrincipal != null)
        {
            vistaMenuPrincipal.AddToClassList("root--visible"); //Muestra el menu (En este momento Opacity = 1)
        }

        root.Q<Button>("btn-play").clicked += () => cargarJuego();
        root.Q<Button>("btn-settings").clicked += () => mostrarConfiguracion(root);
        root.Q<Button>("btn-quit").clicked     += salirJuego;
    }

    private void cargarJuego()
    {
        SceneManager.LoadScene(SceneVideojuego);
    }

    private void mostrarConfiguracion(VisualElement root)
    {
        // Sin una plantilla asignada no se puede construir la vista de configuración.
        if (menuConfiguracion == null)
        {
            //Contexto del error.
            Debug.LogError("No se ha asignado MenuConfigurer.uxml al componente FuncionalidadBotonesInterface.", this);
            return;
        }

        // Instantiate crea los elementos definidos en MenuConfigurer.uxml.
        VisualElement vistaConfiguracion = menuConfiguracion.Instantiate();
        VisualElement raizConfiguracion = vistaConfiguracion.Q<VisualElement>("root");
        VisualElement panelConfiguracion = vistaConfiguracion.Q<VisualElement>("settings-panel");
        Button botonVolver = vistaConfiguracion.Q<Button>("btn-back");
        Slider controlVolumen = vistaConfiguracion.Q<Slider>("slider-volume");
        Toggle controlPantallaCompleta = vistaConfiguracion.Q<Toggle>("toggle-fullscreen");
        DropdownField controlResolucion = vistaConfiguracion.Q<DropdownField>("dropdown-resolution");
        DropdownField controlFps = vistaConfiguracion.Q<DropdownField>("dropdown-fps");

        // Se comprueba que los nombres consultados coincidan con los del archivo UXML.
        // Esto es una buena practica.
        if (vistaMenuPrincipal == null || raizConfiguracion == null || panelConfiguracion == null ||
            botonVolver == null || controlVolumen == null || controlPantallaCompleta == null ||
            controlResolucion == null || controlFps == null)
        {
            Debug.LogError("No se han encontrado los elementos necesarios en los UXML del menú.", this);
            return;
        }

        configurarOpciones(controlVolumen, controlPantallaCompleta, controlResolucion, controlFps);

        // La plantilla ocupa el espacio disponible y se muestra su panel, que en el UXML empieza oculto.
        vistaConfiguracion.style.flexGrow = 1;
        panelConfiguracion.style.display = DisplayStyle.Flex;
        
        // MainMenu.uss deja .root transparente; esta clase vuelve visible el fondo y el contenido.
        raizConfiguracion.AddToClassList("root--visible");
        botonVolver.clicked += () =>
        {
            // Se quita la configuración y se vuelve a colocar el menú original.
            root.Clear();
            root.Add(vistaMenuPrincipal);
        };

        // Reemplaza la vista actual por la nueva configuración.
        root.Clear();
        root.Add(vistaConfiguracion);
    }

    private void configurarOpciones(
        Slider controlVolumen,
        Toggle controlPantallaCompleta,
        DropdownField controlResolucion,
        DropdownField controlFps)
    {
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
        bool pantallaCompleta = PlayerPrefs.GetInt(CLAVE_PANTALLA_COMPLETA, Screen.fullScreen ? 1 : 0) == 1;

        controlVolumen.SetValueWithoutNotify(volumen);
        controlResolucion.SetValueWithoutNotify(opcionesResolucion[indiceResolucion]);
        controlFps.SetValueWithoutNotify(controlFps.choices[indiceFps]);
        controlPantallaCompleta.SetValueWithoutNotify(pantallaCompleta);

        aplicarVolumen(volumen);
        aplicarFps(indiceFps);

        if (PlayerPrefs.HasKey(CLAVE_RESOLUCION) || PlayerPrefs.HasKey(CLAVE_PANTALLA_COMPLETA))
        {
            aplicarResolucion(indiceResolucion, pantallaCompleta);
        }

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
            aplicarResolucion(
                controlResolucion.choices.IndexOf(controlResolucion.value),
                evt.newValue));
        controlFps.RegisterValueChangedCallback(evt =>
        {
            int nuevoIndice = controlFps.choices.IndexOf(evt.newValue);
            if (nuevoIndice >= 0)
            {
                aplicarFps(nuevoIndice);
            }
        });
    }

    private void aplicarPreferenciasGuardadas()
    {
        if (PlayerPrefs.HasKey(CLAVE_VOLUMEN))
        {
            AudioListener.volume = Mathf.Clamp01(PlayerPrefs.GetFloat(CLAVE_VOLUMEN));
        }

        if (PlayerPrefs.HasKey(CLAVE_FPS))
        {
            aplicarFps(Mathf.Clamp(PlayerPrefs.GetInt(CLAVE_FPS), 0, limitesFps.Length - 1));
        }

        if (PlayerPrefs.HasKey(CLAVE_RESOLUCION) || PlayerPrefs.HasKey(CLAVE_PANTALLA_COMPLETA))
        {
            int indiceResolucion = Mathf.Clamp(
                PlayerPrefs.GetInt(CLAVE_RESOLUCION, obtenerResolucionActual()),
                0,
                resoluciones.Length - 1);
            bool pantallaCompleta = PlayerPrefs.GetInt(
                CLAVE_PANTALLA_COMPLETA,
                Screen.fullScreen ? 1 : 0) == 1;
            aplicarResolucion(indiceResolucion, pantallaCompleta);
        }
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

    private void salirJuego()
    {
        Application.Quit();
        //Las siguientes lineas son para comprobar en el editor si el boton funciona de verdad. Aunque solo con Aplication.Quit(); funcionaría.
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
