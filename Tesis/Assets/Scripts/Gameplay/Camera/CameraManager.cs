using UnityEngine;
using UnityEngine.InputSystem;

namespace TimeDistortion.Gameplay.Cameras
{
    public class CameraManager : MonoBehaviour
    {
        public enum Cameras { free, lockOn, time,     _count }

        [Header("Set Values")]
        [SerializeField] CameraController freeCamera;
        [SerializeField] CameraController lockOnCamera;
        [SerializeField] CameraController timeCamera;
        [Header("Runtime Values")]
        [SerializeField] CameraController[] cameras;
        [SerializeField] CameraController currentCamera;
        [SerializeField] Cameras targetCamera;


        //Unity Events
        private void Start()
        {
            //Lock Cursor
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            //Set cameras array
            SetCameras();

            //Set current camera
            currentCamera = freeCamera;
            currentCamera.SetCameraActive(true);

            //Link actions
            LockCameraController lockController = (LockCameraController)lockOnCamera;
            if (lockController)
            {
                lockController.CameraLocked += OnCameraLockOn;
            }
        }
        public void OnTimeCharge(InputAction.CallbackContext context)
        {
            //Check if camera is locked on, then transfer to right camera
            if (context.canceled)
            {
                LockCameraController timeCam = (LockCameraController)timeCamera;
                
                if (timeCam.isLocked)
                {
                    targetCamera = Cameras.lockOn;
                    
                    //Switch lock target from one camera to the other
                    LockCameraController lockCam = (LockCameraController)lockOnCamera;
                    lockCam.SetTarget(timeCam.publicTarget);
                }
                else
                {
                    targetCamera = Cameras.free;
                }
            }
            else if (context.started)
            {
                if (targetCamera == Cameras.lockOn)
                {
                    //Switch lock target from one camera to the other
                    LockCameraController timeCam = (LockCameraController)timeCamera;
                    LockCameraController lockCam = (LockCameraController)lockOnCamera;
                    timeCam.SetTarget(lockCam.publicTarget);
                }
                
                targetCamera = Cameras.time;
            }

            ChangeCamera();
        }

        //Methods
        /// <summary> Set cameras array </summary>
        void SetCameras()
        {
            cameras = new CameraController[(int)Cameras._count];
            for (int i = 0; i < cameras.Length; i++)
            {
                switch ((Cameras)i)
                {
                    case Cameras.free:
                        cameras[i] = freeCamera;
                        break;
                    case Cameras.lockOn:
                        cameras[i] = lockOnCamera;
                        break;
                    case Cameras.time:
                        cameras[i] = timeCamera;
                        break;
                    default:
                        break;
                }
                cameras[i].SetCameraActive(false);
            }
        }
        /// <summary> 
        /// Change current camera, disabling old one and enabling new one
        /// </summary>
        void ChangeCamera()
        {
            currentCamera.SetCameraActive(false);
            currentCamera = cameras[(int)targetCamera];
            currentCamera.SetCameraActive(true);
        }

        //Event Receivers
        void OnCameraLockOn(bool isLocked)
        {
            switch (targetCamera)
            {
                case Cameras.free:
                    if (isLocked)
                        targetCamera = Cameras.lockOn;
                    break;
                case Cameras.lockOn:
                    if (!isLocked)
                        targetCamera = Cameras.free;
                    break;
                default:
                    return;
            }

            ChangeCamera();
        }
    }
}