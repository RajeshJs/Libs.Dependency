using Castle.DynamicProxy;
using Castle.MicroKernel;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Castle.Windsor.Proxy;
using System;
using System.Linq;

namespace Libs.Dependency
{
    public class IocManager : IIocManager
    {
        private static readonly ProxyGenerator ProxyGenerator = new ProxyGenerator();

        public static IocManager Instance { get; private set; }
        public IWindsorContainer IocContainer { get; private set; }

        static IocManager()
        {
            Instance = new IocManager();
        }

        public IocManager()
        {
            IocContainer = CreateContainer();

            IocContainer.Register(
                Component.For<IocManager, IIocManager, IIocRegistrar, IIocResolver>()
                    .Instance(this)
            );
        }

        private static IWindsorContainer CreateContainer()
        {
            return new WindsorContainer(new DefaultProxyFactory(ProxyGenerator));
        }


        public void Register<T>(DependencyLifeStyle lifeStyle = DependencyLifeStyle.Singleton) where T : class
        {
            IocContainer.Register(ApplyLifestyle(Component.For<T>(), lifeStyle));
        }

        public void Register(Type type, DependencyLifeStyle lifeStyle = DependencyLifeStyle.Singleton)
        {
            IocContainer.Register(ApplyLifestyle(Component.For(type), lifeStyle));
        }

        public void Register<TType, TImpl>(DependencyLifeStyle lifeStyle = DependencyLifeStyle.Singleton) where TType : class where TImpl : class, TType
        {
            IocContainer.Register(ApplyLifestyle(Component.For<TType, TImpl>().ImplementedBy<TImpl>(), lifeStyle));
        }

        public void Register(Type type, Type impl, DependencyLifeStyle lifeStyle = DependencyLifeStyle.Singleton)
        {
            IocContainer.Register(ApplyLifestyle(Component.For(type, impl).ImplementedBy(impl), lifeStyle));
        }

        public bool IsRegistered(Type type)
        {
            return IocContainer.Kernel.HasComponent(type);
        }

        public bool IsRegistered<T>()
        {
            return IocContainer.Kernel.HasComponent(typeof(T));
        }

        public T Resolve<T>()
        {
            return IocContainer.Resolve<T>();
        }

        public T Resolve<T>(Type type)
        {
            return (T)IocContainer.Resolve(type);
        }

        public T Resolve<T>(object argumentsAsAnonymousType)
        {
            return IocContainer.Resolve<T>(Arguments.FromProperties(argumentsAsAnonymousType));
        }

        public object Resolve(Type type)
        {
            return IocContainer.Resolve(type);
        }

        public object Resolve(Type type, object argumentsAsAnonymousType)
        {
            return IocContainer.Resolve(type, Arguments.FromProperties(argumentsAsAnonymousType));
        }

        public T[] ResolveAll<T>()
        {
            return IocContainer.ResolveAll<T>();
        }

        public T[] ResolveAll<T>(object argumentsAsAnonymousType)
        {
            return IocContainer.ResolveAll<T>(Arguments.FromProperties(argumentsAsAnonymousType));
        }

        public object[] ResolveAll(Type type)
        {
            return IocContainer.ResolveAll(type).Cast<object>().ToArray();
        }

        public object[] ResolveAll(Type type, object argumentsAsAnonymousType)
        {
            return IocContainer.ResolveAll(type, Arguments.FromProperties(argumentsAsAnonymousType))
                .Cast<object>().ToArray();
        }

        public void Release(object obj)
        {
            IocContainer.Release(obj);
        }

        public void Dispose()
        {
            IocContainer.Dispose();
        }

        private static ComponentRegistration<T> ApplyLifestyle<T>(ComponentRegistration<T> registration, DependencyLifeStyle lifeStyle)
            where T : class
        {
            switch (lifeStyle)
            {
                case DependencyLifeStyle.Transient:
                    return registration.LifestyleTransient();
                case DependencyLifeStyle.Singleton:
                    return registration.LifestyleSingleton();
                default:
                    return registration;
            }
        }
    }
}
