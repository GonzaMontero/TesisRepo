using UnityEngine;

namespace NAMESPACENAME
{
    public class Hitter : MonoBehaviour
    {
        [SerializeField] int damage;
        [SerializeField] bool dealDamageNow;
        IHittable iHittable;

        //Unity Events
        private void Start()
        {
            if (iHittable == null)
            {
                iHittable = GetComponent<IHittable>();
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