using System;
using UnityEngine;
using Universal.Singletons;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace TimeDistortion.Gameplay
{
    public class GameplayManager : MonoBehaviourSingletonInScene<GameplayManager>
    {
        [Header("Set Values")]
        [SerializeField] Handler.PlayerController player;
        [SerializeField] Props.Circuit.CircuitPartController victoryCircuit;
        [SerializeField] Props.InteractableController regenSpawn;
        [SerializeField] GameObject introGO;
        [SerializeField] GameObject outroGO;
        [SerializeField] float playerSpawnTime;
        [SerializeField] float preOutroTime;

        [Header("Runtime Values")]
        [SerializeField] PlayerRespawn playerRespawn;
        [SerializeField] float playerSpawnTimer;
        [SerializeField] float preOutroTimer;
        [SerializeField] bool gameOver = false;
        [SerializeField] bool playerSpawned = false;
        [SerializeField] bool spawnPlayer = false;

        public Action<bool> GameEnded; //bool = playerWon
        //public Action PlayerSpawned;
        public Action PlayerRegenEnabled;
        public Action GameStarted;

        //Unity Events
        private void Start()
        {
            if (!player)
            {
                player = GameObject.FindGameObjectWithTag("Player").GetComponent<Handler.PlayerController>();
            }

            player.gameObject.SetActive(false);
            playerRespawn = PlayerRespawn.Get();

            playerSpawnTimer = playerSpawnTime;
            if (playerRespawn.checkPoint >= 0)
            {
                introGO.SetActive(false);
                StartGame();
            }

            player.Died += OnPlayerDied;
            victoryCircuit.Activated += OnVictoryCircuitActivated;
            regenSpawn.Interacted += OnRegenPickedUp;
        }
        void Update()
        {
            if (spawnPlayer)
            {
                if (playerSpawnTimer > 0)
                {
                    playerSpawnTimer -= Time.deltaTime;
                }
                else if (!playerSpawned)
                {
                    SpawnPlayer();
                }
            }

            if (gameOver)
            {
                if (preOutroTimer > 0)
                {
                    preOutroTimer -= Time.deltaTime;
                }
                else if(player)
                {
                    outroGO.SetActive(true);
                    Destroy(player.gameObject);
                }
            }
        }

        //Methods
        void StartGame()
        {
            spawnPlayer = true;
            GameStarted?.Invoke();
        }
        void SpawnPlayer()
        {
            player.gameObject.SetActive(true);
            playerSpawned = true;
            introGO.SetActive(false);
        }
        public void GameOver(bool playerWon)
        {
            Time.timeScale = 1;

            GameEnded?.Invoke(playerWon);
            player.enabled = false;
            gameOver = true;

            if (playerWon)
            {
                preOutroTimer = preOutroTime;
            }
        }
        public void OnRestartInput(InputAction.CallbackContext context)
        {
            //if(!gameOver) return;
            playerRespawn.OnRestart();
        }

        //Event Receivers
        public void OnIntroEnded()
        {
            StartGame();
        }
        public void OnOutroEnded()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            playerRespawn.checkPoint = -1;
            
            SceneManager.LoadScene("Intro Menu");
        }
        void OnPlayerDied()
        {
            //Player lost, so invoke gameover and set "playerWon" to false
            GameOver(false);
        }
        void OnVictoryCircuitActivated(Props.Circuit.CircuitPartController part)
        {
            if(!part.active) return;
            
            GameOver(true);
        }
        void OnRegenPickedUp(string notUsed)
        {
            player.EnableRegen();
            PlayerRegenEnabled?.Invoke();
        }
    }
}