using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace TimeDistortion.Gameplay.Props
{
    public class BreakableStone : MonoBehaviour, IHittable
    {
        [SerializeField] GameObject brokenRock;
        [SerializeField] Transform floorLevel;

        public void GetHitted(int damage)
        {
            Instantiate(brokenRock, floorLevel.position, Quaternion.identity, this.transform.parent);
            Destroy(this.gameObject);
        }
    }
}
