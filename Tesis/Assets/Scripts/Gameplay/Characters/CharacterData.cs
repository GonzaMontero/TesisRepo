using UnityEngine;

namespace TimeDistortion.Gameplay.Characters
{
    [System.Serializable]
    public struct CharacterData
    {
        [Header("Set Values")]
        public Stats baseStats;
        public LayerMask targetLayers;
        public LayerMask obstacleLayers;
        [Tooltip("Seconds between each attack")]
        public float attackSpeed;
        //public float detectRange;
        public float attackRange;
        [Header("Runtime Values")]
        public Stats currentStats;
    }
}