using System;
using UnityEngine;

namespace TimeDistortion.Gameplay.Characters
{
    public class PlayerController : MonoBehaviour, IHittable
    {
        [Header("Set Values")]
        [SerializeField] CharacterData data;
        //[Header("Runtime Values")]
        //[SerializeField] Stats currentStats;

        public Action Hitted;
        public Action Died;

        public CharacterData publicData { get { return data; } }

        //Unity Events
        private void Awake()
        {
            data.currentStats = data.baseStats;
        }

        //Methods

        //Interface Implementations
        public void GetHitted(int damage)
        {
            data.currentStats.health -= damage;
            Hitted?.Invoke();

            if (data.currentStats.health > 0) return;
            data.currentStats.health = 0;
            //Died?.Invoke();
        }
    }
}