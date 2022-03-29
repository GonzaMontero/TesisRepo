using UnityEngine;

namespace TimeDistortion.Gameplay.Characters
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Set Values")]
        [SerializeField] KinematicMoveController moveController;
        [SerializeField] float rotationSpeed;
        [SerializeField] float jumpForce;

        //Unity Events
        private void Start()
        {
            if (!moveController)
            {
                moveController = GetComponent<KinematicMoveController>();
            }
        }
        private void Update()
        {
            Rotate();
            Move();
        }

        //Methods
        private void Rotate()
        {
            float scaledRotationSpeed = rotationSpeed * Camera.main.scaledPixelWidth;
            float mouseRotation = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
            transform.Rotate(0, mouseRotation, 0, Space.World);
        }
        void Move()
        {
            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");
            float y = Input.GetKey(KeyCode.Space) && moveController.inFloor ? 1 : 0;
            
            Vector3 moveInput = new Vector3(x, y * jumpForce, z);

            if (moveInput.sqrMagnitude > 0)
            {
                moveController.publicMoveInput = moveInput;
            }
        }
    }
}