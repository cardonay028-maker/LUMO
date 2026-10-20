using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

// Constructor de escenas de LUMO (menu "LUMO" arriba en Unity).
// Crea las escenas de los Niveles 1 y 2 como archivos .unity normales: todos los objetos
// quedan guardados en la Hierarchy y se ven en la ventana Scene (no se crea nada "al darle Play").
public static class LumoSceneBuilder
{
    const string SpritesDir = "Assets/Sprites";
    const string ScenesDir = "Assets/Scenes";

    static Sprite sCuad, sCirc, sBrillo, sOsc;
    static Material sMat;
    static int sW, sH;

    // =====================================================================
    //  MENU
    // =====================================================================
    [MenuItem("LUMO/0. Construir TODO (pasos 1 a 4)")]
    public static void ConstruirTodo()
    {
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
        try { CrearSprites(); }
        catch (System.Exception e) { Fallo(e); return; }
        // Se espera entre pasos para que Unity termine de importar los sprites (igual que al hacer clic uno por uno)
        EditorApplication.delayCall += TodoPaso2;
    }

    static void TodoPaso2()
    {
        try { Construir(LumoMapas.Nivel1); }
        catch (System.Exception e) { Fallo(e); return; }
        EditorApplication.delayCall += TodoPaso3;
    }

    static void TodoPaso3()
    {
        try { Construir(LumoMapas.Nivel2); }
        catch (System.Exception e) { Fallo(e); return; }
        EditorApplication.delayCall += TodoPaso4;
    }

    static void TodoPaso4()
    {
        try
        {
            AgregarEscenasABuildSettings();
            AbrirEscena(LumoMapas.Nivel1.escena);
        }
        catch (System.Exception e) { Fallo(e); return; }
        EditorUtility.DisplayDialog("LUMO",
            "Listo. Se crearon los Niveles 1 y 2 en Assets/Scenes.\n\nAbre una escena con doble clic y presiona Play.",
            "OK");
    }

    static void Fallo(System.Exception e)
    {
        Debug.LogError("LUMO: error al construir -> " + e);
        EditorUtility.DisplayDialog("LUMO - error",
            e.Message + "\n\nRevisa la Console. Tambien puedes ejecutar los pasos 1, 2, 3 y 4 del menu uno por uno.",
            "OK");
    }

    [MenuItem("LUMO/1. Crear sprites")]
    public static void CrearSprites()
    {
        Directory.CreateDirectory(SpritesDir);

        Texture2D cuad = NuevaTex(4, delegate (int x, int y) { return new Color32(255, 255, 255, 255); });
        GuardarSprite("cuadrado", cuad, 4f, FilterMode.Point);

        Texture2D circ = NuevaTex(64, delegate (int x, int y)
        {
            float c = 63f * 0.5f;
            float d = Mathf.Sqrt((x - c) * (x - c) + (y - c) * (y - c));
            float a = Mathf.Clamp01(32f - d);
            return new Color32(255, 255, 255, (byte)(a * 255f));
        });
        GuardarSprite("circulo", circ, 64f, FilterMode.Bilinear);

        Texture2D bri = NuevaTex(128, delegate (int x, int y)
        {
            float c = 127f * 0.5f;
            float d = Mathf.Sqrt((x - c) * (x - c) + (y - c) * (y - c)) / 64f;
            float a = Mathf.Pow(1f - Mathf.Clamp01(d), 2f);
            return new Color32(255, 255, 255, (byte)(a * 255f));
        });
        GuardarSprite("brillo", bri, 128f, FilterMode.Bilinear);

        // Capa de oscuridad: negro con un agujero suave en el centro (radio medio = 17 px = 0.17 unidades)
        Texture2D osc = NuevaTex(512, delegate (int x, int y)
        {
            float c = 511f * 0.5f;
            float d = Mathf.Sqrt((x - c) * (x - c) + (y - c) * (y - c));
            float t = Mathf.Clamp01((d - 12f) / (22f - 12f));
            float a = Mathf.SmoothStep(0f, 1f, t);
            return new Color32(0, 0, 0, (byte)(a * 255f));
        });
        GuardarSprite("oscuridad", osc, 100f, FilterMode.Bilinear);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("LUMO: sprites creados en " + SpritesDir);
    }

    [MenuItem("LUMO/2. Construir Nivel 1 (Claro de Inicio)")]
    public static void ConstruirNivel1()
    {
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
        Construir(LumoMapas.Nivel1);
    }

