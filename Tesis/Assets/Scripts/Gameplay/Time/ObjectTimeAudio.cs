using UnityEngine;

namespace TimeDistortion.Gameplay.TimePhys
{
    public class ObjectTimeAudio : MonoBehaviour
    {
        [Header("Set Values")]
        [SerializeField] ObjectTimeController controller;
        [SerializeField] FMODUnity.EventReference loopAudio;
        [Header("Runtime Values")] 
        [SerializeField] bool playingLoop;
        FMOD.Studio.EventInstance loopAudioInstance;


        //Unity Events
        void Start()
        {
            if (!controller)
            {
                controller = GetComponent<ObjectTimeController>();
            }

            controller.TimeChanged += OnTimeChanged;
        }
        // void Update()
        // {
        //     if(!playingLoop) return;
        //     
        //     if (controller.slowMoLeft > 0)
        //     {
        //         //Instantiate audio
        //         loopAudioInstance = FMODUnity.RuntimeManager.CreateInstance(loopAudio);
        //         loopAudioInstance.start();
        //
        //         //Set 3d attributes
        //         FMODUnity.RuntimeManager.AttachInstanceToGameObject(loopAudioInstance, transform);
        //     }
        //     else
        //     {
        //         loopAudioInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        //         loopAudioInstance.release();
        //     }
        // }

        //Methods
        
        //Event Receivers
        void OnTimeChanged()
        {
            //playingLoop = !playingLoop;
            playingLoop = controller.slowMoLeft > 0;
            
            if (controller.slowMoLeft > 0)
            {
                //Instantiate audio
                loopAudioInstance = FMODUnity.RuntimeManager.CreateInstance(loopAudio);
                loopAudioInstance.start();

                //Set 3d attributes
                FMODUnity.RuntimeManager.AttachInstanceToGameObject(loopAudioInstance, transform);
            }
            else
            {
                loopAudioInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                loopAudioInstance.release();
            }
        }
    }
}