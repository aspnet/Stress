using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

#if DNX451
using Microsoft.Diagnostics.Tracing.Session;
using Microsoft.Diagnostics.Tracing.Parsers;
#endif

namespace Microsoft.AspNet.StressFramework.Hosting
{
    internal class StressTestHostProcess : IDisposable
    {
        public static readonly string ReleaseMessage = "RELEASE_STRESS_TEST_HOST";

        private TaskCompletionSource<int> _exited = new TaskCompletionSource<int>();

        private long _totalAllocatedBytes = 0;

#if DNX451
        private TraceEventSession _session;
#endif

        public Process Process { get; }

        public StressTestHostProcess(Process process)
        {
            Process = process;
            Process.Exited += (sender, args) =>
            {
                _exited.SetResult(0);
            };
            Process.Start();
            Process.BeginOutputReadLine();
            Process.BeginErrorReadLine();
        }

        public void StartExecutingHost()
        {
            Process.StandardInput.WriteLine(ReleaseMessage);
        }

        public void AttachCollectors()
        {
#if DNX451
            if (PlatformHelper.IsWindows)
            {
                // Name of the session must be globally unique
                var name = "DnxStress";
                StressTestTrace.WriteLine("Starting ETW Session: " + name);

                _session = new TraceEventSession(name, TraceEventSessionOptions.Create);

                // Attach providers
                _session.EnableProvider(ClrTraceEventParser.ProviderGuid);
                _session.Source.Clr.GCAllocationTick += evt =>
                {
                    _totalAllocatedBytes += evt.AllocationAmount64;
                };
            }
            else
            {
                StressTestTrace.WriteLine("TODO: Collectors on Linux. We'll need to use DTrace probably...");
            }
#else
            StressTestTrace.WriteLine("TODO: Collectors on CoreCLR. We'll need TraceEvent ported to CoreCLR...");
#endif
        }

        public Task ShutdownAsync()
        {
            Process.Kill();
            return WaitForExitAsync();
        }

        public Task WaitForExitAsync()
        {
            return _exited.Task;
        }

        public void Report()
        {
            StressTestTrace.WriteLine($"Allocated {_totalAllocatedBytes / (1024.0 * 1024.0):0.00}MB managed memory throughout execution");
        }

        public void ProcessCollectorsAsync()
        {
#if DNX451
            if (PlatformHelper.IsWindows)
            {
                Task.Run(() =>
                {
                    while (!Process.HasExited)
                    {
                        _session.Source.Process();
                    }
                });
            }
#endif
        }

        public static StressTestHostProcess Launch(MethodInfo testMethod, Action<string> outputWriter)
        {
            var assembly = testMethod.DeclaringType.GetTypeInfo().Assembly.GetName().Name;
            var type = testMethod.DeclaringType.FullName;
            var method = testMethod.Name;

            var me = Process.GetCurrentProcess();
            var process = new Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    FileName = me.MainModule.FileName,
                    Arguments = $"{typeof(Program).GetTypeInfo().Assembly.GetName().Name} \"{assembly}\" \"{type}\" \"{method}\"",
                    WorkingDirectory = Directory.GetCurrentDirectory(),
                    RedirectStandardError = true,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                },
                EnableRaisingEvents = true
            };
            process.OutputDataReceived += (sender, args) =>
            {
                outputWriter(args.Data);
            };
            process.ErrorDataReceived += (sender, args) =>
            {
                outputWriter(args.Data);
            };
            return new StressTestHostProcess(process);
        }

        public void Dispose()
        {
            Process.Dispose();
#if DNX451
            _session?.Dispose();
#endif
        }
    }
}