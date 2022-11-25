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
        [Tooltip("If time is equal or less to 0, it'll never be locked active")] 
        [SerializeField] float timeToLockActive = -1;
        [SerializeField] bool canDeactivate = true;
        [Header("Runtime Values")]
        [SerializeField] List<CircuitPartController> activeCircuitParts;
        [SerializeField] float lockActiveTimer;
        [SerializeField] bool isComplete;
        [SerializeField] bool activeLocked;

        public Action CircuitLocked;
        public Action<bool> CircuitCompleted;

        public bool publicIsComplete => isComplete;

        //Unity Events
        private void Start()
        {
            foreach (var part in circuitParts)
            {
                part.Activated += OnCircuitPartActivated;
            }
        }

        void Update()
        {
            if (ShouldLockActive())
            {
                LockActivation();
            }
        }

        //Methods
        bool ShouldLockActive()
        {
            if (timeToLockActive <= 0) return false; //It can't be locked active, so exit
            if (!isComplete)
            {
                lockActiveTimer = 0;
                return false; 
            } //It's not active yet, so reset timer & exit
            if(activeLocked) return false; //It's already locked, so exit

            lockActiveTimer += Time.deltaTime;
            return lockActiveTimer >= timeToLockActive;
        }
        void LockActivation()
        {
            activeLocked = true;
            CircuitLocked?.Invoke();
        }
        void CheckPartActivation(CircuitPartController part)
        {
            if(activeLocked) return;
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