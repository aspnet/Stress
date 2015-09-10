using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Microsoft.AspNet.StressFramework.Hosting
{
    public class Program
    {
        public async Task<int> Main(string[] args)
        {
            StressTestTrace.SetIsHost();

            StressTestTrace.WriteLine("Host Process Launched");
            if (args.Length != 3)
            {
                Console.Error.WriteLine("Usage: stresshost <TEST_LIBRARY> <TEST_CLASS> <TEST_METHOD>");
                return 1;
            }

            var libraryName = args[0];
            var className = args[1];
            var methodName = args[2];

            // Find the class
            var asm = Assembly.Load(new AssemblyName(libraryName));
            if (asm == null)
            {
                Console.Error.WriteLine($"Failed to load assembly: {libraryName}");
                return 2;
            }
            var typ = asm.GetExportedTypes().FirstOrDefault(t => t.FullName.Equals(className));
            if (typ == null)
            {
                Console.Error.WriteLine($"Failed to locate type: {className} in {libraryName}");
                return 3;
            }
            var method = typ.GetMethods()
                .SingleOrDefault(m =>
                    m.Name.Equals(methodName) &&
                    typeof(StressRunSetup).IsAssignableFrom(m.ReturnType) &&
                    m.IsPublic &&
                    m.GetParameters().Length == 0);
            if (method == null)
            {
                Console.Error.WriteLine($"Failed to locate method: {methodName} in {className}");
                return 4;
            }

            // Construct the class and invoke the method
            var instance = Activator.CreateInstance(typ);
            var setup = (StressRunSetup)method.Invoke(instance, new object[0]);
            var iteratingHost = setup.Host as IteratingHost;

            if(iteratingHost == null)
            {
                Console.Error.WriteLine($"Wrong kind of host for use in the StressTestHostProcess! The host was a {setup.Host.GetType().FullName} but expected a {typeof(IteratingHost).FullName}");
                return 5;
            }

            StressTestTrace.WriteLine("Host Process Ready for release");

            // Read the release message from the standard input
            var released = Console.ReadLine();
            if (!string.Equals(StressTestHostProcess.ReleaseMessage, released, StringComparison.Ordinal))
            {
                Console.Error.WriteLine("Host process received invalid release message. Aborting");
                return 6;
            }

            // Run the iterations
            await iteratingHost.RunInHostProcessAsync(setup);

            StressTestTrace.WriteLine("Host Process Completed");
            return 0;
        }
    }
}
