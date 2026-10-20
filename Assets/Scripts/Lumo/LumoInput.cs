using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

// Entrada unificada: funciona con el Input System nuevo y con el Input Manager clasico.
public static class LumoInput
{
    public static Vector2 Move()
    {
        Vector2 v = Vector2.zero;
#if ENABLE_INPUT_SYSTEM
        Keyboard k = Keyboard.current;
        if (k != null)
        {
            if (k.aKey.isPressed || k.leftArrowKey.isPressed) v.x -= 1f;
            if (k.dKey.isPressed || k.rightArrowKey.isPressed) v.x += 1f;
            if (k.sKey.isPressed || k.downArrowKey.isPressed) v.y -= 1f;
            if (k.wKey.isPressed || k.upArrowKey.isPressed) v.y += 1f;
        }
        Gamepad g = Gamepad.current;
        if (g != null)
        {
            Vector2 s = g.leftStick.ReadValue();
            if (s.sqrMagnitude > 0.04f) v += s;
        }
#else
        v.x = Input.GetAxisRaw("Horizontal");
        v.y = Input.GetAxisRaw("Vertical");
#endif
        if (v.sqrMagnitude > 1f) v.Normalize();
        return v;
    }

    public static bool DestelloHeld()
    {
#if ENABLE_INPUT_SYSTEM
        Keyboard k = Keyboard.current;
        Gamepad g = Gamepad.current;
        bool a = k != null && (k.spaceKey.isPressed || k.leftShiftKey.isPressed);
        bool b = g != null && (g.buttonSouth.isPressed || g.rightTrigger.ReadValue() > 0.3f);
        return a || b;
#else
        return Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.JoystickButton0);
#endif
    }

    public static bool InteractPressed()
    {
#if ENABLE_INPUT_SYSTEM
        Keyboard k = Keyboard.current;
        Gamepad g = Gamepad.current;
        bool a = k != null && k.eKey.wasPressedThisFrame;
        bool b = g != null && g.buttonWest.wasPressedThisFrame;
        return a || b;
#else
        return Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.JoystickButton2);
#endif
    }

    public static bool RestartPressed()
    {
#if ENABLE_INPUT_SYSTEM
        Keyboard k = Keyboard.current;
        return k != null && k.rKey.wasPressedThisFrame;
#else
        return Input.GetKeyDown(KeyCode.R);
#endif
    }

    public static bool NextPressed()
    {
#if ENABLE_INPUT_SYSTEM
        Keyboard k = Keyboard.current;
        Gamepad g = Gamepad.current;
        bool a = k != null && (k.enterKey.wasPressedThisFrame || k.numpadEnterKey.wasPressedThisFrame);
        bool b = g != null && g.startButton.wasPressedThisFrame;
        return a || b;
#else
        return Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.JoystickButton7);
#endif
    }

    public static bool DebugPressed()
    {
#if ENABLE_INPUT_SYSTEM
        Keyboard k = Keyboard.current;
        return k != null && k.f1Key.wasPressedThisFrame;
#else
        return Input.GetKeyDown(KeyCode.F1);
#endif
    }
}
