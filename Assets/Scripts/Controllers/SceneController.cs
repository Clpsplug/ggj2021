using Cysharp.Threading.Tasks;
using Domain;
using EntryPoints.SceneData;
using Messaging;
using UniRx;
using UniSwitcher;
using UnityEngine;
using Zenject;

namespace Controllers
{
    public class SceneController : Switcher
    {
        [SerializeField] private SceneUIController uiController;
        [SerializeField] private AudioSource bgmSource;

        private int _bitCount = 0;
        private int _collectedBitCount = 0;

        [Inject] private IMessageReceiver _messageReceiver;
        [Inject] private IMessagePublisher _messagePublisher;

        private int _currentLevelIndex;
        private bool _completionReported;
        private Scene _currentLevelScene;

        [InjectOptional] private PlaySceneData _data;

        public void Awake()
        {
            _messageReceiver.Receive<GamePlayMessages.DataBitCollectedEvent>()
                .Subscribe(x => { Debug.Log("Data bit collected!"); })
                .AddTo(this);
            _messageReceiver.Receive<GamePlayMessages.PlayerFailureEvent>()
                .Subscribe(x => { BgmTapeStop().Forget(Debug.LogException); })
                .AddTo(this);
            _messageReceiver.Receive<GamePlayMessages.ReportBitCountEvent>()
                .Subscribe(x => { _bitCount = x.BitCount; })
                .AddTo(this);
            _messageReceiver.Receive<GamePlayMessages.ReportHoldingBitCountEvent>()
                .Subscribe(x =>
                {
                    Debug.Log("Report received");
                    if (x.BitCount != 0)
                    {
                        _messagePublisher.Publish(new GamePlayMessages.DirectoryReceivedEvent());
                    }

                    _collectedBitCount += x.BitCount;
                    uiController.UpdateDataBitCount(_collectedBitCount);
                }).AddTo(this);


            _messageReceiver.Receive<GamePlayMessages.RetryRequestedEvent>()
                .Subscribe(x =>
                {
                    if (_currentLevelIndex >= Scene.Levels.Length)
                    {
                        _currentLevelIndex = 0;
                    }

                    PerformSceneTransition(
                        ChangeScene(Scene.Main, new PlaySceneData(_currentLevelIndex))
                    );
                }).AddTo(this);
        }

        private async UniTask BgmTapeStop()
        {
            while (bgmSource.pitch > 0)
            {
                bgmSource.pitch -= 0.01f;
                await UniTask.Yield();
            }

            bgmSource.Stop();
        }

        private void Start()
        {
            if (_data != null) return;
            _currentLevelIndex = 0;
            PrepareLevel();
        }

        private void Update()
        {
            if (_bitCount != 0 && _bitCount == _collectedBitCount)
            {
                if (!_completionReported)
                {
                    _messagePublisher.Publish(new GamePlayMessages.DirectoryCompletedEvent());
                    _completionReported = true;
                }

                _currentLevelIndex++;
                if (_currentLevelIndex < Scene.Levels.Length)
                {
                    Debug.Log("Level complete!");
                    UnloadScene(_currentLevelScene);
                }
                else
                {
                    _messagePublisher.Publish(new GamePlayMessages.GameCompletedEvent());
                    Debug.Log("Game Complete!");
                    return;
                }

                PrepareLevel();
            }
        }

        public void Init()
        {
            _currentLevelIndex = _data.Level;
            PrepareLevel();
        }

        private void PrepareLevel()
        {
            _bitCount = 0;
            _collectedBitCount = 0;
            _currentLevelScene = Scene.Levels[_currentLevelIndex];
            PerformSceneTransition(
                AddScene(_currentLevelScene)
            );
            uiController.UpdateLevel(_currentLevelIndex);
            uiController.UpdateDataBitCount(0);
            _completionReported = false;
        }
    }
}