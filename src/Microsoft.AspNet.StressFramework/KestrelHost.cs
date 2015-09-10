using System;
using System.Diagnostics;

namespace Microsoft.AspNet.StressFramework
{
    public class KestrelHost : IStressTestHost, IDisposable
    {
        private readonly string _applicationPath;
        private Process _process;

        public KestrelHost(string applicationPath)
        {
            _applicationPath = applicationPath;
        }

        public void Setup()
        {
            var dnxProcess = Process.GetCurrentProcess();

            _process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = dnxProcess.MainModule.FileName,
                    Arguments = "kestrel",
                    WorkingDirectory = _applicationPath,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            _process.OutputDataReceived += (sender, args) =>
            {
                StressTestTrace.WriteRawLine(args.Data);
            };
            _process.ErrorDataReceived += (sender, args) =>
            {
                StressTestTrace.WriteRawLine(args.Data);
            };
            _process.Start();
            _process.BeginOutputReadLine();
            _process.BeginErrorReadLine();


        }

        public void Run(StressTestHostContext context)
        {
            // Do nothing
        }

        public void Dispose()
        {
            if (!_process.HasExited)
            {
                _process.Kill();
            }

            _process.Dispose();
        }
    }
}
