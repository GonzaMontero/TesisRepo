using UnityEngine;

namespace TimeDistortion.Gameplay.Props
{
    public class PendullumController : MonoBehaviour, ITimed
    {
        [Header("Set Values")]
        [SerializeField] TimePhys.ObjectTimeController timeController;

        //Unity Events
        private void Start()
        {
            if (timeController == null)
            {
                timeController = GetComponent<TimePhys.ObjectTimeController>();
            }
        }

        //Methods

        //Interface Implementations
        public void ChangeTime(float newTime)
        {
            timeController.ChangeTime(newTime);
        }
    }
}