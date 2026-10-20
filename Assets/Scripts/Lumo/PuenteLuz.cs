using System.Collections.Generic;
using UnityEngine;

// Casilla de puente de luz (Nivel 2). Solo existe (es "solida") dentro del radio de luz de Lumo.
// Si requiereDestello esta activo, ademas solo aparece mientras Lumo mantiene el Destello.
public class PuenteLuz : MonoBehaviour
{
    public static readonly List<PuenteLuz> All = new List<PuenteLuz>();

    public bool requiereDestello;

    SpriteRenderer sr;
    Color baseColor;
    float alpha;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        baseColor = sr.color;
        baseColor.a = 1f;
        // En el editor se ve completo; al jugar empieza invisible y aparece con la luz.
        sr.color = new Color(baseColor.r, baseColor.g, baseColor.b, 0f);
    }

    void OnEnable() { All.Add(this); }
    void OnDisable() { All.Remove(this); }

    // Se calcula en el momento (no se guarda), asi nunca queda desfasado
    public bool Solid
    {
        get
        {
            LumoPlayer p = LumoPlayer.I;
            if (p == null) return false;
            float d = Vector2.Distance(transform.position, p.transform.position);
            return d < p.CurrentRadius && (!requiereDestello || p.DestelloActive);
        }
    }

    void Update()
    {
        LumoPlayer p = LumoPlayer.I;
        if (p == null) return;
        float d = Vector2.Distance(transform.position, p.transform.position);
        bool inLight = d < p.CurrentRadius;

        float target = Solid ? 1f : ((inLight && requiereDestello) ? 0.22f : 0f);
        alpha = Mathf.Lerp(alpha, target, 1f - Mathf.Exp(-14f * Time.deltaTime));
        Color c = baseColor;
        c.a = alpha;
        sr.color = c;
    }

    // true si la posicion pos esta sobre esta casilla (con margen) y la casilla es solida
    public bool Covers(Vector2 pos)
    {
        Vector2 t = transform.position;
        if (Mathf.Abs(pos.x - t.x) >= 0.8f || Mathf.Abs(pos.y - t.y) >= 0.8f) return false;
        return Solid;
    }
}
