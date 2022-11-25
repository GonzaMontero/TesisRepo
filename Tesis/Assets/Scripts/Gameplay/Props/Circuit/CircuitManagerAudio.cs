using System;
using UnityEngine;

namespace TimeDistortion.Gameplay.Props.Circuit
{
    public class CircuitManagerAudio : MonoBehaviour
    {
        [Header("Set Values")]
        [SerializeField] CircuitManager controller;
        [SerializeField] FMODUnity.EventReference completedAudio;
        [SerializeField] float playAudioDelay;
        [SerializeField] bool onlyOnActiveLocked;
        [Header("Runtime Values")]
        [SerializeField] float delayTimer;
        [SerializeField] bool didAudioPlay;

        //Unity Events
        private void Start()
        {
            if (!controller)
            {
                controller = GetComponent<CircuitManager>();
            }

            if (onlyOnActiveLocked)
            {
                controller.CircuitLocked += OnLocked;
            }
            else
            {
                controller.CircuitCompleted += OnCompleted;
            }
        }

        void Update()
        {
            if(!controller.publicIsComplete) return;
            
            //Wait delay, then play audio
            if (delayTimer > 0)
                delayTimer -= Time.deltaTime;
            else if(!didAudioPlay)
                PlayAudio();
        }

        //Methods
        void PlayAudio()
        {
            didAudioPlay = true;
            FMODUnity.RuntimeManager.PlayOneShot(completedAudio);
        }
        
        //Event Receivers
        void OnLocked()
        {
            delayTimer = playAudioDelay;
            didAudioPlay = false;
        }
        void OnCompleted(bool isComplete)
        {
            if(!isComplete) return;
            
            delayTimer = playAudioDelay;
            didAudioPlay = false;
        }
    }
}