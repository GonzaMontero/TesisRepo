using UnityEngine;

namespace TimeDistortion.Gameplay.Props.Circuit
{
    public class CircuitPartAudio : MonoBehaviour
    {
        [Header("Set Values")]
        [SerializeField] CircuitPartController controller;
        [SerializeField] FMODUnity.EventReference activateAudio;

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
        void OnActivated(CircuitPartController notUsed)
        {
            FMODUnity.RuntimeManager.PlayOneShot(activateAudio);
        }
    }
}