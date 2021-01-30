using System;
using Messaging;
using UniRx;
using UnityEngine;
using Zenject;

namespace Controllers
{
    /// <summary>
    /// Used for the goalpost
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class DirectoryController : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        
        [Inject] private IMessagePublisher _messagePublisher;
        [Inject] private IMessageReceiver _messageReceiver;
        private static readonly int DirectoryComplete = Animator.StringToHash("DirectoryComplete");
        private static readonly int DirectoryReceived = Animator.StringToHash("DirectoryReceived");

        private void Awake()
        {
            _messageReceiver.Receive<GamePlayMessages.DirectoryReceivedEvent>()
                .Subscribe(x =>
                {
                    animator.SetTrigger(DirectoryReceived);
                }).AddTo(this);
            _messageReceiver.Receive<GamePlayMessages.DirectoryCompletedEvent>()
                .Subscribe(x =>
                {
                    animator.SetTrigger(DirectoryComplete);
                }).AddTo(this);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.tag.Equals("Player"))
            {
                Debug.Log("Request sent");
                _messagePublisher.Publish(new GamePlayMessages.RequestHoldingBitCountEvent());
            }
        }
    }
}