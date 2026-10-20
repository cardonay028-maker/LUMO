using UnityEngine;

// Interfaz del juego con IMGUI (OnGUI): barra de Brillo, chispas, consejos y pantalla final.
// Es un objeto normal de la escena (Sistema > UI).
public class GameUI : MonoBehaviour
{
    GUIStyle label, centered, big, hint, small;
    const float VH = 720f;
    bool showDebug;

    void Init()
    {
        if (label != null) return;
        label = new GUIStyle(GUI.skin.label) { fontSize = 20, fontStyle = FontStyle.Bold };
        label.normal.textColor = Color.white;
        centered = new GUIStyle(label) { alignment = TextAnchor.MiddleCenter };
        big = new GUIStyle(GUI.skin.label) { fontSize = 54, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter };
        big.normal.textColor = new Color(1f, 0.85f, 0.4f);
        hint = new GUIStyle(GUI.skin.label) { fontSize = 20, alignment = TextAnchor.MiddleCenter, wordWrap = true };
        hint.normal.textColor = Color.white;
        small = new GUIStyle(GUI.skin.label) { fontSize = 15 };
        small.normal.textColor = new Color(1f, 1f, 1f, 0.7f);
    }

    static void DrawBox(Rect r, Color c)
    {
        Color old = GUI.color;
        GUI.color = c;
        GUI.DrawTexture(r, Texture2D.whiteTexture);
        GUI.color = old;
    }

