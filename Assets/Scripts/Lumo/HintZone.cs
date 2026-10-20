using System.Collections.Generic;
using UnityEngine;

// Zona que muestra un consejo de Tia Vela cuando Lumo esta cerca.
public class HintZone : MonoBehaviour
{
    public string text;
    public float radius = 4f;
    public int priority = 1;
    public GameObject hideIfInactive;   // si este objeto desaparece (ej. la barrera), el consejo ya no se muestra

    void Update()
    {
        if (LumoPlayer.I == null || GameManager.I == null) return;
        if (GameManager.I.Finished) return;
        if (hideIfInactive != null && !hideIfInactive.activeInHierarchy) return;
        Vector2 d = LumoPlayer.I.transform.position - transform.position;
        if (d.sqrMagnitude < radius * radius)
            GameManager.I.SetHint(text, priority);
    }
}
