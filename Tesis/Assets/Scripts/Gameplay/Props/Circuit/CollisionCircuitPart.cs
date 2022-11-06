using System;
using System.Collections.Generic;
using UnityEngine;

namespace TimeDistortion.Gameplay.Props.Circuit
{
    public class CollisionCircuitPart : CircuitPartController
    {
        [Serializable] enum TriggerModes { hitLayers, notHitLayers, hitTags, notHitTags}

        [Header("Child Values", order = -1)]
        [Header("Set Values")]
        [SerializeField] LayerMask layers;
        [SerializeField] TriggerModes mode;
        [SerializeField] List<String> tags;

        //Unity Events
        private void OnTriggerEnter(Collider other)
        {
            CheckActivation(other.gameObject);
        }

        //Methods
        void CheckActivation(GameObject triggerObject)
        {
            switch (mode)
            {
                case TriggerModes.hitLayers:
                    if (!(layers == (layers | (1 << triggerObject.layer)))) return;
                    Activated?.Invoke(this);
                    activated = true;
                    break;
                case TriggerModes.notHitLayers:
                    break;
                case TriggerModes.hitTags:
                    break;
                case TriggerModes.notHitTags:
                    break;
                default:
                    break;
            }
        }
    }
}