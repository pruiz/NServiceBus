using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Configuration;

namespace NServiceBus.Host.Internal
{
    /// <summary>
    /// Helpers for assembly scanning operations
    /// </summary>
    public class AssemblyScanner
    {
		public static Func<FileInfo, bool> AssemblyFilter = null;

		private static string[] GetExcludedAssemblies()
		{
			var exclusions = ConfigurationManager.AppSettings["NServiceBus:AssemblyScanner:Exclude"];
Console.WriteLine("==> Exclusions List => {0}", exclusions);
			if (exclusions == null) return new string[] {};
			return exclusions.Split(',').Select(x => x.Trim().ToLowerInvariant()).ToArray();
		}

        /// <summary>
        /// Gets a list with assemblies that can be scanned
        /// </summary>
        /// <returns></returns>
        [DebuggerNonUserCode] //so that exceptions don't jump at the developer debugging their app
        public static IEnumerable<Assembly> GetScannableAssemblies()
        {
			var exclusions = GetExcludedAssemblies();
			var assemblyFiles = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).GetFiles("*.dll", SearchOption.AllDirectories)
			 	.Union(new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).GetFiles("*.exe", SearchOption.AllDirectories));

			if (AssemblyFilter != null)
				assemblyFiles = assemblyFiles.Where(AssemblyFilter);

            foreach (var assemblyFile in assemblyFiles)
            {
                Assembly assembly;

		if (exclusions.Contains(assemblyFile.Name.ToLowerInvariant())) {
Console.WriteLine("===> [NOT] Scanning Assembly => {0}", assemblyFile.Name);
			continue;
		}

                try
                {
Console.WriteLine("===> Scanning Assembly => {0} ({1})", assemblyFile.FullName, assemblyFile.Name);
                    assembly = Assembly.LoadFrom(assemblyFile.FullName);

                    //will throw if assembly cant be loaded
                    assembly.GetTypes();
                }
                catch
                {
                    continue;
                }

                yield return assembly;


            }


        }
    }
}
