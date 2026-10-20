using System.Collections.Generic;
using UnityEngine;

// Pulso suave de escala (flores, chispas, llamas).
public class Pulse : MonoBehaviour
{
    public float amp = 0.1f;
    public float speed = 3f;
    Vector3 baseScale;
    float phase;

    void Start()
    {
        baseScale = transform.localScale;
        phase = Random.value * 6.28f;
    }

    void Update()
    {
        transform.localScale = baseScale * (1f + amp * Mathf.Sin(Time.time * speed + phase));
    }
}
