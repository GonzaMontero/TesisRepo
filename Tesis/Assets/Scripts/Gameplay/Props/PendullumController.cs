using UnityEngine;

namespace TimeDistortion.Gameplay.Props
{
    public class PendullumController : MonoBehaviour, ITimed
    {
        [SerializeField] Animator animator;

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
        public void TimeChanged(bool useModifiedTime)
        {
            animator.updateMode = useModifiedTime ? AnimatorUpdateMode.Normal : AnimatorUpdateMode.UnscaledTime;
        }
    }
}