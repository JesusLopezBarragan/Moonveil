using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform objetivo;
    [SerializeField] private float suavizadoPosicion = 0.15f;
    [SerializeField] private Vector3 offset = new Vector3(0f, 0f, -10f);

    [SerializeField] private float tamanoNormal = 5f;
    [SerializeField] private float tamanoSprint = 6.5f;
    [SerializeField] private float velocidadZoom = 3f;

    private Camera cam;
    private PlayerSprint sprint;
    private Vector3 velocidad = Vector3.zero;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        sprint = Object.FindAnyObjectByType<PlayerSprint>();
        cam.orthographicSize = tamanoNormal;
    }

    private void LateUpdate()
    {
        if (objetivo == null) return;

        // Movimiento
        Vector3 posicionObjetivo = objetivo.position + offset;
        transform.position = Vector3.SmoothDamp(transform.position, posicionObjetivo, ref velocidad, suavizadoPosicion);

        // Zoom
       
        float tamanoObjetivo;

        if (sprint.esprintando)
            tamanoObjetivo = tamanoSprint;
        else
            tamanoObjetivo = tamanoNormal;

        cam.orthographicSize = Mathf.MoveTowards(cam.orthographicSize, tamanoObjetivo, velocidadZoom * Time.deltaTime);
    }
}