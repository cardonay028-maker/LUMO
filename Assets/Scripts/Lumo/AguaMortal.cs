using UnityEngine;

// Agua del pantano (Nivel 2): si Lumo esta sobre el agua y no sobre un puente solido, se hunde.
public class AguaMortal : MonoBehaviour
{
    // Datos para el diagnostico (tecla F1)
    public static bool EnAgua;
    public static bool SobrePuente;

    static int frame = -1;
    static float aguaDesde = -1f;
    static float ultimaVezSinPuente = -10f;
    static float ultimoAviso = -10f;

    Collider2D col;

    void Awake()
    {
        col = GetComponent<Collider2D>();
    }

    void Update()
    {
        LumoPlayer p = LumoPlayer.I;
        GameManager gm = GameManager.I;
        if (p == null || gm == null || col == null) return;

        if (frame != Time.frameCount)
        {
            frame = Time.frameCount;
            EnAgua = false;
            SobrePuente = false;
        }

        if (gm.Finished || p.Invulnerable) return;

        Vector2 pos = p.transform.position;
        if (!col.OverlapPoint(pos)) return;
        EnAgua = true;

        if (PuenteLuz.All.Count == 0)
        {
            // Los puentes no se registraron: no se hunde, se avisa para reconstruir el nivel
            if (Time.time - ultimoAviso > 4f)
            {
                ultimoAviso = Time.time;
                gm.ShowMessage("Error: no hay puentes en la escena. Reconstruye el Nivel 2 (menú LUMO, paso 3).", 4f);
            }
            return;
        }

        for (int i = 0; i < PuenteLuz.All.Count; i++)
        {
            if (PuenteLuz.All[i].Covers(pos))
            {
                SobrePuente = true;
                aguaDesde = -1f;
                return;
            }
        }

        // Sin puente debajo: pequena tolerancia (0.25 s) antes de hundirse
        if (Time.time - ultimaVezSinPuente > 0.1f) aguaDesde = Time.time;
        ultimaVezSinPuente = Time.time;
        if (aguaDesde >= 0f && Time.time - aguaDesde > 0.25f)
        {
            aguaDesde = -1f;
            p.Hundir();
        }
    }
}
