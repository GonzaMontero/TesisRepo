using UnityEngine;

namespace TimeDistortion.Gameplay.Props
{
    public class PendullumController : MonoBehaviour, ITimed
    {
        [Header("Set Values")]
        [SerializeField] Animator animator;
        [SerializeField] bool affectedByTime = true;

        //Unity Events
        private void Start()
        {
            if (animator == null)
            {
                animator = GetComponent<Animator>();
            }
        }

        //Methods

        //Interface Implementations
        public void TimeChanged(float newTime)
        {
            if (!affectedByTime) return;
            animator.speed = newTime;
        }
    }
}