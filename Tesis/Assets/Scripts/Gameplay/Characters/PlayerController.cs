using System;
using UnityEngine;
using UnityEngine.InputSystem;
using TimeDistortion.Gameplay.Characters;

namespace TimeDistortion.Gameplay.Handler
{
    public class PlayerController : MonoBehaviour
    {
        #region Components and Controls
        [SerializeField] GameObject attackHitBox;
        #endregion

        #region Movement Values
        Vector2 moveInput;
        private float deltaTime;
        private float horizontal;
        private float vertical;
        private float moveAmount;
        private float mouseX;
        private float mouseY;

        private bool jumpInput;

        public bool lockOnFlag { set; private get; }
        #endregion

        #region Rigid System Movement Values
        private new Rigidbody rigidbody;
        [SerializeField] Transform forwardRefObject;
        Vector3 moveDirection;
        Vector3 normalVector;
        Vector3 projectedVelocity;
        Vector3 projectedAirVelocity;
        public bool grounded = true;

        [SerializeField] float onAirSpeedMod = .5f;
        [SerializeField] float onAirRotMod = .5f;
        [SerializeField] float movementSpeed = 5;
        [SerializeField] float rotationSpeed = 10;
        Quaternion targetRotation;
        #endregion

        #region Gameplay Actions
        public Action<bool> Moved;
        public Action Jumped;
        public Action Attacked;
        public Action Died;
        public Action Hitted;
        #endregion

        #region Handler Actions and Stuff
        [SerializeField] float jumpHeight = 1.0f;
        bool usingSlowmo;
        [SerializeField] float fallParalysisTime;
        public float paralysisTimer { set; private get; }
        
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
            if (forwardRefObject == null)
            {
                forwardRefObject = Camera.main.transform;
            }

            grounded = true;
        }

        private void Update()
        {
            deltaTime = Time.deltaTime;

            // if (cameraHandler != null)
            // {
            //     cameraHandler.FollowTarget(deltaTime);
            // }

            if(paralysisTimer > 0)
            {
                paralysisTimer -= Time.deltaTime;
                if (projectedVelocity.sqrMagnitude > 0)
                    StopRigidMovement();
                return;
            }

            if (lockOnFlag || ShouldMove() || !grounded)
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
            Vector3 camDir = forwardRefObject.forward;
            camDir.y = 0;

            moveDirection = camDir * moveInput.y;

            camDir = forwardRefObject.right;
            camDir.y = 0;

            moveDirection += camDir * moveInput.x;
            moveDirection.Normalize();

            float speed = movementSpeed;
            moveDirection *= speed;

            return Vector3.ProjectOnPlane(moveDirection, normalVector);            
        }

        /// <summary> 
        /// Sets a new target rotation, taking in account ref obj forward
        /// </summary>
        private void SetNewRotation()
        {
            Vector3 targetDir = Vector3.zero;

            Vector3 refDir = forwardRefObject.right;
            refDir.y = 0;
            targetDir = refDir * (lockOnFlag ? 1 : moveInput.x);

            refDir = forwardRefObject.forward;
            refDir.y = 0;
            targetDir += refDir * (lockOnFlag ? 1 : moveInput.y);

            targetDir.Normalize();
            targetDir.y = 0;

            if (targetDir == Vector3.zero)
            {
                targetDir = transform.forward;
            }

            targetRotation = Quaternion.LookRotation(targetDir);
        }

        /// <summary> 
        /// Rotates character with lerp until it reaches Target Rotation
        /// </summary>
        private void HandleRotation(float delta)
        {
            if (transform.rotation == targetRotation) return;

            float frameRot = rotationSpeed * delta;
            
            if(!grounded)
            {
                frameRot *= onAirRotMod;
            }

            Quaternion rotation = Quaternion.Slerp(transform.rotation, targetRotation, frameRot);

            transform.rotation = rotation;
        }

