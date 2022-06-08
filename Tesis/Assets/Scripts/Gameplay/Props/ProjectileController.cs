using System;
using UnityEngine;

namespace TimeDistortion.Gameplay.Props
{
    public class ProjectileController : MonoBehaviour, IHittable, ITimed
    {
        [Header("Set Values")]
        [SerializeField] float speed;
        [SerializeField] int damage;
        [SerializeField] bool affectedByTime = true;
        [Header("Runtime Values")]
        [SerializeField] Transform player;
        [SerializeField] float speedDelta;
        [SerializeField] float localTime = 1;

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
        private void OnCollisionEnter(Collision other)
        {
            IHittable hittable = other.gameObject.GetComponent<IHittable>();
            if (hittable != null)
            {
                hittable.GetHitted(damage);
            }

            if ((other.transform.root.tag != "Player") && other.transform != transform.parent)
            {
                //Debug.Log("KILL PROJECTILE");
                GetDestroyed();
            }

            //Debug.Log("HIT PROJECTILE");
        }

        //Methods
        void Move()
        {
            float delta = localTime * Time.deltaTime;
            speedDelta = delta;
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
            //if (!PlayerIsClose() && damage > 0)
            //{
            //    GetDestroyed();
            //}
            Debug.Log(transform + "'s old forward " + transform.forward);
            transform.forward = Camera.main.transform.forward;
            Debug.Log(transform + "'s new forward " + transform.forward);
        }

        public void TimeChanged(float newTime)
        {
            if (!affectedByTime) return;
            localTime = newTime;
        }
    }
}