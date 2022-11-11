using UnityEngine;

namespace TimeDistortion.Gameplay.Props.Circuit
{
    public class CircuitPartController : MonoBehaviour
    {
        //[Header("Set Values")]
        //[SerializeField]
        [Header("Runtime Values")]
        [SerializeField] internal bool active;

        public System.Action<CircuitPartController> Activated;
        
        public bool isActive => active;

        //Unity Events

        //Methods
    }
}