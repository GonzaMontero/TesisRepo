using UnityEngine;

namespace Handler
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
        Vector2 cameraInput;

        private Vector3 playerVelocity;
        private bool groundedPlayer;
        private float playerSpeed = 15;
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

        private void FixedUpdate()
        {
            float delta = Time.unscaledDeltaTime;

            TickInput(delta);

            groundedPlayer = characterController.isGrounded;
            if (groundedPlayer && playerVelocity.y < 0)
            {
                playerVelocity.y = 0f;
            }

            Vector3 move;

            Vector3 moveY = Camera.main.transform.forward * Input.GetAxis("Vertical");
            Vector3 moveX = Camera.main.transform.right * Input.GetAxis("Horizontal");

            move = moveY + moveX;

            characterController.Move(move * Time.unscaledDeltaTime * playerSpeed);

            if (move != Vector3.zero)
            {
                gameObject.transform.forward = move;
            }

            if (Input.GetButtonDown("Jump") && groundedPlayer)
            {
                playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
            }

            playerVelocity.y += gravityValue * Time.unscaledDeltaTime;
            characterController.Move(playerVelocity * Time.unscaledDeltaTime);            

            if (cameraHandler != null)
            {
                mouseX = Input.GetAxisRaw("Mouse X");
                mouseY = Input.GetAxisRaw("Mouse Y");

                cameraHandler.FollowTarget(delta);
                cameraHandler.HandleCameraRotation(delta, mouseX, mouseY);
            }
        }

        public void TickInput(float delta)
        {
            HandleLockOnInput();
            HandleAttackInput(delta);
        }

        //private void MoveInput(float delta)
        //{
        //    horizontal = movementInput.x;
        //    vertical = movementInput.y;
        //    moveAmount = Mathf.Clamp01(Mathf.Abs(horizontal) + Mathf.Abs(vertical));
        //    mouseX = cameraInput.x;
        //    mouseY = cameraInput.y;
        //}

        private void HandleJumpInput()
        {
            inputActions.PlayerActions.Jump.performed += i => jumpInput = true;
        }

        private void HandleLockOnInput()
        {
            if (Input.GetKey(KeyCode.F))
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
        }

        private void HandleAttackInput(float delta)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                attacker.HandleLightAttack();
                Debug.Log("Ataque");
            }
        }
    }
}