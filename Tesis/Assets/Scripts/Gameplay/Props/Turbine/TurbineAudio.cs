using System;
using UnityEngine;
using UnityEngine.UI;

namespace TimeDistortion.Gameplay.Props
{
    public class TurbineAudio : MonoBehaviour
    {
        [Header("Set Values")]
        [SerializeField] TurbineController controller;
        [SerializeField] FMODUnity.EventReference windAudio;
        [Header("Runtime Values")] 
        [SerializeField] bool playingWind;
        FMOD.Studio.EventInstance windAudioInstance;


        //Unity Events
        void Start()
        {
            if (!controller)
            {
                controller = GetComponent<TurbineController>();
            }

            controller.Slowed += OnSlowed;
            
            //Start audio
            OnSlowed(false);
        }
        
        //Event Receivers
        void OnSlowed(bool isSlowed)
        {
            playingWind = !isSlowed;
            if (!isSlowed)
            {
                //Instantiate audio
                windAudioInstance = FMODUnity.RuntimeManager.CreateInstance(windAudio);
                windAudioInstance.start();
                
                //Set 3d attributes
                FMODUnity.RuntimeManager.AttachInstanceToGameObject(windAudioInstance, transform);

            }
            else
            {
                windAudioInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                windAudioInstance.release();
            }
        }
    }
}