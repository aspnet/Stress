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
            StressTestTrace.SetIsHost();

            StressTestTrace.WriteLine("Host Launched");
            if (args.Length != 3)
            {
                Console.Error.WriteLine("Usage: stresshost <TEST_LIBRARY> <TEST_CLASS> <TEST_METHOD>");
                return -1;
            }

            var libraryName = args[0];
            var className = args[1];
            var methodName = args[2];

            // Find the class
            var asm = Assembly.Load(new AssemblyName(libraryName));
            if (asm == null)
            {
                Console.Error.WriteLine($"Failed to load assembly: {libraryName}");
                return -2;
            }
            var typ = asm.GetExportedTypes().FirstOrDefault(t => t.FullName.Equals(className));
            if (typ == null)
            {
                Console.Error.WriteLine($"Failed to locate type: {className} in {libraryName}");
                return -3;
            }
            var method = typ.GetMethods()
                .SingleOrDefault(m =>
                    m.Name.Equals(methodName) &&
                    typeof(IStressTestHost).IsAssignableFrom(m.ReturnType) &&
                    m.IsPublic &&
                    m.GetParameters().Length == 0);
            if (method == null)
            {
                Console.Error.WriteLine($"Failed to locate method: {methodName} in {className}");
                return -4;
            }

            // Construct the class and invoke the method
            var instance = Activator.CreateInstance(typ);
            var host = (IStressTestHost)method.Invoke(instance, new object[0]);

            StressTestTrace.WriteLine("Host Ready for release");

            // Read the release message from the standard input
            var released = Console.ReadLine();
            if (!string.Equals(StressTestHostProcess.ReleaseMessage, released, StringComparison.Ordinal))
            {
                StressTestTrace.WriteLine("Host received invalid release message. Aborting");
            }

            // Run the host
            StressTestTrace.WriteLine("Host Released");
            host.Run(new StressTestHostContext());

            StressTestTrace.WriteLine("Host Completed");
            return 0;
        }
    }
}