        private void HandleJumping()
        {
            if (paralysisTimer > 0)
                return;
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
                Jumped?.Invoke();
            }
        }

        /// <summary>  
        /// Calculates projected velocity with last input 
        /// </summary>
        private void ProjectVelocity()
        {
            TickInput(deltaTime);

            Vector3 movement = HandleMovement();

            if (grounded)
            {
                projectedVelocity = new Vector3(movement.x, 0, movement.z);
            }
            else
            {
                projectedAirVelocity = new Vector3(movement.x, 0, movement.z);
                projectedAirVelocity *= onAirSpeedMod;
            }
        }

        /// <summary>  
        /// Updates velocity with projected velocity
        /// </summary>
        private void UpdateRigidVelocity()
        {
            //Check if player is paralyzed OR if it is still while ond the ground
            if ((projectedVelocity.sqrMagnitude < 1 && grounded) || paralysisTimer > 0)
            {
                Moved?.Invoke(false);
                return;
            }

            projectedVelocity.y = rigidbody.velocity.y;
            rigidbody.velocity = projectedVelocity;
            
            if(!grounded)
            {
                rigidbody.velocity += projectedAirVelocity;
            }
            
            Moved?.Invoke(true);
        }

        /// <summary>  
        /// Stops the rigidbody movement (clearing it's velocity) 
        /// and clears projected velocity
        /// </summary>
        private void StopRigidMovement()
        {
            //Set projected velocity to 0
            projectedVelocity = Vector3.zero;

            //Update rigid velocity (without modifying Y)
            projectedVelocity.y = rigidbody.velocity.y;
            rigidbody.velocity = projectedVelocity;
            
            //Clear again projected V, so it stays in 0
            projectedVelocity = Vector3.zero;
         
            //Invoke action
            Moved?.Invoke(false);
        }

        /// <summary>  
        /// Checks if move input is in use, but projected velocity is 0
        /// </summary>
        private bool ShouldMove()
        {
            return moveInput.sqrMagnitude > 0 && projectedVelocity.sqrMagnitude < 0.01f;
        }

        /// <summary>  
        /// Checks if projected velocity is in use, but move input is 0
        /// </summary>
        private bool ShouldStop()
        {
            return projectedVelocity.sqrMagnitude > 0 && moveInput.sqrMagnitude < 0.01f;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.transform.tag == "Ground")
            {
                if (grounded) return;

                grounded = true;
                if(ShouldStop())
                {
                    StopRigidMovement();
                }

                paralysisTimer = fallParalysisTime;
            }
        }
        private void OnCollisionStay(Collision collision) //CLEAN LATER
        {
            if (collision.gameObject.transform.tag == "Ground") return;
            if (grounded) return;

            StopRigidMovement();//good
            projectedAirVelocity = Vector3.zero; //MESSY
            moveInput = Vector2.zero; //MESSY
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
            if (context.canceled)
            {
                if(grounded)
                    StopRigidMovement();
                moveInput = Vector2.zero;
                return;
            }

            moveInput = context.ReadValue<Vector2>();

            ProjectVelocity();
            UpdateRigidVelocity();

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
        public void OnSlowMoInput(InputAction.CallbackContext context)
        {
            if (paralysisTimer > 0) return;
            if (context.canceled)
            {
                TimePhys.TimeChanger.Get().Release();
                usingSlowmo = false;
            }
            else if (context.started)
            {
                TimePhys.TimeChanger.Get().Activate();
                usingSlowmo = true;
            }
        }
        #endregion

        #region Attacking

        private void HandleAttackInput()
        {
            if (attacking) return; //If player is already attacking, exit

            if (usingSlowmo) return; //If player is using time changer, exit

            if (!grounded) return; //If player is on air, exit

            if (attackHitBox.activeSelf) return; //If hit box is on (player is attacking), exit

            paralysisTimer = attackDuration + attackStartUp;
            attacking = true;
            Invoke("StartLightAttack", attackStartUp * Time.timeScale);
            Attacked?.Invoke();
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
    }
}