using System.Diagnostics;

namespace SDLCompileAll
{
    internal class Program
    {
        const string SDLShaderCross = @"shadercross\shadercross.exe";
        const string Raw = @"Raw\";
        const string Compiled = @"Compiled\";
        static void Main(string[] args)
        {
            if (!Path.Exists(Raw))
                Directory.CreateDirectory(Raw);

            if (!Path.Exists(Raw + @"VS\"))
                Directory.CreateDirectory(Raw + @"VS\");
            if (!Path.Exists(Raw + @"Frag\"))
                Directory.CreateDirectory(Raw + @"Frag\");
            if (!Path.Exists(Raw + @"Compute\"))
                Directory.CreateDirectory(Raw + @"Compute\");

            if (!Path.Exists(Compiled))
                Directory.CreateDirectory(Compiled);
            
            if (!File.Exists(SDLShaderCross))
            {
                Console.WriteLine(SDLShaderCross + " Does not Exists");
                return;
            }
            Console.WriteLine("Start Vertex Shaders");
            CompileFromDirectories(Raw + @"VS\", "vertex");
            Console.WriteLine("Start Fragment Shader");
            CompileFromDirectories(Raw + @"Frag\", "fragment");
            Console.WriteLine("Start Compute Shaders");
            CompileFromDirectories(Raw + @"Compute\", "compute");


            Console.WriteLine("\n\n\nJob Finished!");
            Console.ReadKey();
        }

        static void CompileFromDirectories(string directory, string type)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Compiling Files Of : " + directory);
            Console.ForegroundColor = ConsoleColor.White;
            string[] dirs = Directory.GetDirectories(directory);
            foreach (string dir in dirs)
            {
                CompileFromDirectories(dir, type);
            }
            CompileFromFiles(directory, type);
        }

        static void CompileFromFiles(string directory, string type)
        {
            string[] files = Directory.GetFiles(directory);
            foreach (string file in files)
            {
                CompileTheFile(file, type);
            }
        }

        static void CompileTheFile(string file, string type)
        {
            string[] types = { "json", "hlsl", "spv", "msl", "dxil", "dxbc" };

            string Dir = file;
            foreach (string item in types)
            {

                string destDir = Directory.GetCurrentDirectory() + "\\" + file.Replace(Raw, Compiled + item.ToUpper() +@"\").Replace(Path.GetExtension(file).Replace(".", ""), item);

                if (!Directory.Exists(Path.GetDirectoryName(destDir)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(destDir));
                }
                string rawDir = Directory.GetCurrentDirectory() + "\\" + Dir;


                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("        " + rawDir + " ____ Compiling.");
                Console.ForegroundColor = ConsoleColor.White;

                string commands = "";

                Path.GetExtension(file);

                commands = "-s " + Path.GetExtension(file).Replace(".", "").ToUpper();
                commands += " \"" + rawDir+"\"";
                commands += " -o \"" + destDir + "\"";
                // vertex, fragment, compute
                commands += " -t " + type;
                ///

                {
                    // Create a new process start info
                    ProcessStartInfo startInfo = new ProcessStartInfo
                    {
                        FileName = SDLShaderCross,
                        Arguments = commands,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true
                    };

                    // Start the process
                    using (Process process = new Process { StartInfo = startInfo })
                    {
                        process.Start();

                        // Read the output (optional)
                        string output = process.StandardOutput.ReadToEnd();
                        string error = process.StandardError.ReadToEnd();

                        process.WaitForExit();

                        if (output != "")
                        {
                            Console.WriteLine("Output: " + output);
                        }
                        // Display the output (optional)
                        if (error != "")
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.SetCursorPosition(0, Console.GetCursorPosition().Top - 1);
                            Console.WriteLine("        " + rawDir + " ____ Not Compiled to. " + item.ToUpper() + "\n");
                            Console.ForegroundColor = ConsoleColor.White;

                            Console.WriteLine("       " + error);
                        }
                        else
                        {
                            ///

                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.SetCursorPosition(0, Console.GetCursorPosition().Top - 1);
                            Console.WriteLine("        " + rawDir + " ____ Compiled.     ");
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine("        Dest : " + destDir + " \n");
                            Console.ForegroundColor = ConsoleColor.White;
                        }
                    }
                    Thread.Sleep(1);
                }
            }
        }
    }
}
