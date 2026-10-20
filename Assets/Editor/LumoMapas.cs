// Mapas de los niveles de LUMO. Cada letra es una celda de 1 x 1 unidad de Unity.
// Para cambiar un nivel, edita las letras y vuelve a ejecutar el paso del menu LUMO.
//
//  #  arbol (pared)            .  tierra / suelo        ~  agua del pantano
//  b  puente de luz            B  puente tenue (solo aparece con Destello)
//  R  roca 2x2                 X  barrera de sombra     S  inicio de Lumo
//  F  Farol Ancestral (meta)   c  chispa dorada         f  Flor de Luz
//  V  Tia Vela (guia)          k  punto de control
public class LumoNivel
{
    public string escena;
    public string nombre;
    public string tituloFinal;
    public string siguienteEscena;
    public string textoSiguiente;
    public bool pantano;
    public string[] mapa;
}

public static class LumoMapas
{
    public static readonly LumoNivel Nivel1 = new LumoNivel
    {
        escena = "Nivel1_ClaroDeInicio",
        nombre = "Nivel 1: Claro de Inicio",
        tituloFinal = "¡Primer Farol encendido!",
        siguienteEscena = "Nivel2_PantanoDeLuciernagas",
        textoSiguiente = "El bosque empieza a recuperar su luz.",
        pantano = false,
        mapa = new string[]
        {
            "####################################",
            "############c.######################",
            "#...........................XX.....#",
            "#.......c.......RR.......f..XX...F.#",
            "#........f......RR..........XX.....#",
            "#...........................XX.....#",
            "#....###############################",
            "#..................................#",
            "#.........RR......c.........f......#",
            "#.........RR...f.....RR............#",
            "#....................RR............#",
            "###############################....#",
            "#..................................#",
            "#.......f..........................#",
            "#..........RR.........f............#",
            "#.S........RR......................#",
            "#...V.........c.........RR.........#",
            "#.......................RR.........#",
            "#..................................#",
            "##################c.################",
            "####################################"
        }
    };

    public static readonly LumoNivel Nivel2 = new LumoNivel
    {
        escena = "Nivel2_PantanoDeLuciernagas",
        nombre = "Nivel 2: Pantano de Luciérnagas",
        tituloFinal = "¡Segundo Farol encendido!",
        siguienteEscena = "",
        textoSiguiente = "El pantano brilla otra vez.\nSiguiente: Nivel 3, Cueva de Cristales (próximamente)",
        pantano = true,
        mapa = new string[]
        {
            "################################################",
            "################################################",
            "#~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~#",
            "#~~~~~~~~~~~~~~~~~~~~~~~~~~~.......~~.....~~~~~#",
            "#~~~~~~~~~~~~~~~~~~~~~~~~~~~.....c.~~.....~~~~~#",
            "#~~~~~~~~~~~~~~~~~~~~~~~~~~~.......bb..c..~~~~~#",
            "#~~~~~~~~~~~~~~~~~~~~~~~~~~~.......bb.....~~~~~#",
            "#~~~~~~~~~~~~~~~~~~~~~bbbbbb.......~~.....~~~~~#",
            "#~~~~~~~~~~~~~~~~~~~~~bbbbbb.......~~.....~~~~~#",
            "#~~~~~~~~~~~~~~~~~~~~~bb~~~~.k.f...~~~~~~~~~~~~#",
            "#~~~~~~~~~~~~~~~~~~~~~bb~~~~.......~~~~~~~~~~~~#",
            "#~~~~~~~~~~~~~~~~~~.......~~~bb~~~~~~~~~~~~~~~~#",
            "#~~~~~~~~~~~~~~~~~~.....c.~~~bb~~~~~~~~~~~~~~~~#",
            "#~~~~~~~~~~~~~~~~~~..k....~~~bb~~~~~~~~~~~~~~~~#",
            "#~~~~~~~~~~~bbbbbbb.......~~~bb~~~~~~~~........#",
            "#~~~~~~~~~~~bbbbbbb.......~~~bb~~~~~~~~........#",
            "#.c......~~~bb~~~~~.....f.~~~bb~~~~~~~~..f.....#",
            "#....V...~~~bb~~~~~.......~~~bbBBBBBBBB........#",
            "#........bbbbb~~~~~.......~~~bbBBBBBBBB.....F..#",
            "#..S.....bbbbb~~~~~~~~bb~~~~~~~~~~~~~~~........#",
            "#......f.~~~~~~~~~~~~~bb~~~~~~~~~~~~~~~........#",
            "#........~~~~~~~~~~~......~~~~~~~~~~~~~........#",
            "#........~~~~~~~~~~~..c...~~~~~~~~~~~~~........#",
            "#~~~~~~~~~~~~~~~~~~~......~~~~~~~~~~~~~~~~~~~~~#",
            "################################################",
            "################################################"
        }
    };
}
