using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform Patroclo;

    public Vector3 offset = new Vector3(0, 5, -7);

    void LateUpdate()
    {
        if (Patroclo != null)
        {
            transform.position = Patroclo.position + offset;
            transform.LookAt(Patroclo);
        }
    }
}