using System;
using System.Linq;

namespace TheCodeKing.Utils.IoC
{
    public static class IoCContainerExtensions
    {
        public static bool IsRegistered<T>(this IoCContainer container)
        {
            return container.IsRegistered(typeof (T));
        }
        public static bool IsRegistered<T>(this IoCContainer container, string name)
        {
            return container.IsRegistered(typeof (T), name);
        }

        public static void Register<T, TC>(this IoCContainer container)
        {
            container.Register(typeof(T), typeof(TC));
        }
        public static void Register<T, TC>(this IoCContainer container, string name)
        {
            container.Register(typeof(T), typeof(TC), name);
        }

        public static void Register<T>(this IoCContainer container, Func<T> factory)
        {
            var keyType = factory.GetType().GetGenericArguments().Last();
            container.Register(keyType, () => factory());
        }
        public static void Register<T>(this IoCContainer container, Func<T> factory, string name)
        {
            var keyType = factory.GetType().GetGenericArguments().Last();
            container.Register(keyType, () => factory(), name);
        }

        public static void RegisterInstance<T>(this IoCContainer container, T instance)
        {
            container.RegisterInstance(typeof(T), instance);
        }
        public static void RegisterInstance<T>(this IoCContainer container, T instance, string name)
        {
            container.RegisterInstance(typeof(T), instance, name);
        }

        public static T Resolve<T>(this IoCContainer container) where T: class
        {
            return container.Resolve(typeof (T)) as T;
        }

        public static T Resolve<T>(this IoCContainer container, string name) where T : class
        {
            return container.Resolve(typeof(T), name) as T;
        }
    }
}
