using Jague.Common.Bank.Events;
using NLog;
using System;
using System.Collections.Generic;

namespace Jague.Common.Network
{
    public abstract class Session : IUpdate
    {
        protected static readonly ILogger log = LogManager.GetCurrentClassLogger();

        public bool PendingEvent => events.Count != 0;

        private readonly Queue<IEvent> events = new Queue<IEvent>();

        public void EnqueueEvent(IEvent @event)
        {
            events.Enqueue(@event);
        }

        public virtual void Update(double lastTick)
        {
            while (events.TryPeek(out IEvent @event))
            {
                if (!@event.CanExecute())
                    continue;

                events.Dequeue();

                try
                {
                    @event.Execute();
                }
                catch (Exception exception)
                {
                    log.Error(exception);
                }
            }
        }
    }
}
