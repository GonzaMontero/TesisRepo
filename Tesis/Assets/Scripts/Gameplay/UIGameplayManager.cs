using UnityEngine;
using TMPro;

namespace TimeDistortion.Gameplay
{
    public class UIGameplayManager : MonoBehaviour
    {
        [System.Serializable]
        enum GameplayScreens { inGame, /*pause,*/ gameOver, resetGame }

        [Header("Set Values")]
        [SerializeField] GameplayManager manager;
        //[SerializeField] GameObject inGameUI;
        //[SerializeField] GameObject pauseUI;
        [SerializeField] GameObject gameOverUI;


        [Header("Runtime Values")]
        [SerializeField] GameplayScreens currentState = GameplayScreens.inGame;

        //Unity Events
        private void Start()
        {
            //If manager wasn't asigned, get it
            if (manager == null)
            {
                manager = GameplayManager.Get();
            }

            //Link action
            manager.GameEnded += OnGameEnded;
            //manager.GamePaused += OnPause;
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
        void SetGameOver()
        {
            //GameManager.Get().SetPause(true);

            currentState = GameplayScreens.gameOver;
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
                case GameplayScreens.gameOver:
                    //inGameUI.SetActive(false);
                    gameOverUI.SetActive(true);
                    break;
                case GameplayScreens.resetGame:
                    gameOverUI.SetActive(false);
                    //inGameUI.SetActive(true);
                    break;
                default:
                    break;
            }
        }

        //Event receivers
        //void OnPause()
        //{
        //    SetPause(manager.publicPause);
        //}
        void OnGameEnded(bool playerWon)
        {
            SetGameOver();
        }
    }
}