using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Microsoft.AspNet.StressFramework
{
    internal class StressTestHostProcess
    {
        private SyncGate _syncGate;

        public Process Process { get; }

        public StressTestHostProcess(SyncGate syncGate, Process process)
        {
            _syncGate = syncGate;
            Process = process;
        }

        public void BeginIterations()
        {
            _syncGate.Release();
        }

        internal static StressTestHostProcess Launch(string assembly, string type, string method)
        {
            var syncGate = SyncGate.CreateParent();
            var me = Process.GetCurrentProcess();
            var process = new Process();
            process.StartInfo.FileName = me.MainModule.FileName;
            process.StartInfo.Arguments = $"{typeof(Program).GetTypeInfo().Assembly.GetName().Name} {syncGate.Name} {assembly} {type} {method}";
            process.StartInfo.WorkingDirectory = Directory.GetCurrentDirectory();
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.OutputDataReceived += (sender, args) =>
            {
                Console.WriteLine(args.Data);
            };
            process.ErrorDataReceived += (sender, args) =>
            {
                Console.Error.WriteLine(args.Data);
            };
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            return new StressTestHostProcess(syncGate, process);
        }
    }
}