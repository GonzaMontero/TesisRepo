using UnityEngine;

namespace TimeDistortion.Gameplay
{
    public class KinematicMoveController : MovementController
    {
        [Header("Set Values")]
        [SerializeField] Physic.PhysicsManager physicsManager;
        [SerializeField] Rigidbody rigidBody;
        [SerializeField] float bounceSpeedMod;
        [SerializeField] bool useGravity;

        [Header("Runtime Values")]
        [SerializeField] Vector3 rbSpeed;
        [SerializeField] Vector3 rbBounceSpeed;
        [SerializeField] bool isInFloor;

        public bool inFloor { get { return isInFloor; } }
        float mass { get { return rigidBody.mass; } }

        //Unity Events
        private void Start()
        {
            if (!rigidBody)
            {
                rigidBody = GetComponent<Rigidbody>();
            }
            if (!physicsManager)
            {
                physicsManager = Physic.PhysicsManager.Get();
            }
        }
        private new void Update()
        {
            base.Update();

            Fall();
        }

        //Methods
        internal override void Move(Space moveRelativeTo)
        {
            //if there is an obstacle, don't move
            if (Physics.Raycast(transform.position, moveInput, .5f))
            {
                moveInput *= 0;
                return;
            }

            moveInput *= currentSpeedMod * Time.unscaledDeltaTime;
            transform.Translate(moveInput, moveRelativeTo);
            moveInput *= 0;
        }
        void Fall()
        {
            if (useGravity && !GroundIsNear())
            {
                Vector3 gravityPull = physicsManager.GetObjectGravityPull(mass);
                gravityPull = gravityPull.magnitude * -transform.up;

                moveInput = physicsManager.GetObjectGravityPull(mass);

                Move(Space.World);
            }
            else
            {
                //Debug.Log("On Floor");
            }
        }
        bool GroundIsNear()
        {
            Vector3 gravityPull = physicsManager.GetObjectGravityPull(mass);
            //gravityPull = gravityPull.magnitude * -transform.up;
            float timeXspeed = Time.deltaTime * currentSpeedMod;

            isInFloor = Physics.Raycast(transform.position, gravityPull, .5f);

            return isInFloor;
        }
    }
}