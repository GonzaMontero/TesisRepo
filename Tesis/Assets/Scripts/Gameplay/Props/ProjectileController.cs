using System;
using UnityEngine;

namespace TimeDistortion.Gameplay.Props
{
    public class ProjectileController : MonoBehaviour, IHittable, ITimed
    {
        [Header("Set Values")]
        [SerializeField] TimePhys.ObjectTimeController timeController;
        [SerializeField] float selfDestroyTime;
        [SerializeField] float speed;
        [SerializeField] float slowMoMod = 1;
        [SerializeField] int damage;
        [SerializeField] bool affectedByTime = true;
        [SerializeField] bool useLocalSelfDestroy = false;
        [Header("Runtime Values")]
        [SerializeField] Transform player;
        [SerializeField] float delta;
        [SerializeField] float localTime = 1;
        [SerializeField] float destroyTimer;

        public Action Redirected;
        public Action Destroyed;
        
        //Unity Events
        private void Start()
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
            destroyTimer = selfDestroyTime;
            if (timeController == null)
            {
                timeController = GetComponent<TimePhys.ObjectTimeController>();
            }
        }
        void Update()
        {
            delta = localTime * Time.deltaTime;
            destroyTimer -= delta;
            if (destroyTimer < 0)
            {
                GetDestroyed();
                return;
            }
            Move();
        }
        private void OnCollisionEnter(Collision other)
        {
            IHittable hittable = other.gameObject.GetComponent<IHittable>();
            if (hittable != null)
            {
                hittable.GetHitted(damage);
            }

            if ((!other.transform.root.CompareTag("Player")) && other.transform != transform.parent)
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
            transform.Translate(transform.forward * (speed * delta), Space.World);
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
        public void GetHitted(int notUsed)
        {
            transform.forward = player.transform.forward;
            Redirected?.Invoke();
        }
        public void ChangeTime(float newTime)
        {
            timeController.ChangeTime(newTime);
        }
    }
}