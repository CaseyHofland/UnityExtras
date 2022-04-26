#nullable enable
using System.IO;
using UnityEditor;

namespace UnityExtras.Editor
{
    [InitializeOnLoad]
    public class CopyGizmosToAssets
    {
        static CopyGizmosToAssets()
        {
            // Note that Gizmos~ is a hidden folder. This prevents them from showing up twice in the object picker or being unnecessarily picked up by Unity.
            CopyDirectory("Packages/com.CaseyDeCoder.UnityExtras/Editor/Gizmos~", "Assets/Gizmos");

            void CopyDirectory(string sourceDirectory, string destinationDirectory)
            {
                var directory = new DirectoryInfo(sourceDirectory);

                if (!directory.Exists)
                {
                    throw new DirectoryNotFoundException(
                        "Source directory does not exist or could not be found: "
                        + sourceDirectory);
                }

                // If the destination directory doesn't exist, create it.       
                Directory.CreateDirectory(destinationDirectory);

                // Get the files in the directory and copy them to the new location.
                FileInfo[] files = directory.GetFiles();
                foreach (FileInfo file in files)
                {
                    string tempPath = Path.Combine(destinationDirectory, file.Name);
                    if (!File.Exists(tempPath))
                    {
                        file.CopyTo(tempPath, false);
                    }
                }

                // Copy subdirectories and their contents to new location.
                DirectoryInfo[] subDirectories = directory.GetDirectories();
                foreach (DirectoryInfo subDirectory in subDirectories)
                {
                    string tempPath = Path.Combine(destinationDirectory, subDirectory.Name);
                    CopyDirectory(subDirectory.FullName, tempPath);
                }
            }
        }
    }
}

