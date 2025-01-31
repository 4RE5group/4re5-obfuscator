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
            base64Strings = bool.Parse(args[1]);
        }


        Console.WriteLine("Obfuscating files in: '" + args[0] + "'");

        string directoryPath = args[0]; // Change this path
        string outputPath = "Obfuscated";

        Directory.Delete(outputPath, true);
        Directory.CreateDirectory(outputPath);

        string[] files = Directory.GetFiles(directoryPath, "*.cs", SearchOption.AllDirectories);

        foreach (string file in files)
        {
            string code = File.ReadAllText(file);
            code = ObfuscateCode(code);
            File.WriteAllText(Path.Combine(outputPath, Path.GetFileName(file)), code);
            Console.WriteLine("Obfuscated: "+file);
        }
        Console.WriteLine("\nObfuscation complete.");
    }

    static string ObfuscateCode(string code)
    {
        // add usednamespaces
        if(!code.Contains("using System.Text;"))  // System.Text is used for base64 encoding
            code = "using System.Text;\n" + code;


        // Match namespaces, classes, methods, and variables
        string pattern = @"(?<![\w])(?:namespace|class|void|int|string|double|float|bool|char|long|short|byte|decimal)\s+(\w+)(?=\s*[{(])";
        
        code = Regex.Replace(code, pattern, match => {
            string originalName = match.Groups[1].Value;
            Console.WriteLine("Obfuscating: " + originalName + ", "+ match.Value);
            if(match.Value == "void Main" || match.Value == "class Program") { // pass _start
                return match.Value;
            }

            if (!nameMap.ContainsKey(originalName))
                nameMap[originalName] = GenerateRandomName();

            return match.Value.Replace(originalName, nameMap[originalName]);
        });


        // replace function calls
        foreach (var entry in nameMap)
        {
            code = code.Replace(entry.Key+"(", entry.Value+"(");
        }


        // replace strings with base64
        if(base64Strings) {
            string pattern2 = "\"(.*?)\"";
            code = Regex.Replace(code, pattern2, match => {
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
