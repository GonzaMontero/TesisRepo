using System;
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

        public float turnSmoothTime = 0.1f;
        public float turnSmoothVelocity;

        public bool lockOnFlag;

        PlayerControls inputActions;
        CameraHandler cameraHandler;
        PlayerAttacker attacker;
        CharacterController characterController;

        Vector2 movementInput;

        private Vector3 playerVelocity;
        [SerializeField] bool groundedPlayer;
        [SerializeField] float playerSpeed = 15;
        [SerializeField] float rotationSmoothing = 1;
        [SerializeField] float weight;
        [SerializeField] float jumpHeight = 1.0f;
        private float gravityValue = -9.81f;


        [SerializeField] Transform groundcheck;
        [SerializeField] LayerMask groundLayer;
        [SerializeField] float groundRadius;

        Vector3 lastPos;

        public Action<bool> CameraLocked;
        public Action<bool> PlayerMoved;
        public Action PlayerJumped;
        public Action PlayerAttacked;

        public bool publicGroundedPlayer { get { return groundedPlayer && playerVelocity.y == 0; } }

        private void Start()
        {
            cameraHandler = CameraHandler.singleton;
            attacker = GetComponent<PlayerAttacker>();
            characterController = GetComponent<CharacterController>();

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            groundedPlayer = true;
        }

        private void Update()
        {
            float delta = Time.unscaledDeltaTime;
            // && playerVelocity.y < 0
            //Calculate Y Movement            

            characterController.Move(playerVelocity * Time.unscaledDeltaTime);

            //Calculate XZ Movement
            if (movementInput.magnitude > 0.1f)
            {
                //Vector3 moveX = Camera.main.transform.right * movementInput.x;
                //Vector3 moveZ = Camera.main.transform.forward * movementInput.y;

                //Vector3 move = moveZ + moveX;

                //characterController.Move(move * Time.unscaledDeltaTime * playerSpeed);

                ////if (move.magnitude > 0)
                ////{

                //move.y = 0;
                //RotateCharacter(move);
                ////gameObject.transform.forward = move;
                ////}

                // Con este método solo toma angles, no afecta al grounded
                float targetAngle = Mathf.Atan2(movementInput.x, movementInput.y) * Mathf.Rad2Deg + Camera.main.transform.eulerAngles.y;
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
                transform.rotation = Quaternion.Euler(0f, angle, 0f);

                Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                characterController.Move(moveDir.normalized * playerSpeed * Time.unscaledDeltaTime);
            }

            //Calculate Camera Movement
            if (cameraHandler != null)
            {
                cameraHandler.FollowTarget(delta);
                cameraHandler.HandleCameraRotation(delta, mouseX, mouseY);
            }

            groundedPlayer = Physics.CheckSphere(groundcheck.position, groundRadius, (int)groundLayer);

            if (groundedPlayer)
            {
                playerVelocity.y = 0f;
            }
            else
            {
                playerVelocity.y += gravityValue * Time.unscaledDeltaTime * weight;
            }


            lastPos = transform.position;            
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
            PlayerMoved?.Invoke(movementInput.magnitude > 0);
        }

        public void OnJumpInput(InputAction.CallbackContext context)
        {
            if (!context.started)
            {
                Debug.Log("Jump Key Not Detected");
                return;
            }

            if (!groundedPlayer)
            {
                Debug.Log("Is Not Grounded");
                return;
            }

            if (playerVelocity.y == 0)
            {
                Debug.Log("Jumped");
                playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
                characterController.Move(playerVelocity * Time.unscaledDeltaTime);
                groundedPlayer = false;
                PlayerJumped?.Invoke();
            }
        }

        public void OnAttackInput(InputAction.CallbackContext context)
        {
            if (!context.started) return;
            
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
                    CameraLocked?.Invoke(true);
                }
            }
            else if (lockOnFlag)
            {
                lockOnInput = false;
                lockOnFlag = false;
                cameraHandler.ClearLockOnTargets();
                CameraLocked?.Invoke(false);
            }
        }

        private void HandleAttackInput()
        {
            attacker.HandleLightAttack();
            PlayerAttacked?.Invoke();
            Debug.Log("Ataque");
        }

        bool PlayerIsFlying()
        {
            RaycastHit raycastHit;
            bool playerOnFloor = Physics.Raycast(transform.position, -Vector3.up, out raycastHit, Mathf.Infinity);

            if(raycastHit.point.y + transform.GetComponent<CharacterController>().bounds.size.y < transform.position.y)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void SetGrounded(bool gr)
        {
            groundedPlayer = gr;
        }

        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            if (groundedPlayer == false)
            {
                if (hit.transform.tag == "Ground")
                {
                    groundedPlayer = true;
                }
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            if (collision.transform.tag == "Ground")
            {
                groundedPlayer = false;
            }
        }
    }
}