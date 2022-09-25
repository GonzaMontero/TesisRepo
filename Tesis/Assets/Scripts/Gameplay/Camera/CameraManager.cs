using Cinemachine;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TimeDistortion.Gameplay.Cameras
{
    public class CameraManager : MonoBehaviour
    {
        public enum Cameras { free, lockOn, time,     _count }

        [Header("Set Values")]
        [SerializeField] CinemachineVirtualCameraBase freeCamera;
        [SerializeField] CinemachineVirtualCameraBase lockOnCamera;
        [SerializeField] CinemachineVirtualCameraBase timeCamera;
        [Header("Runtime Values")]
        [SerializeField] CinemachineVirtualCameraBase[] cameras;
        [SerializeField] CinemachineVirtualCameraBase currentCamera;
        [SerializeField] Cameras targetCamera;


        //Unity Events
        private void Start()
        {
            //Set cameras array
            SetCameras();

            //Set current camera
            currentCamera = freeCamera;
            currentCamera.enabled = true;

            //Link actions
            LockCameraController lockController;
            lockController = lockOnCamera.GetComponent<LockCameraController>();
            if (lockController)
            {
                lockController.CameraLocked += OnCameraLockOn;
            }
        }
        public void OnTimeCharge(InputAction.CallbackContext context)
        {
            if (context.canceled)
            {
                targetCamera = Cameras.free;
            }
            else if (context.started)
            {
                targetCamera = Cameras.time;
            }

            ChangeCamera();
        }

        //Methods
        /// <summary> Set cameras array </summary>
        void SetCameras()
        {
            cameras = new CinemachineVirtualCameraBase[(int)Cameras._count];
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
                cameras[i].enabled = false;
            }
        }
        /// <summary> 
        /// Change current camera, disabling old one and enabling new one
        /// </summary>
        void ChangeCamera()
        {
            currentCamera.enabled = false;
            currentCamera = cameras[(int)targetCamera];
            currentCamera.enabled = true;
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