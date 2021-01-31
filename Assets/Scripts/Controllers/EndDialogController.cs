using System;
using Messaging;
using UniRx;
using UnityEngine;
using Zenject;

namespace Controllers
{
    public class EndDialogController : MonoBehaviour
    {
        [SerializeField] private Animator animator;

        [Inject] private IMessageReceiver _messageReceiver;
        private static readonly int EndTrigger = Animator.StringToHash("EndTrigger");

        private void Awake()
        {
            _messageReceiver.Receive<GamePlayMessages.GameCompletedEvent>()
                .Subscribe(x => { animator.SetTrigger(EndTrigger); });
        }
    }
}