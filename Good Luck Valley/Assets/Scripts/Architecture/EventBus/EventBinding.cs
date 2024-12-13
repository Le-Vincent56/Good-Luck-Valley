using System;

namespace GoodLuckValley.Architecture.EventBus
{
    internal interface IEventBinding<T>
    {
        public Action<T> OnEvent { get; set; }
        public Action OnEventNoArgs { get; set; }
    }

    public class EventBinding<T> : IEventBinding<T> where T : IEvent
    {
        Action<T> onEvent = delegate { };
        Action onEventNoArgs = delegate { };

        Action<T> IEventBinding<T>.OnEvent
        {
            get => onEvent;
            set => onEvent = value;
        }

        Action IEventBinding<T>.OnEventNoArgs
        {
            get => onEventNoArgs;
            set => onEventNoArgs = value;
        }

        public EventBinding(Action<T> onEvent) => this.onEvent = onEvent;

        public EventBinding(Action onEventNoArgs) => this.onEventNoArgs = onEventNoArgs;

        /// <summary>
        /// Subscribe an Action to the no-argument event
        /// </summary>
        public void Add(Action onEventNoArgs) => this.onEventNoArgs += onEventNoArgs;

        /// <summary>
        /// Unsubscribe an Action to the no-argument event
        /// </summary>
        public void Remove(Action onEventNoArgs) => this.onEventNoArgs -= onEventNoArgs;

        /// <summary>
        /// Subscribe an Action to the argumented event
        /// </summary>
        public void Add(Action<T> onEvent) => this.onEvent += onEvent;

        /// <summary>
        /// Unsubscribe an Action to the argumented event
        /// </summary>
        /// <param name="onEvent"></param>
        public void Remove(Action<T> onEvent) => this.onEvent -= onEvent;
    }
}
