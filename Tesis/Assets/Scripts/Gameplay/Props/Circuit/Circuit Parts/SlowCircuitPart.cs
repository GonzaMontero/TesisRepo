using UnityEngine;

namespace TimeDistortion.Gameplay.Props.Circuit
{
    public class SlowCircuitPart : CircuitPartController
    {
        [Header("Set Values")]
        [SerializeField] TimePhys.ObjectTimeController objTime;
        [SerializeField] bool activeOnSlowMo;
        //[Header("Runtime Values")]


        //Unity Events
        void Start()
        {
            if (!objTime)
            {
                objTime = GetComponent<TimePhys.ObjectTimeController>();
            }
            
            objTime.TimeChanged += OnTimeChanged;
        }

        //Event Receivers
        void OnTimeChanged()
        {
            active = (objTime.publicTime == 1) != activeOnSlowMo;
            Activated?.Invoke(this);
        }
    }
}