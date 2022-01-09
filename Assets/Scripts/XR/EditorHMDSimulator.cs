using UnityEngine;

public class EditorHMDSimulator : MonoBehaviour
{
#if UNITY_EDITOR
  public float walkSpeed = 1;
  public float turnSpeed = 30;
  public float mouseSensitivity = 100.0f;
  public float clampAngle = 80.0f;

  private Vector3 m_euler = Vector3.zero;

  private void UpdateMovement()
  {
    // Rotate by maintaining Euler angles relative to world
    if (Input.GetKey("left"))
    {
      m_euler.y -= turnSpeed * Time.deltaTime;
    }
    if (Input.GetKey("right"))
    {
      m_euler.y += turnSpeed * Time.deltaTime;
    }
    if (Input.GetKey("up"))
    {
      m_euler.x -= turnSpeed * Time.deltaTime;
    }
    if (Input.GetKey("down"))
    {
      m_euler.x += turnSpeed * Time.deltaTime;
    }
    if (Input.GetKey("o"))
    {
      m_euler.x = 0;
      m_euler.z = 0;
    }
    if (Input.GetKey(KeyCode.KeypadDivide) || Input.GetKey(KeyCode.Comma))
    {
      m_euler.z -= turnSpeed * Time.deltaTime;
    }
    if (Input.GetKey(KeyCode.KeypadMultiply) || Input.GetKey(KeyCode.Period))
    {
      m_euler.z += turnSpeed * Time.deltaTime;
    }

    // Motion relative to XZ plane
    float moveSpeed = walkSpeed * Time.deltaTime;
    Vector3 forward = transform.forward;
    forward.y = 0;
    forward.Normalize();  // even if we're looking up or down, will continue to move in XZ
    if (Input.GetKey("w"))
    {
      transform.Translate(forward * moveSpeed, Space.World);
    }
    if (Input.GetKey("s"))
    {
      transform.Translate(-forward * moveSpeed, Space.World);
    }
    if (Input.GetKey("a"))
    {
      transform.Translate(-transform.right * moveSpeed, Space.World);
    }
    if (Input.GetKey("d"))
    {
      transform.Translate(transform.right * moveSpeed, Space.World);
    }

    // Vertical motion
    if (Input.GetKey(KeyCode.PageUp))   // up
    {
      transform.Translate(Vector3.up * moveSpeed, Space.World);
    }
    if (Input.GetKey(KeyCode.PageDown)) // down
    {
      transform.Translate(-Vector3.up * moveSpeed, Space.World);
    }
  }

  private void UpdateMouseLook()
  {
    // On right mouse button down
    if (Input.GetMouseButton(1))
    {
      float mouseX = Input.GetAxis("Mouse X");
      float mouseY = -Input.GetAxis("Mouse Y");

      m_euler.y += mouseX * mouseSensitivity * Time.deltaTime;
      m_euler.x += mouseY * mouseSensitivity * Time.deltaTime;

      m_euler.x = Mathf.Clamp(m_euler.x, -clampAngle, clampAngle);
    }
  }

  private void Update()
  {
    UpdateMovement();
    UpdateMouseLook();
    Quaternion localRotation = Quaternion.Euler(m_euler);
    transform.rotation = localRotation;
  }

  private void Start()
  {
    Vector3 rot = transform.localRotation.eulerAngles;
    m_euler.y = rot.y;
    m_euler.x = rot.x;
  }
#endif
}