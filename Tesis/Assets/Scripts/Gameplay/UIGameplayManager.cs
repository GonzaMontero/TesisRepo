using System;
using TimeDistortion.Gameplay.Handler;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace TimeDistortion.Gameplay
{
    public class UIGameplayManager : MonoBehaviour
    {
        [System.Serializable]
        enum GameplayScreens { inGame, /*pause,*/ defeat, victory }

        [Header("Set Values")]
        [SerializeField] GameplayManager manager;
        [SerializeField] PlayerController player;
        //[SerializeField] GameObject inGameUI;
        //[SerializeField] GameObject pauseUI;
        [SerializeField] GameObject gameOverUI;
        [SerializeField] GameObject healthSpawnTuto;
        [SerializeField] GameObject healthRegen;
        [SerializeField] Image fadeToBlackImage;
        [Tooltip("Seconds needed for the black image to disappear")]
        [SerializeField] float fadeInTime;
        [Tooltip("Seconds needed for the black image to appear")]
        [SerializeField] float fadeOutTime;
        [Tooltip("Seconds needed for game over UI activation")]
        [SerializeField] float gameOverDelay;
        //[SerializeField] float healthSpawnTutoDuration;
        [Header("Runtime Values")]
        [SerializeField] GameplayScreens currentState = GameplayScreens.inGame;
        [SerializeField] GameplayScreens targetState = GameplayScreens.inGame;
        //[SerializeField] float healthSpawnTutoTimer;
        [SerializeField] float delayTimer;
        [SerializeField] float fadeTimer;
        [SerializeField] bool fadingIn;

        //Unity Events
        private void Awake()
        {
            //If manager wasn't asigned, get it
            if (manager == null)
            {
                manager = GameplayManager.Get();
            }

            if (!player)
            {
                player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
            }

            //Link action
            manager.GameStarted += OnGameStarted;
            manager.GameEnded += OnGameEnded;
            //manager.GamePaused += OnPause;
            //manager.PlayerSpawned += OnPlayerSpawned;
            manager.PlayerRegenEnabled += OnPlayerRegenEnabled;
            player.Healing += OnPlayerFirstHealed;
        }

        void Update()
        {
            if (delayTimer > 0)
            {
                delayTimer -= Time.deltaTime;
            }
            else if (currentState != targetState)
            {
                currentState = targetState;

                switch (currentState)
                {
                    case GameplayScreens.defeat:
                        SetGameOver();
                        break;
                    case GameplayScreens.victory:
                        SetGameOver();
                        break;
                    default:
                        break;
                }
            }

            // if (healthSpawnTutoTimer > 0)
            // {
            //     healthSpawnTutoTimer -= Time.deltaTime;
            //     if (!(healthSpawnTutoTimer > 0))
            //     {
            //         healthSpawnTuto.SetActive(false);
            //     }
            // }
            
            if(fadeTimer > 0)
               UpdateFade();
        }
        //private void OnDestroy()
        //{
        //    //Unlink action
        //    manager.GameEnded -= OnGameEnded;
        //    //manager.GamePaused -= OnPause;
        //}

        //Methods
        //public void SetPause(bool pause)
        //{
        //    GameManager.Get().SetPause(pause);

        //    currentState = pause ? GameplayScreens.pause : GameplayScreens.inGame;
        //    SwitchUIStage();
        //}
        void UpdateFade()
        {
            fadeTimer -= Time.deltaTime;

            Color fadeColor = fadeToBlackImage.color;
            
            if(fadingIn)
            {
                fadeColor.a = Mathf.Lerp(0, 1, fadeTimer / fadeInTime);
                if (fadeTimer <= 0)
                {
                    fadeToBlackImage.enabled = false;
                }
            }
            else
            {
                fadeColor.a = Mathf.Lerp(1, 0, fadeTimer / fadeOutTime);
                if (!fadeToBlackImage.enabled)
                    fadeToBlackImage.enabled = true;
            }

            fadeToBlackImage.color = fadeColor;
        }
        void SetGameOver()
        {
            //GameManager.Get().SetPause(true);

            fadingIn = false;
            fadeTimer = fadeOutTime;
            
            SwitchUIStage();
        }
        void SwitchUIStage()
        {
            switch (currentState)
            {
                case GameplayScreens.inGame:
                    //    pauseUI.SetActive(false);
                    //    inGameUI.SetActive(true);
                    break;
                //case GameplayScreens.pause:
                //    inGameUI.SetActive(false);
                //    pauseUI.SetActive(true);
                //    break;
                case GameplayScreens.defeat:
                    //inGameUI.SetActive(false);
                    gameOverUI.SetActive(true);
                    break;
                case GameplayScreens.victory:
                    gameOverUI.SetActive(false);
                    //inGameUI.SetActive(false);
                    break;
                default:
                    break;
            }
        }

        //Event receivers
        void OnGameStarted()
        {
            fadingIn = true;
            fadeTimer = fadeInTime;
            UpdateFade();
        }
        void OnGameEnded(bool playerWon)
        {
            delayTimer = playerWon ? 0 : gameOverDelay;
            targetState = playerWon ? GameplayScreens.victory : GameplayScreens.defeat;
        }
        // void OnPlayerSpawned()
        // {
        //     
        // }
        //void OnPause()
        //{
        //    SetPause(manager.publicPause);
        //}
        void OnPlayerRegenEnabled(bool firstRegen)
        {
            healthRegen.SetActive(true);
            healthSpawnTuto.SetActive(firstRegen);
        }
        void OnPlayerFirstHealed(bool healing)
        {
            if(!healing) return;
            healthSpawnTuto.SetActive(false);
            player.Healing -= OnPlayerFirstHealed;
        }
    }
}