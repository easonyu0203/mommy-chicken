using UnityEngine;
using UnityEngine.SceneManagement;

namespace MommyChicken.MScene
{
    public class AnimationEventFuncCaller : MonoBehaviour
    {
        [SerializeField] private LoadingSceneController _loadingSceneController;
        public void UnloadAnimationFinish()
        {
            _loadingSceneController.UnloadAnimationFinish();
        } 
    }
}