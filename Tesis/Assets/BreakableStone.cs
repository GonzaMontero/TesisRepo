using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace TimeDistortion.Gameplay.Props
{
    public class BreakableStone : MonoBehaviour, IHittable
    {
        public void GetHitted(int damage)
        { Debug.Log("hIT!"); }
    }
}
