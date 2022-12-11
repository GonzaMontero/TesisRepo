using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TimeDistortion.Gameplay.Props.Circuit;

namespace TimeDistortion.Gameplay
{
    public class LevelResetter : MonoBehaviour
    {
        [System.Serializable]
        struct CheckPointCircuitPair
        {
            [SerializeField] string name;
            public CircuitPartController collision;
            public Transform checkPoint;
        }

        [Header("Static Sets")]
        [SerializeField] CheckPointCircuitPair[] pairs;
        [SerializeField] List<CircuitManager> circuitManagers;
        [SerializeField] Transform playerTransform;
        [SerializeField] GameObject healthSpawn;
        

        [Header("Runtime Sets")]
        [SerializeField] GameplayData gData;
        [SerializeField] Transform currentCP;
        Dictionary<CircuitPartController, int> spawnCircuitPair;

        private void Start()
        {
            //Init dictionary
            spawnCircuitPair = new Dictionary<CircuitPartController, int>();

            //Link all checkpoints
            for (short i = 0; i < pairs.Length; i++)
            {
                if (pairs[i].collision == null || pairs[i].checkPoint == null)
                    continue;

                spawnCircuitPair.Add(pairs[i].collision, i);
                pairs[i].collision.Activated += SetCP;
            }

            //Link all puzzles
            for(short i = 0; i < circuitManagers.Count; i++)
            {
                circuitManagers[i].CircuitLocked += OnPuzzleCompleted;

            }

            //Get gameplay data
            gData = GameplayData.Get();

            if (gData.playerHasRegen)
            {
                healthSpawn.SetActive(false);
            }
            
            //Check if player has checkpoint
            if (!gData || gData.checkPoint < 0)
                return;

            //Spawn player in checkpoint
            currentCP = pairs[gData.checkPoint].checkPoint;
            OnPlayerRestart();
        }

        public void OnPlayerRestart()
        {
            if (!currentCP)
                return;

            List<int> cm = gData.GetCompletedPuzzleIndex();

            for (short i = 0; i < cm.Count; i++)
            {
                circuitManagers[cm[i]].ForceActive();
            }

            playerTransform.position = currentCP.position;
            playerTransform.rotation = currentCP.rotation;
        }

        public void SetCP(CircuitPartController part)
        {
            int i = 0;
            if (!spawnCircuitPair.TryGetValue(part, out i))
                return;
            gData.checkPoint = i;
        }

        public void OnPuzzleCompleted(CircuitManager manager)
        {
            int i = circuitManagers.IndexOf(manager);
            gData.OnPuzzleCompleted(i);
        }
    }
}