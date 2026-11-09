using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// Guarda las herramientas del jugador y cuál tiene seleccionada.
// Se cambia con las teclas 1-5 o con la rueda del ratón.
public class InventarioHerramientas : MonoBehaviour
{
    public List<Herramienta> herramientas = new List<Herramienta>();
    public int seleccionada = 0;

    void Update()
    {
        // 1. Teclas numéricas: cada número elige una casilla de la barra
        Keyboard teclado = Keyboard.current;
        if (teclado != null)
        {
            if (teclado.digit1Key.wasPressedThisFrame) Seleccionar(0);
            if (teclado.digit2Key.wasPressedThisFrame) Seleccionar(1);
            if (teclado.digit3Key.wasPressedThisFrame) Seleccionar(2);
            if (teclado.digit4Key.wasPressedThisFrame) Seleccionar(3);
            if (teclado.digit5Key.wasPressedThisFrame) Seleccionar(4);
        }

        // 2. Rueda del ratón: hacia abajo pasa a la siguiente, hacia arriba a la anterior
        Mouse raton = Mouse.current;
        if (raton != null)
        {
            float rueda = raton.scroll.ReadValue().y;
            if (rueda < 0) Seleccionar(seleccionada + 1);
            else if (rueda > 0) Seleccionar(seleccionada - 1);
        }
    }

    public void Seleccionar(int indice)
    {
        if (herramientas.Count == 0) return;

        // Si nos pasamos por un lado aparecemos por el otro (la barra es "circular")
        if (indice < 0) indice = herramientas.Count - 1;
        if (indice >= herramientas.Count) indice = 0;

        seleccionada = indice;
    }

    public Herramienta HerramientaActual()
    {
        return herramientas[seleccionada];
    }
}
