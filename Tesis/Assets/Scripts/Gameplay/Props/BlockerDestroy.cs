using UnityEngine;

namespace TimeDistortion.Gameplay.Props
{
    public class BlockerDestroy : MonoBehaviour, IHittable
    {
        [SerializeField] GameObject BlockerDestroyed;
        [SerializeField] float destroyTimer;
        public Animator rockAnimator;
        public float explosionImpulse;
        private bool wasHited;

        public void GetHitted(int damage)
        {
            wasHited = true;
            rockAnimator.SetTrigger("Hited");
        }

        void Update()
        {
            if (!wasHited)
                return;
            destroyTimer -= Time.deltaTime;
            if (destroyTimer < 0)
            {
                DestroyRock();
            }
        }

        void DestroyRock()
        {
            BlockerDestroyed = Instantiate(BlockerDestroyed, transform.position, transform.rotation, transform.parent);
            foreach (Rigidbody child in BlockerDestroyed.transform.GetComponentsInChildren<Rigidbody>())
            {
                child.AddExplosionForce(explosionImpulse, BlockerDestroyed.transform.position, 5f);
            }

            Destroy(gameObject);
        }
    }
}