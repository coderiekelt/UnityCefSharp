using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.IO;

public class ChromiumPostBuild
{
    [PostProcessBuildAttribute(1)]
    public static void OnPostprocessBuild(BuildTarget target, string path)
    {
        string targetPath = Path.GetDirectoryName(path) + @"\CefServer";
        string sourcePath = Application.dataPath + @"\..\CefServer";

        if (!Directory.Exists(Path.GetDirectoryName(path) + @"\CefServer")) {
            Directory.CreateDirectory(Path.GetDirectoryName(path) + @"\CefServer");
        }

        CopyAll(new DirectoryInfo(sourcePath), new DirectoryInfo(targetPath));
    }

    // All hail stack overflow
    private static void CopyAll(DirectoryInfo source, DirectoryInfo target)
    {
        // Copy each file into the new directory.
        foreach (FileInfo fi in source.GetFiles())
        {
            fi.CopyTo(Path.Combine(target.FullName, fi.Name), true);
        }

        // Copy each subdirectory using recursion.
        foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
        {
            DirectoryInfo nextTargetSubDir = target.CreateSubdirectory(diSourceSubDir.Name);

            CopyAll(diSourceSubDir, nextTargetSubDir);
        }
    }
}
