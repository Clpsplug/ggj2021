using Messaging;
using UniRx;
using UnityEngine;
using Zenject;

namespace Controllers
{
    public class LevelController : MonoBehaviour
    {
        private int _dataBitCount = 0;

        [Inject] private IMessageReceiver _messageReceiver;
        [Inject] private IMessagePublisher _messagePublisher;

        private void Awake()
        {
            var crates = FindObjectsOfType<DataBitController>();
            _dataBitCount = crates.Length;
        }

        private void Start()
        {
            // Do NOT move this to awake. We're not sure in what order Awake()s are called among GameObjects.
            _messagePublisher.Publish(new GamePlayMessages.ReportBitCountEvent(_dataBitCount));
        }
    }
}