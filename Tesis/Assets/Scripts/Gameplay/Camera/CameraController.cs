using UnityEngine;

namespace TimeDistortion.Gameplay.Cameras
{
    public class CameraController : MonoBehaviour
    {
        [Header("Set Values")]
        [SerializeField] internal Cinemachine.CinemachineVirtualCameraBase cam;

        //Unity Events
        internal virtual void Start()
        {
            if (cam == null)
            {
                cam = GetComponent<Cinemachine.CinemachineVirtualCameraBase>();
            }
        }

        //Methods
        public virtual void SetCameraActive(bool isActive)
        {
            if(cam == null)
            {
                cam = GetComponent<Cinemachine.CinemachineVirtualCameraBase>();
            }

            cam.enabled = isActive;
        }
    }
}