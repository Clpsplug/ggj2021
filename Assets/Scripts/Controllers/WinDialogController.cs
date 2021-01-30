using Messaging;
using UniRx;
using UnityEngine;
using Zenject;

namespace Controllers
{
    public class WinDialogController : MonoBehaviour
    {
        [SerializeField] private Animator animator;

        [Inject] private IMessageReceiver _messageReceiver;
        private static readonly int WinTrigger = Animator.StringToHash("WinTrigger");

        private void Awake()
        {
            _messageReceiver.Receive<GamePlayMessages.DirectoryCompletedEvent>()
                .Subscribe(x => { animator.SetTrigger(WinTrigger); });
        }
    }
}