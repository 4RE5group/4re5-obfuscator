using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Linq;


class ARES_OBFUSCATOR
{
    static Random random = new Random();
    static Dictionary<string, string> nameMap = new Dictionary<string, string>();
    static Dictionary<string, string> classesAndNamespaces = new Dictionary<string, string>();
    static bool base64Strings = false;
    
    static void Main(string[] args)
    {
        Console.WriteLine("4re5 group - 2025\n\n4re5 c# obfuscator");

        if (args.Length == 0)
        {
            Console.WriteLine("Usage: 4re5-obfuscator.exe <directory that contains .cs files>  <enable base64 encoding>");
            Console.WriteLine("Example: 4re5-obfuscator.exe C:\\Users\\user\\Desktop\\Project\\ true");
            return;
        }
        // enable base64 encoding
        if(args.Length == 2) {
            try {
                base64Strings = bool.Parse(args[1]);
            } catch {
                Console.WriteLine("Invalid argument for base64 encoding. Using default value: false");
            }
        }


        Console.WriteLine("Obfuscating files in: '" + args[0] + "'");

        string directoryPath = args[0]; // Change this path
        string outputPath = "Obfuscated";

        if(!Directory.Exists(directoryPath)) {
            Console.WriteLine("Source directory does not exist.");
            return;
        }

        if(Directory.Exists(outputPath)) {
            Console.WriteLine("CLEARING......");
            Directory.Delete(outputPath, true); 
        }
        
        Directory.CreateDirectory(outputPath);

        string[] files = Directory.GetFiles(directoryPath, "*.cs", SearchOption.AllDirectories);

        foreach (string file in files)
        {
            string code = File.ReadAllText(file);
            code = ObfuscateCode(code);
            Console.WriteLine("Obfuscated: "+file);

            // replace function calls
            foreach (var entry in nameMap)
            {
                code = code.Replace(entry.Key, entry.Value);
            }

            // replace classes and namespaces
            foreach (var entry in classesAndNamespaces)
            {
                code = code.Replace(entry.Key+".", entry.Value+".");
            }

            File.WriteAllText(Path.Combine(outputPath, Path.GetFileName(file)), code);
        }

        Console.WriteLine("\nObfuscation complete.");
    }

    static string ObfuscateCode(string code)
    {
         // replace function calls
        foreach (var entry in nameMap)
        {
            code = code.Replace(entry.Key+"(", entry.Value+"(");
        }

        // add usednamespaces
        if(!code.Contains("using System.Text;"))  // System.Text is used for base64 encoding
            code = "using System.Text;\n" + code;


        // Match namespaces, classes, methods
        string pattern = @"(?<![\w])(?:namespace|class|IntPtr|uint|void|int|string|double|float|bool|char|long|short|byte|decimal)\s+(\w+)(?=\s*[{(])";
        
        code = Regex.Replace(code, pattern, match => {
            string originalName = match.Groups[1].Value;
            var lines = code.Split('\n').Select((line, index) => new { Line = line, Number = index + 1 }).Where(l => l.Line.Contains(match.Value+"("));

            foreach (var line in lines) // don't obfuscate external functions ==> otherwise won't work
            {
                if (line.Line.Contains("DllImport") || line.Line.Contains("extern")) {
                    return match.Value; // is external
                }
            }

            Console.WriteLine("Obfuscating: " + originalName + ", "+ match.Value);
            if(match.Value == "void Main" || match.Value == "class Program") { // pass _start function
                return match.Value;
            }

            if(match.Value.Contains("class") || match.Value.Contains("namespace")) { // add to namespaces to after replace every class calls like "Class1.MyFunc" 
                            if(classesAndNamespaces.ContainsKey(originalName)) {
                    return match.Value.Replace(originalName, classesAndNamespaces[originalName]);
                }
                classesAndNamespaces[originalName] = GenerateRandomName();
                nameMap[originalName] = classesAndNamespaces[originalName];
            }

            if (!nameMap.ContainsKey(originalName) && !classesAndNamespaces.ContainsKey(originalName)) // check if already obfuscated
                nameMap[originalName] = GenerateRandomName(); // generate new name

            return match.Value.Replace(originalName, nameMap[originalName]);
        });


        // replace strings with base64
        if(base64Strings) {
            string pattern2 = @"@?""(.*?)""";
            
            code = Regex.Replace(code, pattern2, match => {
                var lines = code.Split('\n').Select((line, index) => new { Line = line, Number = index + 1 }).Where(l => l.Line.Contains('('+match.Value));
                foreach (var line in lines)
                {
                    if (line.Line.Contains("DllImport") || line.Line.Contains("extern"))
                    {
                        return match.Value; // is external
                    }
                }


                string originalText = match.Groups[1].Value;
                Console.WriteLine("     => BASE64 >> '"+originalText+"'");
                string base64Text = Convert.ToBase64String(Encoding.UTF8.GetBytes(originalText));
                return "Encoding.UTF8.GetString(Convert.FromBase64String(\""+base64Text+"\"))";
            });
        }
        


        return code;
    }

    static string GenerateRandomName()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return "_" + (new string(Enumerable.Repeat(chars, 8).Select(s => s[random.Next(s.Length)]).ToArray()));
    }
}
