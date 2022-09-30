using UnityEngine;

namespace TimeDistortion.Gameplay.TimePhys
{
    public class TimeRayController : MonoBehaviour
    {
        [Header("Set Values")]
        [SerializeField] float timeToTarget;
        [Header("Runtime Values")]
        [SerializeField] Transform target;
        [SerializeField] float speed;
        [SerializeField] float distanceToTarget;


        //Unity Events
        private void Update()
        {
            Move();
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.transform.GetInstanceID() != target.GetInstanceID()) return;
            Destroy(gameObject);
        }

        //Methods
        public void SetRay(Transform source, Transform target)
        {
            transform.position = source.position;
            this.target = target;

            //Calculate speed needed to get to target in time
            distanceToTarget = Vector3.Distance(transform.position, target.position);
            speed = distanceToTarget / timeToTarget;
        }
        void Move()
        {
            Vector3 dir = (target.position - transform.position).normalized;

            transform.Translate(dir * speed * Time.deltaTime);
        }
    }
}