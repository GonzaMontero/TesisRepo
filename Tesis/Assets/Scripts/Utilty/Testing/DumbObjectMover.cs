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
        [SerializeField] bool usingModifiedTime;

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
            float deltaTime = usingModifiedTime ? Time.deltaTime : Time.unscaledDeltaTime;
            transform.Translate(direction.normalized * speed * deltaTime);
        }

        //Interface Implementations
        public void TimeChanged(bool useModifiedTime)
        {
            usingModifiedTime = useModifiedTime;
            //rb.
        }
    }
}