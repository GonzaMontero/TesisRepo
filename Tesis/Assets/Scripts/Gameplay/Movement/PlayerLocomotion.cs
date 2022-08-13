using UnityEngine;

namespace TimeDistortion.Gameplay.Handler
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerLocomotion : MonoBehaviour
    {
        Transform cameraObject;
        RigidBodyCharacter rigidBodyCharacter;
        Vector3 moveDirection;

        bool grounded = true;

        [HideInInspector] public Transform myTransform;
        [HideInInspector] public AnimatorHandler animatorHandler;
        [HideInInspector] public new Rigidbody rigidbody;
        [HideInInspector] public GameObject normalCamera;

        [Header("Stats")]
        [SerializeField] float movementSpeed = 5;
        [SerializeField] float rotationSpeed = 10;

        void Start()
        {
            rigidbody = GetComponent<Rigidbody>();
            rigidBodyCharacter = GetComponent<RigidBodyCharacter>();
            animatorHandler = GetComponentInChildren<AnimatorHandler>();
            cameraObject = Camera.main.transform;
            myTransform = transform;

            grounded = true;

            animatorHandler.Initialize();
        }

        public void Update()
        {
            float delta = Time.deltaTime;

            rigidBodyCharacter.TickInput(delta);

            moveDirection = cameraObject.forward * rigidBodyCharacter.vertical;
            moveDirection += cameraObject.right * rigidBodyCharacter.horizontal;
            moveDirection.Normalize();

            float speed = movementSpeed;
            moveDirection *= speed;
            
            Vector3 projectedVelocity = Vector3.ProjectOnPlane(moveDirection, normalVector);

            if (projectedVelocity.y != 0 && grounded)
            {
                projectedVelocity = new Vector3(projectedVelocity.x, 0, projectedVelocity.z);
            }

            rigidbody.velocity = projectedVelocity;

            if (animatorHandler.canRotate)
            {
                HandleRotation(delta);
            }

            HandleJumping();                       
        }

        #region Movement
        Vector3 normalVector;

        private void HandleRotation(float delta)
        {
            Vector3 targetDir = Vector3.zero;
            float moveOverride = rigidBodyCharacter.moveAmount;

            targetDir = cameraObject.forward * rigidBodyCharacter.vertical;
            targetDir += cameraObject.right * rigidBodyCharacter.horizontal;

            targetDir.Normalize();
            targetDir.y = 0;

            if (targetDir == Vector3.zero)
            {
                targetDir = myTransform.forward;
            }

            float rs = rotationSpeed;

            Quaternion tr = Quaternion.LookRotation(targetDir);
            Quaternion targetRotation = Quaternion.Slerp(myTransform.rotation, tr, rotationSpeed * delta);

            myTransform.rotation = targetRotation;
        }

        public void HandleJumping()
        {
            if (rigidBodyCharacter.jumpInput && grounded)
            {
                //if (inputHandler.moveAmount > 0)
                //{
                //    moveDirection = cameraObject.forward * inputHandler.vertical;
                //    moveDirection += cameraObject.right * inputHandler.horizontal;
                //    moveDirection.y = 0;
                //    Quaternion jumpRotation = Quaternion.LookRotation(moveDirection);
                //    myTransform.rotation = jumpRotation;
                //}
                rigidbody.AddForce(new Vector3(0, 50, 0), ForceMode.Impulse);
                grounded = false;
            }
        }

        #endregion

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.transform.tag == "Ground")
            {
                grounded = true;
            }
        }
    }
}