    void OnGUI()
    {
        GameManager gm = GameManager.I;
        LumoPlayer p = LumoPlayer.I;
        if (gm == null || p == null) return;
        Init();

        float s = Screen.height / VH;
        GUI.matrix = Matrix4x4.Scale(new Vector3(s, s, 1f));
        float vw = Screen.width / s;

        // --- Barra de Brillo ---
        float frac = Mathf.Clamp01(p.brillo / p.brilloMax);
        GUI.Label(new Rect(20, 12, 200, 28), "BRILLO", label);
        DrawBox(new Rect(20, 42, 264, 22), new Color(0f, 0f, 0f, 0.6f));
        Color barColor = Color.Lerp(new Color(0.9f, 0.25f, 0.2f), new Color(1f, 0.82f, 0.3f), Mathf.Clamp01(frac * 1.6f));
        DrawBox(new Rect(22, 44, 260f * frac, 18), barColor);

        // --- Chispas y tiempo ---
        GUI.Label(new Rect(vw - 260, 12, 240, 28), "Chispas: " + gm.chispas + " / " + gm.chispasTotal, label);
        int sec = Mathf.FloorToInt(gm.time);
        GUI.Label(new Rect(vw - 260, 42, 240, 28), string.Format("Tiempo: {0:00}:{1:00}", sec / 60, sec % 60), label);

        // --- Titulo del nivel ---
        GUI.Label(new Rect(vw * 0.5f - 300, 12, 600, 28), gm.nombreNivel, centered);

        // --- Mensaje corto (punto de control, caida al agua) ---
        string m = gm.CurrentMessage;
        if (!string.IsNullOrEmpty(m))
        {
            GUIStyle ms = new GUIStyle(centered) { fontSize = 22 };
            ms.normal.textColor = new Color(1f, 0.9f, 0.5f);
            GUI.Label(new Rect(vw * 0.5f - 400, 50, 800, 34), m, ms);
        }

        // --- Progreso de la barrera de sombra ---
        if (BarreraSombra.Progreso > 0.01f)
        {
            Rect br = new Rect(vw * 0.5f - 150, 92, 300, 18);
            DrawBox(new Rect(br.x - 2, br.y - 2, br.width + 4, br.height + 4), new Color(0f, 0f, 0f, 0.7f));
            DrawBox(new Rect(br.x, br.y, br.width * BarreraSombra.Progreso, br.height), new Color(0.75f, 0.5f, 1f));
            GUIStyle bs = new GUIStyle(small) { alignment = TextAnchor.MiddleCenter };
            bs.normal.textColor = Color.white;
            GUI.Label(new Rect(br.x, br.y - 22, br.width, 20), "Deshaciendo barrera...", bs);
        }

        // --- Diagnostico (tecla F1) ---
        if (LumoInput.DebugPressed()) showDebug = !showDebug;
        if (showDebug)
        {
            string info = "Destello: " + (p.DestelloActive ? "SI" : "no") + "   Radio de luz: " + p.CurrentRadius.ToString("0.0")
                        + "   Brillo: " + p.brillo.ToString("0") + "   Barrera: " + (BarreraSombra.Progreso * 100f).ToString("0") + "%"
                        + "   Chispas en escena: " + Chispa.All.Count
                        + "   Puentes: " + PuenteLuz.All.Count + "   En agua: " + (AguaMortal.EnAgua ? "SI" : "no") + "   Sobre puente: " + (AguaMortal.SobrePuente ? "SI" : "no");
            DrawBox(new Rect(18, 76, 1100, 26), new Color(0f, 0f, 0f, 0.7f));
            GUI.Label(new Rect(24, 78, 1090, 24), info, small);
        }

        // --- Controles ---
        GUI.Label(new Rect(20, VH - 34, 900, 24), "WASD / flechas: mover   |   ESPACIO (o SHIFT): Destello   |   E: encender farol   |   R: reiniciar   |   F1: diagnostico", small);

        // --- Consejo de Tia Vela ---
        string h = gm.CurrentHint;
        if (!gm.Finished && !string.IsNullOrEmpty(h))
        {
            Rect box = new Rect(vw * 0.5f - 400, VH - 130, 800, 80);
            DrawBox(box, new Color(0.05f, 0.08f, 0.16f, 0.82f));
            GUI.Label(new Rect(box.x + 16, box.y + 6, box.width - 32, box.height - 12), h, hint);
        }

        // --- Pantalla de inicio ---
        if (gm.time < 4f && !gm.Finished)
        {
            float a = Mathf.Clamp01(Mathf.Min(gm.time / 0.6f, (4f - gm.time) / 1.2f));
            Color old = GUI.color;
            GUI.color = new Color(1f, 1f, 1f, a);
            GUI.Label(new Rect(0, VH * 0.18f, vw, 80), "LUMO", big);
            GUIStyle sub = new GUIStyle(centered) { fontSize = 24 };
            GUI.Label(new Rect(0, VH * 0.18f + 70, vw, 40), "El Bosque Sin Luz  -  " + gm.nombreNivel, sub);
            GUI.color = old;
        }

        // --- Pantalla final ---
        if (gm.Finished && gm.finishedTime > 1.8f)
        {
            float a = Mathf.Clamp01((gm.finishedTime - 1.8f) / 0.8f);
            Rect panel = new Rect(vw * 0.5f - 300, VH * 0.5f - 170, 600, 340);
            Color old = GUI.color;
            DrawBox(panel, new Color(0.05f, 0.08f, 0.16f, 0.88f * a));
            GUI.color = new Color(1f, 1f, 1f, a);
            GUIStyle bt = new GUIStyle(big) { fontSize = 34 };
            GUI.Label(new Rect(panel.x, panel.y + 12, panel.width, 50), gm.tituloFinal, bt);
            GUIStyle c = new GUIStyle(hint) { fontSize = 22 };
            GUI.Label(new Rect(panel.x + 20, panel.y + 80, panel.width - 40, 36), "Chispas doradas: " + gm.chispas + " / " + gm.chispasTotal, c);
            int t = Mathf.FloorToInt(gm.time);
            GUI.Label(new Rect(panel.x + 20, panel.y + 118, panel.width - 40, 36), string.Format("Tiempo: {0:00}:{1:00}", t / 60, t % 60), c);
            GUI.Label(new Rect(panel.x + 20, panel.y + 170, panel.width - 40, 70), gm.textoSiguiente, c);
            string bottom = string.IsNullOrEmpty(gm.siguienteEscena)
                ? "Presiona R para jugar otra vez"
                : "Presiona ENTER para ir al siguiente nivel   |   R para repetir";
            GUI.Label(new Rect(panel.x + 20, panel.y + 290, panel.width - 40, 30), bottom, small);
            GUI.color = old;
        }
    }
}
