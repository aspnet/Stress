using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Microsoft.Dnx.Runtime;

namespace Microsoft.AspNet.StressFramework
{
    public class Program
    {
        public int Main(string[] args)
        {
            Console.WriteLine($"[Host:{Process.GetCurrentProcess().Id}] Host Launched");
            if(args.Length != 4)
            {
                Console.Error.WriteLine("Usage: stresshost <LOCK_NAME> <TEST_LIBRARY> <TEST_CLASS> <TEST_METHOD>");
                return -1;
            }

            var lockName = args[0];
            var libraryName = args[1];
            var className = args[2];
            var methodName = args[3];

            // Find the class
            var asm = Assembly.Load(new AssemblyName(libraryName));
            if (asm == null) {
                Console.Error.WriteLine($"Failed to load assembly: {libraryName}");
                return -2;
            }
            var typ = asm.GetExportedTypes().FirstOrDefault(t => t.FullName.Equals(className));
            if (typ == null) {
                Console.Error.WriteLine($"Failed to locate type: {className} in {libraryName}");
                return -3;
            }
            var method = typ.GetMethods()
                .SingleOrDefault(m => 
                    m.Name.Equals(methodName) && 
                    typeof(IStressTestHost).IsAssignableFrom(m.ReturnType) && 
                    m.IsPublic && 
                    m.GetParameters().Length == 0);
            if(method == null)
            {
                Console.Error.WriteLine($"Failed to locate method: {methodName} in {className}");
                return -4;
            }

            // Construct the class and invoke the method
            var instance = Activator.CreateInstance(typ);
            var host = (IStressTestHost)method.Invoke(instance, new object[0]);

            SyncGate.WaitFor(lockName);

            // Run the host
            Console.WriteLine($"[Host:{Process.GetCurrentProcess().Id}] Host Released");
            host.Run(new StressTestHostContext());

            Console.WriteLine($"[Host:{Process.GetCurrentProcess().Id}] Host Completed");
            return 0;
        }
    }
}
