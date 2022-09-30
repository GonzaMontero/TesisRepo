using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TimeDistortion.Gameplay.Characters;
using System.Collections;

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
        Transform cameraObject;
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
        [SerializeField] float slowMoParalysisTime;
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

        #region Dash Variables
        public float dashForce;
        public float dashDuration;
        public float dashCooldown;
        private float dashTime;
        public bool dashing { get; private set; }
        private bool dashCurrent;
        [SerializeField] private bool dashAirCompleted = false;
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

            if (dashTime > 0)
                dashTime -= Time.deltaTime;
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
            if (dashing) return;

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
            if (dashing) return;

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
                dashAirCompleted = false;
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
        #endregion
        
        #region Attacking

        private void HandleAttackInput()
        {
            if (attacking) return; //If player is already attacking, exit

            if (!grounded || dashing) return; //If player is on air or dashing, exit

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

        #region Camera and Slow Motion
        public void OnSlowMo()
        {
            paralysisTimer = slowMoParalysisTime;
        }
        #endregion

        #region Dash Inputs
        public void OnDashInput(InputAction.CallbackContext context)
        {
            if (dashCurrent || dashAirCompleted) return;
            StartCoroutine(Dash());
        }

        private IEnumerator Dash()
        {
            dashTime = dashCooldown;
            dashing = true;
            dashCurrent = true;
            if (!grounded)
                dashAirCompleted = true;
            //VelocityChange ignores mass, contrary to ForceMode.Impulse
            if(moveInput.sqrMagnitude > 0)
            {
                rigidbody.AddForce(HandleDashInput() * dashForce, ForceMode.VelocityChange);
            }
            else
            {
                rigidbody.AddForce(transform.forward * dashForce, ForceMode.VelocityChange); //TOUCH THIS AND YOU WILL PERISH UNDER THE WEIGH OF YOUR ARROGANCE
            }            
            rigidbody.velocity = new Vector3(rigidbody.velocity.x, 0, rigidbody.velocity.z);
            projectedAirVelocity = Vector3.zero;
            projectedVelocity = Vector3.zero;
            rigidbody.useGravity = false;

            yield return new WaitForSeconds(dashDuration);

            rigidbody.velocity = Vector3.zero;
            rigidbody.useGravity = true;
            dashing = false;

            yield return new WaitForSeconds(dashCooldown);

            dashCurrent = false;
        }

        Vector3 HandleDashInput()
        {
            Vector3 camDir = cameraObject.forward;
            camDir.y = 0;

            moveDirection = camDir * moveInput.y;

            camDir = cameraObject.right;
            camDir.y = 0;

            moveDirection += camDir * moveInput.x;
            moveDirection.Normalize();

            return Vector3.ProjectOnPlane(moveDirection, normalVector);
        }
        #endregion
    }
}