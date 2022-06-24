using UnityEngine;

namespace TimeDistortion.Gameplay.Props
{
    public class CircuitManagerAudio : MonoBehaviour
    {
        [Header("Set Values")]
        [SerializeField] CircuitManager controller;
        [SerializeField] FMODUnity.EventReference completedAudio;

        //Unity Events
        private void Start()
        {
            if (!controller)
            {
                controller = GetComponent<CircuitManager>();
            }

            controller.CircuitCompleted += OnCompleted;
        }

        //Event Receivers
        void OnCompleted()
        {
            FMODUnity.RuntimeManager.PlayOneShot(completedAudio);
        }
    }
}