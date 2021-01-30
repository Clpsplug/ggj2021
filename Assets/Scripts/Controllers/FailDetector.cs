using Messaging;
using UniRx;
using UnityEngine;
using Zenject;

namespace Controllers
{
    /// <summary>
    /// Used for kill zones such as pits and spikes
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class FailDetector : MonoBehaviour
    {
        public AudioSource audioSource;
        public AudioClip clipToPlay;
        
        [Inject] private IMessagePublisher _messagePublisher;
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.tag.Equals("Player"))
            {
                // Player collision, this is a sign of failure.
                _messagePublisher.Publish(new GamePlayMessages.PlayerFailureEvent());
                if (audioSource == null || clipToPlay == null) return;
                audioSource.clip = clipToPlay;
                audioSource.Play();
            }
        }
    }
}