using UnityEngine;
using UnityEngine.InputSystem;

public class mover : MonoBehaviour
{
    
    GameObject gameobject;    
    void Start()
    {
        gameobject = gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.dKey.isPressed) gameobject.transform.Translate(new Vector2(0.2f,0f));
        if (Keyboard.current.aKey.isPressed) gameobject.transform.Translate(new Vector2(-0.2f,0f));
    }
}
