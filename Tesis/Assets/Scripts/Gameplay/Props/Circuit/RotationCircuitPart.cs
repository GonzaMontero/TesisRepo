using System;
using UnityEngine;

namespace TimeDistortion.Gameplay.Props.Circuit
{
    public class RotationCircuitPart : CircuitPartController
    {
        [System.Serializable] enum TriggerModes
        {
            allToTargetRotation,
            allSameRotation
        }

        [Header("Child Values", order = -1)]
        [Header("Set Values")]
        [SerializeField] RotatorController[] rotators;
        [SerializeField] TriggerModes triggerMode;
        [SerializeField] float[] rotations = new float [1];
        [Tooltip("On 'Same Rotation' mode, target step will be defined on runtime")]
        [SerializeField] int[] targetSteps = new int [1];
        [Header("Runtime Values")]
        [SerializeField] int[] rotatorsStep;
        [Header("DEBUG")]
        [SerializeField] Vector3 gizmoRotationAxis;
        [SerializeField] float gizmoRotRayLength = 1;


        //Unity Events
        void Start()
        {
            rotatorsStep = new int[rotators.Length];
        }
        void Update()
        {
            RotateAll();
            CheckTrigger();
        }

#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            Quaternion rotation = Quaternion.identity;
            Vector3 euler = Vector3.zero;
            Vector3 pos = transform.position;
            Vector3 offset = Vector3.zero;
            
            //Draw Steps
            Gizmos.color = Color.yellow;
            for (int i = 0; i < rotations.Length; i++)
            {
                rotation.eulerAngles = rotations[i] * gizmoRotationAxis;
                offset = rotation * Vector3.forward;
                offset *= gizmoRotRayLength;
                Gizmos.DrawLine(pos, pos + offset);
            }

            //Draw target step
            Gizmos.color = Color.green;
            for (int i = 0; i < targetSteps.Length; i++)
            {
                euler.y = rotations[targetSteps[i]];
                rotation.eulerAngles = euler.y * gizmoRotationAxis;
                offset = rotation * transform.forward * gizmoRotRayLength;
            
                Gizmos.DrawLine(pos, pos + offset);
            }
        }
#endif
        

        //Methods
        void RotateAll()
        {
            for (int i = 0; i < rotators.Length; i++)
            {
                //Rotate
                rotators[i].Rotate();

                //Update Rotator Step
                rotatorsStep[i] = GetRotationStep(rotatorsStep[i], rotators[i].rotation);
            }
        }
        int GetRotationStep(int step, float rotation)
        {
            //if there's only one step, return only it
            if (rotations.Length == 1) return 0;
            
            //if step is beyond array, set to other side of array
            if (step < 0) step = rotations.Length - 1;
            if (step+1 > rotations.Length) step = 0;
            
            //Check if current step is at edge of array
            bool isRotFirst = step < 1;
            bool isRotLast = step + 2 > rotations.Length;

            //If step is 0, get last step as previous step
            float prevRot;
            prevRot = isRotFirst ? rotations[rotations.Length - 1] : rotations[step - 1];

            //If step is last step, get step 0 as next step
            float nextRot;
            nextRot = isRotLast ? rotations[0] + 360 : rotations[step + 1];

            float stepRot = rotations[step];

            //Calculate distance to midpoint between prev step and current step
            float diffWithPrev;
            diffWithPrev = isRotFirst ? prevRot - (stepRot + 360) : prevRot - stepRot;
            diffWithPrev /= 2;

            //Calculate distance to midpoint between next step and current step
            float diffWithNext;
            //diffWithNext = isRotLast ? nextRot - (stepRot-360) : nextRot - stepRot;
            diffWithNext = nextRot - stepRot;
            diffWithNext /= 2;
            
            bool isRotAfterPrev;
            bool isRotBeforeNext;

            //THIS WORKS, BUT IS UGLY AS FUCK
            if (isRotFirst && rotation > rotations[rotations.Length - 1])
            {
                rotation -= 360;
            }

            //in a nutshell, this only breaks when it's on step 0, but still rot > 300
            isRotAfterPrev = rotation >= stepRot + diffWithPrev;
            isRotBeforeNext = rotation <= stepRot + diffWithNext;
            
            //if rotation is inside limits, return current step
            if (isRotAfterPrev && isRotBeforeNext) return step;
            
            //If rotation is before previous step rotation, check with the previous step
            else if (!isRotAfterPrev) return GetRotationStep(step - 1, rotation);
            
            //If rotation is after next step rotation, check with the next step
            else return GetRotationStep(step + 1, rotation);
        }
        void CheckTrigger()
        {
            bool isTrigger = true;

            if (triggerMode == TriggerModes.allSameRotation)
            {
                targetSteps[0] = rotatorsStep[0];
            }
            
            //THIS IS UGLY, CLEAN
            for (int i = 0; i < targetSteps.Length; i++)
            {
                //If only one rotator is not in target, trigger = false & go to next target step
                for (int j = 0; j < rotatorsStep.Length; j++)
                {
                    if (rotatorsStep[j] != targetSteps[i])
                    {
                        isTrigger = false;
                        
                        break;
                    }
            
                    isTrigger = true;
                }
                
                if(isTrigger) break;
            }

            if (isTrigger == activated) return;
            
            activated = isTrigger;
            Activated?.Invoke(this); 
        }
    }
}