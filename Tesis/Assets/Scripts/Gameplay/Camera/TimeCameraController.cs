using Cinemachine.Utility;
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
            
            Rotate();
            base.Update();
        }
        public void OnRotateInput(InputAction.CallbackContext context)
        {
            if (isLocked) return;
            if(!cam.enabled) return;

            input = context.ReadValue<Vector2>();
            SetRotatation();
        }

        //Methods
        public override void SetCameraActive(bool isActive)
        {
            base.SetCameraActive(isActive);

            //Reset cam follow rotation
            camFollow.localRotation = Quaternion.identity;
            targetRotationY = camFollow.localRotation;
            targetRotationX = player.localRotation;
        }
        internal override void UpdateLockOn()
        {
            base.UpdateLockOn();

            //Reset cam follow rotation
            camFollow.rotation = Quaternion.identity;
        }
        void SetRotatation()
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
            
            #region DEPRECATED
            // if (Mathf.Pow(input.y, 2) > 0 && rotationSpeeds.y > 0)
            // {
            //     SetRotationVertical(input.y);
            // }
            // if (Mathf.Pow(input.x, 2) > 0 && rotationSpeeds.x > 0)
            // {
            //     SetRotationHorizontal(input.x);
            // }
            #endregion
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
            float frameRot = Time.unscaledDeltaTime * rotationSpeed;

            //Rotate player (in horizontal axis)
            Quaternion rotation = Quaternion.Slerp(player.localRotation, targetRotationX, frameRot);
            player.localRotation = rotation;
            
            //Rotate cam follow (in vertical axis)
            rotation = Quaternion.Slerp(camFollow.localRotation, targetRotationY, frameRot);
            camFollow.localRotation = rotation;
        }
        #region DEPRECATED
        void SetRotationVertical(float rotation)
        {
            // //Get rotation
            // float rot = -rotation * rotationSpeeds.y * Time.unscaledDeltaTime;
            //
            // //Rotate
            // camFollow.Rotate(rot, 0, 0);
            //
            // //Check if rotation is beyond limits
            // float localRot = camFollow.localRotation.eulerAngles.x;
            // float minAngleExcess;
            // minAngleExcess = (localRot > 180 ? (localRot - 360) : localRot) - verticalAngleLimits.x;
            // float maxAngleExcess;
            // maxAngleExcess = verticalAngleLimits.y - (localRot > 180 ? (localRot - 360) : localRot);
            //
            // //Fix rotation
            // if (minAngleExcess < 0)
            // {
            //     camFollow.Rotate(-minAngleExcess, 0, 0);
            // }
            // else if (maxAngleExcess < 0)
            // {
            //     camFollow.Rotate(maxAngleExcess, 0, 0);
            // }
        }
        void SetRotationHorizontal(float rotation)
        {
            // //Get rotation
            // float rot = rotation * rotationSpeeds.x * Time.unscaledDeltaTime;
            //
            // //Rotate
            // player.Rotate(0, rot, 0);
        }
        #endregion
    }
}