using System.Diagnostics;

namespace SDLCompileAll
{
    internal class Program
    {


        const string SDLShaderCross = @"shadercross\shadercross.exe";
        static string Raw = @"Raw\";
        static string Compiled = @"Compiled\";

        static string DestinationFormat = "default";
        static string InputFormat = "default";

        static string GetHelpString()
        {
            return """
                SDLCompileAll - A tool for compiling shaders using SDLShaderCross
                Usage:
                SDLCompileAll [options]
                Options:
                -R, --raw <path>            Specify the path to the raw shader files (default: Raw\)

                -C, --compiled <path>       Specify the path to the compiled shader files (default: Compiled\)

                -D, --destination <format>  Specify the destination format (default, same)
                                            default: Create diffrent folder for each shader type.
                                            same: will put all shaders at same folder.

                -F                          Specify the source format (default, same)
                                            default: Shaders in (VS is vertex) in (Frag is fragment) and in (Compute is compute).
                                            ext, same, extention: Will find shader format based on extention.
                                            (input will be example.extention.hlsl or example.extention.glsl)
                                            (output will be .InType.OutType).
                                            (frag: .frag, .fragment, .pixel, .ps).
                                            (vert: .vs, .vertex, .vert)
                                            (comp: .comp, .compute)

                -S, --stop                  Wait for keys to finish (true, false or anything else)

                """;
        }

        // formats
        static string[] vertexFormats = { "vs", "vertex", "vert" };
        static string[] fragmentFormats = { "frag", "fragment", "pixel", "ps" };
        static string[] computeFormats = { "comp", "compute" };

        static bool WaitAfter = true;

        static bool IsInputExt()
        {
            return (InputFormat == "ext" || InputFormat == "same" || InputFormat == "extention");
        }
        static void SetRaw(string path)
        {
            Raw = path;
            if (!Raw.EndsWith('\\'))
            {
                Raw += "\\";
            }
        }
        static void SetCompiled(string path)
        {
            Compiled = path;
            if (!Compiled.EndsWith('\\'))
            {
                Compiled += "\\";
            }
        }

        static string HandleArgs(string[] args)
        {
            if (args.Length == 1 && args[0] == "--help")
            {
                return GetHelpString();
            }
            if (args.Length % 2 != 0)
            {
                return "Args are wrong. Please write --help";
            }
            for (int i = 0; i < args.Length; i+=2)
            {
                switch (args[i])
                {
                    case "--help":
                        Console.WriteLine(GetHelpString());
                        break;
                    case "-R":
                        SetRaw(args[i + 1]);
                        break;
                    case "--raw":
                        SetRaw(args[i + 1]);
                        break;
                    case "-C":
                        SetCompiled(args[i + 1]);
                        break;
                    case "--compiled":
                        SetCompiled(args[i + 1]);
                        break;
                    case "-D":
                        DestinationFormat = args[i + 1];
                        break;
                    case "-F":
                        InputFormat = args[i + 1];
                        break;
                    case "-S":
                        {
                            bool canWaitAfter = true;
                            WaitAfter = bool.TryParse(args[i + 1], out canWaitAfter) ? canWaitAfter : false;
                        }
                        break;
                    case "--stop":
                        {

                            bool canWaitAfter = true;
                            WaitAfter = bool.TryParse(args[i + 1], out canWaitAfter) ? canWaitAfter : false;
                        }
                        break;
                    default:
                        return "Args are wrong. Please write --help";
                }
            }
            return "\0";
        }

