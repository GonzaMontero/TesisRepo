using UnityEngine;

namespace TimeDistortion.Gameplay.Characters
{
    public class PlayerModel : MonoBehaviour
    {
        [Header("Set Values")]
        [SerializeField] Handler.PlayerController controller;
        [SerializeField] Physic.TimeManager timeManager;
        [SerializeField] Animator animator;
        [SerializeField] GameObject swordTrail;
        [SerializeField] float swordTrailTimer;
        [Header("Runtime Values")]
        [SerializeField] float timer;

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
                controller = GetComponent<Handler.PlayerController>();
            }

            //controller.CameraLocked += OnCameraLocked;
            controller.Attacked += OnPlayerAttacked;
            controller.Jumped += OnPlayerJumped;
            controller.Moved += OnPlayerMoved;
            //timeManager.SlowMoReady += ;
            timeManager.ObjectSlowed += OnObjectSlowed;
            //timeManager.ObjectUnSlowed +=;
        }
        private void Update()
        {
            if (controller.grounded == animator.GetBool("OnAir"))
            {
                animator.SetBool("OnAir", !controller.grounded);
            }

            if (timer > 0)
            {
                timer -= Time.deltaTime;
            }
            else if(swordTrail.activeSelf)
            {
                swordTrail.SetActive(false);
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
            timer = swordTrailTimer;
            swordTrail.SetActive(true);
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