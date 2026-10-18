using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public class FrasesData //Clase contenedora para recoger los datos de JSON.
{
    public string[] frases;
}

public class MostrarFrases : MonoBehaviour
{
    
    void Awake()
    {
        GetComponent<PanelRenderer>().RegisterUIReloadCallback(cargarTexto);
    }
    
    void OnDestroy()
    {
        GetComponent<PanelRenderer>().UnregisterUIReloadCallback(cargarTexto);
    }

    private void cargarTexto(PanelRenderer panelRenderer, VisualElement root)
    {
        root.Q<Label>("subtitle").text = devolverFrase();
    }

    private string devolverFrase()
    {
        TextAsset archivo = Resources.Load<TextAsset>("Frases"); //Convierte lo que hay en Frases.json en un string

        if (archivo != null)
        {
            FrasesData datos = JsonUtility.FromJson<FrasesData>(archivo.text); //Aquí convierte el string a un objeto para que se puedan utilizar todas las frases.
            return datos.frases[Random.Range(0, datos.frases.Length)]; //Aquí devulve la lsita de todas las frases del archivo.
        }

        return null;
    }
}
