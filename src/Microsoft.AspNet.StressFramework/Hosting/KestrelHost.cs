using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;

namespace Microsoft.AspNet.StressFramework.Hosting
{
    public class KestrelHost : IStressTestHost, IDisposable
    {
        private readonly string _applicationPath;
        private readonly TaskCompletionSource<object> _tcs = new TaskCompletionSource<object>();
        private Process _process;

        public KestrelHost(string applicationPath)
        {
            _applicationPath = applicationPath;
        }

        public Process LaunchHost(MethodInfo method)
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
                },
                EnableRaisingEvents = true
            };

            _process.OutputDataReceived += (sender, args) =>
            {
                StressTestTrace.WriteRawLine(args.Data);
            };
            _process.ErrorDataReceived += (sender, args) =>
            {
                StressTestTrace.WriteRawLine(args.Data);
            };
            _process.Exited += (sender, args) =>
            {
                _tcs.SetResult(null);
            };
            _process.Start();
            _process.BeginOutputReadLine();
            _process.BeginErrorReadLine();
            return _process;
        }

        public void Dispose()
        {
            if (!_process.HasExited)
            {
                _process.Kill();
            }

            _process.Dispose();
        }

        public Task StartAsync()
        {
            // Already running... kinda... we probably need to sync up with Kestrel to figure out when
            // it is actually listening...

            // Wait 200ms for Kestrel to start up (for now)
            return Task.Delay(200);
        }

        public Task WaitForExitAsync()
        {
            return _tcs.Task;
        }

        public Task ShutdownAsync()
        {
            _process.Kill();
            return _tcs.Task;
        }
    }
}
