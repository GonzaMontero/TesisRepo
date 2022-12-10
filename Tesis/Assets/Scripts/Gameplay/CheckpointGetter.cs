using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TimeDistortion.Gameplay.Props.Circuit;

namespace TimeDistortion
{
    public class CheckpointGetter : MonoBehaviour
    {
        [System.Serializable]
        struct CheckPointCircuitPair
        {
            public CircuitPartController collision;
            public Transform checkPoint;
        }

        [Header("Static Sets")]
        [SerializeField] CheckPointCircuitPair[] pairs;
        [SerializeField] List<CircuitManager> circuitManagers;
        public Transform playerTransform;
        

        [Header("Runtime Sets")]
        public PlayerRespawn pr;
        public Transform currentCP;
        Dictionary<CircuitPartController, int> spawnCircuitPair;

        private void Start()
        {
            spawnCircuitPair = new Dictionary<CircuitPartController, int>();

            for (short i = 0; i < pairs.Length; i++)
            {
                if (pairs[i].collision == null || pairs[i].checkPoint == null)
                    continue;

                spawnCircuitPair.Add(pairs[i].collision, i);
                pairs[i].collision.Activated += SetCP;
            }

            for(short i = 0; i < circuitManagers.Count; i++)
            {
                circuitManagers[i].CircuitLocked += OnPuzzleCompleted;

            }

            pr = PlayerRespawn.Get();

            if (!pr || pr.checkPoint < 0)
                return;

            currentCP = pairs[pr.checkPoint].checkPoint;
            OnPlayerRestart();
        }

        public void OnPlayerRestart()
        {
            if (!currentCP)
                return;

            List<int> cm = pr.GetCompletedPuzzleIndex();

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
            pr.checkPoint = i;
        }

        public void OnPuzzleCompleted(CircuitManager manager)
        {
            int i = circuitManagers.IndexOf(manager);
            pr.OnPuzzleCompleted(i);
        }
    }
}