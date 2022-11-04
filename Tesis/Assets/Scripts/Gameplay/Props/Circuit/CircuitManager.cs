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
        [Header("Runtime Values")]
        [SerializeField] List<CircuitPartController> activatedCircuitParts;
        [SerializeField] bool completed;
        
        public Action CircuitCompleted;

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
            switch (mode)
            {
                case TriggerModes.allTriggers:
                    //If part was already in activated list, exit
                    if (activatedCircuitParts.Contains(part))
                    {
                        if (!part.activated)
                        {
                            activatedCircuitParts.Remove(part);
                        }
                        return;
                    }
                    if(!part.activated) return;
                    
                    activatedCircuitParts.Add(part);
                    completed = activatedCircuitParts.Count >= circuitParts.Count;
                    break;
                default:
                    break;
            }

            if (!completed) return;
            CircuitCompleted?.Invoke();
        }

        //Event Receivers
        void OnCircuitPartActivated(CircuitPartController part)
        {
            CheckPartActivation(part);
        }
    }
}