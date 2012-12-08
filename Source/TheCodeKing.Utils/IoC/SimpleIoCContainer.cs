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
    public sealed class SimpleIocContainer : IocContainer
    {
        #region Constants and Fields

        private const string defaultName = "IoCDefault";
        private readonly IocActivator activator;
        private readonly IDictionary<string, Func<IocContainer, IocActivator, object>> map;

        private readonly Func<IocContainer, IocActivator> activatorFactory;
        private readonly Func<IocContainer, IocScanner> scannerFactory;
        private readonly IocScanner scanner;

        #endregion

        #region Constructors and Destructors

        public SimpleIocContainer(Func<IocContainer, IocActivator> activatorFactory,
                                  Func<IocContainer, IocScanner> scannerFactory)
        {
            Validate.That(activatorFactory).IsNotNull();
            Validate.That(scannerFactory).IsNotNull();

            this.activatorFactory = activatorFactory;
            this.scannerFactory = scannerFactory;
            this.scanner = scannerFactory(this);
            this.activator = activatorFactory(this);
            this.map = new Dictionary<string, Func<IocContainer, IocActivator, object>>(StringComparer.InvariantCultureIgnoreCase);
        }

        public SimpleIocContainer(Func<IocContainer, IocActivator> activatorFactory,
                                  Func<IocContainer, IocScanner> scannerFactory, IDictionary<string, Func<IocContainer, IocActivator, object>> map)
        {
            Validate.That(activatorFactory).IsNotNull();
            Validate.That(scannerFactory).IsNotNull();
            Validate.That(map).IsNotNull();

            this.activatorFactory = activatorFactory;
            this.scannerFactory = scannerFactory;
            this.scanner = scannerFactory(this);
            this.activator = activatorFactory(this);
            this.map = map;
        }

        #endregion

        #region Properties

        public IocScanner Scan
        {
            get { return scanner; }
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

        #region IocContainer

        public bool IsRegistered(Type type, string name)
        {
            return map.ContainsKey(GetKey(type, name));
        }

        public bool IsRegistered(Type type)
        {
            return IsRegistered(type, defaultName);
        }

        public void Register(Type abstractType, Type concreteType)
        {
            Register(abstractType, concreteType, LifeTime.Instance);
        }

        public void Register(Type abstractType, Type concreteType, string name)
        {
            Register(abstractType, concreteType, name, LifeTime.Instance);
        }

        public void Register(Type abstractType, Func<object> factory)
        {
            Register(abstractType, factory, LifeTime.Instance);
        }

        public void Register(Type abstractType, Func<object> factory, string name)
        {
            Register(abstractType, factory, name, LifeTime.Instance);
        }

        public void Register(Type abstractType, Func<object> factory, LifeTime lifetime)
        {
            Register(abstractType, factory, defaultName, lifetime);
        }

        public void Register(Type abstractType, Func<object> factory, string name, LifeTime lifetime)
        {
            Validate.That(abstractType).IsNotNull();
            Validate.That(factory).IsNotNull();
            Validate.That(name).IsNotNullOrEmpty();

            var key = GetKey(abstractType, name);
            Register(key, (c,a) => factory(), lifetime);
        }

        public void Register(Type concreteType, LifeTime lifetime)
        {
            Register(concreteType, concreteType, defaultName, lifetime);
        }

        public void Register(Type abstractType, Type concreteType, LifeTime lifetime)
        {
            Register(abstractType, concreteType, defaultName, lifetime);
        }

        public void Register(Type abstractType, Type concreteType, string name, LifeTime lifetime)
        {
            Validate.That(abstractType).IsNotNull();
            Validate.That(concreteType).IsNotNull();
            Validate.That(name).IsNotNullOrEmpty();

            var key = GetKey(abstractType, name);
            Register(key, (c, a) => a.CreateInstance(concreteType), lifetime);
        }

        public void UpdateRegistration(Type abstractType, Type concreteType)
        {
            UpdateRegistration(abstractType, concreteType, LifeTime.Instance);
        }

        public void UpdateRegistration(Type abstractType, Type concreteType, string name)
        {
            UpdateRegistration(abstractType, concreteType, name, LifeTime.Instance);
        }

        public void UpdateRegistration(Type abstractType, Func<object> factory)
        {
            UpdateRegistration(abstractType, factory, LifeTime.Instance);
        }

        public void UpdateRegistration(Type abstractType, Func<object> factory, string name)
        {
            UpdateRegistration(abstractType, factory, name, LifeTime.Instance);
        }

        public void UpdateRegistration(Type abstractType, Func<object> factory, LifeTime lifetime)
        {
            UpdateRegistration(abstractType, factory, defaultName, lifetime);
        }

        public void UpdateRegistration(Type abstractType, Func<object> factory, string name, LifeTime lifetime)
        {
            Validate.That(abstractType).IsNotNull();
            Validate.That(factory).IsNotNull();
            Validate.That(name).IsNotNullOrEmpty();

            var key = GetKey(abstractType, name);
            Update(key, (c, a) => factory(), lifetime);
        }

        public void UpdateRegistration(Type concreteType, LifeTime lifetime)
        {
            UpdateRegistration(concreteType, concreteType, defaultName, lifetime);
        }

        public void UpdateRegistration(Type abstractType, Type concreteType, LifeTime lifetime)
        {
            UpdateRegistration(abstractType, concreteType, defaultName, lifetime);
        }

        public void UpdateRegistration(Type abstractType, Type concreteType, string name, LifeTime lifetime)
        {
            Validate.That(abstractType).IsNotNull();
            Validate.That(concreteType).IsNotNull();
            Validate.That(name).IsNotNullOrEmpty();

            var key = GetKey(abstractType, name);
            Update(key, (c, a) => a.CreateInstance(concreteType), lifetime);
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
            Register(key, (c, a) => instance);
        }

        public object Resolve(Type type)
        {
            return Resolve(type, defaultName);
        }

        public object Resolve(Type type, string name)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof (Func<>) &&
                type.GetGenericArguments().Length == 1)
            {
                var keyType = type.GetGenericArguments().Last();
                Func<IocActivator,object> value = a => Resolve(keyType, name);
                return value;
            }
            var key = GetKey(type, name);
            if (map.ContainsKey(key))
            {
                return map[key](this, activator);
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

        private void Register(string key, Func<IocContainer, IocActivator, object> factory, LifeTime lifeTime)
        {
            Func<IocContainer, IocActivator,object> value;
            if (lifeTime.Equals(LifeTime.Singleton))
            {
                var singleton = new Lazy<object>(() => factory(this, activator));
                value = (c, a) => singleton.Value;
            }
            else
            {
                value = (c, a) => factory(c, a);
            }
            Register(key, value);
        }

        private void Register(string key, Func<IocContainer, IocActivator, object> factory)
        {
            if (!map.ContainsKey(key))
            {
                map[key] = factory;
            }
        }

        private void Update(string key, Func<IocContainer, IocActivator, object> factory, LifeTime lifeTime)
        {
            Func<IocContainer, IocActivator, object> value;
            if (lifeTime.Equals(LifeTime.Singleton))
            {
                var singleton = new Lazy<object>(() => factory(this, activator));
                value = (c,a) => singleton.Value;
            }
            else
            {
                value = factory;
            }
            Update(key, value);
        }

        private void Update(string key, Func<IocContainer, IocActivator, object> factory)
        {
            map[key] = factory;
        }

        #endregion

        public IocContainer Clone()
        {
            return new SimpleIocContainer(activatorFactory, 
                scannerFactory,
                new Dictionary<string, Func<IocContainer, IocActivator, object>>(map, StringComparer.InvariantCultureIgnoreCase));
        }

        object ICloneable.Clone()
        {
            return this.Clone();
        }
    }
}