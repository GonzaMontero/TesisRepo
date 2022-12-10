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
        [SerializeField] Props.InteractableController regenSpawn;
        [SerializeField] float playerSpawnTime;

        [Header("Runtime Values")]
        [SerializeField] float playerSpawnTimer;
        [SerializeField] bool gameOver = false;
        [SerializeField] bool playerSpawned = false;
        [SerializeField] PlayerRespawn playerRespawn;

        public Action<bool> GameEnded; //bool = playerWon
        //public Action PlayerSpawned;
        public Action PlayerRegenEnabled;

        //Unity Events
        private void Start()
        {
            if (!player)
            {
                player = GameObject.FindGameObjectWithTag("Player").GetComponent<Handler.PlayerController>();
            }

            player.gameObject.SetActive(false);
            playerSpawnTimer = playerSpawnTime;
            playerRespawn = PlayerRespawn.Get();

            player.Died += OnPlayerDied;
            stone.StoneBroke += OnStoneBroke;
            regenSpawn.Interacted += OnRegenPickedUp;
        }

        void Update()
        {
            if (playerSpawnTimer > 0)
            {
                playerSpawnTimer -= Time.deltaTime;
            }
            else if (!playerSpawned)
            {
                player.gameObject.SetActive(true);
                playerSpawned = true;
            }
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
            //if(!gameOver) return;
            playerRespawn.OnRestart();
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
        void OnRegenPickedUp(string notUsed)
        {
            player.EnableRegen();
            PlayerRegenEnabled?.Invoke();
        }
    }
}