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
using System.Reflection;
using TheCodeKing.Utils.Contract;

namespace TheCodeKing.Utils.IoC
{
    public sealed class IocActivator
    {
        #region Constants and Fields

        private const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic |
                                                  BindingFlags.CreateInstance | BindingFlags.Instance;

        private readonly IocContainer container;

        #endregion

        #region Constructors and Destructors

        public IocActivator(IocContainer container)
        {
            Validate.That(container).IsNotNull();

            this.container = container;
        }

        #endregion

        #region Public Methods

        public object CreateInstance(Type type)
        {
            if (type.IsAbstract || type.IsInterface)
            {
                return null;
            }
            var args = GetConstructorParameters(type);
            return Activator.CreateInstance(type,
                                            bindingFlags,
                                            null, args, null);
        }

        #endregion

        #region Methods

        private ConstructorInfo FindConstructorWithLeastParameters(Type type)
        {
            return type.GetConstructors(bindingFlags)
                //.Where(o => !o.GetParameters().Any(p => !(p.ParameterType.IsAbstract && p.ParameterType.IsInterface && container.IsRegistered(p.ParameterType))))
                .OrderBy(o => o.GetParameters().Length).ElementAtOrDefault(0);
        }

        private object[] GetConstructorParameters(Type type)
        {
            var constructor = FindConstructorWithLeastParameters(type);
            if (constructor == null)
            {
                return null;
            }
            return constructor.GetParameters().Select(param => container.Resolve(param.ParameterType)).ToArray();
        }

        #endregion
    }
}