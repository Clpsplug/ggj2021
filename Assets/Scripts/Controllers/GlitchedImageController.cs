using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using Zenject;

namespace Controllers
{
    public class GlitchedImageController : MonoBehaviour
    {
        [SerializeField] private int gateOpenPeriod = 10;
        [SerializeField, Range(0, 1)] private float gateOpenProb = default;
        [SerializeField] private Material glitchMaterial = default;

        private static List<string> _vectorNames = new List<string> {"_ROffset", "_GOffset", "_BOffset"};

#pragma warning disable 649
        [Inject] private IMessageReceiver _messageReceiver;
#pragma warning restore 649
        
        private void FixedUpdate()
        {
            if (Random.value < gateOpenProb)
            {
                Trigger();
            }
        }

        public void Trigger()
        {
            TriggerGlitch().Forget(Debug.LogException);
        }

        private async UniTask TriggerGlitch()
        {
            var frame = 0;
            while (frame < gateOpenPeriod)
            {
                ApplyGlitch();
                frame++;
                await UniTask.Yield();
            }

            ResetGlitch();
        }

        private void ApplyGlitch()
        {
            foreach (var vname in _vectorNames)
            {
                glitchMaterial.SetVector(vname, new Vector4(
                    (Random.value * 2 - 1) * 0.05f, (Random.value * 2 - 1) * 0.05f, 0, 0
                ));
            }
        }

        private void ResetGlitch()
        {
            foreach (var vname in _vectorNames)
            {
                glitchMaterial.SetVector(vname, Vector4.zero);
            }
        }
    }
}