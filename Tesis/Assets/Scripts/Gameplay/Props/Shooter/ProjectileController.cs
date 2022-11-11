using System;
using UnityEngine;

namespace TimeDistortion.Gameplay.Props
{
    public class ProjectileController : MonoBehaviour, IPushable
    {
        [Header("Set Values")]
        [SerializeField] TimePhys.ObjectTimeController time;
        [SerializeField] float selfDestroyTime;
        [SerializeField] float speed;
        [SerializeField] int damage;
        [SerializeField] bool useLocalSelfDestroy = false;
        [Header("Runtime Values")]
        [SerializeField] float destroyTimer;

        public Action Redirected;
        public Action Destroyed;
        
        //Unity Events
        private void Start()
        {
            destroyTimer = selfDestroyTime;
            if (time == null)
            {
                time = GetComponent<TimePhys.ObjectTimeController>();
            }
        }
        void Update()
        {
            destroyTimer -= time.delta;
            if (destroyTimer < 0)
            {
                GetDestroyed();
                return;
            }
            Move();
        }
        private void OnCollisionEnter(Collision other) //CLEAN LATER?
        {
            IHittable hittable = other.gameObject.GetComponent<IHittable>();
            if (hittable != null)
            {
                hittable.GetHitted(damage);
                if (other.transform.root.CompareTag("Player"))
                {
                    GetDestroyed();
                    return;
                }
            }

            if (!other.transform.root.CompareTag("Player") && other.transform != transform.parent)
            {
                //Debug.Log("KILL PROJECTILE");
                GetDestroyed();
            }

            //Debug.Log("HIT PROJECTILE");
        }

        //Methods
        public void SetSelfDestroyTimer(float newTime)
        {
            if (useLocalSelfDestroy)
                return;
            selfDestroyTime = newTime;
        }
        void Move()
        {
            transform.Translate(transform.forward * (speed * time.delta), Space.World);
        }
        void GetDestroyed()
        {
            Destroyed?.Invoke();
            Destroy(gameObject);
        }

        //Interface Implementations
        public void GetPushed(Vector3 pushDirection)
        {
            transform.forward = pushDirection;
            Redirected?.Invoke();
        }
    }
}