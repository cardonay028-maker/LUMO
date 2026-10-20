using System.Collections.Generic;
using UnityEngine;

// Flor de Luz: recarga el Brillo cuando Lumo esta cerca. Brilla incluso en la oscuridad.
public class FlorDeLuz : MonoBehaviour
{
    public float radius = 2.2f;
    public float rechargePerSecond = 45f;
    public Transform glow;
    public float baseGlow = 1.6f;

    void Update()
    {
        if (LumoPlayer.I == null) return;
        Vector2 d = LumoPlayer.I.transform.position - transform.position;
        bool near = d.sqrMagnitude < radius * radius;
        if (near) LumoPlayer.I.Recharge(rechargePerSecond * Time.deltaTime);
        if (glow != null)
        {
            float target = near ? baseGlow * 1.5f : baseGlow;
            float s = Mathf.Lerp(glow.localScale.x, target, 1f - Mathf.Exp(-8f * Time.deltaTime));
            glow.localScale = new Vector3(s, s, 1f);
        }
    }
}
