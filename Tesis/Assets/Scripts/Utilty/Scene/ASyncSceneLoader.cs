using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Universal.SceneManaging
{
    public class ASyncSceneLoader : Singletons.MonoBehaviourSingleton<ASyncSceneLoader>
    {
        public float loadingProgress { get { return asyncLoad.progress; } }
        public bool sceneIsLoading { get { return !loadIsDone; } }
        public string sceneLoading { get; private set; }
        [SerializeField] float minLoadSeconds;
        AsyncOperation asyncLoad;
        bool loadIsDone;

        //Unity Events
        public void StartLoad(string sceneToLoad)
        {
            sceneLoading = sceneToLoad;
            SceneManager.LoadScene("Load Scene");
            StartCoroutine(LoadAsyncScene());
        }

        //Methods
        IEnumerator LoadAsyncScene()
        {
            //The Application loads the Scene in the background as the current Scene runs.
            //asyncLoad = SceneManager.LoadSceneAsync(sceneLoading, LoadSceneMode.Additive);
            //asyncLoad.allowSceneActivation = false;

            //Set timer
            float timer = minLoadSeconds;

            // Wait until the asynchronous scene fully loads
            do
            {
                timer -= Time.deltaTime;
                yield return null;
            } while (/*asyncLoad.progress < 0.9f || */timer > 0);


            //asyncLoad.allowSceneActivation = true;
            //SceneManager.UnloadSceneAsync("Load Scene");
            SceneManager.LoadScene(sceneLoading);
            yield break;
        }
    }
}