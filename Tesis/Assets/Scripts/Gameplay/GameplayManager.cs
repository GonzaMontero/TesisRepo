using System;
using UnityEngine;
using Universal.Singletons;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

namespace TimeDistortion.Gameplay
{
    public class GameplayManager : MonoBehaviourSingletonInScene<GameplayManager>
    {
        [Header("Set Values")]
        [SerializeField] Characters.PlayerController player;
        [SerializeField] Props.BreakableStone stone;
        private bool gameOver = false;

        public Action<bool> GameEnded; //bool = playerWon

        //Unity Events
        private void Start()
        {
            stone.StoneBroke += OnStoneBroke;
            if (!player)
            {
                player = GameObject.FindGameObjectWithTag("Player").GetComponent<Characters.PlayerController>();
            }

            player.Died += OnPlayerDied;
        }
        private void OnDestroy()
        {
            //player.Died -= OnPlayerDied;
        }

        //Methods
        public void GameOver(bool playerWon)
        {
            GameEnded?.Invoke(playerWon);
            gameOver = true;
        }

        public void OnRestartInput(InputAction.CallbackContext context)
        {
            if(!gameOver)
                return;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        //Event Receivers
        void OnPlayerDied()
        {
            //Player lost, so invoke gameover and set "playerWon" to false
            GameOver(false);
        }

        void OnStoneBroke()
        {
            GameOver(true);
        }
    }
}