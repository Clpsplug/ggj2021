using Cysharp.Threading.Tasks;
using Domain;
using EntryPoints.SceneData;
using UniSwitcher;
using UnityEngine;

namespace Controllers
{
    public class TitleSceneController : Switcher
    {
        [SerializeField] private AudioSource audioSource;
        
        // Update is called once per frame
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                FadeAudio().Forget(Debug.LogException);
                PerformSceneTransition(
                    ChangeScene(Scene.Main, new PlaySceneData(0))
                        .WithTransitionEffect()
                ).Forget(Debug.LogException);
                enabled = false;
            }
        }

        private async UniTask FadeAudio()
        {
            while (audioSource.volume > 0)
            {
                audioSource.volume -= 0.05f;
                await UniTask.Yield();
            }
        }
    }
}