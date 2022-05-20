using UnityEngine;

namespace TimeDistortion.Gameplay.Props
{
    public class CircuitPartModel : MonoBehaviour
    {
        [Header("Set Values")]
        [SerializeField] CircuitPartController controller;
        [SerializeField] Animator animator;
        [SerializeField] string animationTrigger;

        //Unity Events
        private void Start()
        {
            if (controller == null)
            {
                controller = GetComponent<CircuitPartController>();
            }
            if (animator == null)
            {
                animator = GetComponent<Animator>();
            }

            controller.Activated += OnControllerActivated;
        }

        //Methods

        //Event Receivers
        void OnControllerActivated(CircuitPartController controller)
        {
            animator.SetTrigger(animationTrigger);
        }
    }
}