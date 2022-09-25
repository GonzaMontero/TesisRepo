using UnityEngine;

namespace TimeDistortion.Gameplay.Cameras
{
    public class TimeCameraController : LockCameraController
    {
        [Header("--Child--", order = -1)]
        [Header("Set Values")]
        [SerializeField] Cinemachine.CinemachineInputProvider input;
        //[Header("Runtime Values")]


        //Unity Events
        internal new void Start()
        {
            base.Start();

            if(input == null)
            {
                input = GetComponent<Cinemachine.CinemachineInputProvider>();
            }
        }

        //Methods
        internal override void UpdateLockOn()
        {
            base.UpdateLockOn();
            input.enabled = !isLocked;
        }
    }
}