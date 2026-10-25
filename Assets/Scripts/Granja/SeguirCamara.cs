using UnityEngine;

// La cámara sigue al jugador de forma suave sin salirse del mapa.
public class SeguirCamara : MonoBehaviour
{
    public Transform jugador;
    public float suavizado = 5f;

    // Límites para que la cámara no enseñe lo que hay fuera del mapa
    public float minX = 10.7f;
    public float maxX = 85.3f;
    public float minY = 6f;
    public float maxY = 66f;

    void LateUpdate()
    {
        Vector3 destino = new Vector3(jugador.position.x, jugador.position.y, -10f);

        destino.x = Mathf.Clamp(destino.x, minX, maxX);
        destino.y = Mathf.Clamp(destino.y, minY, maxY);

        transform.position = Vector3.Lerp(transform.position, destino, suavizado * Time.deltaTime);
    }
}
