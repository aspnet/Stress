using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Microsoft.AspNet.StressFramework
{
    internal class StressTestHostProcess : IDisposable
    {
        public static readonly string ReleaseMessage = "RELEASE_STRESS_TEST_HOST";

        public Process Process { get; }

        public StressTestHostProcess(Process process)
        {
            Process = process;
        }

        public void BeginIterations()
        {
            Process.StandardInput.WriteLine(ReleaseMessage);
        }

        internal static StressTestHostProcess Launch(MethodInfo testMethod, Action<string> outputWriter)
        {
            var assembly = testMethod.DeclaringType.GetTypeInfo().Assembly.GetName().Name;
            var type = testMethod.DeclaringType.FullName;
            var method = testMethod.Name;

            var me = Process.GetCurrentProcess();
            var process = new Process();
            process.StartInfo.FileName = me.MainModule.FileName;
            process.StartInfo.Arguments = $"{typeof(Program).GetTypeInfo().Assembly.GetName().Name} \"{assembly}\" \"{type}\" \"{method}\"";
            process.StartInfo.WorkingDirectory = Directory.GetCurrentDirectory();
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.OutputDataReceived += (sender, args) =>
            {
                outputWriter(args.Data);
            };
            process.ErrorDataReceived += (sender, args) =>
            {
                outputWriter(args.Data);
            };
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            return new StressTestHostProcess(process);
        }

        public void Dispose()
        {
            Process.Dispose();
        }
    }
}