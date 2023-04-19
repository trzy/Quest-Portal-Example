using UnityEngine;

[RequireComponent(typeof(Camera))]
[DisallowMultipleComponent]
[DefaultExecutionOrder(-30000)]
public class PortalCamera : MonoBehaviour
{
  [Tooltip("Where the portal is rendered (entrance to portal).")]
  [SerializeField]
  private MeshRenderer m_portalRenderer;

  [Tooltip("A transfor on the local end (entrance) of the portal.")]
  [SerializeField]
  private Transform m_portalLocalObservationPoint;

  [Tooltip("A transform on the remote end (observed by this camera) of the portal corresponding exactly to the local-end point.")]
  [SerializeField]
  private Transform m_portalRemoteObservationPoint;

  [Tooltip("Left eye transform used for stereoscopic rendering. If not set, searches for a game object named LeftEyeAnchor.")]
  [SerializeField]
  private Transform m_leftEyeAnchor;

  [Tooltip("Right eye transform used for stereoscopic rendering. If null, searches for a game object named RightEyeAnchor.")]
  [SerializeField]
  private Transform m_rightEyeAnchor;

  private Camera m_centerCamera;  // for non-stereoscopic rendering
  private Camera m_leftCamera;
  private Camera m_rightCamera;

  private void LateUpdate()
  {
    if (Camera.main.stereoEnabled)
    {
      // Get eye poses
      Vector3 leftEyePosition = m_leftEyeAnchor.position;
      Vector3 rightEyePosition = m_rightEyeAnchor.position;
      Quaternion leftEyeRotation = m_leftEyeAnchor.rotation;
      Quaternion rightEyeRotation = m_rightEyeAnchor.rotation;

      // Reposition center anchor point at the other side of the portal based on relative position of each eye
      // to the portal entrance
      Vector3 leftEyeOffsetFromPortal = leftEyePosition - m_portalLocalObservationPoint.position;
      Vector3 rightEyeOffsetFromPortal = rightEyePosition - m_portalLocalObservationPoint.position;

      m_leftCamera.transform.position = m_portalRemoteObservationPoint.position + leftEyeOffsetFromPortal;
      m_rightCamera.transform.position = m_portalRemoteObservationPoint.position + rightEyeOffsetFromPortal;

      m_leftCamera.transform.rotation = m_portalRemoteObservationPoint.rotation * Quaternion.Inverse(m_portalLocalObservationPoint.rotation) * leftEyeRotation;
      m_rightCamera.transform.rotation = m_portalRemoteObservationPoint.rotation * Quaternion.Inverse(m_portalLocalObservationPoint.rotation) * rightEyeRotation;

      m_leftCamera.projectionMatrix = Camera.main.GetStereoProjectionMatrix(Camera.StereoscopicEye.Left);
      m_rightCamera.projectionMatrix = Camera.main.GetStereoProjectionMatrix(Camera.StereoscopicEye.Right);

      m_leftCamera.nonJitteredProjectionMatrix = Camera.main.GetStereoNonJitteredProjectionMatrix(Camera.StereoscopicEye.Left);
      m_rightCamera.nonJitteredProjectionMatrix = Camera.main.GetStereoNonJitteredProjectionMatrix(Camera.StereoscopicEye.Right);
    }
    else
    {
      // Reposition center anchor point at the other side of the portal based on relative position of our head
      // to the portal entrance
      Transform cameraTransform = Camera.main.transform;
      Vector3 userOffsetFromPortal = cameraTransform.position - m_portalLocalObservationPoint.position;
      m_centerCamera.transform.position = m_portalRemoteObservationPoint.position + userOffsetFromPortal;
      m_centerCamera.transform.rotation = m_portalRemoteObservationPoint.rotation * Quaternion.Inverse(m_portalLocalObservationPoint.rotation) * cameraTransform.rotation;
      m_centerCamera.projectionMatrix = Camera.main.projectionMatrix;
    }
  }

  private void CreateRenderTarget(Camera camera)
  {
    if (camera.targetTexture != null)
    {
      camera.targetTexture.Release();
    }
    camera.targetTexture = new RenderTexture(Camera.main.pixelWidth, Camera.main.pixelHeight, 24);
  }

  // Creates a portal-observing camera representing a stereoscopic eye anchored to center camera
  private Camera CreateEyeCamera(Camera.StereoscopicEye eye)
  {
    GameObject cameraContainer = new GameObject(eye == Camera.StereoscopicEye.Left ? "LeftEye" : "RightEye");
    cameraContainer.tag = m_centerCamera.gameObject.tag;
    cameraContainer.transform.parent = m_centerCamera.transform;
    cameraContainer.transform.localPosition = Vector3.zero;
    cameraContainer.transform.localRotation = Quaternion.identity;
    
    Camera camera = cameraContainer.AddComponent<Camera>();
    camera.CopyFrom(m_centerCamera);  // can't use main camera because we would inherit pose tracking
    camera.projectionMatrix = Camera.main.GetStereoProjectionMatrix(eye);
    CreateRenderTarget(camera);
    return camera;
  }

  private void Start()
  {
    if (!Camera.main.stereoEnabled)
    {
      CreateRenderTarget(m_centerCamera);
      m_portalRenderer.material.SetTexture("_LeftEyeTexture", m_centerCamera.targetTexture);
    }
    else
    {
      m_leftCamera = CreateEyeCamera(Camera.StereoscopicEye.Left);
      m_rightCamera = CreateEyeCamera(Camera.StereoscopicEye.Right);

      m_portalRenderer.material.SetTexture("_LeftEyeTexture", m_leftCamera.targetTexture);
      m_portalRenderer.material.SetTexture("_RightEyeTexture", m_rightCamera.targetTexture);

      m_centerCamera.enabled = false;
      m_leftCamera.enabled = true;
      m_rightCamera.enabled = true;
    }
  }

  private void Awake()
  {
    m_centerCamera = GetComponent<Camera>();
    Debug.Assert(m_centerCamera != null);

    if (m_leftEyeAnchor == null)
    {
      GameObject leftEyeAnchor = GameObject.Find("LeftEyeAnchor");
      Debug.Assert(leftEyeAnchor != null);
      m_leftEyeAnchor = leftEyeAnchor.transform;
    }

    if (m_rightEyeAnchor == null)
    {
      GameObject rightEyeAnchor = GameObject.Find("RightEyeAnchor");
      Debug.Assert(rightEyeAnchor != null);
      m_rightEyeAnchor = rightEyeAnchor.transform;
    }

    Debug.Assert(m_portalRenderer != null);
    Debug.Assert(m_portalLocalObservationPoint != null);
    Debug.Assert(m_portalRemoteObservationPoint != null);
  }
}