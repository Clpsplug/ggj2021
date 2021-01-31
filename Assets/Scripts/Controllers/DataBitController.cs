using Cysharp.Threading.Tasks;
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
        [SerializeField] private SpriteRenderer sRenderer;
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private ParticleSystem collectParticle;
        
        private bool _collected = false;
        [Inject] private IMessagePublisher _messagePublisher;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_collected) return;
            if (other.gameObject.tag.Equals("Player"))
            {
                sRenderer.color = new Color(1, 1, 1, 0);
                audioSource.Play();
                collectParticle.Play();
                _collected = true;
                _messagePublisher.Publish(new GamePlayMessages.DataBitCollectedEvent());
            }
        }

        private async UniTask Destroy()
        {
            await UniTask.WaitWhile(() => audioSource.isPlaying);
            Destroy(gameObject);
        }
    }
}