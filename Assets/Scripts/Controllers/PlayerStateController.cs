using Messaging;
using UniRx;
using UnityEngine;
using UnityStandardAssets._2D;
using Zenject;

namespace Controllers
{
    public class PlayerStateController : MonoBehaviour
    {
        [SerializeField] private Rigidbody2D rb2d;
        [SerializeField] private float bitMassPenalty = 0.05f;
        [SerializeField] private Platformer2DUserControl userControl;
        [SerializeField] private SpriteRenderer sRenderer;
        [SerializeField] private ParticleSystem spark;
        private int _heldDataBitCount = 0;

        [Inject] private IMessageReceiver _messageReceiver;
        [Inject] private IMessagePublisher _messagePublisher;

        private void Awake()
        {
            _messageReceiver.Receive<GamePlayMessages.PlayerFailureEvent>()
                .Subscribe(x =>
                {
                    // Killswitch
                    userControl.enabled = false;
                    sRenderer.color = new Color(1, 1, 1, 0);
                    spark.Play();
                }).AddTo(this);

            _messageReceiver.Receive<GamePlayMessages.DataBitCollectedEvent>()
                .Subscribe(x => { _heldDataBitCount++; }).AddTo(this);

            _messageReceiver.Receive<GamePlayMessages.RequestHoldingBitCountEvent>()
                .Subscribe(x =>
                {
                    Debug.Log("Request received");
                    _messagePublisher.Publish(new GamePlayMessages.ReportHoldingBitCountEvent(_heldDataBitCount));
                    // The player would drop the databits at this moment, so the count will be reverted to zero.
                    _heldDataBitCount = 0;
                }).AddTo(this);
        }

        private void Update()
        {
            rb2d.mass = 1 + _heldDataBitCount * bitMassPenalty;
        }
    }
}