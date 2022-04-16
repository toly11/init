using System;
using System.IO;
using System.Collections.Generic;
using System.Text.Json;

namespace init;

internal class Program
{

    static readonly string jsonFile = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly()!.Location)!, "config.json");

    static void Main(string[] args)
    {
        if (args.Length < 1 || args.Length > 2)
        {
            Console.WriteLine($"expected 1 to 2 arguments but got {args.Length}. exiting...");
            Environment.Exit(1);
        }

        if (!File.Exists(jsonFile))
        {
            Console.WriteLine("json file does not exists");

            try
            {
                var stream = File.CreateText(jsonFile);
                stream.WriteLine("[]");
                stream.Close();

                Console.WriteLine("created it for you, fill in some items and try again");
                Console.WriteLine(
                    "example of a schema:" + "\n" +
                    "{" + "\n" +
                    "  \"command\": \"web\"," + "\n" +
                    "  \"path\": \"c:\\users\\user\\schemas\\web\"" + "\n" +
                    "}"
                    );
            }
            catch
            {
                Console.WriteLine("error trying to create the file for you, create it manually and try again");
            }

            Environment.Exit(4);
        }

        string command = args[0], dest = Environment.CurrentDirectory;
        if (args.Length > 1)
        {
            string tmpDest = Path.Combine(Environment.CurrentDirectory, args[1]);
            if (!Directory.Exists(tmpDest))
            {
                try
                {
                    Directory.CreateDirectory(tmpDest);
                    dest = tmpDest;
                }
                catch
                {
                    Console.WriteLine($"error creating directory: '{tmpDest}'");
                    Environment.Exit(2);
                }
            }
        }

        string jsonData = File.ReadAllText(jsonFile);
        List<SchemaItem> items = JsonSerializer.Deserialize<List<SchemaItem>>(jsonData)!;

        var item = items.Find((item) => item.command == command);
        if (String.IsNullOrEmpty(item.path))
        {
            Console.WriteLine($"no item in the json matches the command: '{command}'");
            Environment.Exit(3);
        }

        Console.WriteLine($"executing {item.command}...");
        Move(item.path, dest);

        Console.WriteLine("Goodbye!");
        Environment.Exit(0);

    }

    static void Move(string src, string dest)
    {
        var srcFiles = Directory.GetFiles(src, "*.*", SearchOption.AllDirectories);
        Console.WriteLine($"src:\t\"{src}\" (found {srcFiles.Length} files)");
        Console.WriteLine($"dest:\t\"{dest}\"");
        Console.WriteLine();
        var srcDirLen = src.Length;

        foreach (var file in srcFiles)
        {
            var fileName = file.Substring(srcDirLen + 1);
            var targetFullName = Path.Combine(dest, fileName);
            var targetPath = Path.GetDirectoryName(targetFullName)!;
            Console.WriteLine($"copying:\t\"{fileName}\"");
            if (!Directory.Exists(targetPath))
            {
                Console.WriteLine("targetPath does not exist, creating it...");
                Directory.CreateDirectory(targetPath);
            }

            File.Copy(file, targetFullName, true);
        }
        Console.WriteLine();
    }
}








