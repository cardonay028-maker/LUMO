using UnityEngine;
using UnityEngine.SceneManagement;

// Controla el estado del nivel: chispas, tiempo, victoria, reinicio y consejos.
// Es un objeto normal de la escena (Sistema > GameManager): se configura desde el Inspector.
public class GameManager : MonoBehaviour
{
    public static GameManager I;

    [Header("Configuracion del nivel")]
    public string nombreNivel = "Nivel 1: Claro de Inicio";
    public string tituloFinal = "¡Farol encendido!";
    public string textoSiguiente = "";        // texto de la pantalla final
    public string siguienteEscena = "";       // nombre de la escena que sigue (vacio = no hay mas)
    public float oscuridadInicial = 0.96f;    // 1 = negro total
    public float oscuridadFinal = 0.55f;      // oscuridad al encender el farol

    [Header("Estado (solo lectura)")]
    public int chispas;
    public int chispasTotal;
    public float time;
    public bool Finished;
    public float finishedTime;
    public float Darkness = 0.96f;
    public Vector2 checkpoint;

    string hintText = "";
    int hintFrame = -10;
    int hintPrio = -1;
    string msgText = "";
    float msgUntil;

    void Awake()
    {
        I = this;
        Darkness = oscuridadInicial;
    }

    void Start()
    {
        chispasTotal = Chispa.All.Count;
        if (LumoPlayer.I != null) checkpoint = LumoPlayer.I.transform.position;
    }

    void Update()
    {
        if (!Finished) time += Time.deltaTime;
        else finishedTime += Time.deltaTime;

        float targetDark = Finished ? oscuridadFinal : oscuridadInicial;
        Darkness = Mathf.Lerp(Darkness, targetDark, 1f - Mathf.Exp(-0.8f * Time.deltaTime));

        if (LumoPlayer.I != null && !LumoPlayer.I.Moved)
            SetHint("Tía Vela: Muévete con WASD o las flechas, Lumo. ¡Tu luz es lo único que ilumina el bosque!", 5);

        if (LumoInput.RestartPressed())
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        if (Finished && finishedTime > 2.2f && !string.IsNullOrEmpty(siguienteEscena) && LumoInput.NextPressed())
            SceneManager.LoadScene(siguienteEscena);
    }

    public void CollectChispa() { chispas++; }

    public void Win()
    {
        Finished = true;
        finishedTime = 0f;
    }

    public void SetHint(string text, int priority)
    {
        if (Time.frameCount != hintFrame)
        {
            hintFrame = Time.frameCount;
            hintPrio = -1;
        }
        if (priority > hintPrio)
        {
            hintPrio = priority;
            hintText = text;
        }
    }

    public string CurrentHint
    {
        get { return (Time.frameCount - hintFrame <= 1) ? hintText : ""; }
    }

    public void ShowMessage(string text, float seconds)
    {
        msgText = text;
        msgUntil = Time.time + seconds;
    }

    public string CurrentMessage
    {
        get { return Time.time < msgUntil ? msgText : ""; }
    }
}
