using System;
using UnityEngine;
using UnityEngine.InputSystem;
using TimeDistortion.Gameplay.Characters;
using System.Collections;
using TimeDistortion.Gameplay.Props;

namespace TimeDistortion.Gameplay.Handler
{
    public class PlayerController : MonoBehaviour, IHittable
    {
        #region Components and Controls

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

        // [SerializeField] GameObject stepRayUpper;
        // [SerializeField] GameObject stepRayLower;
        // [SerializeField] float stepHeight = 0.3f;
        // [SerializeField] float stepSmooth = 2f;

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
        [SerializeField] float maxMoveSpeed = 20;
        Quaternion targetRotation;

        #endregion

        #region Gameplay Actions

        public Action<bool> Moved;
        public Action Jumped;
        public Action Attacked;
        public Action Dashed;
        public Action Died;
        public Action<bool> Healing;
        public Action<int> LifeChanged;

        #endregion

        #region Handler Actions and Stuff

        [Header("Interact")]
        [SerializeField] float interactCheckOffset;
        [SerializeField] Vector3 interactCheckSize = Vector3.one;
        [SerializeField] LayerMask interactableLayers;
        [SerializeField] InteractableController interactable;
        [SerializeField] [Tooltip("THIS IS RUNTIME")] bool canInteract;
        
        [Header("Jump & Fall")]
        [SerializeField] float jumpHeight = 1.0f;
        [SerializeField] bool jumping;
        [SerializeField] float coyoteDuration;
        [SerializeField] float coyoteTimer;
        [SerializeField] float fallParalysisTime;
        
        [Header("Slow mo")]
        [SerializeField] bool usingSlowmo;
        
        [Header("Spawn")]
        [SerializeField] float spawnParalysisTime;
        [SerializeField] bool spawning = true;
        
        [Header("Health")]
        [Tooltip("How much damage is needed for heavy threshold")] 
        [SerializeField] int minHeavyDamage;
        [SerializeField] float hittedLightParalysisTime;
        [SerializeField] float hittedHeavyParalysisTime;
        [SerializeField] float hittedInvulnerabilityTime;
        [SerializeField] float invulnerabilityTimer;
        [SerializeField] int regenMod;
        [SerializeField] float regenDelay;
        [SerializeField] float regenDuration;
        [SerializeField] float regenMoveMod;
        [SerializeField] float regenTimer = -1;
        [SerializeField] bool regenerating;
        [SerializeField] int baseRegens;
        [SerializeField] int currentRegens;

        [Header("Attack")]
        [SerializeField] AttackController attack;
        
        #region Dash Variables
        [Header("Dash")]
        public float dashForce;
        public float dashDuration;
        public float dashCooldown;
        private float dashTime;
        public bool dashing { get; private set; }
        private bool dashCurrent;
        [SerializeField] private bool dashAirCompleted = false;

        #endregion
        
        public InteractableController publicInteractable => interactable;

        public int minHeavyDmg => minHeavyDamage;
        
        public int regenerators => currentRegens;

        public bool publicCanInteract => canInteract;

        public bool isSpawning => spawning;

        public bool isRegenerating => regenerating;

        public float paralysisTimer;

        #endregion

        [SerializeField] CharacterData data;

        public CharacterData publicData
        {
            get { return data; }
        }

        //Unity Events
        private void Awake()
        {
            data.Set();
        }
        
        private void Start()
        {
            //cameraHandler = CameraHandler.singleton;

            if (!coll)
            {
                coll = GetComponent<CapsuleCollider>();
            }

            InitRigidSystem();

            //stepRayUpper.transform.position = new Vector3(stepRayUpper.transform.position.x, stepHeight, stepRayUpper.transform.position.z);
            paralysisTimer += spawnParalysisTime;
        }

