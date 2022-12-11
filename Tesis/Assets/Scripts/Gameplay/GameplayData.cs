using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Universal.Singletons;
using UnityEngine.SceneManagement;

namespace TimeDistortion.Gameplay
{
    public class GameplayData : MonoBehaviourSingletonInScene<GameplayData>
    {
        [SerializeField] List<int> puzzlesCompleted = new List<int>();

        public int checkPoint = -1;
        
        public bool playerHasRegen = false;


        //Methods
        public List<int> GetCompletedPuzzleIndex()
        {
            return puzzlesCompleted;
        }

        public void Restart()
        {
            playerHasRegen = false;
            checkPoint = -1;
            puzzlesCompleted.Clear();
        }
        
        //Event Receivers
        public void OnPuzzleCompleted(int i)
        {
            puzzlesCompleted.Add(i);
            puzzlesCompleted.Sort();
        }
        public void OnRestart()
        {
            Time.timeScale = 1;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
