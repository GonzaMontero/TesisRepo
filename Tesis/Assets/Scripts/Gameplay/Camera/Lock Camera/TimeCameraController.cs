using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TimeDistortion.Gameplay.Cameras
{
    public class TimeCameraController : LockCameraController
    {
        [Header("--Child--", order = -1)]
        [Header("Set Values")]
        [Tooltip("X = min angle, Y = max angle | (X rotates up & Y rotates down)")]
        [SerializeField] Vector2 verticalAngleLimits;
        [SerializeField] [Range(0, 1)] float rotationImpulseY;
        [SerializeField] [Range(0, 1)] float rotationImpulseX;
        [SerializeField] float rotationSpeed;
        [Header("Runtime Values")]
        [SerializeField] Transform camFollow;
        [SerializeField] Quaternion targetRotationY;
        [SerializeField] Quaternion targetRotationX;
        [SerializeField] Vector2 input;

        //Unity Events
        internal new void Start()
        {
            base.Start();

            camFollow = cam.Follow;
        }
        internal override void Update()
        {
            if(!cam.enabled) return;
            if (input.magnitude > 0)
            {
                SetRotation();
            }
            
            Rotate();
            base.Update();
        }
        public void OnRotateInput(InputAction.CallbackContext context)
        {
            if (isLocked) return;
            if(!cam.enabled) return;

            input = context.ReadValue<Vector2>();
        }
        public override void OnLockOnInput(InputAction.CallbackContext context)
        {
            if(!cam.enabled) return;
            base.OnLockOnInput(context);
        }

        //Methods
        public override void SetCameraActive(bool isActive)
        {
            base.SetCameraActive(isActive);

            //Set rotations
            if (!camFollow) return;
            if (isActive)
            {
                //Take other cameras rotation
                SetInitialRotation();
            }
            else
            {
                //Reset cam follow for other cameras
                camFollow.localRotation = Quaternion.identity;
            }
        }
        internal override void UpdateLockOn(bool shouldLock)
        {
            base.UpdateLockOn(shouldLock);
            
            //Reset cam follow rotation
            camFollow.localRotation = Quaternion.identity;

            //Replace lock on mode with aim mode
            CinemachineVirtualCamera vCam = (CinemachineVirtualCamera)cam;
            if (shouldLock)
            {
                vCam.AddCinemachineComponent<CinemachineComposer>();
            }
            else
            {
                vCam.AddCinemachineComponent<CinemachineSameAsFollowTarget>();
            }
        }

        void SetInitialRotation()
        {
            //Get camera rotation
            Vector3 mainRot = mainCam.eulerAngles;
            
            //Set Y rotation
            //camFollow.localRotation = Quaternion.Euler(Vector3.right * mainRot.x);
            targetRotationY = Quaternion.Euler(Vector3.right * mainRot.x);
            
            //Set X rotation
            //player.localRotation = Quaternion.Euler(Vector3.up * mainRot.y);
            targetRotationX = Quaternion.Euler(Vector3.up * mainRot.y);
        }
        void SetRotation()
        {
            if (isLocked) return;

            //Get how much should the target rotation rotate
            Quaternion inputRotY;
            Quaternion inputRotX;
            inputRotY = Quaternion.Euler(Vector3.right * input.y * -rotationImpulseY); //positive impulse inverts rot
            inputRotX = Quaternion.Euler(Vector3.up * input.x * rotationImpulseX);
            
            //Get the target rotation rotated by the input rotation (if target rotation is 0, take input rotation)
            Quaternion newRotY;
            Quaternion newRotX;
            newRotY = targetRotationY.eulerAngles.magnitude == 0 ? inputRotY : targetRotationY * inputRotY;
            newRotX = targetRotationX.eulerAngles.magnitude == 0 ? inputRotX : targetRotationX * inputRotX;

            //Make sure the new target rotation is beyond Y angle limits
            newRotY = ClampYAngle(newRotY);
            
            //Update target rotation
            targetRotationY = newRotY;
            targetRotationX = newRotX;
        }
        Quaternion ClampYAngle(Quaternion rot)
        {
            //Check if rotation is beyond Y limits
            float localRot = rot.eulerAngles.x;
            float minAngleExcess;
            minAngleExcess = (localRot > 180 ? (localRot - 360) : localRot) - verticalAngleLimits.x;
            float maxAngleExcess;
            maxAngleExcess = verticalAngleLimits.y - (localRot > 180 ? (localRot - 360) : localRot);
            
            //Fix rotation
            if (minAngleExcess < 0)
            {
                rot *= Quaternion.Euler(Vector3.right * -minAngleExcess); 
            }
            else if (maxAngleExcess < 0)
            {
                rot *= Quaternion.Euler(Vector3.right * maxAngleExcess); 
            }

            return rot;
        }
        void Rotate()
        {
            if(isLocked) return;
            
            float frameRot = Time.unscaledDeltaTime * rotationSpeed;

            //Rotate player (in horizontal axis)
            Quaternion rotation = Quaternion.Slerp(player.localRotation, targetRotationX, frameRot);
            player.localRotation = rotation;
            
            //Rotate cam follow (in vertical axis)
            rotation = Quaternion.Slerp(camFollow.localRotation, targetRotationY, frameRot);
            camFollow.localRotation = rotation;
        }
    }
}