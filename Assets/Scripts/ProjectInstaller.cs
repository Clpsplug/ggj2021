using UniRx;
using Zenject;

public class ProjectInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind(typeof(IMessagePublisher), typeof(IMessageReceiver))
            .FromInstance(new MessageBroker())
            .AsSingle();
    }
}