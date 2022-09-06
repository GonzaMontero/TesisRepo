using System;
using UnityEngine;
using UnityEngine.InputSystem;
using TimeDistortion.Gameplay.Characters;

namespace TimeDistortion.Gameplay.Handler
{
    public class PlayerController : MonoBehaviour
    {
        #region Components and Controls
        //[SerializeField] CameraHandler cameraHandler;
        [SerializeField] GameObject attackHitBox;
        #endregion

        #region Movement and Camera Input
        Vector2 moveInput;
        //Vector2 cameraInput;
        #endregion

        #region Movement Values
        private float deltaTime;
        private float horizontal;
        private float vertical;
        private float moveAmount;
        private float mouseX;
        private float mouseY;

        private bool jumpInput;
        private bool lockOnInput;

        public bool lockOnFlag;
        #endregion

        #region Rigid System Movement Values
        Transform cameraObject;
        Vector3 moveDirection;
        Vector3 normalVector;
        Vector3 projectedVelocity;

        public bool grounded = true;

        private new Rigidbody rigidbody;
        //private GameObject normalCamera;

        [Header("Stats")]
        [SerializeField] float movementSpeed = 5;
        [SerializeField] float rotationSpeed = 10;
        Quaternion targetRotation;
        #endregion

        #region Gameplay Actions
        public Action Died;
        public Action Hitted;
        #endregion

        #region Handler Actions and Stuff
        //public Action<bool> CameraLocked;
        public Action<bool> PlayerMoved;
        public Action PlayerJumped;
        public Action PlayerAttacked;

        [SerializeField] float playerSpeed = 15;
        [SerializeField] float rotationSmoothing = 1;
        [SerializeField] float weight;
        [SerializeField] float jumpHeight = 1.0f;
        [SerializeField] float slowMoParalysisTime;
        public float paralysisTimer;

        
        [SerializeField] float attackDuration;
        [SerializeField] float attackStartUp;
        private bool attacking = false;
        #endregion

        #region Stats and UI Refs
        [SerializeField] CharacterData data;

        public CharacterData publicData { get { return data; } }

        //Unity Events
        private void Awake()
        {
            data.currentStats = data.baseStats;
        }

        //Methods

        //Interface Implementations
        public void GetHitted(int damage)
        {
            data.currentStats.health -= damage;
            Hitted?.Invoke();

            if (data.currentStats.health > 0) return;
            data.currentStats.health = 0;
            //Died?.Invoke();
        }
        #endregion

        private void Start()
        {
            //cameraHandler = CameraHandler.singleton;

            InitRigidSystem();
        }

        private void InitRigidSystem()
        {
            rigidbody = GetComponent<Rigidbody>();
            cameraObject = Camera.main.transform;

            grounded = true;
        }

        private void Update()
        {
            deltaTime = Time.deltaTime;

            // if (cameraHandler != null)
            // {
            //     cameraHandler.FollowTarget(deltaTime);
            // }

            if (lockOnFlag)
            {
                ProjectVelocity();
                SetNewRotation();
            }

            UpdateRigidVelocity();
            HandleRotation(deltaTime);
        }

        private void LateUpdate()
        {
            jumpInput = false;
        }

        #region Movement Calculations

        public void TickInput(float delta)
        {
            MoveInput(delta);
        }

        private void MoveInput(float delta)
        {
            horizontal = moveInput.x;
            vertical = moveInput.y;
            moveAmount = Mathf.Clamp01(Mathf.Abs(horizontal) + Mathf.Abs(vertical));
            //mouseX = cameraInput.x;
            //mouseY = cameraInput.y;
        }

        private Vector3 HandleMovement()
        {
            Vector3 camDir = cameraObject.forward;
            camDir.y = 0;

            moveDirection = camDir * moveInput.y;

            camDir = cameraObject.right;
            camDir.y = 0;

            moveDirection += camDir * moveInput.x;
            moveDirection.Normalize();

            float speed = movementSpeed;
            moveDirection *= speed;

            return Vector3.ProjectOnPlane(moveDirection, normalVector);            
        }

        /// <summary> 
        /// Sets a new target rotation, taking in account camera forward
        /// </summary>
        private void SetNewRotation()
        {
            Vector3 targetDir = Vector3.zero;
            float moveOverride = moveAmount;

            Vector3 camDir = cameraObject.right;
            camDir.y = 0;
            targetDir = camDir * moveInput.x;

            camDir = cameraObject.forward;
            camDir.y = 0;
            targetDir += camDir * moveInput.y;

            targetDir.Normalize();
            targetDir.y = 0;

            if (targetDir == Vector3.zero)
            {
                targetDir = transform.forward;
            }

            float rs = rotationSpeed;

            targetRotation = Quaternion.LookRotation(targetDir);
        }

