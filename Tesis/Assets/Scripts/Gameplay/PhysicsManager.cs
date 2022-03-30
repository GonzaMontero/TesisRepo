using System.Linq;
using UnityEngine;
using Universal.Singletons;

namespace TimeDistortion.Gameplay.Physic
{
    public class PhysicsManager : MonoBehaviourSingletonInScene<PhysicsManager>
    {
        [Header("Set Values")]
        [SerializeField] float gravityValue;

        public float publicGravity { get { return gravityValue; } }

        //Methods
        public Vector3 GetObjectGravityPull(float mass)
        {
            return gravityValue * mass * Vector3.down;
        }
    }
}