using UnityEngine;

namespace TimeDistortion.Gameplay.Characters
{
    public class PlayerModel : MonoBehaviour
    {
        [Header("Set Values")]
        [SerializeField] Handler.PlayerController controller;
        [SerializeField] TimePhys.TimeChanger timeChanger;
        [SerializeField] GameObject swordTrail;
        [SerializeField] Animator animator;
        [SerializeField] Animator healOrbAnimator;
        [SerializeField] TrailRenderer dashTrail;
        [SerializeField] float swordTrailTime;
        [SerializeField] float dashTrailTime;
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
            animator.SetBool("AimingSlowMo", true);
        }
        void OnTimeReleased()
        {
            //If TimeChanger still has a target, an object was slowed
            if(timeChanger.publicTargetTransform) animator.SetTrigger("SlowObject");
            
            //Player stopped aiming with slow mo
            animator.SetBool("AimingSlowMo", false);
        }
        void OnLifeChanged(int healthChange)
        {
            if(healthChange > -1) return;
            
            if (healthChange > controller.minHeavyDmg)
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
        void OnHealing(bool healing)
        {
            if (healing)
            {
                animator.SetTrigger("Heal");
                healOrbAnimator.SetTrigger("Appear");
            }
            else if(controller.isRegenerating) //if player is in regen, make heal orb fade
            {
                healOrbAnimator.SetTrigger("Disappear");
            }
            else //if player's heal was interrupted, make orb go puff
            {
                healOrbAnimator.SetTrigger("GoPuff");
            }
        }
    }
}