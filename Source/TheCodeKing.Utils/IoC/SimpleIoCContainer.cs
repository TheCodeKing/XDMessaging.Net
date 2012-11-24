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
using System.Collections.Generic;
using System.Linq;
using TheCodeKing.Utils.Contract;

namespace TheCodeKing.Utils.IoC
{
    public sealed class SimpleIoCContainer : IocContainer
    {
        #region Constants and Fields

        private const string defaultName = "IoCDefault";
        private readonly IoCActivator activator;

        private readonly IDictionary<string, Func<object>> map =
            new Dictionary<string, Func<object>>(StringComparer.InvariantCultureIgnoreCase);

        #endregion

        #region Constructors and Destructors

        public SimpleIoCContainer(Func<IocContainer, IoCActivator> activatorFactory)
        {
            activator = activatorFactory(this);
        }

        #endregion

        #region Public Methods

        public IocContainer Initialize(Action<IocContainer> configure)
        {
            configure(this);
            return this;
        }

        #endregion

        #region Implemented Interfaces

        #region IoCContainer

        public bool IsRegistered(Type type, string name)
        {
            return map.ContainsKey(GetKey(type, name));
        }

        public bool IsRegistered(Type type)
        {
            return IsRegistered(type, defaultName);
        }

        public void Register(Type abstractType, Func<object> factory)
        {
            Register(abstractType, factory, defaultName);
        }

        public void Register(Type abstractType, Func<object> factory, string name)
        {
            Validate.That(abstractType).IsNotNull();
            Validate.That(factory).IsNotNull();
            Validate.That(name).IsNotNullOrEmpty();

            var key = GetKey(abstractType, name);
            Register(key, factory);
        }

        public void Register(Type abstractType, Type concreteType)
        {
            Register(abstractType, concreteType, defaultName);
        }

        public void Register(Type abstractType, Type concreteType, string name)
        {
            Validate.That(abstractType).IsNotNull();
            Validate.That(concreteType).IsNotNull();
            Validate.That(name).IsNotNullOrEmpty();

            var key = GetKey(abstractType, name);
            Register(key, () => activator.CreateInstance(concreteType));
        }

        public void RegisterInstance(Type abstractType, object instance)
        {
            RegisterInstance(abstractType, instance, defaultName);
        }

        public void RegisterInstance(Type abstractType, object instance, string name)
        {
            Validate.That(abstractType).IsNotNull();
            Validate.That(instance).IsNotNull();
            Validate.That(name).IsNotNullOrEmpty();

            var key = GetKey(abstractType, name);
            Register(key, () => instance);
        }

        public object Resolve(Type type)
        {
            return Resolve(type, defaultName);
        }

        public object Resolve(Type type, string name)
        {
            if (typeof (Func<>).IsAssignableFrom(type) && type.GetType().GetGenericArguments().Length == 1)
            {
                var keyType = type.GetType().GetGenericArguments().Last();
                return map[GetKey(keyType, name)];
            }
            var key = GetKey(type, name);
            if (map.ContainsKey(key))
            {
                return map[key]();
            }
            return activator.CreateInstance(type);
        }

        #endregion

        #endregion

        #region Methods

        private static string GetKey(Type type, string name)
        {
            return string.Concat(name, "-", type.FullName);
        }

        private void Register(string key, Func<object> factory)
        {
            if (!map.ContainsKey(key))
            {
                map[key] = factory;
            }
        }

        #endregion
    }
}