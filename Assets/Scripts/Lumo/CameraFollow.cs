using UnityEngine;

// Hace que la camara siga a Lumo suavemente.
public class CameraFollow : MonoBehaviour
{
    public Transform target;     // se asigna en la escena (Lumo)
    public float smooth = 6f;

    void LateUpdate()
    {
        Transform t = target;
        if (t == null && LumoPlayer.I != null) t = LumoPlayer.I.transform;
        if (t == null) return;
        Vector3 goal = new Vector3(t.position.x, t.position.y, -10f);
        Vector3 p = Vector3.Lerp(transform.position, goal, 1f - Mathf.Exp(-smooth * Time.deltaTime));
        p.z = -10f;
        transform.position = p;
    }
}
