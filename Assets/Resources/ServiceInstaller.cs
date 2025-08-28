using InitializationSystem;
using UnityEngine;
using Zenject;

public class ServiceInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<SaveService>()
        .AsSingle()
        .NonLazy();

        Container.Bind<AnalyticsService>()
        .AsSingle()
        .NonLazy();

        Container.Bind<AdsService>()
        .AsSingle()
        .NonLazy();
    }
}
