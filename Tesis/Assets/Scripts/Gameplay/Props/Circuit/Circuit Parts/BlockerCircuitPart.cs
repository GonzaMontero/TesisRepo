using UnityEngine;

namespace TimeDistortion.Gameplay.Props.Circuit
{
    public class BlockerCircuitPart : CircuitPartController
    {
        [Header("Set Values")]
        [SerializeField] BlockerController blocker;
        [SerializeField] bool activeOnDestruction = true;
        //[Header("Runtime Values")]


        //Unity Events
        void Start()
        {
            if (!blocker)
            {
                blocker = GetComponent<BlockerController>();
            }
            
            blocker.Destroyed += OnDestruction;
        }

        //Event Receivers
        void OnDestruction()
        {
            active = activeOnDestruction;
            Activated?.Invoke(this);
        }
    }
}