    [MenuItem("LUMO/3. Construir Nivel 2 (Pantano de Luciernagas)")]
    public static void ConstruirNivel2()
    {
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
        Construir(LumoMapas.Nivel2);
    }

    [MenuItem("LUMO/4. Agregar escenas a Build Settings")]
    public static void AgregarEscenasABuildSettings()
    {
        string p1 = ScenesDir + "/" + LumoMapas.Nivel1.escena + ".unity";
        string p2 = ScenesDir + "/" + LumoMapas.Nivel2.escena + ".unity";
        EditorBuildSettings.scenes = new EditorBuildSettingsScene[]
        {
            new EditorBuildSettingsScene(p1, true),
            new EditorBuildSettingsScene(p2, true)
        };
        Debug.Log("LUMO: escenas agregadas a Build Settings (Nivel 1 y Nivel 2).");
    }

    [MenuItem("LUMO/Abrir Nivel 1")]
    public static void AbrirNivel1()
    {
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
        AbrirEscena(LumoMapas.Nivel1.escena);
    }

    [MenuItem("LUMO/Abrir Nivel 2")]
    public static void AbrirNivel2()
    {
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
        AbrirEscena(LumoMapas.Nivel2.escena);
    }

    static void AbrirEscena(string nombre)
    {
        string path = ScenesDir + "/" + nombre + ".unity";
        if (!File.Exists(path)) return;
        EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
        GameObject f = GameObject.Find("Fondo");
        if (f != null)
        {
            Selection.activeGameObject = f;
            SceneView.FrameLastActiveSceneView();
        }
    }

    // =====================================================================
    //  SPRITES
    // =====================================================================
    static Texture2D NuevaTex(int n, System.Func<int, int, Color32> f)
    {
        Texture2D t = new Texture2D(n, n, TextureFormat.RGBA32, false);
        Color32[] px = new Color32[n * n];
        for (int y = 0; y < n; y++)
            for (int x = 0; x < n; x++)
                px[y * n + x] = f(x, y);
        t.SetPixels32(px);
        t.Apply();
        return t;
    }

    static void GuardarSprite(string nombre, Texture2D tex, float ppu, FilterMode filtro)
    {
        string path = SpritesDir + "/" + nombre + ".png";
        File.WriteAllBytes(path, tex.EncodeToPNG());
        Object.DestroyImmediate(tex);
        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);

