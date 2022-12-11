using System;
using UnityEngine;

namespace TimeDistortion.Gameplay
{
    public class GameplayAudioManager : MonoBehaviour
    {
        [Header("Set Values")]
        [SerializeField] GameplayManager controller;
        [SerializeField] FMODUnity.EventReference ambienceAudio;
        [SerializeField] FMODUnity.EventReference musicAudio;
        [SerializeField] FMODUnity.EventReference debrisAudio;
        [SerializeField] FMODUnity.EventReference reverb;
        [SerializeField] [Range(0,1)] float musicVolume = 1;
        [SerializeField] [Range(0,1)] float ambienceVolume = 1;
        [Header("EDITOR")]
        [SerializeField] [Tooltip("Press to set volume")] bool setMusicVolume;
        [SerializeField] [Tooltip("Press to set volume")] bool setAmbienceVolume;
        FMOD.Studio.EventInstance ambienceAudioInstance;
        FMOD.Studio.EventInstance musicAudioInstance;


        //Unity Events
        void Start()
        {
            controller.GameStarted += OnGameStarted;
            controller.GameEnded += OnGameEnded;
        }

        void Update()
        {
            if (setMusicVolume)
            {
                UpdateVolume(musicAudioInstance, "Music", musicVolume);
                setMusicVolume = false;
            }
            if (setAmbienceVolume)
            {
                UpdateVolume(ambienceAudioInstance, "Ambience", ambienceVolume);
                setAmbienceVolume = false;    
            }
        }

        void OnDestroy()
        {
            StopAudio(ambienceAudioInstance);
            StopAudio(musicAudioInstance);
        }

        //Methods
        void StartAudio(FMODUnity.EventReference audioEvent, FMOD.Studio.EventInstance audioInstance)
        {
            audioInstance = FMODUnity.RuntimeManager.CreateInstance(audioEvent);
            audioInstance.start();
        }
        void StopAudio(FMOD.Studio.EventInstance audioInstance)
        {
            audioInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            audioInstance.release();
        }
        void UpdateVolume(FMOD.Studio.EventInstance audioInstance, string name, float newVol)
        {
            Debug.Log("Updating " + name + " Volume to: " + newVol);
            audioInstance.setVolume(newVol);
            
            float currentVol;
            audioInstance.getVolume(out currentVol);
            Debug.Log(name + " Volume Updated to: " + currentVol);
        }
        
        //Event Receivers
        void OnGameStarted()
        {
            //Start audios
            StartAudio(ambienceAudio, ambienceAudioInstance);
            StartAudio(musicAudio, musicAudioInstance);
            FMODUnity.RuntimeManager.PlayOneShot(debrisAudio);
            FMODUnity.RuntimeManager.PlayOneShot(reverb);
            
            //Set volumes
            musicAudioInstance.setVolume(musicVolume);
            ambienceAudioInstance.setVolume(ambienceVolume);
        }
        void OnGameEnded(bool playerWon)
        {
            StopAudio(ambienceAudioInstance);
            StopAudio(musicAudioInstance);
        }
    }
}