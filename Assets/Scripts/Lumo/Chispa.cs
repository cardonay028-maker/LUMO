using System.Collections.Generic;
using UnityEngine;

// Chispa dorada: coleccionable escondido en la oscuridad. Solo se ve cuando la luz la toca.
public class Chispa : MonoBehaviour
{
    public static readonly List<Chispa> All = new List<Chispa>();
    bool got;

    void OnEnable() { All.Add(this); }
    void OnDisable() { All.Remove(this); }

    void Update()
    {
        if (got || LumoPlayer.I == null || GameManager.I == null) return;
        Vector2 d = LumoPlayer.I.transform.position - transform.position;
        if (d.sqrMagnitude < 0.75f * 0.75f)
        {
            got = true;
            GameManager.I.CollectChispa();
            Destroy(gameObject);
        }
    }
}
