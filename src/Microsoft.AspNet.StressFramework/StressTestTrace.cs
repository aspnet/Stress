using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.AspNet.StressFramework
{
    internal static class StressTestTrace
    {
        private static readonly Process Me = Process.GetCurrentProcess();

        private static string _hostOrDriver = "Driver";

        public static void WriteLine(string line)
        {
            WriteRawLine($"[{_hostOrDriver}:{Me.Id}] {line}");
        }

        internal static void WriteRawLine(string line)
        {
            Console.WriteLine(line);
        }

        public static void SetIsHost()
        {
            _hostOrDriver = "Host";
        }
    }
}