        static void Main(string[] args)
        {

            string argsmessage = HandleArgs(args);
            if (argsmessage != "\0")
            {
                Console.WriteLine(argsmessage);
                if (WaitAfter) Console.ReadKey();
                return;
            }

            if (!Path.Exists(Raw))
                Directory.CreateDirectory(Raw);

            if (DestinationFormat == "default")
            {
                if (!Path.Exists(Raw + @"VS\"))
                    Directory.CreateDirectory(Raw + @"VS\");
                if (!Path.Exists(Raw + @"Frag\"))
                    Directory.CreateDirectory(Raw + @"Frag\");
                if (!Path.Exists(Raw + @"Compute\"))
                    Directory.CreateDirectory(Raw + @"Compute\");
            }

            if (!Path.Exists(Compiled))
                Directory.CreateDirectory(Compiled);
            
            if (!File.Exists(SDLShaderCross))
            {
                Console.WriteLine(SDLShaderCross + " Does not Exists");

                if (WaitAfter) Console.ReadKey();
                return;
            }
            if (true)
            {

            }

            if (InputFormat == "default")
            {

                Console.WriteLine("Start Vertex Shaders");
                CompileFromDirectories(Raw + @"VS\", "vertex");
                Console.WriteLine("Start Fragment Shader");
                CompileFromDirectories(Raw + @"Frag\", "fragment");
                Console.WriteLine("Start Compute Shaders");
                CompileFromDirectories(Raw + @"Compute\", "compute");
            }
            else if (IsInputExt())
            {
                Console.WriteLine("Compiling all shaders: ");
                CompileFromDirectories(Raw, "BasedOnExtentions");
            }


            Console.WriteLine("\n\n\nJob Finished!");

            if (WaitAfter) Console.ReadKey();
            return;
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

        static bool IsVertex(string ext)
        {
            for (int i = 0; i < vertexFormats.Length; i++)
            {
                if (ext == vertexFormats[i])
                {
                    return true;
                }
            }
            return false;
        }
        static bool IsFragment(string ext)
        {
            for (int i = 0; i < fragmentFormats.Length; i++)
            {
                if (ext == fragmentFormats[i])
                {
                    return true;
                }
            }
            return false;
        }
        static bool IsCompute(string ext)
        {
            for (int i = 0; i < computeFormats.Length; i++)
            {
                if (ext == computeFormats[i])
                {
                    return true;
                }
            }
            return false;
        }

        static string FindFileTypeDynamic(string file)
        {
            file = file.Replace(Path.GetExtension(file), "");
            string ext = Path.GetExtension(file).Replace(".", "");
            Console.WriteLine(file);
            if (IsVertex(ext))
            {
                return "vertex";
            }
            else if (IsFragment(ext))
            {
                return "fragment";
            }
            else if (IsCompute(ext))
            {
                return "compute";
            }
            else
            {
                ConsoleColor lastcolor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("        " + file + " ____ Not Compiled to. " + ext.ToUpper() + "\n");
                Console.ForegroundColor = lastcolor;
            }
            return "none";
        }

        static void CompileFromFiles(string directory, string type)
        {
            string[] files = Directory.GetFiles(directory);

            if (type == "BasedOnExtentions")
            {
                foreach (string file in files)
                {
                    string newtype = FindFileTypeDynamic(file);
                    if (newtype == "none") continue;
                    CompileTheFile(file, newtype);
                }
            }
            else
            {
                foreach (string file in files)
                {
                    CompileTheFile(file, type);
                }
            }

        }

        static void CompileTheFile(string file, string type)
        {
            string[] types = { "json", "hlsl", "spv", "msl", "dxil", "dxbc" };

            string Dir = file;
            foreach (string item in types)
            {

                string destDir = "";
                if (DestinationFormat == "default")
                {
                    destDir = file.Replace(Raw, Compiled + item.ToUpper() + @"\").Replace(Path.GetExtension(file).Replace(".", ""), item);
                }
                else if (DestinationFormat == "same")
                {
                    if (InputFormat == "default")
                    {
                        destDir = file.Replace(Raw, Compiled).Replace(Path.GetExtension(file).Replace(".", ""), type.Remove(4)) + "." + item;
                        destDir = destDir.Replace("VS\\", "").Replace("Frag\\", "").Replace("Compute\\", "");
                    }
                    else if (IsInputExt())
                    {
                        destDir = file.Replace(Raw, Compiled).Replace(Path.GetExtension(file).Replace(".", ""), "") + item;
                    }
                }

                if (!Directory.Exists(Path.GetDirectoryName(destDir)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(destDir));
                }
                string rawDir = Directory.GetCurrentDirectory() + "\\" + Dir;


                // just copy old spv if its already is
                if (Path.GetExtension(file) == ".spirv" && item == "spv")
                {

                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine("        " + rawDir + " ____ Trying copy.");
                    Console.ForegroundColor = ConsoleColor.White;
                    File.Copy(file, destDir, true);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.SetCursorPosition(0, Console.GetCursorPosition().Top - 1);
                    Console.WriteLine("        " + rawDir + " ____ Copied.");
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("        Dest : " + destDir + " \n");
                    Console.ForegroundColor = ConsoleColor.White;
                    continue;
                }

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
