using Messaging;
using UniRx;
using UnityEngine;
using Zenject;

namespace Controllers
{
    /// <summary>
    /// Used for Items
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class DataBitController : MonoBehaviour
    {
        private bool _collected = false;
        [Inject] private IMessagePublisher _messagePublisher;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_collected) return;
            if (other.gameObject.tag.Equals("Player"))
            {
                _collected = true;
                _messagePublisher.Publish(new GamePlayMessages.DataBitCollectedEvent());
                Destroy(gameObject);
            }
        }
    }
}