        private void Update()
        {
            deltaTime = Time.deltaTime;
            
            bool wasOnGround = grounded;
            bool isOnGround = IsOnGround();
            //grounded = true;

            if (isOnGround)
            {
                coyoteTimer = -1;
                
                if (!wasOnGround)
                {
                    CollideWithGround();
                    //Debug.Log("Collide!");
                }
            }
            else
            {
                if (wasOnGround)
                {
                    if (jumping)
                    {
                        grounded = false;
                    }
                    //If player was on ground and now it isn't, stay "grounded" for a fixed duration
                    else if (coyoteTimer <= -1)
                    {
                        coyoteTimer = coyoteDuration;
                        //Debug.Log("Coyote On!");
                    }
                }

                //If using slowmo while on air, stop using slow mo
                if (usingSlowmo)
                {
                    TimePhys.TimeChanger.Get().Release();
                    usingSlowmo = false;

                    //SHOULD DO SOMETHING WITH THIS TOO
                    //cameraManager.OnTimeCharge(InputAction.CallbackContext);
                }
            }

            //Check for interactables
            CanInteract();

            UpdateTimers();

            if (lockOnFlag || ShouldMove() || !grounded)
            {
                ProjectVelocity();
                SetNewRotation(true);
            }
            else if (usingSlowmo)
            {
                SetNewRotation(true);
                // Debug.Log("Rotating while slow mo");
                // Debug.Log("\n Target Rot: " + targetRotation.eulerAngles + 
                //                     "\n Current Rot: " + transform.rotation.eulerAngles);
            }

            UpdateRigidVelocity();
            HandleRotation(Time.unscaledDeltaTime);
        }

        private void LateUpdate()
        {
            jumpInput = false;
        }

        // private void FixedUpdate()
        // {
        //     StepClimb();
        // }

#if UNITY_EDITOR
        [ExecuteInEditMode]
        void OnDrawGizmos()
        {
            //Draw Ground Check Capsule Collider
            Vector3 bottom = coll.bounds.center + coll.bounds.extents.y * Vector3.down * 1.01f;
            float radius = coll.bounds.extents.x * groundCheckSizeMod;
            Gizmos.color = grounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(bottom, radius);

            //Draw Interactable Check Box Collider
            Color color = canInteract ? Color.green : Color.red;
            color.a = 0.5f;
            Gizmos.color = color;
            Gizmos.DrawCube(transform.position + interactCheckOffset * transform.forward, interactCheckSize);
        }
#endif

        //Methods
        void UpdateTimers()
        {
            if (coyoteTimer > 0)
            {
                coyoteTimer -= deltaTime;
                grounded = true;
            }
            else if(coyoteTimer > -1)
            {
                coyoteTimer = -1;
                //Debug.Log("Coyote Off!");
                grounded = false;
            }
            if (dashTime > 0)
            {
                dashTime -= deltaTime;
            }
            if (invulnerabilityTimer > 0)
            {
                invulnerabilityTimer -= deltaTime;
            }
            if (regenTimer > 0)
            {
                regenTimer -= deltaTime;
            }
            else if (regenTimer > -1)
            {
                UpdateRegeneration();
            }
            if (paralysisTimer > 0)
            {
                paralysisTimer -= deltaTime;
                if (projectedVelocity.sqrMagnitude > 0)
                    StopRigidMovement();
            }
            else if (spawning)
            {
                spawning = false;
            }
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

            Vector3 refDir;

            refDir = forwardRefObject.right;
            //refDir = (rotateStill) ? transform.right : forwardRefObject.right;
            refDir.y = 0;
            targetDir = refDir * (rotateStill ? 0 : moveInput.x);

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
            //if (usingSlowmo) return;
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
                jumping = true;
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
            if (moveInput.sqrMagnitude < 0.1) return;

            //Check if player is paralyzed OR if it is still while ond the ground
            if ((projectedVelocity.sqrMagnitude < 1 && grounded) || paralysisTimer > 0)
            {
                Moved?.Invoke(false);
                return;
            }

            projectedVelocity.y = rigidbody.velocity.y;

            Vector3 localProjVel = projectedVelocity;

            //If regenerating, multiply x / z velocity by regen Mod
            if (regenTimer > 0)
            {
                localProjVel.x *= regenMoveMod;
                localProjVel.z *= regenMoveMod;
            }

            rigidbody.velocity = localProjVel;

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

            grounded = true;
            jumping = false;
            paralysisTimer += fallParalysisTime;
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
            if (collision.gameObject.CompareTag("Ground")) return;
            OnAir();
        }

