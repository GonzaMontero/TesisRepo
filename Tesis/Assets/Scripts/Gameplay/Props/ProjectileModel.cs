using UnityEngine;

namespace TimeDistortion.Gameplay.Props
{
    public class ProjectileModel : MonoBehaviour
    {
        [Header("Set Values")]
        [SerializeField] ProjectileController controller;
        [SerializeField] GameObject impactVFXPrefab;


        //Unity Events
        private void Start()
        {
            if (!controller)
            {
                controller = GetComponent<ProjectileController>();
            }

            controller.Redirected += OnRedirected;
        }

        //Event Receivers
        void OnRedirected()
        {
            Instantiate(impactVFXPrefab).transform.position = transform.position;            
        }
    }
}