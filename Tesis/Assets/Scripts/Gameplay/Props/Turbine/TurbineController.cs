using UnityEngine;

namespace TimeDistortion.Gameplay.Props
{
    public class TurbineController : MonoBehaviour, ITimed
    {
        [Header("Set Values")]
        [SerializeField] TimePhys.ObjectTimeController timeController;
        [SerializeField] TurbineWindArea windArea;
        [SerializeField] Collider coll;
        //[Header("Runtime Values")]

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

        //Event Receivers
        void OnTimeChanged()
        {
            windArea.enabled = !(timeController.slowMoLeft > 0);
            coll.enabled = !(timeController.slowMoLeft > 0);
        }
        
        //Interface implementations
        public void ChangeTime(float newTime)
        {
            timeController.ChangeTime(newTime);
        }
    }
}