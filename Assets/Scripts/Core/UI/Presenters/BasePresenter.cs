using Core.Events;
using Core.Services;
using Core.UI.Views;

namespace Core.UI.Presenters
{
    public abstract class BasePresenter : IPresenter
    {
        protected readonly IView view;
        protected readonly IEventBus eventBus;

        protected BasePresenter(IView view)
        {
            this.view = view;
            eventBus = ServiceLocator.Instance.Get<IEventBus>();
        }

        public abstract void Initialize();
        public abstract void SubscribeEvents();
        public abstract void UnSubscribeEvents();
        public abstract void Dispose();
    }
}