        /// <summary> 
        /// Rotates character with lerp until it reaches Target Rotation
        /// </summary>
        private void HandleRotation(float delta)
        {
            if (transform.rotation == targetRotation)
                return;

            Quaternion rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * delta);

            transform.rotation = rotation;
        }

        private void HandleJumping()
        {
            if (grounded)
            {
                //if (inputHandler.moveAmount > 0)
                //{
                //    moveDirection = cameraObject.forward * inputHandler.vertical;
                //    moveDirection += cameraObject.right * inputHandler.horizontal;
                //    moveDirection.y = 0;
                //    Quaternion jumpRotation = Quaternion.LookRotation(moveDirection);
                //    myTransform.rotation = jumpRotation;
                //}
                rigidbody.AddForce(new Vector3(0, jumpHeight, 0), ForceMode.Impulse);
                grounded = false;
                PlayerJumped?.Invoke();
            }
        }

        /// <summary>  
        /// Calculates projected velocity with last input 
        /// </summary>
        private void ProjectVelocity()
        {
            projectedVelocity = Vector3.zero;

            TickInput(deltaTime);

            Vector3 movement = HandleMovement();

            if (movement.y == 0)
            {
                projectedVelocity = new Vector3(movement.x, 0, movement.z);
            }
        }

        /// <summary>  
        /// Updates velocity with projected velocity
        /// </summary>
        private void UpdateRigidVelocity()
        {
            if (projectedVelocity.sqrMagnitude < 1)
            {
                PlayerMoved?.Invoke(false);
                return;
            }
            projectedVelocity.y = rigidbody.velocity.y;
            rigidbody.velocity = projectedVelocity;
            
            PlayerMoved?.Invoke(true);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.transform.tag == "Ground")
            {
                grounded = true;
            }
        }
        #endregion

        #region On Inputs

        public void OnRotateInput(InputAction.CallbackContext context)
        {
            //Calculate velocity and rotation after camera moved;
            ProjectVelocity();
            SetNewRotation();
        }

        public void OnMovementInput(InputAction.CallbackContext context)
        {
            moveInput = context.ReadValue<Vector2>();

            ProjectVelocity();
            rigidbody.velocity = projectedVelocity;

            SetNewRotation();
        }

        public void OnJumpInput(InputAction.CallbackContext context)
        {
            if (!context.started)
                return;
            HandleJumping();

            //if (!context.started)
            //{
            //    Debug.Log("Jump Key Not Detected");
            //    return;
            //}
            //if (!groundedPlayer)
            //{
            //    Debug.Log("Is Not Grounded");
            //    return;
            //}
            //if (playerVelocity.y == 0)
            //{
            //    Debug.Log("Jumped");
            //    if (attacking)
            //        StopLightAttack();
            //    playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
            //    characterController.Move(playerVelocity * deltaTime);
            //    groundedPlayer = false;
            //    PlayerJumped?.Invoke();
            //}
        }

        public void OnAttackInput(InputAction.CallbackContext context)
        {
            if (!context.started) return;
            HandleAttackInput();
        }
        #endregion
        
        #region Attacking

        private void HandleAttackInput()
        {
            if (attacking) return; //If player is already attacking, exit

            if (!grounded) return; //If player is on air, exit

            if (attackHitBox.activeSelf) return; //If hit box is on (player is attacking), exit

            paralysisTimer = attackDuration + attackStartUp;
            attacking = true;
            Invoke("StartLightAttack", attackStartUp * Time.timeScale);
            PlayerAttacked?.Invoke();
        }

        public void StartLightAttack()
        {
            attackHitBox.SetActive(true);
            Invoke("StopLightAttack", attackDuration * Time.timeScale);
        }

        public void StopLightAttack()
        {
            if (attackHitBox.activeSelf == true)
            {
                attackHitBox.SetActive(false);
                attacking = false;
            }
        }
        #endregion

        #region Camera and Slow Motion
        void OnSlowMo(Transform _notUsed, float __notUsed)
        {
            paralysisTimer = slowMoParalysisTime;
        }
        #endregion
    }
}


