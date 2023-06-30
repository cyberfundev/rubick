using Cysharp.Threading.Tasks;
using Facebook.Unity;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Structure
{
    public class BootstrapperScene : MonoBehaviour
    {
        [SerializeField] private SceneContext _sceneContext;
        [SerializeField] private string _firstSceneName;
        
        private async void Start()
        {
            _sceneContext.Run();

            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(_firstSceneName, LoadSceneMode.Additive);

            while (!asyncOperation.isDone)
            {
                await UniTask.Yield();
            }
            
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(_firstSceneName));
        }
    }
}
