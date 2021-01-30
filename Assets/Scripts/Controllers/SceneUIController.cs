using System;
using System.Collections.Generic;
using System.Linq;
using Messaging;
using TMPro;
using UniRx;
using UnityEngine;
using Zenject;

namespace Controllers
{
    public class SceneUIController : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI levelIndicator;
        [SerializeField] private TextMeshProUGUI dataBitCounter;
        [SerializeField] private TextMeshProUGUI carryingBitCounter;

        [SerializeField] private List<string> levelNames;

        [SerializeField] private FailDialogController failDialogController;

        private int _currentDataBitCount;
        private int _dataBitCount;
        private int _carryingBitCount;
        private bool _isFailDialogActive;

        [Inject] private IMessageReceiver _messageReceiver;
        [Inject] private IMessagePublisher _messagePublisher;

        private void Awake()
        {
            _messageReceiver.Receive<GamePlayMessages.DataBitCollectedEvent>()
                .Subscribe(x =>
                {
                    _carryingBitCount++;
                    UpdateCarryingBitCount(_carryingBitCount);
                });
            
            _messageReceiver.Receive<GamePlayMessages.ReportBitCountEvent>()
                .Subscribe(x =>
                {
                    _dataBitCount = x.BitCount;
                    UpdateDataBitCountImpl();
                });
            _messageReceiver.Receive<GamePlayMessages.ReportHoldingBitCountEvent>()
                .Subscribe(x =>
                {
                    // Because the player would've dropped all the bits
                    _carryingBitCount = 0;
                    UpdateCarryingBitCount(0);
                });
            _messageReceiver.Receive<GamePlayMessages.PlayerFailureEvent>()
                .Subscribe(x =>
                {
                    failDialogController.SpawnFailDialog();
                    _isFailDialogActive = true;
                }).AddTo(this);
        }

        public void UpdateLevel(int level)
        {
            var levelName = levelNames.ElementAtOrDefault(level) ?? "Level name not found";

            levelIndicator.SetText($"Level {level + 1} {levelName}");
        }

        public void UpdateDataBitCount(int count)
        {
            _currentDataBitCount = count;
            UpdateDataBitCountImpl();
        }

        private void UpdateDataBitCountImpl()
        {
            dataBitCounter.SetText($"Data Bits {_currentDataBitCount} / {_dataBitCount}");
        }

        public void UpdateCarryingBitCount(int count)
        {
            carryingBitCounter.SetText($"Carrying: {count}");
        }

        private void Update()
        {
            if (_isFailDialogActive && Input.GetKeyDown(KeyCode.Space))
            {
                _messagePublisher.Publish(new GamePlayMessages.RetryRequestedEvent());
                enabled = false;
            }
        }
    }
}