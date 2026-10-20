using System.Collections.Generic;
using UnityEngine;

// Farol Ancestral: meta del nivel. Se enciende con E.
public class Farol : MonoBehaviour
{
    public SpriteRenderer lamp;
    public SpriteRenderer glow;
    public bool lit;
    float litTime;

    void Update()
    {
        if (LumoPlayer.I == null || GameManager.I == null) return;

        if (!lit)
        {
            Vector2 d = LumoPlayer.I.transform.position - transform.position;
            if (d.sqrMagnitude < 2.4f * 2.4f && LumoInput.InteractPressed() && !GameManager.I.Finished)
            {
                lit = true;
                lamp.color = new Color(1f, 0.85f, 0.35f, 1f);
                GameManager.I.Win();
            }
            float pulse = 1f + 0.08f * Mathf.Sin(Time.time * 2.5f);
            glow.transform.localScale = new Vector3(2.2f * pulse, 2.2f * pulse, 1f);
        }
        else
        {
            litTime += Time.deltaTime;
            float s = Mathf.Lerp(2.2f, 14f, 1f - Mathf.Exp(-1.4f * litTime));
            glow.transform.localScale = new Vector3(s, s, 1f);
            glow.color = new Color(1f, 0.85f, 0.4f, 0.55f);
        }
    }
}
