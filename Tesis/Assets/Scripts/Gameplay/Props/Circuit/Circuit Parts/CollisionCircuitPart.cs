using System;
using UnityEngine;

namespace TimeDistortion.Gameplay.Props.Circuit
{
    public class CollisionCircuitPart : CircuitPartController
    {
        [System.Serializable] enum TriggerModes { AllTriggering, AnyTriggering, NoneTriggering }

        [System.Serializable]
        struct CollisionCheck
        {
            public Vector3 posOffset;
            public Vector3 radius;
        }
        
        [Header("Child Values", order = -1)]
        [Header("Set Values")]
        [SerializeField] CollisionCheck[] checkBoxes;
        [SerializeField] TriggerModes mode;
        [SerializeField] LayerMask layers;
        [Header("DEBUG")]
        [SerializeField] bool gizmosShouldDraw = true;

        //Unity Events
        void Update()
        {
            CheckActivation();
        }
        // private void OnTriggerEnter(Collider other)
        // {
        //     CheckActivation(other.gameObject);
        //      if (!(layers == (layers | (1 << triggerObject.layer)))) return; ///THIS WAS IN CHECKACTIVATION
        // }
#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            if(!gizmosShouldDraw) return;
            
            Gizmos.color = Color.yellow;

            Vector3 pos;
            
            //Draw all check boxes
            for (int i = 0; i < checkBoxes.Length; i++)
            {
                pos = transform.position + checkBoxes[i].posOffset;
                Gizmos.DrawCube(pos, checkBoxes[i].radius * 2);
            }
        }
#endif

        //Methods
        void CheckActivation()
        {
            if(active && !canDeactivate) return;
            
            bool wasActivated = active;

            //Only "Any Triggering" mode should start with active false
            active = mode != TriggerModes.AnyTriggering;

            bool isBoxTriggered;
            for (int i = 0; i < checkBoxes.Length; i++)
            {
                isBoxTriggered = IsBoxTriggered(checkBoxes[i]);

                //If a box was triggered, activate circuit part and stop loop
                if (mode == TriggerModes.AnyTriggering)
                {
                    if (isBoxTriggered)
                    {
                        active = true;
                        break;
                    }
                }
                else if(active)
                {
                    active = isBoxTriggered == (mode == TriggerModes.AllTriggering);
                }
            }
            
            //Only call event in activation changed state
            if(active == wasActivated) return;
            Activated?.Invoke(this);
        }
        bool IsBoxTriggered(CollisionCheck checkBox)
        {
            Vector3 pos = checkBox.posOffset + transform.position;
            Quaternion rot = transform.rotation;
            
            Collider[] collisions;
            collisions = Physics.OverlapBox(pos, checkBox.radius, rot, layers);

            return collisions.Length > 0;
            
            ///DEPRECATED / CHECK LATER
            //If in "No Collisions" mode, return true when there was no collisions
            if(mode == TriggerModes.NoneTriggering) return collisions.Length < 1;
            
            //Else return true when there was at least one collision
            return collisions.Length > 0;
        }
    }
}