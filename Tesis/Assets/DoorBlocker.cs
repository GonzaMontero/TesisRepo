using System.Collections;
using System.Collections.Generic;
using TimeDistortion.Gameplay.TimePhys;
using UnityEngine;
namespace TimeDistortion.Gameplay.Props
{
    public class DoorBlocker : BlockerController, ITimed
    {
        [SerializeField] ObjectTimeController timeController;
        [SerializeField] Animator doorSpiningAnimator;

        public void ChangeTime(float newTime)
        {
            timeController.ChangeTime(newTime);
        }

        internal override void DestroyRock()
        {
            base.DestroyRock();
            doorSpiningAnimator.enabled = false;
        }

    }
}


