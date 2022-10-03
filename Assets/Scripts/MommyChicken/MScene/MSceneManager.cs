using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Utility;

namespace MommyChicken.MScene
{
    public class MSceneManager : Utility.Singleton<MSceneManager>
    {
        public int LoadingProgressPercentage { get; private set; } = 0;
        public Action ReadyToUnloadLoadingSceneAction;
        public UnityEvent CanUnloadLoadingScene;

        public IEnumerator LoadScene(int sceneIndex, bool withLoadingScene)
        {
            LoadingProgressPercentage = 0;
            if (!withLoadingScene)
            {
                // load target scene
                AsyncOperation loadSceneAO = SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Additive);
                yield return new WaitUntil(() => loadSceneAO.isDone);
                // unload current scene
                AsyncOperation unloadCurrentSceneAO = UnloadCurrentSceneAsync();
                if (unloadCurrentSceneAO != null) yield return new WaitUntil(() => unloadCurrentSceneAO.isDone);
                // set target scene active
                SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(sceneIndex));
                yield return null;
            }
            else
            {
                // loading scene
                AsyncOperation loadLoadingSceneAO = SceneManager.LoadSceneAsync("LoadingScene", LoadSceneMode.Additive);
                yield return new WaitUntil(() => loadLoadingSceneAO.isDone);
                // load target scene & unload current scene
                AsyncOperation loadTargetSceneAO = SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Additive);
                loadTargetSceneAO.allowSceneActivation = false;
                UnloadCurrentSceneAsync();

                // track progress
                Debug.Log("loading scene...");
                while (loadTargetSceneAO.isDone != true)
                {
                    // track progress
                    LoadingProgressPercentage = Mathf.CeilToInt(loadTargetSceneAO.progress * 100);

                    // check finish
                    if (loadTargetSceneAO.progress >= 0.9f)
                    {
                        LoadingProgressPercentage = 100;

                        // can activate target scene
                        loadTargetSceneAO.allowSceneActivation = true;
                    }

                    yield return null;
                }

                Debug.Log("scene loaded");

                // activate target scene
                SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(sceneIndex));

                // unload loading scene
                if (ReadyToUnloadLoadingSceneAction.GetInvocationList().Length == 0)
                {
                    Debug.LogError("No object handle unload loading Scene");
                }
                
                ReadyToUnloadLoadingSceneAction.Invoke();
                yield return UltiFunc.WaitUntilEvent(CanUnloadLoadingScene);
                SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName("LoadingScene"));
            }
        }

        private AsyncOperation UnloadCurrentSceneAsync()
        {
            if (SceneManager.GetActiveScene() != SceneManager.GetSceneByBuildIndex(0))
            {
                AsyncOperation unLoadSceneAO = SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
                return unLoadSceneAO;
            }

            return null;
        }
    }
}