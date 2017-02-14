﻿using Serilog.Core;
using Serilog.Events;
using System.Collections.Generic;
using System.Linq;

namespace Serilog.Filters.Expressions.Tests.Support
{
    class CollectingSink : ILogEventSink
    {
        readonly List<LogEvent> _events = new List<LogEvent>();

        public List<LogEvent> Events { get { return _events; } }

        public LogEvent SingleEvent { get { return _events.Single(); } }

        public void Emit(LogEvent logEvent)
        {
            _events.Add(logEvent);
        }
    }
}
