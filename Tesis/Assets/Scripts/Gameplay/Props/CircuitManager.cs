using System;
using System.Collections.Generic;
using UnityEngine;

namespace TimeDistortion.Gameplay.Props
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
                    if (activatedCircuitParts.Contains(part)) return;
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