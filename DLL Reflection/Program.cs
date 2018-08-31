using System;
using System.IO;
using System.Reflection;
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.CSharp;
using System.CodeDom.Compiler;
using Microsoft.CSharp;

namespace DLL_Reflection
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = Assembly.GetEntryAssembly().Location;
            path = Path.GetDirectoryName(path);
            Console.WriteLine(path);
            string input = Console.ReadLine();
            DirectoryInfo d = new DirectoryInfo(path);
            var decompiler = new CSharpDecompiler(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + @"\" + "ClassLibrary.dll", new DecompilerSettings());
            string value = decompiler.DecompileWholeModuleAsString();
            string s = value.Substring(value.IndexOf("namespace "));
            Console.WriteLine(s);
            CompileCSharpCode(s, "testcompile.dll");
            foreach (var file in d.GetFiles("*.dll"))
            {
                Console.WriteLine(file.FullName);
            }
            Assembly DLL = Assembly.LoadFile(d.GetFiles("*.dll")[9].FullName);
            foreach (Type type in DLL.GetExportedTypes())
            {
                var c = Activator.CreateInstance(type);
                type.InvokeMember("Output", BindingFlags.InvokeMethod, null, c, new object[] { input });
            }
            Console.ReadLine();
        }

        public static bool CompileCSharpCode(string source, string exeFile)
        {
            source = "using System;" + source;

            CSharpCodeProvider provider = new CSharpCodeProvider();

            // Build the parameters for source compilation.
            CompilerParameters cp = new CompilerParameters();

            // Add an assembly reference.
            cp.ReferencedAssemblies.Add("System.dll");

            // Generate an executable instead of
            // a class library.
            cp.GenerateExecutable = true;

            // Set the assembly file name to generate.
            cp.OutputAssembly = exeFile;

            // Save the assembly as a physical file.
            cp.GenerateInMemory = false;

            // Invoke compilation.
            CompilerResults cr = provider.CompileAssemblyFromSource(cp, source);

            if (cr.Errors.Count > 0)
            {
                // Display compilation errors.
                Console.WriteLine("Errors building {0} into {1}",
                    source, cr.PathToAssembly);
                foreach (CompilerError ce in cr.Errors)
                {
                    Console.WriteLine("  {0}", ce.ToString());
                    Console.WriteLine();
                }
            }
            else
            {
                Console.WriteLine("Source {0} built into {1} successfully.",
                    source, cr.PathToAssembly);
            }

            // Return the results of compilation.
            if (cr.Errors.Count > 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}