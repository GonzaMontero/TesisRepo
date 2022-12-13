using UnityEngine;

namespace TimeDistortion.Gameplay.Props.Shooter
{
    public class ShooterAudio : MonoBehaviour
    {
        [Header("Set Values")]
        [SerializeField] ShooterController controller;
        [SerializeField] FMODUnity.EventReference shootAudio;
        //[Header("Runtime Values")]
        [Header("Optional Values")]
        [Tooltip("Use as event trigger/source")]
        [SerializeField] FMODUnity.StudioEventEmitter source;


        //Unity Events
        void Start()
        {
            controller.Shot += OnShot;
        }

        //Event Receivers
        void OnShot()
        {
            if (source)
            {
                source.gameObject.SetActive(false);
                source.EventReference = shootAudio;
                source.gameObject.SetActive(true);
            }
            else
            {
                FMODUnity.RuntimeManager.PlayOneShot(shootAudio);
            }
        }
    }
}