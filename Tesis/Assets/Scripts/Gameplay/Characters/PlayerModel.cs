using UnityEngine;

namespace TimeDistortion.Gameplay.Characters
{
    public class PlayerModel : MonoBehaviour
    {
        [Header("Set Values")]
        [SerializeField] Handler.PlayerController controller;
        [SerializeField] TimePhys.TimeChanger timeChanger;
        [SerializeField] Animator animator;
        [SerializeField] GameObject swordTrail;
        [SerializeField] TrailRenderer dashTrail;
        [SerializeField] float swordTrailTime;
        [SerializeField] float dashTrailTime;
        [SerializeField] int minHeavyDmg;
        [Header("Runtime Values")]
        [SerializeField] float swordTrailTimer;
        [SerializeField] float dashTrailTimer;
        //[SerializeField] bool spawned;

        //Unity Events
        private void Start()
        {
            if (!animator)
            {
                animator = GetComponentInChildren<Animator>();
            }
            if (!timeChanger)
            {
                timeChanger = TimePhys.TimeChanger.Get();
            }
            if (!controller)
            {
                controller = GetComponent<Handler.PlayerController>();
            }

            //controller.CameraLocked += OnCameraLocked;
            controller.Attacked += OnPlayerAttacked;
            controller.Jumped += OnPlayerJumped;
            controller.Moved += OnPlayerMoved;
            controller.LifeChanged += OnLifeChanged;
            controller.Died += OnDied;
            controller.Healing += OnHealing;
            timeChanger.ActivatingCharge += OnTimeCharging;
            timeChanger.ReleasedCharge += OnTimeReleased;
        }
        private void Update()
        {
            // if (!spawned && !controller.isSpawning)
            // {
            //     spawned = true;
            //     animator.SetTrigger("Spawned");
            // }
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
        void OnTimeCharging()
        {
            animator.SetBool("ChargingSlowMo", true);
        }
        void OnTimeReleased()
        {
            bool timeChanged = timeChanger.publicCharge >= 1 && timeChanger.publicTargetTransform;
            animator.SetBool("SlowMoCharged", timeChanged);
            animator.SetBool("ChargingSlowMo", false);
        }
        void OnLifeChanged(int healthChange)
        {
            if(healthChange > -1) return;
            
            if (healthChange > minHeavyDmg)
            {
                animator.SetTrigger("LightlyHitted");
            }
            else
            {
                animator.SetTrigger("HeavilyHitted");
            }
        }
        void OnDied()
        {
            animator.SetTrigger("Died");
        }
        void OnHealing()
        {
            animator.SetTrigger("Heal");
        }
    }
}