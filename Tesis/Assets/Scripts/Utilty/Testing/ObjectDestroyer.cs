using UnityEngine;

namespace Universal.Testing
{
    public class ObjectDestroyer : MonoBehaviour
    {
        [Header("Set Values")]
        [SerializeField] float destroyTime;
        [Header("Runtime Values")]
        [SerializeField] float timer;

        //Unity Events
        private void Start()
        {
            timer = destroyTime;
        }
        private void Update()
        {
            timer -= Time.deltaTime;
            if (timer<0)
            {
                Destroy(gameObject);
            }
        }
    }
}