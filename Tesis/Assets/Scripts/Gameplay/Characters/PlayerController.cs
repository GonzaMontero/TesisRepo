using UnityEngine;

namespace TimeDistortion.Gameplay.Characters
{
    public class PlayerController : MonoBehaviour, IHittable
    {
        [Header("Cheat Values")]
        [SerializeField] int damageToReceive;
        [SerializeField] bool receiveDamageNow;
        [Header("Set Values")]
        [SerializeField] KinematicMoveController moveController;
        [SerializeField] CharacterData data;
        [SerializeField] float rotationSpeed;
        [SerializeField] float jumpForce;
        //[Header("Runtime Values")]
        //[SerializeField] Stats currentStats;

        //Unity Events
        private void Start()
        {
            if (!moveController)
            {
                moveController = GetComponent<KinematicMoveController>();
            }

            data.currentStats = data.baseStats;
        }
        private void Update()
        {
            Rotate();
            Move();
            if (receiveDamageNow)
            {
                receiveDamageNow = false;
                GetHitted(damageToReceive);
            }
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

        //Interface Implementations
        public void GetHitted(int damage)
        {
            data.currentStats.health -= damage;

            if (data.currentStats.health > 0) return;
            data.currentStats.health = 0;
            Destroy(gameObject); //replace for EVENT
        }
    }
}