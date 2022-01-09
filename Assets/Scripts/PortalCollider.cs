using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Collider))]
public class PortalCollider : MonoBehaviour
{
  [SerializeField]
  [Tooltip("The corresponding point on the destination portal to which the player will be teleported.")]
  private Transform m_destination;

  private Collider m_portalCollider;

  private void OnTriggerEnter(Collider other)
  {
        Debug.LogFormat("Got here: {0}", other.name);
    // Note: Portal forward axis is assumed to point *into* the portal
    Vector3 nearestIntersectionPoint = m_portalCollider.ClosestPointOnBounds(other.transform.position);
    Vector3 objectToPortal = nearestIntersectionPoint - other.transform.position;
    if (Vector3.Dot(objectToPortal, transform.forward) >= 0)
    {
      Debug.LogFormat("WARP {0}", other.name);
      Quaternion rotate180 = Quaternion.AngleAxis(180, Vector3.up);
      Vector3 localPosition = rotate180 * transform.InverseTransformPoint(other.transform.position);
      Quaternion localRotation = rotate180 * Quaternion.Inverse(transform.rotation) * other.transform.rotation;
      other.transform.position = m_destination.TransformPoint(localPosition);
      other.transform.rotation = m_destination.rotation * localRotation;
    }
  }

  private void Awake()
  {
    m_portalCollider = GetComponent<Collider>();
    Debug.Assert(m_destination != null);
  }
}
