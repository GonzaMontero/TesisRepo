using UnityEngine;

namespace TimeDistortion.Gameplay.Characters
{
    public class PlayerAudio : MonoBehaviour
    {
        [Header("Set Values")]
        [SerializeField] Handler.InputHandler controller;
        [SerializeField] SwordHit playerHitter;
        [SerializeField] Physic.TimeManager timeManager;
        [SerializeField] FMODUnity.EventReference walkAudio;
        [SerializeField] FMODUnity.EventReference jumpAudio;
        [SerializeField] FMODUnity.EventReference landAudio;
        [SerializeField] FMODUnity.EventReference attackAudio;
        [SerializeField] FMODUnity.EventReference hitAudio;
        [SerializeField] FMODUnity.EventReference setSlowMoAudio;
        [SerializeField] FMODUnity.EventReference activateSlowMoAudio;
        [SerializeField] FMODUnity.EventReference slowMoAudio;
        [SerializeField] FMODUnity.EventReference slowMoFailedAudio;
        [Header("Runtime Values")]
        [SerializeField] bool playerOnAir;
        [SerializeField] bool playerWalking;
        [SerializeField] bool slowMoReady;
        FMOD.Studio.EventInstance walkAudioInstance;
        FMOD.Studio.EventInstance slowMoReadyAudioInstance;

        //Unity Events

        private void Start()
        {
            if (!timeManager)
            {
                timeManager = Physic.TimeManager.Get();
            }
            if (!controller)
            {
                controller = GetComponent<Handler.InputHandler>();
            }
            if (!playerHitter)
            {
                playerHitter = GetComponentInChildren<SwordHit>();
            }

            controller.PlayerJumped += OnPlayerJumped;
            controller.PlayerMoved += OnPlayerMoved;
            controller.PlayerAttacked += OnPlayerAttacked;
            playerHitter.HittedSomething += OnPlayerHittedSomething;
            timeManager.SlowMoReady += OnSlowMoReady;
            timeManager.ObjectSlowed += OnObjectSlowed;
            //timeManager.ObjectUnSlowed +=;
        }
        private void Update()
        {
            if (controller.publicGroundedPlayer != playerOnAir) return;
            playerOnAir = !controller.publicGroundedPlayer;
            
            if (playerOnAir) return;
            OnPlayerLanded();
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
        void OnSlowMoReady(bool isReady)
        {
            //If already using right audio, exit
            if (slowMoReady == isReady) return;

            //If moving play loop, else stop
            if (isReady)
            {
                slowMoReadyAudioInstance = FMODUnity.RuntimeManager.CreateInstance(setSlowMoAudio);
                slowMoReadyAudioInstance.start();
            }
            else
            {
                slowMoReadyAudioInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                slowMoReadyAudioInstance.release();
            }

            //Update value
            slowMoReady = isReady;
        }
        void OnObjectSlowed(Transform notUsed, float _notUsed)
        {
            FMODUnity.RuntimeManager.PlayOneShot(activateSlowMoAudio);
        }
    }
}