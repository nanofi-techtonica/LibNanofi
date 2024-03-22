using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Text;

namespace LibNanofi.Build
{
    internal static class Util
    {
        public static string ExecuteCommand(string command, string args, string workingDir)
        {
            using (var process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = command,
                    Arguments = args,
                    RedirectStandardOutput = true,
                    StandardOutputEncoding = Encoding.UTF8,
                    RedirectStandardError = true,
                    StandardErrorEncoding = Encoding.UTF8,
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    WorkingDirectory = workingDir,
                }
            })
            {
                process.Start();
                process.WaitForExit();
                string result = process.StandardOutput.ReadToEnd();
                if (process.ExitCode != 0)
                {
                    var message = process.StandardError.ReadToEnd();
                    throw new Exception($"Command '{command} {args}' failed with exit code {process.ExitCode}:\n{message}");
                }
                return result;
            }
        }

        public static T WaitAndReturn<T>(this System.Threading.Tasks.Task<T> task)
        {
            task.Wait();
            return task.Result;
        }
    }
}
