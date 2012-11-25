/*=============================================================================
*
*	(C) Copyright 2011, Michael Carlisle (mike.carlisle@thecodeking.co.uk)
*
*   http://www.TheCodeKing.co.uk
*  
*	All rights reserved.
*	The code and information is provided "as-is" without waranty of any kind,
*	either expressed or implied.
*
*=============================================================================
*/
using System;
using System.Linq;

namespace TheCodeKing.Utils.IoC
{
    public static class IocContainerExtensions
    {
        public static bool IsRegistered<T>(this IocContainer container)
        {
            return container.IsRegistered(typeof (T));
        }
        public static bool IsRegistered<T>(this IocContainer container, string name)
        {
            return container.IsRegistered(typeof (T), name);
        }

        public static void Register<T, TC>(this IocContainer container)
        {
            container.Register(typeof(T), typeof(TC));
        }
        public static void Register<T, TC>(this IocContainer container, string name)
        {
            container.Register(typeof(T), typeof(TC), name);
        }

        public static void Register<T>(this IocContainer container, Func<T> factory)
        {
            var keyType = factory.GetType().GetGenericArguments().Last();
            container.Register(keyType, () => factory());
        }
        public static void Register<T>(this IocContainer container, Func<T> factory, string name)
        {
            var keyType = factory.GetType().GetGenericArguments().Last();
            container.Register(keyType, () => factory(), name);
        }

        public static void RegisterInstance<T>(this IocContainer container, T instance)
        {
            container.RegisterInstance(typeof(T), instance);
        }
        public static void RegisterInstance<T>(this IocContainer container, T instance, string name)
        {
            container.RegisterInstance(typeof(T), instance, name);
        }

        public static T Resolve<T>(this IocContainer container) where T: class
        {
            return container.Resolve(typeof (T)) as T;
        }

        public static T Resolve<T>(this IocContainer container, string name) where T : class
        {
            return container.Resolve(typeof(T), name) as T;
        }
    }
}
