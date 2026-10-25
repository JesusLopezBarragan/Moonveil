using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

[RequireComponent(typeof(PanelRenderer))]
public class FuncionalidadBotonesInterface : MonoBehaviour
{
    [SerializeField] private string SceneVideojuego;
    [SerializeField] private VisualTreeAsset menuConfiguracion;
    private VisualElement vistaMenuPrincipal; // La vista de el archivo uxml de menuprincipal

    private void Awake() //Hace que se ejecute solo una vez cuando se le llama.
    {
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

        // Se comprueba que los nombres consultados coincidan con los del archivo UXML.
        // Esto es una buena practica.
        if (vistaMenuPrincipal == null || raizConfiguracion == null || panelConfiguracion == null || botonVolver == null)
        {
            Debug.LogError("No se han encontrado los elementos necesarios en los UXML del menú.", this);
            return;
        }

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

    private void salirJuego()
    {
        Application.Quit();
        //Las siguientes lineas son para comprobar en el editor si el boton funciona de verdad. Aunque solo con Aplication.Quit(); funcionaría.
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