        TextureImporter ti = AssetImporter.GetAtPath(path) as TextureImporter;
        if (ti == null) return;
        ti.textureType = TextureImporterType.Sprite;
        ti.spriteImportMode = SpriteImportMode.Single;
        ti.spritePixelsPerUnit = ppu;
        ti.alphaIsTransparency = true;
        ti.mipmapEnabled = false;
        ti.filterMode = filtro;
        ti.wrapMode = TextureWrapMode.Clamp;
        ti.textureCompression = TextureImporterCompression.Uncompressed;
        ti.SaveAndReimport();
    }

    static Sprite Spr(string nombre)
    {
        return AssetDatabase.LoadAssetAtPath<Sprite>(SpritesDir + "/" + nombre + ".png");
    }

    // =====================================================================
    //  AYUDAS PARA CREAR OBJETOS
    // =====================================================================
    static Vector2 Cell(int c, int r)
    {
        return new Vector2(c - sW * 0.5f + 0.5f, sH * 0.5f - r - 0.5f);
    }

    static Vector2 CentroRect(RectInt rc)
    {
        return new Vector2(rc.x + rc.width * 0.5f - sW * 0.5f, sH * 0.5f - (rc.y + rc.height * 0.5f));
    }

    static bool EsAgua(char ch) { return ch == '~' || ch == 'b' || ch == 'B'; }
    static bool EsPared(char ch) { return ch == '#'; }
    static bool EsTierra(char ch) { return !EsAgua(ch) && !EsPared(ch); }

    static Transform Grupo(string nombre)
    {
        return new GameObject(nombre).transform;
    }

    static GameObject Hijo(string nombre, Transform padre, Sprite sprite, Color color, Vector2 pos, Vector2 escala, int orden)
    {
        GameObject go = new GameObject(nombre);
        if (padre != null) go.transform.SetParent(padre, false);
        go.transform.localPosition = new Vector3(pos.x, pos.y, 0f);
        go.transform.localScale = new Vector3(escala.x, escala.y, 1f);
        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = sprite;
        sr.color = color;
        sr.sortingOrder = orden;
        if (sMat != null) sr.sharedMaterial = sMat;
        return go;
    }

    static GameObject Vacio(string nombre, Transform padre, Vector2 posMundo)
    {
        GameObject go = new GameObject(nombre);
        if (padre != null) go.transform.SetParent(padre, false);
        go.transform.position = new Vector3(posMundo.x, posMundo.y, 0f);
        return go;
    }

    // Une celdas vecinas del mismo tipo en rectangulos grandes (menos objetos en la Hierarchy)
    static List<RectInt> MergeRects(char[,] g, System.Func<char, bool> pred)
    {
        List<RectInt> lista = new List<RectInt>();
        bool[,] used = new bool[sH, sW];
        for (int r = 0; r < sH; r++)
        {
            for (int c = 0; c < sW; c++)
            {
                if (used[r, c] || !pred(g[r, c])) continue;
                int w = 1;
                while (c + w < sW && !used[r, c + w] && pred(g[r, c + w])) w++;
                int h = 1;
                bool ok = true;
                while (r + h < sH && ok)
                {
                    for (int k = 0; k < w; k++)
                    {
                        if (used[r + h, c + k] || !pred(g[r + h, c + k])) { ok = false; break; }
                    }
                    if (ok) h++;
                }
                for (int rr = r; rr < r + h; rr++)
                    for (int cc = c; cc < c + w; cc++)
                        used[rr, cc] = true;
                lista.Add(new RectInt(c, r, w, h));
            }
        }
        return lista;
    }

    // =====================================================================
    //  CONSTRUIR UN NIVEL
    // =====================================================================
    static void Construir(LumoNivel d)
    {
        if (Spr("circulo") == null || Spr("oscuridad") == null) CrearSprites();
        sCuad = Spr("cuadrado");
        sCirc = Spr("circulo");
        sBrillo = Spr("brillo");
        sOsc = Spr("oscuridad");
        if (sCuad == null || sCirc == null || sBrillo == null || sOsc == null)
            throw new System.Exception("No se encontraron los sprites en Assets/Sprites. Ejecuta primero LUMO > 1. Crear sprites.");
        // Material sin luces: se ve igual en Built-in y en URP 2D (sin necesidad de Light 2D)
        sMat = AssetDatabase.GetBuiltinExtraResource<Material>("Sprites-Default.mat");

        Scene escena = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        sH = d.mapa.Length;
        sW = d.mapa[0].Length;
        char[,] g = new char[sH, sW];
        for (int r = 0; r < sH; r++)
            for (int c = 0; c < sW; c++)
                g[r, c] = d.mapa[r][c];

        // ---------- Grupos de la Hierarchy ----------
        Transform sistema = Grupo("Sistema");
        Transform entorno = Grupo("Entorno");
        Transform suelo = Grupo("Suelo"); suelo.SetParent(entorno, false);
        Transform agua = Grupo("Agua"); agua.SetParent(entorno, false);
        Transform arboles = Grupo("Arboles"); arboles.SetParent(entorno, false);
        Transform rocas = Grupo("Rocas"); rocas.SetParent(entorno, false);
        Transform deco = Grupo("Decoracion"); deco.SetParent(entorno, false);
        Transform puentes = d.pantano ? Grupo("PuentesDeLuz") : null;
        Transform luciernagas = d.pantano ? Grupo("Luciernagas") : null;
        Transform puntos = d.pantano ? Grupo("PuntosDeControl") : null;
        Transform chispasG = Grupo("Chispas");
        Transform floresG = Grupo("FloresDeLuz");
        Transform personajes = Grupo("Personajes");
        Transform obstaculos = d.pantano ? null : Grupo("Obstaculos");
        Transform meta = Grupo("Meta");
        Transform consejos = Grupo("Consejos");

        // ---------- Fondo y suelo ----------
        GameObject fondo = Hijo("Fondo", entorno, sCuad, new Color(0.05f, 0.11f, 0.09f), Vector2.zero, new Vector2(sW + 12, sH + 12), -5);
        Color colSuelo = d.pantano ? new Color(0.24f, 0.31f, 0.21f) : new Color(0.18f, 0.29f, 0.23f);
        foreach (RectInt rc in MergeRects(g, EsTierra))
            Hijo("Suelo", suelo, sCuad, colSuelo, CentroRect(rc), new Vector2(rc.width, rc.height), 0);

        // ---------- Agua (solo Nivel 2) ----------
        foreach (RectInt rc in MergeRects(g, EsAgua))
        {
            GameObject a = Hijo("Agua", agua, sCuad, new Color(0.08f, 0.24f, 0.32f), CentroRect(rc), new Vector2(rc.width, rc.height), 1);
            BoxCollider2D bc = a.AddComponent<BoxCollider2D>();
            bc.isTrigger = true;
            bc.size = Vector2.one;
            a.AddComponent<AguaMortal>();
        }

        // ---------- Puentes de luz (celdas b y B) ----------
        for (int r = 0; r < sH; r++)
        {
            for (int c = 0; c < sW; c++)
            {
                char ch = g[r, c];
                if (ch != 'b' && ch != 'B') continue;
                bool tenue = ch == 'B';
                Color col = tenue ? new Color(0.72f, 0.88f, 1f) : new Color(1f, 0.82f, 0.45f);
                GameObject p = Hijo("Puente_" + c + "_" + r, puentes, sCuad, col, Cell(c, r), new Vector2(0.94f, 0.94f), 3);
                PuenteLuz pl = p.AddComponent<PuenteLuz>();
                pl.requiereDestello = tenue;
            }
        }

        // ---------- Arboles (paredes) ----------
        for (int r = 0; r < sH; r++)
        {
            for (int c = 0; c < sW; c++)
            {
                if (g[r, c] != '#') continue;
                bool cerca = false;
                for (int dr = -1; dr <= 1; dr++)
                    for (int dc = -1; dc <= 1; dc++)
                    {
                        int rr = r + dr, cc = c + dc;
                        if (rr < 0 || cc < 0 || rr >= sH || cc >= sW) continue;
                        if (g[rr, cc] != '#') cerca = true;
                    }
                if (!cerca) continue;
                GameObject t = Hijo("Arbol", arboles, sCuad, new Color(0.09f, 0.20f, 0.15f), Cell(c, r), Vector2.one, 2);
                BoxCollider2D bc = t.AddComponent<BoxCollider2D>();
                bc.size = Vector2.one;
                float v = ((c * 7 + r * 13) % 4) * 0.03f;
                Hijo("Copa", t.transform, sCirc, new Color(0.12f + v, 0.34f + v, 0.22f + v), new Vector2(0f, 0.05f), new Vector2(1.35f, 1.35f), 3);
            }
        }

        // ---------- Rocas (celdas R, bloques de 2 x 2) ----------
        bool[,] vistoR = new bool[sH, sW];
        for (int r = 0; r < sH; r++)
        {
            for (int c = 0; c < sW; c++)
            {
                if (g[r, c] != 'R' || vistoR[r, c]) continue;
                int w = 0; while (c + w < sW && g[r, c + w] == 'R') w++;
                int h = 0; while (r + h < sH && g[r + h, c] == 'R') h++;
                for (int rr = r; rr < r + h; rr++)
                    for (int cc = c; cc < c + w; cc++)
                        vistoR[rr, cc] = true;
                GameObject roca = Hijo("Roca", rocas, sCirc, new Color(0.36f, 0.39f, 0.44f), CentroRect(new RectInt(c, r, w, h)), new Vector2(w * 1.05f, h * 1.05f), 3);
                CircleCollider2D cc2 = roca.AddComponent<CircleCollider2D>();
                cc2.radius = 0.5f;
                Hijo("Brillo", roca.transform, sCirc, new Color(0.5f, 0.54f, 0.6f), new Vector2(-0.14f, 0.14f), new Vector2(0.5f, 0.5f), 4);
            }
        }

        // ---------- Decoracion (pasto y nenufares) ----------
        System.Random rng = new System.Random(7);
        int pastos = 0, intentos = 0;
        while (pastos < (d.pantano ? 70 : 160) && intentos < 3000)
        {
            intentos++;
            int c = rng.Next(1, sW - 1), r = rng.Next(1, sH - 1);
            if (!EsTierra(g[r, c])) continue;
            Vector2 p = Cell(c, r) + new Vector2((float)rng.NextDouble() - 0.5f, (float)rng.NextDouble() - 0.5f);
            float sc = 0.15f + 0.25f * (float)rng.NextDouble();
            Hijo("Pasto", deco, sCirc, d.pantano ? new Color(0.30f, 0.40f, 0.26f) : new Color(0.24f, 0.38f, 0.30f), p, new Vector2(sc, sc), 1);
            pastos++;
        }
        if (d.pantano)
        {
            int nen = 0; intentos = 0;
            while (nen < 90 && intentos < 4000)
            {
                intentos++;
                int c = rng.Next(1, sW - 1), r = rng.Next(1, sH - 1);
                if (g[r, c] != '~') continue;
                Vector2 p = Cell(c, r) + new Vector2((float)rng.NextDouble() - 0.5f, (float)rng.NextDouble() - 0.5f);
                float sc = 0.5f + 0.5f * (float)rng.NextDouble();
                Hijo("Nenufar", deco, sCirc, new Color(0.20f, 0.46f, 0.28f), p, new Vector2(sc, sc * 0.7f), 2);
                nen++;
            }
            // Luciernagas decorativas: brillan incluso en la oscuridad
            int lu = 0; intentos = 0;
            while (lu < 20 && intentos < 4000)
            {
                intentos++;
                int c = rng.Next(1, sW - 1), r = rng.Next(1, sH - 1);
                if (g[r, c] != '~') continue;
                GameObject l = Hijo("Luciernaga", luciernagas, sBrillo, new Color(0.85f, 1f, 0.45f, 0.9f), Cell(c, r), new Vector2(0.55f, 0.55f), 11);
                Flotar fl = l.AddComponent<Flotar>();
                fl.radio = 0.4f + (float)rng.NextDouble() * 0.6f;
                fl.velocidad = 0.4f + (float)rng.NextDouble() * 0.5f;
                Pulse pu = l.AddComponent<Pulse>();
                pu.amp = 0.25f; pu.speed = 3f;
                lu++;
            }
        }

        // ---------- Objetos marcados en el mapa ----------
        Vector2 inicio = Vector2.zero;
        int nCh = 0, nFl = 0, nCk = 0;
        int bc0 = int.MaxValue, bc1 = -1, br0 = int.MaxValue, br1 = -1;
        for (int r = 0; r < sH; r++)
        {
            for (int c = 0; c < sW; c++)
            {
                switch (g[r, c])
                {
                    case 'S': inicio = Cell(c, r); break;
                    case 'c': nCh++; CrearChispa(chispasG, Cell(c, r), nCh); break;
                    case 'f': nFl++; CrearFlor(floresG, Cell(c, r), nFl); break;
                    case 'k': nCk++; CrearCheckpoint(puntos, Cell(c, r), nCk); break;
                    case 'V': CrearTiaVela(personajes, Cell(c, r)); break;
                    case 'F': CrearFarol(meta, Cell(c, r)); break;
                    case 'X':
                        if (c < bc0) bc0 = c;
                        if (c > bc1) bc1 = c;
                        if (r < br0) br0 = r;
                        if (r > br1) br1 = r;
                        break;
                }
            }
        }
        GameObject barrera = null;
        if (bc1 >= 0)
            barrera = CrearBarrera(obstaculos, CentroRect(new RectInt(bc0, br0, bc1 - bc0 + 1, br1 - br0 + 1)), bc1 - bc0 + 1, br1 - br0 + 1);

        // ---------- Consejos de Tia Vela ----------
        if (!d.pantano)
        {
            AddHint(consejos, Cell(8, 15), 7f, 1, "Tía Vela: Sigue el sendero hacia la derecha. Las Flores de Luz brillan a lo lejos y recargan tu Brillo.", null);
            AddHint(consejos, Cell(8, 13), 3.5f, 2, "Tía Vela: Párate cerca de una Flor de Luz para recargar tu Brillo.", null);
            AddHint(consejos, Cell(14, 16), 3f, 3, "Tía Vela: ¡Una chispa dorada! Solo se ven cuando tu luz las toca. Hay 5 escondidas en el bosque.", null);
            AddHint(consejos, Cell(32, 11), 4f, 2, "Tía Vela: Sube y sigue por este pasillo hacia la izquierda.", null);
            AddHint(consejos, Cell(2, 6), 4f, 2, "Tía Vela: Ya casi. Sube y sigue hacia la derecha, hacia el farol.", null);
            if (barrera != null)
                AddHint(consejos, barrera.transform.position, 7f, 4, "Tía Vela: Una barrera de sombra bloquea el camino. Mantén ESPACIO para hacer un Destello y deshacerla. ¡Gasta Brillo!", barrera);
            AddHint(consejos, Cell(33, 3), 5f, 5, "Tía Vela: Llegaste al Farol Ancestral. Presiona E (o X en mando) para encenderlo.", null);
        }
        else
        {
            AddHint(consejos, Cell(5, 17), 7f, 1, "Tía Vela: Este es el Pantano de Luciérnagas. Si caes al agua, vuelves al último punto de control.", null);
            AddHint(consejos, Cell(9, 18), 4f, 3, "Tía Vela: Los puentes de luz solo existen dentro de tu luz. Camina por ellos y se irán formando.", null);
            AddHint(consejos, Cell(21, 14), 4f, 2, "Tía Vela: Esta isla tiene un punto de control y una Flor de Luz. Sigue hacia el norte.", null);
            AddHint(consejos, Cell(22, 8), 3f, 2, "Tía Vela: Con ESPACIO tu luz llega más lejos y ves mejor el camino.", null);
            AddHint(consejos, Cell(30, 8), 5f, 5, "Tía Vela: El puente tenue del sur solo aparece mientras haces un Destello. Recarga Brillo en la flor y cruza manteniendo ESPACIO.", null);
            AddHint(consejos, Cell(44, 18), 5f, 5, "Tía Vela: Llegaste al Farol Ancestral. Presiona E (o X en mando) para encenderlo.", null);
        }

        // ---------- Lumo ----------
        GameObject lumo = CrearLumo(personajes, inicio);

        // ---------- Camara, GameManager e interfaz ----------
        CrearCamara(sistema, lumo.transform, inicio);

        GameObject gmGo = new GameObject("GameManager");
        gmGo.transform.SetParent(sistema, false);
        GameManager gm = gmGo.AddComponent<GameManager>();
        gm.nombreNivel = d.nombre;
        gm.tituloFinal = d.tituloFinal;
        gm.textoSiguiente = d.textoSiguiente;
        gm.siguienteEscena = d.siguienteEscena;
        gm.oscuridadInicial = 0.96f;
        gm.oscuridadFinal = 0.55f;
        gm.Darkness = 0.96f;

        GameObject uiGo = new GameObject("UI");
        uiGo.transform.SetParent(sistema, false);
        uiGo.AddComponent<GameUI>();

        // ---------- Guardar ----------
        Directory.CreateDirectory(ScenesDir);
        string path = ScenesDir + "/" + d.escena + ".unity";
        EditorSceneManager.SaveScene(escena, path);
        AssetDatabase.Refresh();

        Selection.activeGameObject = fondo;
        SceneView.FrameLastActiveSceneView();
        Debug.Log("LUMO: escena creada -> " + path);
    }

    // =====================================================================
    //  OBJETOS DEL JUEGO
    // =====================================================================
    static void CrearChispa(Transform padre, Vector2 pos, int n)
    {
        GameObject cg = Vacio("Chispa_" + n, padre, pos);
        Hijo("Nucleo", cg.transform, sCirc, new Color(1f, 0.88f, 0.35f), Vector2.zero, new Vector2(0.32f, 0.32f), 3);
        Hijo("Brillo", cg.transform, sBrillo, new Color(1f, 0.85f, 0.3f, 0.6f), Vector2.zero, new Vector2(1.1f, 1.1f), 3);
        cg.AddComponent<Chispa>();
        Pulse p = cg.AddComponent<Pulse>();
        p.amp = 0.15f; p.speed = 4f;
    }

    static void CrearFlor(Transform padre, Vector2 pos, int n)
    {
        GameObject fg = Vacio("FlorDeLuz_" + n, padre, pos);
        GameObject glow = Hijo("Brillo", fg.transform, sBrillo, new Color(0.5f, 0.8f, 1f, 0.55f), Vector2.zero, new Vector2(1.6f, 1.6f), 11);
        for (int k = 0; k < 5; k++)
        {
            float a = k * Mathf.PI * 2f / 5f;
            Hijo("Petalo", fg.transform, sCirc, new Color(0.65f, 0.88f, 1f), new Vector2(Mathf.Cos(a), Mathf.Sin(a)) * 0.2f, new Vector2(0.22f, 0.22f), 12);
        }
        Hijo("Centro", fg.transform, sCirc, new Color(1f, 0.92f, 0.5f), Vector2.zero, new Vector2(0.2f, 0.2f), 13);
        FlorDeLuz fl = fg.AddComponent<FlorDeLuz>();
        fl.glow = glow.transform;
        Pulse pu = fg.AddComponent<Pulse>();
        pu.amp = 0.06f; pu.speed = 2.5f;
    }

    static void CrearCheckpoint(Transform padre, Vector2 pos, int n)
    {
        GameObject cg = Vacio("PuntoDeControl_" + n, padre, pos);
        Hijo("Piedra", cg.transform, sCirc, new Color(0.55f, 0.58f, 0.62f), Vector2.zero, new Vector2(0.6f, 0.6f), 4);
        GameObject luz = Hijo("Luz", cg.transform, sBrillo, new Color(1f, 0.85f, 0.4f, 0.25f), Vector2.zero, new Vector2(2f, 2f), 11);
        Checkpoint ck = cg.AddComponent<Checkpoint>();
        ck.luz = luz.GetComponent<SpriteRenderer>();
    }

    static void CrearTiaVela(Transform padre, Vector2 pos)
    {
        GameObject vg = Vacio("TiaVela", padre, pos);
        Hijo("Brillo", vg.transform, sBrillo, new Color(1f, 0.75f, 0.3f, 0.35f), new Vector2(0f, 0.6f), new Vector2(2.4f, 2.4f), 11);
        Hijo("Base", vg.transform, sCuad, new Color(0.55f, 0.47f, 0.33f), new Vector2(0f, -0.38f), new Vector2(0.8f, 0.12f), 4);
        Hijo("Cuerpo", vg.transform, sCuad, new Color(1f, 0.96f, 0.83f), new Vector2(0f, 0.1f), new Vector2(0.5f, 0.85f), 4);
        Hijo("OjoIzq", vg.transform, sCirc, new Color(0.1f, 0.1f, 0.2f), new Vector2(-0.1f, 0.22f), new Vector2(0.08f, 0.08f), 5);
        Hijo("OjoDer", vg.transform, sCirc, new Color(0.1f, 0.1f, 0.2f), new Vector2(0.1f, 0.22f), new Vector2(0.08f, 0.08f), 5);
        GameObject llama = Hijo("Llama", vg.transform, sCirc, new Color(1f, 0.65f, 0.2f), new Vector2(0f, 0.7f), new Vector2(0.22f, 0.36f), 5);
        Pulse fp = llama.AddComponent<Pulse>();
        fp.amp = 0.15f; fp.speed = 6f;
    }

    static void CrearFarol(Transform padre, Vector2 pos)
    {
        GameObject fa = Vacio("FarolAncestral", padre, pos);
        GameObject fglow = Hijo("Brillo", fa.transform, sBrillo, new Color(0.7f, 0.8f, 1f, 0.35f), new Vector2(0f, 0.9f), new Vector2(2.2f, 2.2f), 11);
        Hijo("Poste", fa.transform, sCuad, new Color(0.3f, 0.25f, 0.2f), new Vector2(0f, -0.1f), new Vector2(0.2f, 1.8f), 11);
        Hijo("Techo", fa.transform, sCuad, new Color(0.22f, 0.18f, 0.15f), new Vector2(0f, 1.4f), new Vector2(0.95f, 0.15f), 12);
        GameObject lamp = Hijo("Lampara", fa.transform, sCuad, new Color(0.45f, 0.5f, 0.6f), new Vector2(0f, 0.9f), new Vector2(0.7f, 0.8f), 12);
        Farol f = fa.AddComponent<Farol>();
        f.lamp = lamp.GetComponent<SpriteRenderer>();
        f.glow = fglow.GetComponent<SpriteRenderer>();
    }

    static GameObject CrearBarrera(Transform padre, Vector2 centro, int celdasAncho, int celdasAlto)
    {
        GameObject bg = Vacio("BarreraSombra", padre, centro);
        BoxCollider2D bc = bg.AddComponent<BoxCollider2D>();
        bc.size = new Vector2(celdasAncho, celdasAlto);
        Hijo("Base", bg.transform, sCuad, new Color(0.14f, 0.07f, 0.24f, 0.95f), Vector2.zero, new Vector2(celdasAncho, celdasAlto), 4);
        System.Random rb = new System.Random(3);
        for (int i = 0; i < 14; i++)
        {
            Vector2 p = new Vector2(((float)rb.NextDouble() - 0.5f) * (celdasAncho - 0.2f), ((float)rb.NextDouble() - 0.5f) * (celdasAlto - 0.2f));
            float sc = 0.7f + (float)rb.NextDouble() * 0.7f;
            Hijo("Espina", bg.transform, sCirc, new Color(0.22f, 0.12f, 0.36f, 0.95f), p, new Vector2(sc, sc), 5);
        }
        for (int i = 0; i < 3; i++)
        {
            Vector2 p = new Vector2(((float)rb.NextDouble() - 0.5f) * (celdasAncho - 0.8f), ((float)rb.NextDouble() - 0.5f) * (celdasAlto - 0.8f));
            Hijo("Ojo", bg.transform, sCirc, new Color(1f, 0.95f, 0.6f, 0.85f), p, new Vector2(0.14f, 0.14f), 6);
        }
        bg.AddComponent<BarreraSombra>();
        return bg;
    }

    static void AddHint(Transform padre, Vector2 pos, float radio, int prio, string texto, GameObject barrera)
    {
        GameObject h = Vacio("Consejo", padre, pos);
        HintZone z = h.AddComponent<HintZone>();
        z.text = texto;
        z.radius = radio;
        z.priority = prio;
        z.hideIfInactive = barrera;
    }

    static GameObject CrearLumo(Transform padre, Vector2 inicio)
    {
        GameObject lg = Vacio("Lumo", padre, inicio);

        GameObject luz = Hijo("LuzCalida", lg.transform, sBrillo, new Color(1f, 0.85f, 0.45f, 0.32f), Vector2.zero, new Vector2(7f, 7f), 5);
        Hijo("AlaIzq", lg.transform, sCirc, new Color(0.7f, 0.9f, 1f, 0.65f), new Vector2(-0.42f, 0.15f), new Vector2(0.5f, 0.3f), 5);
        Hijo("AlaDer", lg.transform, sCirc, new Color(0.7f, 0.9f, 1f, 0.65f), new Vector2(0.42f, 0.15f), new Vector2(0.5f, 0.3f), 5);
        GameObject cuerpo = Hijo("Cuerpo", lg.transform, sCirc, new Color(1f, 0.74f, 0.24f), Vector2.zero, new Vector2(0.8f, 0.8f), 6);
        Hijo("OjoIzq", cuerpo.transform, sCirc, new Color(0.1f, 0.12f, 0.2f), new Vector2(-0.18f, 0.1f), new Vector2(0.16f, 0.16f), 7);
        Hijo("OjoDer", cuerpo.transform, sCirc, new Color(0.1f, 0.12f, 0.2f), new Vector2(0.18f, 0.1f), new Vector2(0.16f, 0.16f), 7);
        Hijo("Antena", cuerpo.transform, sCuad, new Color(0.55f, 0.35f, 0.05f), new Vector2(0f, 0.58f), new Vector2(0.06f, 0.35f), 6);
        Hijo("Farolito", cuerpo.transform, sCirc, new Color(1f, 0.95f, 0.7f), new Vector2(0f, 0.82f), new Vector2(0.28f, 0.28f), 7);

        // Capa de oscuridad: se ve apagada en el editor y se enciende al darle Play
        float s0 = 3.2f / LumoPlayer.HoleRadius;
        GameObject osc = Hijo("Oscuridad", lg.transform, sOsc, new Color(1f, 1f, 1f, 0.96f), Vector2.zero, new Vector2(s0, s0), 10);
        osc.GetComponent<SpriteRenderer>().enabled = false;

        Rigidbody2D rb = lg.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        CircleCollider2D cc = lg.AddComponent<CircleCollider2D>();
        cc.radius = 0.35f;

        LumoPlayer lp = lg.AddComponent<LumoPlayer>();
        lp.glow = luz.transform;
        lp.overlay = osc.transform;
        lp.body = cuerpo.transform;
        return lg;
    }

    static void CrearCamara(Transform padre, Transform objetivo, Vector2 inicio)
    {
        GameObject cg = new GameObject("Main Camera");
        cg.tag = "MainCamera";
        cg.transform.SetParent(padre, false);
        cg.transform.position = new Vector3(inicio.x, inicio.y, -10f);
        Camera cam = cg.AddComponent<Camera>();
        cg.AddComponent<AudioListener>();
        cam.orthographic = true;
        cam.orthographicSize = 7.5f;
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = Color.black;
        CameraFollow cf = cg.AddComponent<CameraFollow>();
        cf.target = objetivo;
    }
}
