using UnityEngine;

namespace TimeDistortion.Gameplay.Props
{
    public class ProjectileModel : MonoBehaviour
    {
        [Header("Set Values")]
        [SerializeField] ProjectileController controller;
        [SerializeField] GameObject impactVFXPrefab;
        [SerializeField] float impactVFXTimer;
        [Header("Set Values")]
        [SerializeField] float timer;

        //Unity Events
        private void Start()
        {
            if (!controller)
            {
                controller = GetComponent<ProjectileController>();
            }

            controller.Redirected += OnRedirected;
            controller.HittedSomething += OnRedirected;
        }
        private void Update()
        {
            if (timer > 0)
            {
                timer -= Time.deltaTime;
            }
        }

        //Event Receivers
        void OnRedirected()
        {
            if (timer > 0) return;
            Instantiate(impactVFXPrefab).transform.position = transform.position;
            timer = impactVFXTimer;
        }
    }
}