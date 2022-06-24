using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace TimeDistortion.Gameplay.Props
{
    public class BreakableStone : MonoBehaviour, IHittable
    {
        [SerializeField] GameObject brokenRock;
        [SerializeField] Transform floorLevel;

        public System.Action StoneBroke;
        public void GetHitted(int damage)
        {
            StoneBroke?.Invoke();
            Instantiate(brokenRock, floorLevel.position, Quaternion.identity, this.transform.parent);
            Destroy(this.gameObject);
        }
    }
}
