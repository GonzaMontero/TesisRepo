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
        [SerializeField] Vector2 rotationSpeeds;
        [Header("Runtime Values")]
        [SerializeField] Transform camFollow;
        [SerializeField] Vector2 input;

        //Unity Events
        internal new void Start()
        {
            base.Start();

            camFollow = cam.Follow;
        }

        internal override void Update()
        {
            Rotate();
            
            base.Update();
        }

        public void OnRotateInput(InputAction.CallbackContext context)
        {
            if (isLocked) return;

            input = context.ReadValue<Vector2>();
        }

        //Methods
        public new void SetCameraActive(bool isActive)
        {
            base.SetCameraActive(isActive);

            //Reset cam follow rotation
            camFollow.localRotation = Quaternion.identity;
        }
        internal override void UpdateLockOn()
        {
            base.UpdateLockOn();

            //Reset cam follow rotation
            camFollow.rotation = Quaternion.identity;
        }

        void Rotate()
        {
            if (isLocked) return;
            
            if (Mathf.Pow(input.y, 2) > 0 && rotationSpeeds.y > 0)
            {
                RotateVertical(input.y);
            }
            if (Mathf.Pow(input.x, 2) > 0 && rotationSpeeds.x > 0)
            {
                RotateHorizontal(input.x);
            }
        }
        void RotateVertical(float rotation)
        {
            //Get rotation
            float rot = -rotation * rotationSpeeds.y * Time.unscaledDeltaTime;

            //Rotate
            camFollow.Rotate(rot, 0, 0);

            //Check if rotation is beyond limits
            float localRot = camFollow.localRotation.eulerAngles.x;
            float minAngleExcess;
            minAngleExcess = (localRot > 180 ? (localRot - 360) : localRot) - verticalAngleLimits.x;
            float maxAngleExcess;
            maxAngleExcess = verticalAngleLimits.y - (localRot > 180 ? (localRot - 360) : localRot);

            //Fix rotation
            if (minAngleExcess < 0)
            {
                camFollow.Rotate(-minAngleExcess, 0, 0);
            }
            else if (maxAngleExcess < 0)
            {
                camFollow.Rotate(maxAngleExcess, 0, 0);
            }
        }
        void RotateHorizontal(float rotation)
        {
            //Get rotation
            float rot = rotation * rotationSpeeds.x * Time.unscaledDeltaTime;

            //Rotate
            player.Rotate(0, rot, 0);
        }
    }
}