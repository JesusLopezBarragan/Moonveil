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
    // Referencias a los controles de creacion definidos en MainMenu.uxml.
    private VisualElement contenidoMenu;
    private VisualElement pantallaCreacion;
    private VisualElement vistaPreviaPersonaje;
    private TextField campoNombrePersonaje;
    private Label nombreVistaPrevia;
    private Label aspectoVistaPrevia;
    private Label estadoSeleccion;
    private Button[] botonesAspecto;
    private Button[] botonesPredeterminados;
    private int aspectoSeleccionado;
    private int personajePredeterminado = -1;
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
    private const string CLAVE_ASPECTO_PERSONAJE = "moonveil_personaje_aspecto";
    private const string CLAVE_PERSONAJE_PREDETERMINADO = "moonveil_personaje_predeterminado";
    private static readonly string[] nombresAspectos = { "Natural", "Bosque", "Luna" };

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
        if (vistaMenuPrincipal == null)
        {
            Debug.LogError("No se ha encontrado el elemento raíz del menú.", this);
            return;
        }

        vistaMenuPrincipal.AddToClassList("root--visible"); //Muestra el menu (En este momento Opacity = 1)
        // Los nombres deben coincidir con los atributos name definidos en el UXML.
        contenidoMenu = vistaMenuPrincipal.Q<VisualElement>("menu-content");
        pantallaCreacion = vistaMenuPrincipal.Q<VisualElement>("creation-screen");
        campoNombrePersonaje = vistaMenuPrincipal.Q<TextField>("character-name");
        vistaPreviaPersonaje = vistaMenuPrincipal.Q<VisualElement>("character-preview");
        nombreVistaPrevia = vistaMenuPrincipal.Q<Label>("preview-name");
        aspectoVistaPrevia = vistaMenuPrincipal.Q<Label>("preview-appearance");
        estadoSeleccion = vistaMenuPrincipal.Q<Label>("selection-status");
        Button botonJugar = vistaMenuPrincipal.Q<Button>("btn-play");
        Button botonConfiguracion = vistaMenuPrincipal.Q<Button>("btn-settings");
        Button botonSalir = vistaMenuPrincipal.Q<Button>("btn-quit");

        if (contenidoMenu == null || pantallaCreacion == null || campoNombrePersonaje == null ||
            vistaPreviaPersonaje == null || nombreVistaPrevia == null || aspectoVistaPrevia == null ||
            estadoSeleccion == null || botonJugar == null || botonConfiguracion == null || botonSalir == null)
        {
            Debug.LogError("Faltan elementos necesarios en MainMenu.uxml.", this);
            return;
        }

        botonJugar.clicked += mostrarCreacionPersonaje;
        botonConfiguracion.clicked += () => mostrarConfiguracion(root);
        botonSalir.clicked += salirJuego;
        configurarCreacionPersonaje();
    }

    private void configurarCreacionPersonaje()
    {
        botonesAspecto = new Button[nombresAspectos.Length];
        for (int i = 0; i < botonesAspecto.Length; i++)
        {
            // Copia el indice para que cada callback conserve su boton correspondiente.
            int indice = i;
            botonesAspecto[i] = pantallaCreacion.Q<Button>("appearance-" + indice);
            if (botonesAspecto[i] == null)
            {
                Debug.LogError("Falta el botón para el aspecto " + indice + " en MainMenu.uxml.", this);
                return;
            }

            botonesAspecto[i].clicked += () => seleccionarAspecto(indice);
        }

        botonesPredeterminados = new Button[5];
        for (int i = 0; i < botonesPredeterminados.Length; i++)
        {
            // Las tarjetas se identifican con indices de 0 a 4 en el UXML.
            int indice = i;
            botonesPredeterminados[i] = pantallaCreacion.Q<Button>("btn-preset-" + indice);
            if (botonesPredeterminados[i] == null)
            {
                Debug.LogError("Falta el espacio del personaje predeterminado " + indice + " en MainMenu.uxml.", this);
                return;
            }

            botonesPredeterminados[i].clicked += () => seleccionarPersonajePredeterminado(indice);
        }

        campoNombrePersonaje.RegisterValueChangedCallback(_ =>
        {
            // Si se cambia el nombre de ejemplo, ya no se considera elegida esa plantilla.
            personajePredeterminado = -1;
            foreach (Button boton in botonesPredeterminados)
            {
                boton.RemoveFromClassList("preset-card--selected");
            }

            actualizarVistaPrevia();
        });

        Button botonCancelar = pantallaCreacion.Q<Button>("btn-cancel-creation");
        Button botonCrear = pantallaCreacion.Q<Button>("btn-create-character");
        if (botonCancelar == null || botonCrear == null)
        {
            Debug.LogError("Faltan los botones para volver o empezar en MainMenu.uxml.", this);
            return;
        }

        botonCancelar.clicked += volverAlMenu;
        botonCrear.clicked += confirmarPersonaje;
        seleccionarAspecto(0);
    }

    private void mostrarCreacionPersonaje()
    {
        // Ambas vistas comparten el arbol visual; se alterna cual esta visible.
        contenidoMenu.style.display = DisplayStyle.None;
        pantallaCreacion.style.display = DisplayStyle.Flex;
        actualizarVistaPrevia();
    }

    private void volverAlMenu()
    {
        pantallaCreacion.style.display = DisplayStyle.None;
        contenidoMenu.style.display = DisplayStyle.Flex;
    }

    private void seleccionarAspecto(int indice)
    {
        aspectoSeleccionado = indice;
        for (int i = 0; i < botonesAspecto.Length; i++)
        {
            botonesAspecto[i].EnableInClassList("appearance-button--selected", i == indice);
        }

        // Se deja una sola clase de tono para que USS actualice el color de la vista previa.
        vistaPreviaPersonaje.RemoveFromClassList("character-preview--0");
        vistaPreviaPersonaje.RemoveFromClassList("character-preview--1");
        vistaPreviaPersonaje.RemoveFromClassList("character-preview--2");
        vistaPreviaPersonaje.AddToClassList("character-preview--" + indice);
        actualizarVistaPrevia();
    }

    private void seleccionarPersonajePredeterminado(int indice)
    {
        // Cambiar el TextField dispara su callback, por eso se asigna el indice despues.
        campoNombrePersonaje.value = "Aldeano " + (indice + 1);
        personajePredeterminado = indice;
        seleccionarAspecto(0);

        for (int i = 0; i < botonesPredeterminados.Length; i++)
        {
            botonesPredeterminados[i].EnableInClassList("preset-card--selected", i == indice);
        }

        estadoSeleccion.text = "Plantilla " + (indice + 1) + " seleccionada. Puedes cambiar su nombre.";
    }

    private void actualizarVistaPrevia()
    {
        string nombre = campoNombrePersonaje.value.Trim();
        nombreVistaPrevia.text = string.IsNullOrEmpty(nombre) ? "Tu personaje" : nombre;
        aspectoVistaPrevia.text = "Apariencia: " + nombresAspectos[aspectoSeleccionado];
    }

    private void confirmarPersonaje()
    {
        // Elimina espacios sobrantes y evita iniciar con un nombre vacio.
        string nombre = campoNombrePersonaje.value.Trim();
        if (string.IsNullOrEmpty(nombre))
        {
            estadoSeleccion.text = "Escribe un nombre antes de continuar.";
            return;
        }

        // PlayerPrefs conserva la seleccion al cambiar de escena.
        PlayerPrefs.SetString(CLAVE_NOMBRE_PERSONAJE, nombre);
        PlayerPrefs.SetInt(CLAVE_ASPECTO_PERSONAJE, aspectoSeleccionado);
        PlayerPrefs.SetInt(CLAVE_PERSONAJE_PREDETERMINADO, personajePredeterminado);
        PlayerPrefs.Save();
        vistaMenuPrincipal.style.display = DisplayStyle.None; //Escondemos el menú.
        PantallaCarga.Empezar(SceneVideojuego); //La pantalla de carga se encarga de cargar la escena del juego.
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
