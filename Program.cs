using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ConsoleTables;

var dirsToCompare = new List<string>();

var path = string.Empty;

Console.WriteLine("Digite ou cole os diretorios, um por linha.. vazio para encerrar");

while (string.IsNullOrWhiteSpace(path = Console.ReadLine()) == false)
{
    dirsToCompare.Add(path);
}


var filesByDir = dirsToCompare
                .ToDictionary(path => path, path => 
                {
                    var files = GetFilesFrom(path);
                    var fileInfoByRelativeName = files
                        .ToDictionary(f => f.FullName.Substring(path.Length));
                        
                    return fileInfoByRelativeName;
                });

var allFileNames = filesByDir
                .SelectMany(x => x.Value)
                .Select(x => x.Key)
                .Distinct()
                .ToList();

 var table = new ConsoleTable(dirsToCompare.Prepend("File").ToArray());

foreach (var fileName in allFileNames)
{
    var lengths = dirsToCompare
        .Select(rootDirName => 
        {
            var localFiles = filesByDir[rootDirName];
            var fileLength = localFiles.TryGetValue(fileName, out var fileInfo)
                                ? fileInfo.Length
                                : (long?) null;

            return fileLength;
        })
        .ToArray();

    var allEquals = lengths.Distinct().Count() == 1;

    if (allEquals is false)
    {
        var values = lengths.Cast<object>().Prepend(fileName).ToArray();
        table.AddRow(values);
    }
}

table.Write();
Console.Read();

FileInfo[] GetFilesFrom(string dir) =>
    Directory
        .GetFiles(dir, "*.*", SearchOption.AllDirectories)
        .Select(x => new FileInfo(x))
        .ToArray();
