using System;
using UnityEngine;

namespace NAMESPACENAME
{
    public class ProjectileController : MonoBehaviour, IHittable, ITimed
    {
        [Header("Set Values")]
        [SerializeField] float speed;
        [SerializeField] int damage;
        [Header("Runtime Values")]
        [SerializeField] Transform player;
        [SerializeField] bool slowed;

        public Action Destroyed;

        //Unity Events
        private void Start()
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }
        void Update()
        {
            Move();
        }
        private void OnTriggerEnter(Collider other)
        {
            IHittable hittable = other.GetComponent<IHittable>();
            if (hittable != null)
            {
                hittable.GetHitted(damage);
            }

            if (!PlayerIsClose())
            {
                GetDestroyed();
            }
        }

        //Methods
        void Move()
        {
            float delta = (slowed ? Time.deltaTime : Time.unscaledDeltaTime);
            transform.Translate(transform.forward * speed * delta, Space.World);
        }
        void GetDestroyed()
        {
            Destroyed?.Invoke();
            Destroy(gameObject);
        }
        bool PlayerIsClose()
        {
            if (player == null) return false;
            bool playerClose = (player.transform.position - transform.position).magnitude < 3;
            Debug.Log("Player is " + (playerClose ? "Close" : "Far"));
            return playerClose;
        }

        //Interface Implementations
        public void GetHitted(int damage)
        {
            if (!PlayerIsClose())
            {
                GetDestroyed();
            }
            transform.forward = player.forward;
        }

        public void TimeChanged(bool useModifiedTime)
        {
            slowed = useModifiedTime;
        }
    }
}