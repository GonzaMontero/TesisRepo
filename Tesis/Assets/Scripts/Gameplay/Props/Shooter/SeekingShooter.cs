using UnityEngine;

namespace TimeDistortion.Gameplay.Props.Shooter
{
    public class SeekingShooter : ShooterController
    {
        [Header("Set Values")]
        [SerializeField] LookAtController seeker;

        //Unity Events
        internal override void Update()
        {
            if(!seeker) return;
            if(!seeker.hasTarget) return;
            base.Update();
        }

        //Methods
        internal override void SetProjectile(Transform proj)
        {
            base.SetProjectile(proj);
            if(!seeker) return;
            if(!seeker.hasTarget) return;
            
            proj.LookAt(seeker.targetPos);
        }
    }
}