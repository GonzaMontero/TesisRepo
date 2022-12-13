using UnityEngine;

namespace TimeDistortion.Gameplay.Props.Circuit
{
    public class CircuitPartAudio : MonoBehaviour
    {
        [Header("Set Values")]
        [SerializeField] CircuitPartController controller;
        [SerializeField] FMODUnity.EventReference activatedAudio;
        [SerializeField] FMODUnity.EventReference deactivatedAudio;
        [Header("Optional Values")]
        [Tooltip("Use as event trigger/source")]
        [SerializeField] FMODUnity.StudioEventEmitter source;

        //Unity Events
        private void Start()
        {
            if (!controller)
            {
                controller = GetComponent<CircuitPartController>();
            }

            controller.Activated += OnActivated;
        }

        //Event Receivers
        void OnActivated(CircuitPartController part)
        {
            FMODUnity.EventReference audio = part.isActive ? activatedAudio : deactivatedAudio;

            if(audio.IsNull) return;

            if (source)
            {
                source.gameObject.SetActive(false);
                source.EventReference = audio;
                source.gameObject.SetActive(true);
            }
            else
            {
                FMODUnity.RuntimeManager.PlayOneShot(audio);
            }
        }
    }
}