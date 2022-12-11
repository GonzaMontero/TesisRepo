using System.Collections.Generic;
using UnityEngine;

namespace TimeDistortion.Gameplay.Characters
{
    [System.Serializable]
    public class AttackController : MonoBehaviour
    {
        [Header("Set Values")]
        [SerializeField] LayerMask hittableLayers;
        [SerializeField] float radius;
        [SerializeField] float range;
        [SerializeField] float hitImpulse;
        [SerializeField] float attackDuration;
        [SerializeField] float attackStartUp;
        [SerializeField] int damage;
        [Header("Runtime Values")]
        [SerializeField] float attackTimer = -1;
        List<RaycastHit> hittedObjs = new List<RaycastHit>();

        public System.Action HittedSomething;
        public System.Action HittedStone;
        
        public float attackTime => attackDuration + attackStartUp;
        public bool attacking => attackTimer > 0;
        public RaycastHit collision { get; private set; }

        //Unity Events
        void Update()
        {
            if (attackTimer < 0) return;

            attackTimer -= Time.deltaTime;
            
            if (attackTimer > attackDuration) return;

            Attack();
        }
        void OnDrawGizmos()
        {
            Gizmos.color = Color.Lerp(Color.red, Color.yellow, 0.5f);
            //Gizmos.DrawLine(transform.position, transform.position + transform.forward * range);
            
            //Draw custom cube
            Vector3 pos = transform.position;
            Vector3 target = transform.position + transform.forward * range;
            Gizmos.DrawLine(pos + transform.up * radius, target + transform.up * radius);
            Gizmos.DrawLine(pos + -transform.up * radius, target + -transform.up * radius);
            Gizmos.DrawLine(pos + transform.right * radius, target + transform.right * radius);
            Gizmos.DrawLine(pos + -transform.right * radius, target + -transform.right * radius);
            
            // Vector3 pos = transform.position + transform.forward * range / 2;
            // Vector3 size = new Vector3(radius, radius, range);
            // Gizmos.DrawWireCube(pos, size);
        }
        
        //Events
        public void StartAttack()
        {
            attackTimer = attackTime;
            if(hittedObjs.Count > 0) hittedObjs.Clear();
        }
        void Attack()
        {
            RaycastHit[] hits;
            Vector3 pos = transform.position;
            Vector3 dir = transform.forward;
            Vector3 size = new Vector3(radius, radius, radius / 2);
            Quaternion rot = transform.rotation;

            hits = Physics.BoxCastAll(pos, size, dir, rot, range, hittableLayers);
            //hits = Physics.RaycastAll(pos, dir, range, hittableLayers);
            
            if(hits.Length < 1) return;

            for (int i = 0; i < hits.Length; i++)
            {
                //Check if obj was already hitted, if not, add it to hitted list
                if(hittedObjs.Contains(hits[i])) continue;
                hittedObjs.Add(hits[i]);
                
                collision = hits[i];
                
                Hit(hits[i]);
                
                Debug.Log("Hitted!");
            }
        }
        void Hit(RaycastHit hit)
        {
            //If hittable, hit
            IHittable hittable;
            hittable = hit.transform.gameObject.GetComponent<IHittable>();
            if (hittable != null)
            {
                hittable.GetHitted(damage);
                HittedSomething?.Invoke();
            }
            else
            {
                HittedStone?.Invoke();
            }
            
            //If pushable, push (currently used with projectiles)
            IPushable push = hit.transform.gameObject.GetComponent<IPushable>();
            if (push != null)
            {
                push.GetPushed(transform.forward);
            }
            
            //If it has a rigidbody, add an impulse
            Rigidbody rb = hit.transform.gameObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddForce(hitImpulse * transform.forward);
            }
        }
    }
}