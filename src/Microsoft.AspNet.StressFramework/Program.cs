using System;
using System.Linq;
using System.Reflection;
using Microsoft.Dnx.Runtime;

namespace Microsoft.AspNet.StressFramework
{
    public class Program
    {
        public Program(ILibraryManager libraries)
        {

        }

        public int Main(string[] args)
        {
            if(args.Length != 3)
            {
                Console.Error.WriteLine("Usage: stresshost <LOCK_NAME> <TEST_LIBRARY> <TEST_CLASS>");
                return -1;
            }

            var lockName = args[0];
            var libraryName = args[1];
            var className = args[2];

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
            if(!typeof(IStressTestHost).IsAssignableFrom(typ))
            {
                Console.Error.WriteLine($"{className} does not implement {typeof(IStressTestHost).FullName}");
                return -4;
            }

            // Construct the class
            var host = (IStressTestHost)Activator.CreateInstance(typ);

            // TODO: Wait on the lock!

            // Run the host
            host.Run(new StressTestHostContext());

            return 0;
        }
    }
}
