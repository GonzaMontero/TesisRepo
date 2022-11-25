using UnityEngine;

namespace TimeDistortion.Gameplay.Characters
{
    [System.Serializable]
    public struct CharacterData
    {
        [Header("Set Values")]
        public Stats baseStats;
        [SerializeField] bool setted;
        [Header("Runtime Values")]
        public Stats currentStats;

        public void Set()
        {
            if (setted) return;
            currentStats = baseStats;
            setted = true;
        }
    }
}