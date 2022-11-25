using UnityEngine;

namespace TimeDistortion.Gameplay.Characters
{
    public class PlayerAudio : MonoBehaviour
    {
        [Header("Set Values")]
        [SerializeField] Handler.PlayerController controller;
        [SerializeField] SwordHit playerHitter;
        [SerializeField] TimePhys.TimeChanger timeChanger;
        [SerializeField] FMODUnity.EventReference walkAudio;
        [SerializeField] FMODUnity.EventReference jumpAudio;
        [SerializeField] FMODUnity.EventReference landAudio;
        [SerializeField] FMODUnity.EventReference attackAudio;
        [SerializeField] FMODUnity.EventReference hitAudio;
        [SerializeField] FMODUnity.EventReference hitStoneAudio;
        [SerializeField] FMODUnity.EventReference castSlowMoAudio;
        [SerializeField] FMODUnity.EventReference slowMoChargingAudio;
        [SerializeField] FMODUnity.EventReference slowMoChargedAudio;
        [SerializeField] FMODUnity.EventReference slowMoFailedAudio;
        [SerializeField] FMODUnity.EventReference dashAudio;
        [SerializeField] FMODUnity.EventReference lightDamageAudio;
        [SerializeField] FMODUnity.EventReference heavyDamageAudio;
        [SerializeField] FMODUnity.EventReference dieAudio;
        [SerializeField] FMODUnity.EventReference healAudio;
        [SerializeField] FMODUnity.EventReference spawnAudio;
        [Header("Runtime Values")]
        [SerializeField] bool playerOnAir;
        [SerializeField] bool playerWalking;
        [SerializeField] bool slowMoCharged;
        [SerializeField] bool playerSpawned;
        FMOD.Studio.EventInstance walkAudioInstance;
        FMOD.Studio.EventInstance slowMoChargedAudioInstance;

        //Unity Events
        private void Start()
        {
            //Get list
            if (!timeChanger)
            {
                timeChanger = TimePhys.TimeChanger.Get();
            }
            if (!controller)
            {
                controller = GetComponent<Handler.PlayerController>();
            }
            if (!playerHitter)
            {
                playerHitter = GetComponentInChildren<SwordHit>();
            }

            //Link events
            controller.Jumped += OnPlayerJumped;
            controller.Moved += OnPlayerMoved;
            controller.Attacked += OnPlayerAttacked;
            controller.Dashed += OnPlayerDashed;
            playerHitter.HittedSomething += OnPlayerHittedSomething;
            playerHitter.HittedStone += OnPlayerHittedStone;
            //timeChanger.TargetInScope += OnSlowMoTargetting;
            timeChanger.ReleasedCharge += OnSlowMoReleased;
            timeChanger.ActivatingCharge += OnSlowMoCharging;
            controller.LifeChanged += OnPlayerLifeChanged;
            controller.Died += OnPlayerDied;
        }
        private void Update()
        {
            if (!playerSpawned && controller.isSpawning)
            {
                OnPlayerSpawned();
                playerSpawned = true;
            }
            
            if (timeChanger.publicCharge >= 1)
            {
                if (!slowMoCharged)
                {
                    slowMoCharged = true;
                    OnSlowMoCharged();
                }
            }
            else if (slowMoCharged)
            {
                slowMoCharged = false;
            }
            
            if (controller.grounded != playerOnAir) return;
            playerOnAir = !controller.grounded;

            if (playerOnAir)
            {
                //If player is moving and jumps, stop footsteps audio
                if (!playerWalking) return;
                OnPlayerMoved(false);
                return;
            }

            OnPlayerLanded();
        }
        private void OnDestroy()
        {
            FMODUnity.RuntimeManager.GetBus("Bus:/").stopAllEvents(FMOD.Studio.STOP_MODE.IMMEDIATE);
            walkAudioInstance.release();
            //slowMoReadyAudioInstance.release();
        }

