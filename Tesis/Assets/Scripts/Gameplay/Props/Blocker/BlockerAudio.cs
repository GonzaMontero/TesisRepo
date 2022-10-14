using UnityEngine;

namespace TimeDistortion.Gameplay.Props
{
    public class BlockerAudio : MonoBehaviour
    {
        [Header("Set Values")]
        [SerializeField] BlockerController controller;
        [SerializeField] FMODUnity.EventReference loopAudio;
        [Header("Runtime Values")] 
        [SerializeField] bool playingLoop;
        FMOD.Studio.EventInstance loopAudioInstance;


        //Unity Events
        void Start()
        {
            if (!controller)
            {
                controller = GetComponent<BlockerController>();
            }

            controller.Hitted += OnHit;
            
            SetLoopAudio();
        }
        
        //Methods
        void SetLoopAudio()
        {
            //Instantiate audio
            loopAudioInstance = FMODUnity.RuntimeManager.CreateInstance(loopAudio);
            loopAudioInstance.start();
                
            //Set 3d attributes
            FMODUnity.RuntimeManager.AttachInstanceToGameObject(loopAudioInstance, transform);
        }
        
        //Event Receivers
        void OnHit()
        {
            loopAudioInstance.setParameterByName("blockerState", 1);
        }
    }
}