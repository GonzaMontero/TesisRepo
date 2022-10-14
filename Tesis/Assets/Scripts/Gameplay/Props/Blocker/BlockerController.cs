using UnityEngine;

namespace TimeDistortion.Gameplay.Props
{
    public class BlockerController : MonoBehaviour, IHittable
    {
        [Header("Set Values")] 
        [SerializeField] Rigidbody[] stones;
        [SerializeField] float explosionImpulse;
        [SerializeField] float destroyTime;
        [Header("Runtime Values")] 
        [SerializeField] float timer;
        [SerializeField] bool destroying;
        [SerializeField] bool destroyed;

        public System.Action Hitted;
        public System.Action Destroyed;

        //Unity Events
        void Update()
        {
            if (destroyed) return;
            if (!destroying) return;
            
            timer -= Time.deltaTime;
            if (timer < 0)
            {
                DestroyRock();
            }
        }
        
        //Methods
        void DestroyRock()
        {
            foreach (var stone in stones)
            {
                stone.isKinematic = false;
                stone.AddExplosionForce(explosionImpulse, transform.position, 5f);
            }
            
            Destroyed?.Invoke();
            destroyed = true;
        }
        
        //Interface implementations
        public void GetHitted(int damage)
        {
            if (destroyed) return;
            Hitted?.Invoke();
            timer = destroyTime;
            destroying = true;
        }
    }
}