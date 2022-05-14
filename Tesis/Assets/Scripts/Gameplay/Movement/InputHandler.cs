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
        private bool attackInput;

        public bool lockOnFlag;

        PlayerControls inputActions;
        CameraHandler cameraHandler;
        PlayerAttacker attacker;

        Vector2 movementInput;
        Vector2 cameraInput;
        

        private void Start()
        {
            cameraHandler = CameraHandler.singleton;
            attacker = GetComponent<PlayerAttacker>();

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void FixedUpdate()
        {
            float delta = Time.fixedDeltaTime;

            if (cameraHandler != null)
            {
                cameraHandler.FollowTarget(delta);
                cameraHandler.HandleCameraRotation(delta, mouseX, mouseY);
            }
        }

        private void LateUpdate()
        {
            if (jumpInput == true)
            {
                Debug.Log("Jump!");
            }
            jumpInput = false;
            attackInput = false;
        }

        public void OnEnable()
        {
            if (inputActions == null)
            {
                inputActions = new PlayerControls();
                inputActions.PlayerMovement.Movement.performed += inputActions => movementInput = inputActions.ReadValue<Vector2>();
                inputActions.PlayerMovement.Camera.performed += i => cameraInput = i.ReadValue<Vector2>();
                inputActions.PlayerActions.Jump.performed += i => jumpInput = true;
                inputActions.PlayerActions.LockOn.performed += i => lockOnInput = true;
            }

            inputActions.Enable();
        }

        private void OnDisable()
        {
            inputActions.Disable();
        }

        public void TickInput(float delta)
        {
            MoveInput(delta);

            HandleLockOnInput();
            HandleJumpInput();
            HandleAttackInput(delta);
        }

        private void MoveInput(float delta)
        {
            horizontal = movementInput.x;
            vertical = movementInput.y;
            moveAmount = Mathf.Clamp01(Mathf.Abs(horizontal) + Mathf.Abs(vertical));
            mouseX = cameraInput.x;
            mouseY = cameraInput.y;
        }

        private void HandleJumpInput()
        {
            inputActions.PlayerActions.Jump.performed += i => jumpInput = true;
        }

        private void HandleLockOnInput()
        {
            if (lockOnInput && !lockOnFlag)
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
            else if(lockOnInput && lockOnFlag)
            {
                lockOnInput = false;
                lockOnFlag = false;
                cameraHandler.ClearLockOnTargets();
            }
                
            
        }

        private void HandleAttackInput(float delta)
        {
            inputActions.PlayerActions.Attack.performed += i => attackInput = true;

            if (attackInput)
            {
                attacker.HandleLightAttack();
                Debug.Log("Ataque");
            }
        }
    }
}