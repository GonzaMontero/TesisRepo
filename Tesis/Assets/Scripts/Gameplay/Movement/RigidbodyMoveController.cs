using UnityEngine;

namespace TimeDistortion.Gameplay
{
    [RequireComponent(typeof(Rigidbody))]
    public class RigidbodyMoveController : MovementController
    {
        [Header("Set Values")]
        [SerializeField] Physic.PhysicsManager physicsManager;
        [SerializeField] Rigidbody rigidBody;
        [SerializeField] float bounceSpeedMod;
        [SerializeField] bool doFunnyThings;

        [Header("Runtime Values")]
        [SerializeField] Vector3 rbSpeed;
        [SerializeField] Vector3 rbBounceSpeed;
        [SerializeField] bool isInFloor;
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
        private void OnCollisionEnter(Collision collision)
        {
            rbSpeed = rigidBody.velocity;

            if (!doFunnyThings) return;
            //rigidBody.velocity = Vector3.zero;
        }
        private void OnCollisionStay(Collision collision)
        {
            rbBounceSpeed = rigidBody.velocity + collision.impulse * bounceSpeedMod;

            //Debug.Break();

            if (!doFunnyThings) return;
            moveInput = rbBounceSpeed;
        }

        //Methods
        internal override void Move(Space moveRelativeTo)
        {
            rigidBody.AddForce(moveInput * currentSpeedMod);
            moveInput = Vector3.zero;
        }
        void Fall()
        {
            if (!GroundIsNear())
            {
                moveInput = physicsManager.GetObjectGravityPull(mass);
                Move(Space.World);
            }
        }
        bool GroundIsNear()
        {
            isInFloor = Physics.Raycast(transform.position, moveInput, 1);
            return isInFloor;
        }
    }
}