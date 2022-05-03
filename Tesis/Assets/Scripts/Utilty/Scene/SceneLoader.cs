using UnityEngine.SceneManagement;

namespace Universal.SceneManaging
{
    public static class SceneLoader
    {
        public static Scenes GetCurrentScene()
        {
            Scene currentScene = SceneManager.GetActiveScene();
            if (currentScene.name.Contains("Gameplay"))
            {
                return Scenes.gameplay;
            }
            else if (currentScene.name.Contains("Menu"))
            {
                return Scenes.menu;
            }
            else if (currentScene.name.Contains("Highscores"))
            {
                return Scenes.highscores;
            }
            else if (currentScene.name.Contains("Credits"))
            {
                return Scenes.credits;
            }
            else
            {
                return Scenes.menu;
            }
        }
        public static void LoadScene(Scenes sceneToLoad, int level = -1)
        {
            string sceneName = "ERROR";

            switch (sceneToLoad)
            {
                case Scenes.proto:
                    sceneName = SceneManager.GetActiveScene().name;
                    break;
                case Scenes.gameplay:
                    sceneName = LoadLevel(level);
                    break;
                case Scenes.menu:
                    sceneName = "Menu";
                    break;
                case Scenes.highscores:
                    sceneName = "Highscores";
                    break;
                case Scenes.credits:
                    sceneName = "Credits";
                    break;
                default:
                    break;
            }

            ASyncSceneLoader.Get().StartLoad(sceneName);
        }
        static string LoadLevel(int level)
        {
            if (level < 0)
            {
                return "ERROR";
            }
            return "Gameplay Level " + level;
        }
    }
}