using UnityEngine;

// Lumo: movimiento, luz, Destello y Brillo (energia).
[RequireComponent(typeof(Rigidbody2D))]
public class LumoPlayer : MonoBehaviour
{
    public static LumoPlayer I;

    // Radio (en unidades) del agujero de la imagen "oscuridad" cuando su escala es 1
    public const float HoleRadius = 0.17f;

    [Header("Movimiento")]
    public float speed = 5f;

    [Header("Luz")]
    public float baseRadius = 3.2f;
    public float destelloMultiplier = 2f;

    [Header("Brillo (energia)")]
    public float brilloMax = 100f;
    public float brillo = 100f;
    public float destelloCost = 28f;    // Brillo que gasta por segundo el Destello
    public float passiveRegen = 8f;     // Recarga lenta cuando no usa Destello

    [Header("Referencias (hijos de Lumo)")]
    public Transform overlay;   // capa de oscuridad
    public Transform glow;      // brillo calido alrededor
    public Transform body;      // cuerpo (para el movimiento de flotar)

    public bool DestelloActive { get; private set; }
    public float CurrentRadius { get; private set; }
    public bool Moved { get; private set; }
    public bool Invulnerable { get { return Time.time < invulnerableUntil; } }

    Rigidbody2D rb;
    SpriteRenderer overlaySr;
    Vector2 input;
    float radiusSmooth;
    float invulnerableUntil;

    void Awake()
    {
        I = this;
        rb = GetComponent<Rigidbody2D>();
        radiusSmooth = baseRadius;
        CurrentRadius = baseRadius;
        if (overlay != null)
        {
            overlaySr = overlay.GetComponent<SpriteRenderer>();
            // En el editor la oscuridad esta apagada para ver toda la escena; al jugar se enciende.
            if (overlaySr != null) overlaySr.enabled = true;
        }
    }

    void Update()
    {
        GameManager gm = GameManager.I;
        bool finished = gm != null && gm.Finished;
        float dt = Time.deltaTime;

        input = finished ? Vector2.zero : LumoInput.Move();
        if (input.sqrMagnitude > 0.01f) Moved = true;

        bool want = !finished && LumoInput.DestelloHeld() && brillo > 1f;
        DestelloActive = want;
        if (want) brillo -= destelloCost * dt;
        else brillo += passiveRegen * dt;
        brillo = Mathf.Clamp(brillo, 0f, brilloMax);

        // La luz se hace mas pequena cuando queda poco Brillo
        float low = Mathf.Lerp(0.6f, 1f, Mathf.Clamp01(brillo / 30f));
        float target = baseRadius * (want ? destelloMultiplier : 1f) * low;
        radiusSmooth = Mathf.Lerp(radiusSmooth, target, 1f - Mathf.Exp(-10f * dt));
        CurrentRadius = radiusSmooth;

        if (overlay != null)
        {
            float s = CurrentRadius / HoleRadius;
            overlay.localScale = new Vector3(s, s, 1f);
            if (overlaySr != null && gm != null)
                overlaySr.color = new Color(1f, 1f, 1f, gm.Darkness);
        }
        if (glow != null)
        {
            float g = CurrentRadius * 2.3f;
            glow.localScale = new Vector3(g, g, 1f);
        }
        if (body != null)
            body.localPosition = new Vector3(0f, Mathf.Sin(Time.time * 4f) * 0.05f, 0f);
    }

    void FixedUpdate()
    {
        if (rb != null)
            rb.MovePosition(rb.position + input * speed * Time.fixedDeltaTime);
    }

    public void Recharge(float amount)
    {
        brillo = Mathf.Min(brilloMax, brillo + amount);
    }

    public void TeleportTo(Vector2 pos)
    {
        transform.position = new Vector3(pos.x, pos.y, 0f);
        if (rb != null) rb.position = pos;
    }

    // Lumo cae al agua (Nivel 2): vuelve al ultimo punto de control y pierde algo de Brillo
    public void Hundir()
    {
        brillo = Mathf.Max(10f, brillo - 25f);
        invulnerableUntil = Time.time + 0.8f;
        if (GameManager.I != null)
        {
            TeleportTo(GameManager.I.checkpoint);
            GameManager.I.ShowMessage("¡Lumo cayó al agua! Vuelves al último punto de control.", 2.8f);
        }
    }
}
