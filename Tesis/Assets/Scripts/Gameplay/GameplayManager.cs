using System;
using UnityEngine;
using Universal.Singletons;

namespace TimeDistortion.Gameplay
{
    public class GameplayManager : MonoBehaviourSingletonInScene<GameplayManager>
    {
        [Header("Set Values")]
        [SerializeField] Characters.PlayerController player;

        public Action<bool> GameEnded; //bool = playerWon

        //Unity Events
        private void Start()
        {
            if (!player)
            {
                player = GameObject.FindGameObjectWithTag("Player").GetComponent<Characters.PlayerController>();
            }

            player.Died += OnPlayerDied;
        }
        private void OnDestroy()
        {
            player.Died -= OnPlayerDied;
        }

        //Methods
        void GameOver(bool playerWon)
        {
            GameEnded?.Invoke(playerWon);
        }

        //Event Receivers
        void OnPlayerDied()
        {
            //Player lost, so invoke gameover and set "playerWon" to false
            GameOver(false);
        }
    }
}