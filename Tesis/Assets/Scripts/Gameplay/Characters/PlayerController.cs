using System;
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
        [SerializeField] Cameras.CameraManager cameraManager;
        [SerializeField] CapsuleCollider coll;

        #endregion

        #region Movement Values

        [SerializeField] LayerMask steppableLayers;
        [SerializeField] float groundCheckSizeMod;
        Vector2 moveInput;
        private float deltaTime;
        private float horizontal;
        private float vertical;
        private float moveAmount;
        private float mouseX;
        private float mouseY;
        private bool jumpInput;

        [SerializeField] GameObject stepRayUpper;
        [SerializeField] GameObject stepRayLower;
        [SerializeField] float stepHeight = 0.3f;
        [SerializeField] float stepSmooth = 2f;
        
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
        public Action Dashed;
        public Action Died;
        public Action Hitted;

        #endregion

        #region Handler Actions and Stuff

        [SerializeField] float jumpHeight = 1.0f;
        [SerializeField] bool usingSlowmo;
        [SerializeField] float fallParalysisTime;
        public float paralysisTimer { set; private get; }

        [SerializeField] float attackDuration;
        [SerializeField] float attackStartUp;
        private bool attacking = false;

        #endregion

        #region Stats and UI Refs

        [SerializeField] CharacterData data;

        public CharacterData publicData
        {
            get { return data; }
        }

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

            if (!coll)
            {
                coll = GetComponent<CapsuleCollider>();
            }
            InitRigidSystem();
            
            stepRayUpper.transform.position = new Vector3(stepRayUpper.transform.position.x, stepHeight, stepRayUpper.transform.position.z);
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
            bool wasOnGround = grounded;
            grounded = IsOnGround();
            if (!wasOnGround && grounded)
            {
                CollideWithGround();
            }
            
            // if (cameraHandler != null)
            // {
            //     cameraHandler.FollowTarget(deltaTime);
            // }

            if (paralysisTimer > 0)
            {
                paralysisTimer -= Time.deltaTime;
                if (projectedVelocity.sqrMagnitude > 0)
                    StopRigidMovement();
                return;
            }

            if (lockOnFlag || ShouldMove() || !grounded)
            {
                ProjectVelocity();
                SetNewRotation(true);
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
        
        private void FixedUpdate()
        {
            StepClimb();
        }

#if UNITY_EDITOR
        [ExecuteInEditMode]
        void OnDrawGizmos()
        {
            //Draw Ground Check Capsule Collider
            Vector3 bottom = coll.bounds.center + coll.bounds.extents.y * Vector3.down * 1.01f;
            float radius = coll.bounds.extents.x * groundCheckSizeMod;
            Gizmos.color = grounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(bottom, radius);
        }   
#endif

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
        private void SetNewRotation(bool rotateStill)
        {
            Vector3 targetDir = Vector3.zero;

            Vector3 refDir = forwardRefObject.right;
            refDir.y = 0;
            targetDir = refDir * (rotateStill ? 1 : moveInput.x);

            refDir = forwardRefObject.forward;
            refDir.y = 0;
            targetDir += refDir * (rotateStill ? 1 : moveInput.y);

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
            if (usingSlowmo) return;
            float frameRot = rotationSpeed * delta;

            if (!grounded)
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

            if (!grounded)
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
        /// Stops the rigidbody movement and clears input
        /// </summary>
        private void ClearMovement()
        {
            StopRigidMovement();
            moveInput = Vector2.zero;
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

        /// <summary>  
        /// Returns if player is colliding against the floor
        /// </summary>
        bool IsOnGround()
        {
            //Set bool
            bool isOnGround = false;

            //Get size and pos
            Vector3 top = coll.bounds.center + Vector3.up * (coll.bounds.extents.y * 0.99f);
            Vector3 bottom = coll.bounds.center + Vector3.down * (coll.bounds.extents.y * 1.01f);
            //a capsule is the same in x & z, only y is different
            float radius = coll.bounds.extents.x * groundCheckSizeMod; 
            
            //Check for collisions
            Collider[] hits;
            hits = Physics.OverlapCapsule(top, bottom, radius, steppableLayers);

            //Update bool
            isOnGround = hits.Length > 0;
            
            return isOnGround;
        }

        void CollideWithGround()
        {
            dashAirCompleted = false;
            // if (ShouldStop())
            // {
            //     StopRigidMovement();
            // }

            paralysisTimer = fallParalysisTime;
        }

        void OnAir()
        {
            StopRigidMovement(); //good
            projectedAirVelocity = Vector3.zero; //MESSY
            moveInput = Vector2.zero; //MESSY
        }

        void OnCollisionEnter(Collision collision)
        {
            if (grounded) return;
            if(collision.gameObject.CompareTag("Ground")) return;
            OnAir();
        }

        void StepClimb()
        {
            RaycastHit hitLower;
            if (Physics.Raycast(stepRayLower.transform.position, transform.TransformDirection(Vector3.forward),
                    out hitLower, 0.1f))
            {
                RaycastHit hitUpper;
                if (!Physics.Raycast(stepRayUpper.transform.position, transform.TransformDirection(Vector3.forward),
                        out hitUpper, 0.2f))
                {
                    transform.position -= new Vector3(0f, -stepSmooth * Time.deltaTime, 0f);
                }
            }

            RaycastHit hitLower45;
            if (Physics.Raycast(stepRayLower.transform.position, transform.TransformDirection(1.5f, 0, 1),
                    out hitLower45, 0.1f))
            {
                RaycastHit hitUpper45;
                if (!Physics.Raycast(stepRayUpper.transform.position, transform.TransformDirection(1.5f, 0, 1),
                        out hitUpper45, 0.2f))
                {
                    transform.position -= new Vector3(0f, -stepSmooth * Time.deltaTime, 0f);
                }
            }

            RaycastHit hitLowerMinus45;
            if (Physics.Raycast(stepRayLower.transform.position, transform.TransformDirection(-1.5f, 0, 1),
                    out hitLowerMinus45, 0.1f))
            {
                RaycastHit hitUpperMinus45;
                if (!Physics.Raycast(stepRayUpper.transform.position, transform.TransformDirection(-1.5f, 0, 1),
                        out hitUpperMinus45, 0.2f))
                {
                    transform.position -= new Vector3(0f, -stepSmooth * Time.deltaTime, 0f);
                }
            }
        }

        #endregion

        #region On Inputs

        public void OnRotateInput(InputAction.CallbackContext context)
        {
            if (usingSlowmo) return;

            //Calculate velocity and rotation after camera moved;
            ProjectVelocity();
            SetNewRotation(false);
        }

        public void OnMovementInput(InputAction.CallbackContext context)
        {
            if (usingSlowmo) return;
            if (context.canceled)
            {
                if (grounded)
                    StopRigidMovement();
                moveInput = Vector2.zero;
                return;
            }

            moveInput = context.ReadValue<Vector2>();

            ProjectVelocity();
            UpdateRigidVelocity();

            SetNewRotation(false);
        }

        public void OnJumpInput(InputAction.CallbackContext context)
        {
            if (usingSlowmo) return;
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
            if (attacking || dashing || !grounded) return;
            if (context.canceled)
            {
                //SetNewRotation(true);
                //transform.localRotation = targetRotation;
                TimePhys.TimeChanger.Get().Release();
                usingSlowmo = false;
                cameraManager.OnTimeCharge(context);
            }
            else if (context.started)
            {
                //SetNewRotation(true);
                //transform.localRotation = targetRotation;
                ClearMovement();
                TimePhys.TimeChanger.Get().Activate();
                usingSlowmo = true;
                cameraManager.OnTimeCharge(context);
            }
        }

        #endregion

        #region Attacking

        private void HandleAttackInput()
        {
            if (attacking) return; //If player is already attacking, exit

            if (usingSlowmo) return; //If player is using time changer, exit

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

        #region Dash Inputs

        public void OnDashInput(InputAction.CallbackContext context)
        {
            if (attacking || usingSlowmo) return;
            if (dashCurrent || dashAirCompleted) return;
            StartCoroutine(Dash());
        }

        private IEnumerator Dash()
        {
            dashTime = dashCooldown;
            dashing = true;
            Dashed?.Invoke();
            dashCurrent = true;
            if (!grounded)
                dashAirCompleted = true;
            //VelocityChange ignores mass, contrary to ForceMode.Impulse
            if (moveInput.sqrMagnitude > 0)
            {
                rigidbody.AddForce(HandleDashInput() * dashForce, ForceMode.VelocityChange);
            }
            else
            {
                rigidbody.AddForce(transform.forward * dashForce,
                    ForceMode.VelocityChange); //TOUCH THIS AND YOU WILL PERISH UNDER THE WEIGH OF YOUR ARROGANCE
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
            Vector3 camDir = forwardRefObject.forward;
            camDir.y = 0;

            moveDirection = camDir * moveInput.y;

            camDir = forwardRefObject.right;
            camDir.y = 0;

            moveDirection += camDir * moveInput.x;
            moveDirection.Normalize();

            return Vector3.ProjectOnPlane(moveDirection, normalVector);
        }

        #endregion
    }
}