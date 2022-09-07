using UnityEngine;

namespace TimeDistortion.Gameplay.Characters
{
    public class PlayerAudio : MonoBehaviour
    {
        [Header("Set Values")]
        [SerializeField] Handler.PlayerController controller;
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
        FMOD.Studio.EventInstance slowMoAudioInstance;

        //Unity Events
        private void Start()
        {
            //Get list
            if (!timeManager)
            {
                timeManager = Physic.TimeManager.Get();
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
            playerHitter.HittedSomething += OnPlayerHittedSomething;
            timeManager.TargetInScope += OnSlowMoReady;
            timeManager.ObjectSlowed += OnObjectSlowed;
            timeManager.ObjectUnSlowed += OnObjectUnSlowed;
            timeManager.SlowMoFailed += OnSlowMoFailed;
        }
        private void Update()
        {
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
            slowMoReadyAudioInstance.release();
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
            FMODUnity.RuntimeManager.PlayOneShot(slowMoAudio);
            //slowMoAudioInstance = FMODUnity.RuntimeManager.CreateInstance(slowMoAudio);
            //slowMoAudioInstance.start();
        }
        void OnObjectUnSlowed(Transform notUsed)
        {
            //slowMoAudioInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            //slowMoAudioInstance.release(); 
        }
        void OnSlowMoFailed()
        {
            FMODUnity.RuntimeManager.PlayOneShot(slowMoFailedAudio);
        }
    }
}