using UnityEngine;

// Barrera de sombra: se deshace si Lumo usa el Destello cerca de ella unos segundos.
public class BarreraSombra : MonoBehaviour
{
    public static float Progreso;      // 0 a 1, lo usa la interfaz para mostrar la barra "Deshaciendo barrera"

    public float needed = 1.6f;        // segundos de Destello necesarios
    public float progress;             // se puede ver en el Inspector mientras juegas

    bool done;
    BoxCollider2D col;
    SpriteRenderer[] rs;
    float[] baseAlpha;

    void Start()
    {
        Progreso = 0f;
        col = GetComponent<BoxCollider2D>();
        rs = GetComponentsInChildren<SpriteRenderer>();
        baseAlpha = new float[rs.Length];
        for (int i = 0; i < rs.Length; i++) baseAlpha[i] = rs[i].color.a;
    }

    void Update()
    {
        if (done || LumoPlayer.I == null) return;
        if (col == null) col = GetComponent<BoxCollider2D>();
        if (col == null) return;

        LumoPlayer p = LumoPlayer.I;
        Vector3 pos = p.transform.position;
        Bounds b = col.bounds;
        pos.z = b.center.z;
        float d = Vector3.Distance(b.ClosestPoint(pos), pos);

        if (p.DestelloActive && d < p.CurrentRadius)
            progress += Time.deltaTime;
        else
            progress = Mathf.Max(0f, progress - Time.deltaTime * 0.5f);

        Progreso = Mathf.Clamp01(progress / needed);

        float k = 1f - Mathf.Clamp01(progress / needed);
        for (int i = 0; i < rs.Length; i++)
        {
            Color c = rs[i].color;
            c.a = baseAlpha[i] * k;
            rs[i].color = c;
        }

        if (progress >= needed)
        {
            done = true;
            Progreso = 0f;
            col.enabled = false;
            gameObject.SetActive(false);
        }
    }
}
