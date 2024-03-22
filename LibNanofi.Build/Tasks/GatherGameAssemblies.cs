using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LibNanofi.Build.Tasks
{
    public class GatherGameAssemblies : Task
    {
        [Required]
        public string ConfigPath { get; set; } = "";

        [Required]
        public ITaskItem[] Dependencies { get; set; } = new ITaskItem[0];

        [Output]
        public ITaskItem[] CopiedAssemblies => _copiedAssemblies;
        private ITaskItem[] _copiedAssemblies = new ITaskItem[0];

        public override bool Execute()
        {
            Log.LogMessage(MessageImportance.High, "Gathering game assemblies...");
            try
            {
                Log.LogMessage(MessageImportance.Normal, $"Loading config from {ConfigPath}");
                var config = Config.Load(ConfigPath);
                if (config == null)
                {
                    Log.LogError("Failed to load config");
                    return false;
                }
                var assemblies = new List<ITaskItem>();
                foreach (var dep in Dependencies)
                {
                    var name = dep.GetMetadata("FileName");
                    Log.LogMessage(MessageImportance.Normal, $"Checking {name}");
                    var path = FindGameAssembly(config, name);
                    if (path != null)
                    {
                        Log.LogMessage(MessageImportance.Normal, $"Found {name} at {path}");
                    }
                    else
                    {
                        Log.LogError($"Failed to find {name}");
                        continue;
                    }
                    try
                    {
                        var destPath = dep.GetMetadata("FullPath");
                        var destDir = Path.GetDirectoryName(destPath);
                        Log.LogMessage(MessageImportance.Normal, $"Ensure directory {destDir}");
                        Directory.CreateDirectory(destDir);
                        Log.LogMessage(MessageImportance.Normal, $"Copying {path} to {destPath}");
                        File.Copy(path, destPath, true);
                        assemblies.Add(new TaskItem(destPath));
                    }
                    catch (Exception e)
                    {
                        Log.LogErrorFromException(e, showStackTrace: true);
                    }
                }
                _copiedAssemblies = assemblies.ToArray();
            }
            catch (Exception e)
            {
                Log.LogErrorFromException(e, showStackTrace: true);
                return false;
            }
            return !Log.HasLoggedErrors;
        }

        private string? FindGameAssembly(Config config, string assemblyName)
        {
            var assemblyPath = Path.Combine(config.GameRoot, "Techtonica_Data", "Managed", $"{assemblyName}.dll");
            if (File.Exists(assemblyPath))
            {
                return assemblyPath;
            }
            assemblyPath = Path.Combine(config.BepinExRoot, "core", $"{assemblyName}.dll");
            if (File.Exists(assemblyPath))
            {
                return assemblyPath;
            }
            return null;
        }
    }
}
