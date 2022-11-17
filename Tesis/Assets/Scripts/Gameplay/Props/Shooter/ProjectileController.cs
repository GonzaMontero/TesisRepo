﻿using System;
using UnityEngine;

namespace TimeDistortion.Gameplay.Props
{
    public class ProjectileController : MonoBehaviour, IPushable
    {
        [Header("Set Values")]
        [SerializeField] TimePhys.ObjectTimeController time;
        [SerializeField] BoxCollider coll;
        [SerializeField] LayerMask collisionLayers;
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
            if (coll == null)
            {
                coll = GetComponent<BoxCollider>();
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
            CheckCollisions();
            Move();
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
        void CheckCollisions()
        {
            Vector3 pos = transform.position;
            Vector3 size = coll.bounds.extents;
            Quaternion rot = transform.rotation;

            Collider[] collisions;
            collisions = Physics.OverlapBox(pos, size, rot, collisionLayers);

            bool shouldGetDestoyed = false;
            for (int i = 0; i < collisions.Length; i++)
            {
                if(collisions[i] == coll) continue;
                
                IHittable hittable = collisions[i].gameObject.GetComponent<IHittable>();
                if (hittable != null)
                {
                    bool collidedWithPlayer;
                    collidedWithPlayer = collisions[i].transform.root.CompareTag("Player"); 
                    if (collidedWithPlayer && time.publicTime < 1) return;

                    hittable.GetHitted(damage);
                    shouldGetDestoyed = true;
                    return;
                }

                if (!collisions[i].transform.root.CompareTag("Player"))
                {
                    //Debug.Log("KILL PROJECTILE");
                    shouldGetDestoyed = true;
                }
            }

            if (shouldGetDestoyed)
            {
                GetDestroyed();
            }
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
            time.ChangeTime(1);
            Redirected?.Invoke();
        }
    }
}