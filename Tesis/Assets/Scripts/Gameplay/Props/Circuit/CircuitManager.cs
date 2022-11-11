using System;
using System.Collections.Generic;
using UnityEngine;

namespace TimeDistortion.Gameplay.Props.Circuit
{
    public class CircuitManager : MonoBehaviour
    {
        [Serializable] enum ActivationModes { AllActive, AnyActive }

        [Header("Set Values")]
        [SerializeField] List<CircuitPartController> circuitParts;
        [SerializeField] ActivationModes mode;
        [SerializeField] bool canDeactivate = true;
        [Header("Runtime Values")]
        [SerializeField] List<CircuitPartController> activeCircuitParts;
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

            //If list update failed, exit
            if(!UpdatePartList(part)) return;
            
            bool wasComplete = isComplete;
            switch (mode)
            {
                case ActivationModes.AllActive:
                    isComplete = activeCircuitParts.Count >= circuitParts.Count;
                    break;
                case ActivationModes.AnyActive:
                    isComplete = activeCircuitParts.Count > 0;
                    break;
            }

            if (wasComplete == isComplete) return;
            CircuitCompleted?.Invoke(isComplete);
        }
        bool UpdatePartList(CircuitPartController part)
        {
            bool partWasInList = activeCircuitParts.Contains(part);
                    
            //If part is active, but it was already in list, exit
            if (part.active)
            {
                if (partWasInList) return false;
                activeCircuitParts.Add(part);
            }
            else if (partWasInList)
            {
                activeCircuitParts.Remove(part);
            }

            return true;
        }

        //Event Receivers
        void OnCircuitPartActivated(CircuitPartController part)
        {
            CheckPartActivation(part);
        }
    }
}