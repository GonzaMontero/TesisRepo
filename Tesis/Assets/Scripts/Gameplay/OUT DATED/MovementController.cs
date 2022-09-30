using UnityEngine;

namespace TimeDistortion.Gameplay
{
    public abstract class MovementController : MonoBehaviour, ITimed
    {
        [Header("Set Values")]
        [SerializeField] float speedMod = 1;
        [SerializeField] bool affectedByTime = true;
        
        [Header("Runtime Values")]
        [SerializeField] internal Vector3 moveInput;
        [SerializeField] internal float currentSpeedMod;

        public Vector3 publicMoveInput { set { moveInput += value; } }

        //Unity Events
        void Start()
        {
            currentSpeedMod = speedMod;
        }
        internal void Update()
        {
            if (moveInput.sqrMagnitude <= 0) return;

            Move(Space.Self);
        }

        //Methods
        internal abstract void Move(Space moveRelativeTo);

        //Interface Implementations
        public void ChangeTime(float newTime)
        {
            if (!affectedByTime) return;
            
            currentSpeedMod = speedMod * newTime;
        }
    }
}