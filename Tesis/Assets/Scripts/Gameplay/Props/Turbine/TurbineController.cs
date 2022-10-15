using System;
using UnityEngine;

namespace TimeDistortion.Gameplay.Props
{
    public class TurbineController : MonoBehaviour, ITimed
    {
        [Header("Set Values")]
        [SerializeField] TimePhys.ObjectTimeController timeController;
        [SerializeField] TurbineWindArea windArea;
        [SerializeField] Collider coll;
        [SerializeField] int damage;
        //[Header("Runtime Values")]

        public System.Action<bool> Slowed;

        //Unity Events
        private void Start()
        {
            if (windArea == null)
            {
                windArea = GetComponentInChildren<TurbineWindArea>();
            }
            if (timeController == null)
            {
                timeController = GetComponent<TimePhys.ObjectTimeController>();
            }

            timeController.TimeChanged += OnTimeChanged;
        }

        void OnCollisionEnter(Collision collision)
        {
            IHittable hittable = collision.gameObject.GetComponent<IHittable>();
            
            if(hittable == null) return;
            
            hittable.GetHitted(damage);
        }

        //Event Receivers
        void OnTimeChanged()
        {
            bool isSlowMoOn = timeController.slowMoLeft > 0;
            windArea.enabled = !isSlowMoOn;
            coll.enabled = !isSlowMoOn;
            Slowed.Invoke(isSlowMoOn);
        }
        
        //Interface implementations
        public void ChangeTime(float newTime)
        {
            timeController.ChangeTime(newTime);
        }
    }
}