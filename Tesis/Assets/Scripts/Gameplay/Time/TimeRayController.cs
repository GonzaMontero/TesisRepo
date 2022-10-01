using System.Security.Cryptography;
using UnityEngine;

namespace TimeDistortion.Gameplay.TimePhys
{
    public class TimeRayController : MonoBehaviour
    {
        [Header("Set Values")]
        [SerializeField] float timeToTarget;
        [Header("Runtime Values")]
        [SerializeField] Transform target;
        [SerializeField] Vector3 offset;
        [SerializeField] float speed;
        [SerializeField] float timer;
        [SerializeField] float distanceToTarget;


        //Unity Events
        private void Update()
        {
            Move();
            
            timer -= Time.deltaTime;
            
            if(timer > 0) return;
            Destroy(gameObject);
        }

        //Methods
        public void SetRay(Transform source, Transform target, Vector3 hitPos)
        {
            //Set position and target
            transform.position = source.position;
            this.target = target;
            offset = hitPos - target.position;

            //Set Destroy timer
            timer = timeToTarget;

            //Calculate speed needed to get to target in time
            distanceToTarget = Vector3.Distance(transform.position, hitPos);
            speed = distanceToTarget / timeToTarget;
        }
        void Move()
        {
            Vector3 updatedPos = target.position + offset;
            Vector3 dir = (updatedPos - transform.position).normalized;

            transform.Translate(dir * speed * Time.deltaTime);
        }
    }
}