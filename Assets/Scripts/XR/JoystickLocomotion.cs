using UnityEngine;

public class JoystickLocomotion : MonoBehaviour
{
    [SerializeField]
    private float m_speed = 1;

    private Vector3 xzNormalized(Vector3 v)
    {
        return new Vector3(v.x, 0, v.z).normalized;
    }

    private void Update()
    {
        OVRInput.Update();
        Vector2 joy = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick) * m_speed * Time.deltaTime;
        Vector3 cameraForward = xzNormalized(Camera.main.transform.forward);
        Vector3 cameraRight = xzNormalized(Camera.main.transform.right);
        transform.position = transform.position + cameraForward * joy.y + cameraRight * joy.x;
    }
}
