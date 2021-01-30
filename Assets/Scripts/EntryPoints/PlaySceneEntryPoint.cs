using System;
using Controllers;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using EntryPoints.SceneData;
using UniSwitcher.Domain;
using UnityEngine;
using Zenject;

namespace EntryPoints
{
    public class PlaySceneEntryPoint : MonoBehaviour, ISceneEntryPoint
    {
        public SceneController sceneController;
        
        public async UniTask Fire()
        {
            sceneController.Init();
            await UniTask.Delay(TimeSpan.Zero);
        }

        public bool Validate()
        {
            return true;
        }

        public void OnFailure(Exception e)
        {
            Debug.LogException(e);
        }
    }
}