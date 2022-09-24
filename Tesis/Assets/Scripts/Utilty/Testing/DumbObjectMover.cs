using UnityEngine;

namespace TimeDistortion.Testing
{
    public class DumbObjectMover : MonoBehaviour, ITimed
    {
        [Header("Set Values")]
        [SerializeField] Vector3 direction;
        [SerializeField] float speed;
        [Header("Runtime Values")]
        [SerializeField] Rigidbody rb;
        [SerializeField] internal float localTime;

        //Unity Events
        private void Start()
        {
            if (!rb)
            {
                rb = GetComponent<Rigidbody>();
            }
        }
        void Update()
        {
            float deltaTime = localTime * Time.deltaTime;
            transform.Translate(direction.normalized * speed * deltaTime);
        }

        //Interface Implementations
        public void ChangeTime(float newTime)
        {
            localTime = newTime;
            //rb.
        }
    }
}