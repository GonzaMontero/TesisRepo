using UnityEngine;

namespace Universal.Animations
{
        public class AnimViewer : MonoBehaviour
        {
            [SerializeField] Animator animator;
            [SerializeField] string triggerName;
            [SerializeField] bool runAnimation;

            //Unity Events
            private void Start()
            {
                if (!animator)
                {
                    animator = GetComponent<Animator>();
                }
            }
            private void Update()
            {
                if (runAnimation)
                {
                    runAnimation = false;
                    animator.SetTrigger(triggerName);
                    Debug.Log("Running " + triggerName + " in " + gameObject);
                }
            }
        }
}