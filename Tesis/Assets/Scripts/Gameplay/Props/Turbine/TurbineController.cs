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
        [SerializeField] Vector3 passCheckCenter;
        [SerializeField] Vector3 passCheckSize;
        [SerializeField] Vector3 passThroughDistance;
        [SerializeField] bool canBePassedThrough;
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
        void OnCollisionStay(Collision collision)
        {
            if(!canBePassedThrough) return;
            Vector3 checkPos = passCheckCenter;
            Vector3 checkSize = passCheckSize / 2;
            Quaternion checkRot = transform.rotation;
            LayerMask checkLayer = gameObject.layer;
            if(Physics.OverlapBox(checkPos, checkSize, checkRot, checkLayer) != null)
                collision.transform.Translate(passThroughDistance);
        }
#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            if(!canBePassedThrough) return;
            Gizmos.color = Color.yellow;
            
            Vector3 checkPos = passCheckCenter + transform.position;
            Vector3 checkSize = passCheckSize;
            Quaternion checkRot = transform.rotation;
            
            Gizmos.DrawCube(checkPos, checkSize);
            
            Gizmos.color = Color.green;
            
            Gizmos.DrawCube(passThroughDistance + transform.position, Vector3.one);
        }
#endif


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