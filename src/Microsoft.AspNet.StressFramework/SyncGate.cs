using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.AspNet.StressFramework
{
    /// <summary>
    /// Provides a cross-platform way for one process to control when another process should continue execution
    /// </summary>
    /// <remarks>
    /// On Windows this will use a shared semaphore, on Unix it will use Signals or file locking.
    /// </remarks>
    public class SyncGate : IDisposable
    {
        private Semaphore _sem;

        public string Name { get; }

        private SyncGate()
            : this("SyncGate_" + Guid.NewGuid().ToString("N"))
        {
        }

        private SyncGate(string name)
        {
            // Create a named semaphore
            Name = name;
            _sem = new Semaphore(initialCount: 0, maximumCount: 1, name: name);
        }

        public static SyncGate CreateParent()
        {
            return new SyncGate();
        }

        public static void WaitFor(string name)
        {
            using (var syncGate = new SyncGate(name))
            {
                syncGate.Wait();
            }
        }

        private void Wait()
        {
            _sem.WaitOne();
        }

        public void Release()
        {
            _sem.Release();
        }

        public void Dispose()
        {
            _sem.Dispose();
        }
    }
}
