using System.Collections.Generic;
using UnityEngine;

// Punto de control: al acercarse Lumo guarda el progreso (si cae al agua vuelve aqui).
public class Checkpoint : MonoBehaviour
{
    public SpriteRenderer luz;
    bool active;

    void Update()
    {
        if (LumoPlayer.I == null || GameManager.I == null) return;
        Vector2 d = LumoPlayer.I.transform.position - transform.position;
        if (!active && d.sqrMagnitude < 2.2f * 2.2f)
        {
            active = true;
            GameManager.I.checkpoint = transform.position;
            GameManager.I.ShowMessage("Punto de control guardado", 2f);
        }
        if (luz != null)
        {
            Color c = luz.color;
            float target = active ? 0.85f : 0.25f;
            c.a = Mathf.Lerp(c.a, target, 1f - Mathf.Exp(-5f * Time.deltaTime));
            luz.color = c;
        }
    }
}