        //https://youtu.be/DrFk5Q_IwG0
        // void StepClimb()
        // {
        //     RaycastHit hitLower;
        //     if (Physics.Raycast(stepRayLower.transform.position, transform.TransformDirection(Vector3.forward),
        //             out hitLower, 0.1f))
        //     {
        //         RaycastHit hitUpper;
        //         if (!Physics.Raycast(stepRayUpper.transform.position, transform.TransformDirection(Vector3.forward),
        //                 out hitUpper, 0.2f))
        //         {
        //             transform.position -= new Vector3(0f, -stepSmooth * Time.deltaTime, 0f);
        //         }
        //     }
        //
        //     RaycastHit hitLower45;
        //     if (Physics.Raycast(stepRayLower.transform.position, transform.TransformDirection(1.5f, 0, 1),
        //             out hitLower45, 0.1f))
        //     {
        //         RaycastHit hitUpper45;
        //         if (!Physics.Raycast(stepRayUpper.transform.position, transform.TransformDirection(1.5f, 0, 1),
        //                 out hitUpper45, 0.2f))
        //         {
        //             transform.position -= new Vector3(0f, -stepSmooth * Time.deltaTime, 0f);
        //         }
        //     }
        //
        //     RaycastHit hitLowerMinus45;
        //     if (Physics.Raycast(stepRayLower.transform.position, transform.TransformDirection(-1.5f, 0, 1),
        //             out hitLowerMinus45, 0.1f))
        //     {
        //         RaycastHit hitUpperMinus45;
        //         if (!Physics.Raycast(stepRayUpper.transform.position, transform.TransformDirection(-1.5f, 0, 1),
        //                 out hitUpperMinus45, 0.2f))
        //         {
        //             transform.position -= new Vector3(0f, -stepSmooth * Time.deltaTime, 0f);
        //         }
        //     }
        // }

        #endregion

        #region On Inputs

        public void OnRotateInput(InputAction.CallbackContext context)
        {
            if (usingSlowmo)
            {
                // SetNewRotation(true);
                // Debug.Log("Rotating while slow mo");
                // Debug.Log("\n Target Rot: " + targetRotation.eulerAngles + 
                //                     "\n Current Rot: " + transform.rotation.eulerAngles);
            }
            else
            {
                //Calculate velocity and rotation after camera moved;
                ProjectVelocity();
                SetNewRotation(false);
            }
        }

        public void OnMovementInput(InputAction.CallbackContext context)
        {
            if (usingSlowmo || regenerating) return;
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

            //If moving, cancel regen
            CancelRegeneration();
        }

        public void OnJumpInput(InputAction.CallbackContext context)
        {
            if (paralysisTimer > 0) return;
            if (usingSlowmo || regenerating || attack.attacking) return;
            if (!context.started)
                return;
            HandleJumping();

            //If jumping, cancel regen
            CancelRegeneration();

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
            if (regenerating || paralysisTimer > 0) return;
            if (!context.started) return;
            SetNewRotation(true);
            transform.localRotation = targetRotation;
            HandleAttackInput();

            //If attacking, cancel regen
            CancelRegeneration();
        }

        public void OnInteract(InputAction.CallbackContext context)
        {
            if (!context.started) return;
            if (!canInteract) return;
            interactable.GetInteracted();
        }

        public void OnRegenerateInput(InputAction.CallbackContext context)
        {
            if (!context.started) return;
            if (!grounded) return;
            if (regenTimer > 0) return;
            if (regenerators < 1) return;
            if (data.currentStats.health >= data.baseStats.health) return;
            UpdateRegeneration();
        }

        public void OnSlowMoInput(InputAction.CallbackContext context)
        {
            if (paralysisTimer > 0) return;
            if (attack.attacking || dashing || regenerating || !grounded) return;
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
                SetNewRotation(true);
                transform.localRotation = targetRotation;
                ClearMovement();
                TimePhys.TimeChanger.Get().Activate();
                usingSlowmo = true;
                cameraManager.OnTimeCharge(context);
            }

            //If slowing, cancel regen
            CancelRegeneration();
        }

        #endregion

        #region Attacking

