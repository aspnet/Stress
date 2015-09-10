using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.AspNet.StressFramework.Collectors
{
    [EventSource(Name = "Microsoft-AspNet-StressFramework")]
    internal class StressTestEventSource : EventSource
    {
        public static readonly StressTestEventSource Log = new StressTestEventSource();

        private StressTestEventSource() { }

        [Event(eventId: 1,
            Keywords = Keywords.IterationTracking,
            Level = EventLevel.Informational,
            Message = "Starting iteration #{0}",
            Opcode = EventOpcode.Start,
            Task = Tasks.Iteration,
            Version = 1)]
        public void IterationStart(int iterationNumber)
        {
            WriteEvent(1, iterationNumber);
        }

        [Event(eventId: 2,
            Keywords = Keywords.IterationTracking,
            Level = EventLevel.Informational,
            Message = "Finished iteration #{0}",
            Opcode = EventOpcode.Stop,
            Task = Tasks.Iteration,
            Version = 1)]
        public void IterationStop(int iterationNumber)
        {
            WriteEvent(2, iterationNumber);
        }

        [Event(eventId: 3,
            Level = EventLevel.Informational,
            Message = "Starting Run",
            Opcode = EventOpcode.Start,
            Task = Tasks.Run,
            Version = 1)]
        public void RunStart()
        {
            WriteEvent(3);
        }

        [Event(eventId: 4,
            Level = EventLevel.Informational,
            Message = "Finished Run",
            Opcode = EventOpcode.Stop,
            Task = Tasks.Run,
            Version = 1)]
        public void RunEnd()
        {
            WriteEvent(4);
        }

        public static class Keywords
        {
            public const EventKeywords IterationTracking = (EventKeywords)0x0001;
        }

        public static class Tasks
        {
            public const EventTask Run = (EventTask)0x0001;
            public const EventTask Iteration = (EventTask)0x0002;
        }
    }
}
