using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TimeDistortion.Gameplay.Props
{
    public class HazardController : MonoBehaviour
    {
        [SerializeField] int damage;

        public System.Action<Collision> HittedSomething;
        
        private void OnCollisionStay(Collision collision)
        {
            IHittable hittedObject = collision.gameObject.GetComponent<IHittable>();
            
            if (hittedObject == null) return;
            
            hittedObject.GetHitted(damage);
            HittedSomething?.Invoke(collision);
        }
    }
}