using UnityEngine;

namespace TimeDistortion.Gameplay.Props.Circuit
{
    public class CircuitPartAudio : MonoBehaviour
    {
        [Header("Set Values")]
        [SerializeField] CircuitPartController controller;
        [SerializeField] FMODUnity.EventReference activatedAudio;
        [SerializeField] FMODUnity.EventReference deactivatedAudio;

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

            FMODUnity.RuntimeManager.PlayOneShot(audio);
        }
    }
}