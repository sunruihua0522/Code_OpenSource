using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IrixiStepperControllerHelper
{
    public class ConnectionEventArgs : EventArgs
    {
        public enum EventType
        {
            Connecting,
            ConnectionSuccess,
            ConnectionFailure,
            ConnectionLost
        }

        public ConnectionEventArgs(EventType Event, object Content = null)
        {
            this.Event = Event;
            this.Content = Content;
        }

        /// <summary>
        /// Get the event type
        /// </summary>
        public EventType Event { private set;  get;}

        /// <summary>
        /// Get the arguments of the event
        /// </summary>
        public object Content { private set; get; }
    }
}
