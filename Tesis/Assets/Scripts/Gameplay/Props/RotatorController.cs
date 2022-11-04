using System;
using UnityEngine;

namespace TimeDistortion.Gameplay.Props
{
    public class RotatorController : MonoBehaviour, ITimed
    {
        [Header("Set Values")]
        [SerializeField] TimePhys.ObjectTimeController time;
        [SerializeField] Vector3 rotAngle;
        [SerializeField] float rotationSpeed;
        [Header("Runtime Values")]
        [SerializeField] float currentRot;

        public float rotation => currentRot;

        //Unity Events

        //Methods
        public void Rotate()
        {
            float rotValue = rotationSpeed * time.publicTime;
            transform.Rotate(rotAngle.normalized * rotValue);
            currentRot += rotValue;
            
            //If current rot already made a loop, reset counter
            if (currentRot > 360)
            {
                currentRot -= 360;
            }
        }

        public void ChangeTime(float newTime)
        {
            time.ChangeTime(newTime);
        }
    }
}