        //Event Receivers
        void OnPlayerAttacked()
        {
            FMODUnity.RuntimeManager.PlayOneShot(attackAudio);
        }
        void OnPlayerHittedSomething()
        {
            FMODUnity.RuntimeManager.PlayOneShot(hitAudio);
        }
        void OnPlayerHittedStone()
        {
            FMODUnity.RuntimeManager.PlayOneShot(hitStoneAudio);
        }
        void OnPlayerJumped()
        {
            FMODUnity.RuntimeManager.PlayOneShot(jumpAudio);
        }
        void OnPlayerLanded()
        {
            FMODUnity.RuntimeManager.PlayOneShot(landAudio);
        }
        void OnPlayerMoved(bool playerMoved)
        {
            //If on air, player can't be moving
            if (playerOnAir)
            {
                playerMoved = false;
            }

            //If already using right audio, exit
            if (playerWalking == playerMoved) return;

            //If moving play loop, else stop
            if (playerMoved)
            {
                walkAudioInstance = FMODUnity.RuntimeManager.CreateInstance(walkAudio);
                walkAudioInstance.start();
            }
            else
            {
                walkAudioInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                walkAudioInstance.release();
            }

            //Update value
            playerWalking = playerMoved;
        }
        void OnPlayerDashed()
        {
            FMODUnity.RuntimeManager.PlayOneShot(dashAudio);
        }
        void OnSlowMoCharging()
        {
            FMODUnity.RuntimeManager.PlayOneShot(slowMoChargingAudio);
            // slowMoAudioInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            // slowMoAudioInstance.release(); 
        }
        void OnSlowMoCharged()
        {
            slowMoChargedAudioInstance = FMODUnity.RuntimeManager.CreateInstance(slowMoChargedAudio);
            slowMoChargedAudioInstance.start();
            //FMODUnity.RuntimeManager.PlayOneShot(slowMoChargedAudio);
        }
        void OnSlowMoReleased()
        {
            if (timeChanger.publicTargetTransform)
            {
                FMODUnity.RuntimeManager.PlayOneShot(castSlowMoAudio);
            }
            else
            {
                FMODUnity.RuntimeManager.PlayOneShot(slowMoFailedAudio);
            }

            //DIRTY, AVISARLE A MATI QUE AGREGE EL EVENTO A SLOWMOFAILED
            slowMoChargedAudioInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            slowMoChargedAudioInstance.release();
            
            //FMODUnity.RuntimeManager.PlayOneShot(slowMoAudio);
            //slowMoAudioInstance = FMODUnity.RuntimeManager.CreateInstance(slowMoAudio);
            //slowMoAudioInstance.start();
        }
        void OnPlayerLifeChanged(int healthChange)
        {
            if (healthChange > -1)
            {
                FMODUnity.RuntimeManager.PlayOneShot(healAudio);
                return;
            }
            
            if (healthChange > controller.minHeavyDmg)
            {
                FMODUnity.RuntimeManager.PlayOneShot(lightDamageAudio);
            }
            else
            {
                FMODUnity.RuntimeManager.PlayOneShot(heavyDamageAudio);
            }
        }
        void OnPlayerDied()
        {
            FMODUnity.RuntimeManager.PlayOneShot(dieAudio);
        }
        void OnPlayerSpawned()
        {
            FMODUnity.RuntimeManager.PlayOneShot(spawnAudio);
        }

        #region DEPRECATED
        /*void OnSlowMoTargetting(bool isReady)
        {
            //If already using right audio, exit
            if (slowMoReady == isReady) return;

            //If moving play loop, else stop
            if (isReady)
            {
                //slowMoReadyAudioInstance = FMODUnity.RuntimeManager.CreateInstance(setSlowMoAudio);
                slowMoReadyAudioInstance.start();
            }
            else
            {
                slowMoReadyAudioInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                slowMoReadyAudioInstance.release();
            }

            //Update value
            slowMoReady = isReady;
        }*/
        
        #endregion
    }
}