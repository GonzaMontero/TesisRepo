using System.Security.Cryptography;
using UnityEngine;

namespace TimeDistortion.Gameplay.Props
{
    public class InteractableController : MonoBehaviour
    {
        [Header("Set Values")]
        [SerializeField] string message;
        [SerializeField] int maxInteractions;
        [Header("Runtime Values")]
        [SerializeField] int interactions;

        public System.Action<string> Interacted;

        public string interactedMessage { get { return message; } }
        
        //Unity Events

        //Methods
        public void GetInteracted()
        {
            Interacted?.Invoke(message);
            interactions--;
            
            //If obj ran out of interactions and max interactions are not infinite, destroy obj
            if (interactions > 0 || maxInteractions < 1) return;
            Destroy(gameObject);
        }
    }
}