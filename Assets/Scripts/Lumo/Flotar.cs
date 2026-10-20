using System.Collections.Generic;
using UnityEngine;

// Movimiento flotante en circulos pequenos (luciernagas decorativas).
public class Flotar : MonoBehaviour
{
    public float radio = 0.6f;
    public float velocidad = 0.6f;
    Vector3 origen;
    float fase;

    void Start()
    {
        origen = transform.position;
        fase = Random.value * 10f;
    }

    void Update()
    {
        float t = Time.time * velocidad + fase;
        transform.position = origen + new Vector3(Mathf.Sin(t * 1.3f), Mathf.Cos(t), 0f) * radio;
    }
}
