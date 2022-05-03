using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Universal.SceneManaging
{
    public class UIASyncSceneLoader : MonoBehaviour
    {
        [SerializeField] Slider loadProgressBar;
        [SerializeField] TextMeshProUGUI sceneToLoadName;
        ASyncSceneLoader sceneLoader;

        //Unity Events
        private void Start()
        {
            //Get variables
            sceneLoader = ASyncSceneLoader.Get();

            SetLoadName();
        }
        private void Update()
        {
            //Update load bar
            //if (loadProgressBar == null) return;
            //loadProgressBar.value = sceneLoader.loadingProgress;
        }

        //Methods
        void SetLoadName()
        {
            string sceneName = "ERROR";

            if (sceneLoader.sceneLoading != "")
            {
                sceneName = sceneLoader.sceneLoading;
            }

            sceneToLoadName.text = "Loading " + sceneName + "...";
        }
    }
}