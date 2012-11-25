using TheCodeKing.Utils.Contract;
using TheCodeKing.Utils.IoC;

namespace XDMessaging.Fluent
{
    public abstract class Implementations
    {
        #region Constants and Fields

        private readonly IocContainer container;

        #endregion

        #region Constructors and Destructors

        protected Implementations(IocContainer container)
        {
            Validate.That(container).IsNotNull();

            this.container = container;
        }

        #endregion

        #region Properties

        public IocContainer Container
        {
            get { return container; }
        }

        #endregion
    }
}