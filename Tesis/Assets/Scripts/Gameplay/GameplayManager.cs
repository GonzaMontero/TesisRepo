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
        [SerializeField] Handler.PlayerController player;
        [SerializeField] Props.BreakableStone stone;
        [SerializeField] float playerSpawnTime;
        [Header("Set Values")]
        [SerializeField] float playerSpawnTimer;
        private bool gameOver = false;

        public Action<bool> GameEnded; //bool = playerWon
        //public Action PlayerSpawned;

        //Unity Events
        private void Start()
        {
            stone.StoneBroke += OnStoneBroke;
            if (!player)
            {
                player = GameObject.FindGameObjectWithTag("Player").GetComponent<Handler.PlayerController>();
            }

            player.gameObject.SetActive(false);
            
            player.Died += OnPlayerDied;
            playerSpawnTimer = playerSpawnTime;
        }

        void Update()
        {
            if (playerSpawnTimer > 0)
            {
                playerSpawnTimer -= Time.deltaTime;
                return;
            }
            player.gameObject.SetActive(true);
        }

        private void OnDestroy()
        {
            //player.Died -= OnPlayerDied;
        }

        //Methods
        public void GameOver(bool playerWon)
        {
            GameEnded?.Invoke(playerWon);
            player.enabled = false;
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