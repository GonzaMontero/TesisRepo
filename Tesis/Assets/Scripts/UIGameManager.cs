using UnityEngine;

namespace TimeDistortion
{
    public class UIGameManager : MonoBehaviour
    {
        GameManager gameManager;

        private void Start()
        {
            gameManager = GameManager.Get();
        }

        public void LoadScene(Universal.SceneManaging.SceneGetter sceneGetter)
        {
            gameManager.LoadScene(sceneGetter.scene, sceneGetter.level);
        }
        public void QuitGame()
        {
            gameManager.QuitGame();
        }
    }
}