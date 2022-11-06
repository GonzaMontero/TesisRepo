using UnityEngine;

namespace TimeDistortion.Gameplay.Props.Circuit
{
    public class CircuitPartController : MonoBehaviour
    {
        //[Header("Set Values")]
        //[SerializeField]
        [Header("Runtime Values")]
        [SerializeField] internal bool activated;

        public System.Action<CircuitPartController> Activated;
        
        public bool isActive => activated;

        //Unity Events

        //Methods
    }
}