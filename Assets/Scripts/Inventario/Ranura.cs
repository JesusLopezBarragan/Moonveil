// Un hueco del inventario: qué objeto hay y cuántos.
// Si nombre está vacío ("") es que el hueco está libre.
[System.Serializable]
public class Ranura
{
    public string nombre = "";
    public int cantidad = 0;

    public bool EstaVacia()
    {
        return nombre == "";
    }
}
