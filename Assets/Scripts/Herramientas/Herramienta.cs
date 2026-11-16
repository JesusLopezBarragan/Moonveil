using UnityEngine;

// Datos de una herramienta: su nombre, su tipo y el dibujo (icono) que sale en la barra.
// [System.Serializable] hace que se pueda ver y editar en el Inspector de Unity.
[System.Serializable]
public class Herramienta
{
    public string nombre;
    public TipoHerramienta tipo;
    public Sprite icono;

    // Constructor: sirve para crear una herramienta rellenando sus datos de golpe
    public Herramienta(string nombre, TipoHerramienta tipo, Sprite icono)
    {
        this.nombre = nombre;
        this.tipo = tipo;
        this.icono = icono;
    }
}
