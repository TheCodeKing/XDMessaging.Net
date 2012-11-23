using System;

namespace TheCodeKing.Utils.IoC
{
    public interface IoCContainer
    {
        #region Public Methods

        bool IsRegistered(Type type);
        bool IsRegistered(Type type, string name);

        void Register(Type abstractType, Type concreteType);
        void Register(Type abstractType, Type concreteType, string name);
        void Register(Type abstractType, Func<object> factory);
        void Register(Type abstractType, Func<object> factory, string name);
        
        void RegisterInstance(Type type, object instance);
        void RegisterInstance(Type type, object instance, string name);

        object Resolve(Type type);
        object Resolve(Type type, string name);

        #endregion
    }
}