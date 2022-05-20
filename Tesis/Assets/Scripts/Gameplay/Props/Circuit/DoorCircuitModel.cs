using UnityEngine;

namespace TimeDistortion.Gameplay.Props
{
    public class DoorCircuitModel : MonoBehaviour
    {
        [Header("Set Values")]
        [SerializeField] CircuitManager manager;
        [SerializeField] Animator animator;

        //Unity Events
        private void Start()
        {
            if (!animator)
            {
                animator = transform.GetComponent<Animator>();
            }

            manager.CircuitCompleted += OnCircuitCompleted;
        }

        //Methods


        //Event Receiver
        void OnCircuitCompleted()
        {
            animator.SetTrigger("Open");
        }
    }
}