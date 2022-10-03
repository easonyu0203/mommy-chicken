using System;
using System.Collections;
using MommyChicken.MScene;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MommyChicken.App
{
    /// <summary>
    /// the first thing to run when app start
    /// </summary>
    public class Bootstraper : MonoBehaviour
    {
        private IEnumerator Start()
        {
            Debug.Log($"App start...");
            // Load Game menu
            yield return MSceneManager.Instance.LoadScene(1, true);
            // Debug.Log("Loading first Scene...");
            // yield return LoadFirstSceneAsync();
            // Debug.Log("Menu first Loaded");
        }

        private IEnumerator LoadFirstSceneAsync()
        {
            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive);
            yield return new WaitUntil(() => asyncOperation.isDone);
            SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(1));
        }
    }
}