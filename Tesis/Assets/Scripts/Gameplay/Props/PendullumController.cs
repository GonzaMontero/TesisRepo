using UnityEngine;

namespace TimeDistortion.Gameplay.Props
{
    public class PendullumController : MonoBehaviour, ITimed
    {
        [Header("Set Values")]
        [SerializeField] TimePhys.ObjectTimeController timeController;
        [SerializeField] Animator animator;
        [SerializeField] bool affectedByTime = true;

        //Unity Events
        private void Start()
        {
            if (animator == null)
            {
                animator = GetComponent<Animator>();
            }
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