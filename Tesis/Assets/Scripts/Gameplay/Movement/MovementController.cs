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
        private void Awake()
        {
            TimeChanged(false);
        }
        internal void Update()
        {
            if (moveInput.sqrMagnitude <= 0) return;

            Move(Space.Self);
        }

        //Methods
        internal abstract void Move(Space moveRelativeTo);

        //Interface Implementations
        public void TimeChanged(bool useModifiedTime)
        {
            float currentTimeSpeed = affectedByTime ? Time.timeScale : 1; //1=unscaled timeScale

            currentSpeedMod = speedMod * currentTimeSpeed;
        }
    }
}