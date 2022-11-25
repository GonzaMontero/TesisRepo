using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TimeDistortion.Gameplay.TimePhys {
    public class TimeGetter : MonoBehaviour, ITimed
    {
        [SerializeField] ObjectTimeController timeController;

        public void ChangeTime(float newTime)
        {
            timeController.ChangeTime(newTime);
        }
    }
}


