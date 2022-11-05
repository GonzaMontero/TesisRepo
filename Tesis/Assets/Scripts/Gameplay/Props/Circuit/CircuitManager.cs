using System;
using System.Collections.Generic;
using UnityEngine;

namespace TimeDistortion.Gameplay.Props.Circuit
{
    public class CircuitManager : MonoBehaviour
    {
        [Serializable] enum TriggerModes { allTriggers }

        [Header("Set Values")]
        [SerializeField] List<CircuitPartController> circuitParts;
        [SerializeField] TriggerModes mode;
        [SerializeField] bool canDeactivate = true;
        [Header("Runtime Values")]
        [SerializeField] List<CircuitPartController> activatedCircuitParts;
        [SerializeField] bool isComplete;
        
        public Action<bool> CircuitCompleted;

        //Unity Events
        private void Start()
        {
            foreach (var part in circuitParts)
            {
                part.Activated += OnCircuitPartActivated;
            }
        }

        //Methods
        void CheckPartActivation(CircuitPartController part)
        {
            if(isComplete && !canDeactivate) return;

            bool wasComplete = isComplete;
            switch (mode)
            {
                case TriggerModes.allTriggers:
                    //If part was already in activated list, exit

                    bool partWasInList = activatedCircuitParts.Contains(part);
                    
                    if (part.activated)
                    {
                        if (partWasInList) return;
                        activatedCircuitParts.Add(part);
                    }
                    else if (partWasInList)
                    {
                        activatedCircuitParts.Remove(part);
                    }
                    
                    isComplete = activatedCircuitParts.Count >= circuitParts.Count;
                    break;
                default:
                    break;
            }

            if (wasComplete == isComplete) return;
            CircuitCompleted?.Invoke(isComplete);
        }

        //Event Receivers
        void OnCircuitPartActivated(CircuitPartController part)
        {
            CheckPartActivation(part);
        }
    }
}