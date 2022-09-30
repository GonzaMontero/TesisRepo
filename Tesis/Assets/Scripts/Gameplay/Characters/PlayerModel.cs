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
        [SerializeField] TrailRenderer dashTrail;
        [SerializeField] float swordTrailTime;
        [SerializeField] float dashTrailTime;
        [Header("Runtime Values")]
        [SerializeField] float swordTrailTimer;
        [SerializeField] float dashTrailTimer;

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
            if (controller.dashing != animator.GetBool("Dashing"))
            {
                animator.SetBool("Dashing", controller.dashing);
                UpdateDashTrail();
            }

            if (swordTrailTimer > 0)
            {
                swordTrailTimer -= Time.deltaTime;
            }
            if (!controller.dashing && dashTrailTimer > 0)
            {
                dashTrailTimer -= Time.deltaTime;
                UpdateDashTrail();
            }
            else if(swordTrail.activeSelf)
            {
                swordTrail.SetActive(false);
            }
        }

        //Methods
        void UpdateDashTrail()
        {
            if(controller.dashing)
            {
                dashTrail.enabled = true;
                dashTrailTimer = dashTrailTime;
            }
            else if(!(dashTrailTimer > 0))
            {
                dashTrail.enabled = false;
            }

            dashTrail.time = dashTrailTimer / dashTrailTime;
        }

        //Event Receivers
        void OnCameraLocked(bool cameraLocked)
        {
        }
        void OnPlayerAttacked()
        {
            animator.SetTrigger("Attack");
            swordTrailTimer = swordTrailTime;
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