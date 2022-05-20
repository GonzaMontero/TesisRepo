using UnityEngine;

namespace TimeDistortion.Gameplay.Characters
{
    public class PlayerModel : MonoBehaviour
    {
        [Header("Set Values")]
        [SerializeField] Handler.InputHandler controller;
        [SerializeField] Physic.TimeManager timeManager;
        [SerializeField] Animator animator;

        //Unity Events
        private void Start()
        {
            if (!animator)
            {
                animator = GetComponentInChildren<Animator>();
            }
            if (!timeManager)
            {
                timeManager = Physic.TimeManager.Get();
            }
            if (!controller)
            {
                controller = GetComponent<Handler.InputHandler>();
            }

            //controller.CameraLocked += OnCameraLocked;
            controller.PlayerAttacked += OnPlayerAttacked;
            controller.PlayerJumped += OnPlayerJumped;
            controller.PlayerMoved += OnPlayerMoved;
            //timeManager.SlowMoReady += ;
            timeManager.ObjectSlowed += OnObjectSlowed;
            //timeManager.ObjectUnSlowed +=;
        }
        private void Update()
        {
            if (controller.publicGroundedPlayer == animator.GetBool("OnAir"))
            {
                animator.SetBool("OnAir", !controller.publicGroundedPlayer);
            }
        }

        //Methods

        //Event Receivers
        void OnCameraLocked(bool cameraLocked)
        {
        }
        void OnPlayerAttacked()
        {
            animator.SetTrigger("Attack");
        }
        void OnPlayerJumped()
        {
            animator.SetTrigger("Jumping");
        }
        void OnPlayerMoved(bool playerMoved)
        {
            animator.SetBool("Walking", playerMoved);
        }
        void OnObjectSlowed(Transform notUsed, float _notUsed)
        {
            animator.SetTrigger("StopTime");
        }
    }
}