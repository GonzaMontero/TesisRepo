using UnityEngine;

namespace Universal.Testing
{
    public class Hitter : MonoBehaviour
    {
        [SerializeField] int damage;
        [SerializeField] bool dealDamageNow;
        TimeDistortion.Gameplay.IHittable iHittable;

        //Unity Events
        private void Start()
        {
            if (iHittable == null)
            {
                iHittable = GetComponent<TimeDistortion.Gameplay.IHittable>();
            }
        }
        private void Update()
        {
            if (dealDamageNow)
            {
                dealDamageNow = false;
                iHittable.GetHitted(damage);
                Debug.Log("Hitted " + iHittable + " with " + damage + " raw damage");
            }
        }
    }
}