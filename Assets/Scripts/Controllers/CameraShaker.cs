using System;
using Cysharp.Threading.Tasks;
using Messaging;
using UniRx;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace Controllers
{
    public class CameraShaker : MonoBehaviour
    {
        [SerializeField] private int shakeFrames = 20;
        [SerializeField] private float magnitude = 0.5f;
        
        [Inject] private IMessageReceiver _messageReceiver;

        private Vector3 _origin;

        private Camera _camera;
        
        public void Awake()
        {
            _camera = Camera.main;
            if (_camera == null)
            {
                return;
            }

            _messageReceiver.Receive<GamePlayMessages.PlayerFailureEvent>()
                .Subscribe(x =>
                {
                    _origin = _camera.transform.position;
                    DoShake().Forget(Debug.LogException);
                }).AddTo(this);
        }

        private async UniTask DoShake()
        {
            var frameCount = 0;
            while (frameCount < shakeFrames)
            {
                if (_camera == null)
                {
                    return;
                }

                _camera.transform.position
                    = _origin
                      + new Vector3(Random.value, Random.value, Random.value) * magnitude;
                
                frameCount++;
                await UniTask.Yield();
            }
        }
    }
}