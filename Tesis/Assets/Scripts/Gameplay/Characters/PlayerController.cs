using System;
using UnityEngine;

namespace TimeDistortion.Gameplay.Characters
{
    public class PlayerController : MonoBehaviour, IHittable
    {
        [Header("Set Values")]
        [SerializeField] KinematicMoveController moveController;
        [SerializeField] CharacterData data;
        [SerializeField] float rotationSpeed;
        [SerializeField] float jumpForce;
        //[Header("Runtime Values")]
        //[SerializeField] Stats currentStats;

        public Action Hitted;
        public Action Died;

        public CharacterData publicData { get { return data; } }

        //Unity Events
        private void Awake()
        {
            data.currentStats = data.baseStats;
        }
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

        //Interface Implementations
        public void GetHitted(int damage)
        {
            data.currentStats.health -= damage;
            Hitted?.Invoke();

            if (data.currentStats.health > 0) return;
            data.currentStats.health = 0;
            Died?.Invoke();
        }
    }
}