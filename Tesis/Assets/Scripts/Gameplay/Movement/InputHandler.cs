using UnityEngine;
using UnityEngine.InputSystem;

namespace TimeDistortion.Gameplay.Handler
{
    public class InputHandler : MonoBehaviour
    {
        public float horizontal;
        public float vertical;
        public float moveAmount;
        public float mouseX;
        public float mouseY;

        public bool jumpInput;
        public bool lockOnInput;

        public bool lockOnFlag;

        PlayerControls inputActions;
        CameraHandler cameraHandler;
        PlayerAttacker attacker;
        CharacterController characterController;

        Vector2 movementInput;

        private Vector3 playerVelocity;
        private bool groundedPlayer;
        [SerializeField] float playerSpeed = 15;
        [SerializeField] float rotationSmoothing = 1;
        private float jumpHeight = 1.0f;
        private float gravityValue = -9.81f;

        private void Start()
        {
            cameraHandler = CameraHandler.singleton;
            attacker = GetComponent<PlayerAttacker>();
            characterController = GetComponent<CharacterController>();

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Update()
        {
            float delta = Time.unscaledDeltaTime;

            //Calculate Y Movement
            groundedPlayer = characterController.isGrounded;
            if (groundedPlayer && playerVelocity.y < 0)
            {
                playerVelocity.y = 0f;
            }

            playerVelocity.y += gravityValue * Time.unscaledDeltaTime;
            characterController.Move(playerVelocity * Time.unscaledDeltaTime);

            //Calculate XZ Movement
            if (movementInput.magnitude > 0)
            {
                Vector3 moveX = Camera.main.transform.right * movementInput.x;
                Vector3 moveZ = Camera.main.transform.forward * movementInput.y;

                Vector3 move = moveZ + moveX;

                characterController.Move(move * Time.unscaledDeltaTime * playerSpeed);

                if (move.magnitude > 0)
                {
                    move.y = 0;
                    RotateCharacter(move);
                    //gameObject.transform.forward = move;
                }
            }
            
            //Calculate Camera Movement
            if (cameraHandler != null)
            {
                cameraHandler.FollowTarget(delta);
                cameraHandler.HandleCameraRotation(delta, mouseX, mouseY);
            }
        }

        public void OnMoveCameraInput(InputAction.CallbackContext context)
        {
            if (cameraHandler == null) return;

            mouseX = context.ReadValue<Vector2>().x;
            mouseY = context.ReadValue<Vector2>().y;
        }
        public void OnLockCameraInput(InputAction.CallbackContext context)
        {
            HandleLockOnInput();
        }
        public void OnMovementInput(InputAction.CallbackContext context)
        {
            //if (!context.performed) return;
            movementInput = context.ReadValue<Vector2>();
                        
        }
        public void OnJumpInput(InputAction.CallbackContext context)
        {
            if (!groundedPlayer) return;
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
            characterController.Move(playerVelocity * Time.unscaledDeltaTime);
        }
        public void OnAttackInput(InputAction.CallbackContext context)
        {
            HandleAttackInput();
        }

        //private void MoveInput(float delta)
        //{
        //    horizontal = movementInput.x;
        //    vertical = movementInput.y;
        //    moveAmount = Mathf.Clamp01(Mathf.Abs(horizontal) + Mathf.Abs(vertical));
        //    mouseX = cameraInput.x;
        //    mouseY = cameraInput.y;
        //}

        void RotateCharacter(Vector3 direction)
        {
            var newRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, Time.deltaTime * rotationSmoothing);
        }

        private void HandleLockOnInput()
        {
            if (!lockOnFlag)
            {
                cameraHandler.ClearLockOnTargets();
                lockOnInput = false;
                cameraHandler.HandleLockOn();
                if (cameraHandler.nearestLockOnTarget != null)
                {
                    cameraHandler.currentLockOnTarget = cameraHandler.nearestLockOnTarget;
                    lockOnFlag = true;
                }
            }
            else if (lockOnFlag)
            {
                lockOnInput = false;
                lockOnFlag = false;
                cameraHandler.ClearLockOnTargets();
            }
        }

        private void HandleAttackInput()
        {
            attacker.HandleLightAttack();
            Debug.Log("Ataque");
        }
    }
}