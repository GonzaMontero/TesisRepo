using System;
using UnityEngine;

namespace TimeDistortion.Gameplay.Cameras
{
    public class UILockOn : MonoBehaviour
    {
        [Header("Set Values")]
        [SerializeField] CameraManager manager;
        [SerializeField] Camera mainCam;
        [SerializeField] Transform pointer;
        [Header("Runtime Values")]
        [SerializeField] bool isCameraLocked;
 

        //Unity Events
        void Start()
        {
            manager.CameraLocked += OnCameraLocked;
        }
        void Update()
        {
            if (!isCameraLocked) return;
            UpdatePointer();
        }

        //Methods
        void UpdatePointer()
        {
            Transform target = manager.GetCurrentLockTarget();
            
            if(!target) return;

            pointer.position = mainCam.WorldToScreenPoint(target.position);
        }
        
        //Event Receiver
        void OnCameraLocked(bool didCameraLock)
        {
            isCameraLocked = didCameraLock;
            pointer.gameObject.SetActive(didCameraLock);
        }
    }
}