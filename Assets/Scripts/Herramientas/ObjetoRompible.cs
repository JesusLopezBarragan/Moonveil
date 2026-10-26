using System.Collections;
using UnityEngine;

// Algo del mapa que se puede romper con una herramienta (rocas, tocones, árboles, hierba...).
// Cada golpe le quita una "vida". Cuando llega a 0, desaparece con una pequeña animación
// y deja un objeto en la mochila.
// Los árboles tienen DOS fases: primero se quedan en un tronco cortado y después desaparecen.
public class ObjetoRompible : MonoBehaviour
{
    public TipoHerramienta herramientaNecesaria = TipoHerramienta.Pico;
    public int golpes = 3;

    [Header("Lo que se guarda en la mochila al romperlo")]
    public string objetoQueDa = "";   // por ejemplo "Madera" o "Piedra" ("" = nada)
    public int cantidadQueDa = 1;

    [Header("Solo para árboles")]
    public Sprite spriteTronco;      // si tiene sprite, primero se convierte en tronco
    public int golpesTronco = 2;     // golpes que aguanta el tronco

    private bool temblando = false;
    private bool roto = false;

    public void Golpear()
    {
        if (roto) return;

        golpes--;

        // Todavía le quedan golpes: solo tiembla
        if (golpes > 0)
        {
            if (!temblando) StartCoroutine(Temblar());
            return;
        }

        // Se ha quedado sin golpes y es un árbol entero -> FASE 1: se queda en tronco
        if (spriteTronco != null)
        {
            GetComponent<SpriteRenderer>().sprite = spriteTronco;
            spriteTronco = null;      // ya no es un árbol: la próxima vez se rompe del todo
            golpes = golpesTronco;
            DarObjeto();
            if (!temblando) StartCoroutine(Temblar());
            return;
        }

        // FASE FINAL: se rompe y desaparece
        roto = true;
        DarObjeto();
        StartCoroutine(Romper());
    }

    void DarObjeto()
    {
        if (objetoQueDa != "" && Inventario.instancia != null)
        {
            Inventario.instancia.Guardar(objetoQueDa, cantidadQueDa);
        }
    }

    // Una corrutina es una función que se puede repartir entre varios frames (yield return null = "espera al siguiente frame")
    IEnumerator Temblar()
    {
        temblando = true;
        Vector3 posicionInicial = transform.position;
        float tiempo = 0f;

        while (tiempo < 0.15f)
        {
            tiempo += Time.deltaTime;
            float desplazamiento = Mathf.Sin(tiempo * 80f) * 0.06f;
            transform.position = posicionInicial + new Vector3(desplazamiento, 0f, 0f);
            yield return null;
        }

        transform.position = posicionInicial;
        temblando = false;
    }

    IEnumerator Romper()
    {
        // Quitamos las colisiones para que el jugador pueda pasar al momento
        foreach (Collider2D c in GetComponentsInChildren<Collider2D>())
        {
            c.enabled = false;
        }

        // Se va haciendo pequeño hasta desaparecer
        Vector3 escalaInicial = transform.localScale;
        float tiempo = 0f;
        while (tiempo < 0.2f)
        {
            tiempo += Time.deltaTime;
            transform.localScale = escalaInicial * (1f - tiempo / 0.2f);
            yield return null;
        }

        Destroy(gameObject);
    }
}
