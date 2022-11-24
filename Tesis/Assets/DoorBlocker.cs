using System.Collections;
using System.Collections.Generic;
using TimeDistortion.Gameplay.TimePhys;
using UnityEngine;
namespace TimeDistortion.Gameplay.Props
{
    public class DoorBlocker : BlockerController
    {
        [SerializeField] Animator doorSpiningAnimator;

        internal override void DestroyRock()
        {
            base.DestroyRock();
            doorSpiningAnimator.enabled = false;
        }

    }
}


