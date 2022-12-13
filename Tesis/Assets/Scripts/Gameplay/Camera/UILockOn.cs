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
        [SerializeField] float sizeMod;
        [Header("Runtime Values")]
        //[SerializeField] Vector2 screenCenter;
        [SerializeField] bool isCameraLocked;
 

        //Unity Events
        void Start()
        {
            manager.CameraLocked += OnCameraLocked;
            
            //screenCenter = new Vector2(mainCam.pixelWidth / 2, mainCam.pixelHeight / 2);
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

            // transform.position = target.position;
            //
            // Vector3 dir = mainCam.transform.position - transform.position;
            // transform.rotation = Quaternion.LookRotation(dir);
            //
            // Vector3 size = Vector3.one * Vector3.Distance(target.position, mainCam.transform.position);
            // transform.localScale = size * sizeMod;
        }
        
        //Event Receiver
        void OnCameraLocked(bool didCameraLock)
        {
            isCameraLocked = didCameraLock;
            pointer.gameObject.SetActive(didCameraLock);
        }
    }
}