using UnityEngine;

// La mochila del jugador: 5 filas x 5 columnas = 25 huecos.
// En cada hueco caben hasta 99 objetos iguales (como en Stardew Valley).
public class Inventario : MonoBehaviour
{
    // static: así cualquier script puede hacer Inventario.instancia.Guardar(...)
    public static Inventario instancia;

    public const int FILAS = 5;
    public const int COLUMNAS = 5;
    public int maximoPorRanura = 99;

    public Ranura[] ranuras = new Ranura[FILAS * COLUMNAS];

    void Awake()
    {
        instancia = this;

        // Al principio todos los huecos están vacíos
        for (int i = 0; i < ranuras.Length; i++)
        {
            ranuras[i] = new Ranura();
        }
    }

    // Mete objetos en la mochila. Devuelve false si no caben todos.
    public bool Guardar(string nombre, int cantidad)
    {
        while (cantidad > 0)
        {
            int i = BuscarRanura(nombre);
            if (i == -1)
            {
                Debug.Log("La mochila está llena");
                return false;
            }

            ranuras[i].nombre = nombre;

            // Metemos lo que quepa en ese hueco; si sobra, la vuelta siguiente busca otro hueco
            int cabe = maximoPorRanura - ranuras[i].cantidad;
            int meter = Mathf.Min(cabe, cantidad);
            ranuras[i].cantidad += meter;
            cantidad -= meter;
        }
        return true;
    }

    // Busca dónde meter un objeto: primero un hueco con el mismo objeto que no esté lleno,
    // y si no hay, el primer hueco vacío. Si no hay ninguno devuelve -1.
    int BuscarRanura(string nombre)
    {
        for (int i = 0; i < ranuras.Length; i++)
        {
            if (ranuras[i].nombre == nombre && ranuras[i].cantidad < maximoPorRanura) return i;
        }
        for (int i = 0; i < ranuras.Length; i++)
        {
            if (ranuras[i].EstaVacia()) return i;
        }
        return -1;
    }

    // Cuenta cuántos objetos de un tipo hay en total en la mochila
    public int Contar(string nombre)
    {
        int total = 0;
        for (int i = 0; i < ranuras.Length; i++)
        {
            if (ranuras[i].nombre == nombre) total += ranuras[i].cantidad;
        }
        return total;
    }
}
