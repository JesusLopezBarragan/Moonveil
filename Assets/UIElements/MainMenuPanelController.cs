using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

[RequireComponent(typeof(PanelRenderer))]
public class MainMenuPanelController : MonoBehaviour
{
    [SerializeField] private string gameSceneName;

    private void Awake() //Hace que se ejecute solo una vez cuando se le llama.
    {
        GetComponent<PanelRenderer>().RegisterUIReloadCallback(cargarBotones); //Hace que el panel se carga junto a los botones.
    }

    private void OnDestroy()
    {
        //Cuando se cambie de escena Unity de por si destruira este GameObject.
        GetComponent<PanelRenderer>().UnregisterUIReloadCallback(cargarBotones); //Hace que no se pueda volver a llamada al metodo cargarBotones();
    }
    
    private void cargarBotones(PanelRenderer panelRenderer, VisualElement root, int version)
    {
        root.Q<Button>("btn-play").clicked     += () => /*SceneManager.LoadScene(gameSceneName);*/ Debug.Log("Proximamente");
        root.Q<Button>("btn-settings").clicked += () => Debug.Log("Proximamente");
        root.Q<Button>("btn-quit").clicked     += salirJuego;
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
