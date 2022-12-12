using UnityEngine;

namespace TimeDistortion.Gameplay.Props.Circuit
{
    public class CircuitPartModel : MonoBehaviour
    {
        [Header("Set Values")]
        [SerializeField] CircuitPartController controller;
        [SerializeField] GameObject VFX;
        [SerializeField] Animator animator;
        [SerializeField] string animationTrigger;

        //Unity Events
        private void Start()
        {
            if (controller == null)
            {
                controller = GetComponent<CollisionCircuitPart>();
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
            if (animator)
            {
                animator.SetTrigger(animationTrigger);
            }

            if (VFX)
            {
                GameObject vfx = Instantiate(VFX);
                vfx.transform.position = transform.position;
            }
        }
    }
}