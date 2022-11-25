using System;
using UnityEngine;

namespace TimeDistortion.Gameplay.Props
{
    public class BlockerStoneAudio : MonoBehaviour
    {
        [Header("Set Values")]
        [SerializeField] FMODUnity.EventReference collisionAudio;
        [SerializeField] LayerMask audibleLayers;
        //[SerializeField]
        //[Header("Runtime Values")]
        FMOD.Studio.EventInstance collisionAudioInstance;


        //Unity Events
        void OnCollisionEnter(Collision collision)
        {
            //if object layer is not inside audible layers, don't play sound
            if(audibleLayers != (audibleLayers | (1 << collision.gameObject.layer))) return;

            if (collisionAudioInstance.isValid())
            {
                collisionAudioInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                collisionAudioInstance.release();
            }
            
            //Instantiate audio
            collisionAudioInstance = FMODUnity.RuntimeManager.CreateInstance(collisionAudio);
            collisionAudioInstance.start();
                
            //Set 3d attributes
            FMODUnity.RuntimeManager.AttachInstanceToGameObject(collisionAudioInstance, transform);
            
            //Debug.Log("Playing audio collision");
        }

        //Methods
        
    }
}