        private void HandleAttackInput()
        {
            if (attack.attacking) return; //If player is already attacking, exit

            if (usingSlowmo) return; //If player is using time changer, exit

            if (!grounded || dashing) return; //If player is on air or dashing, exit

            paralysisTimer += attack.attackTime;
            attack.StartAttack();
            Attacked?.Invoke();
        }

        #endregion

        #region Dash Inputs

        public void OnDashInput(InputAction.CallbackContext context)
        {
            if (paralysisTimer > 0) return;
            if (attack.attacking || usingSlowmo || regenerating) return;
            if (dashCurrent || dashAirCompleted) return;
            StartCoroutine(Dash());

            //If dashing, cancel regen
            CancelRegeneration();
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

            float dashTimer = dashDuration;

            while (dashTimer > 0 && !(paralysisTimer > 0))
            {
                dashTimer -= Time.deltaTime;
                yield return null;
            }

            rigidbody.velocity = Vector3.zero;
            rigidbody.useGravity = true;
            dashing = false;

            dashTimer = dashCooldown;

            while (dashTimer > 0)
            {
                dashTimer -= Time.deltaTime;
                yield return null;
            }

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

        #region HP

        public void HealAll()
        {
            data.currentStats.health = data.baseStats.health;
            LifeChanged.Invoke(1);
        }
        
        public void EnableRegen()
        {
            //Healing?.Invoke(true);
            currentRegens = baseRegens;
            //StartCoroutine(RegenerateAll());
        }

        IEnumerator RegenerateAll()
        {
            Healing?.Invoke(true);
            float missingHP = data.baseStats.health - data.currentStats.health;
            while (data.currentStats.health < data.baseStats.health && regenerators > 0)
            {
                data.currentStats.health += regenMod;
                currentRegens--;
                LifeChanged?.Invoke(1);
                yield return new WaitForSeconds(regenDuration / missingHP);
            }

            Healing?.Invoke(false);
        }

        void Regenerate()
        {
            if (regenTimer > 0) return;

            data.currentStats.health += regenMod;
            if (data.currentStats.health > data.baseStats.health)
                data.currentStats.health = data.baseStats.health;
            regenerating = true;
            currentRegens--;

            LifeChanged?.Invoke(1);
            Healing?.Invoke(false);
        }

        void CancelRegeneration()
        {
            if (regenerating) return;
            Healing?.Invoke(false);
            regenTimer = -1;
        }

        void UpdateRegeneration()
        {
            if (regenTimer == -1)
            {
                Healing?.Invoke(true);
                regenTimer = regenDelay;
            }
            else if (!regenerating)
            {
                Regenerate();
                regenTimer = regenerating ? regenDuration : -1;
            }
            else
            {
                Healing?.Invoke(false);
                regenerating = false;
                regenTimer = -1;
            }
        }

        public void GetHitted(int damage)
        {
            if (invulnerabilityTimer > 0) return;

            //Start timers
            invulnerabilityTimer = hittedInvulnerabilityTime;
            if (damage > minHeavyDamage)
            {
                paralysisTimer += hittedLightParalysisTime;
            }
            else
            {
                paralysisTimer += hittedHeavyParalysisTime;
            }

            //Reduce health and send event
            data.currentStats.health -= damage;
            LifeChanged?.Invoke(-damage);

            //If died, send event
            if (data.currentStats.health > 0) return;
            data.currentStats.health = 0;
            Died?.Invoke();
            coll.enabled = false;
            rigidbody.isKinematic = true;

            //If died disable controller
            this.enabled = false;
            //Destroy(this);
        }

        #endregion

        #region Interact

        void CanInteract() //ALL OF THIS REALLY DIRTY, RETHINK
        {
            canInteract = false;
            interactable = null;

            Vector3 pos = transform.position + interactCheckOffset * transform.forward;
            Quaternion rot = transform.rotation;

            Collider[] cols = Physics.OverlapBox(pos, interactCheckSize / 2, rot, interactableLayers);

            for (int i = 0; i < cols.Length; i++)
            {
                interactable = cols[i].GetComponent<InteractableController>();

                if (interactable == null) continue;
                
                canInteract = true;
                return;
            }
        }

        #endregion
    }
}