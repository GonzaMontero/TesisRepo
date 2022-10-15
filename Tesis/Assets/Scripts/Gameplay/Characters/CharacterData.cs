using UnityEngine;

namespace TimeDistortion.Gameplay.Characters
{
    [System.Serializable]
    public struct CharacterData
    {
        [Header("Set Values")]
        public Stats baseStats;
        [Header("Runtime Values")]
        public Stats currentStats;
    }
}