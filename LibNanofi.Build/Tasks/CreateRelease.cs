using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Octokit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LibNanofi.Build.Tasks
{
    public class CreateRelease : Task
    {
        [Required]
        public string RootDir { get; set; } = "";
        [Required]
        public string ConfigPath { get; set; } = "";
        [Required]
        public string Version { get; set; } = "0.1.0";

        [Required]
        public ITaskItem[] Artifacts { get; set; } = new ITaskItem[0];

        public string GitRemote { get; set; } = "origin";

        public override bool Execute()
        {
            Log.LogMessage(MessageImportance.High, "Creating a release...");
            try
            {
                Log.LogMessage(MessageImportance.Normal, $"Loading config from {ConfigPath}");
                var config = Config.Load(ConfigPath);
                if (config == null)
                {
                    Log.LogError("Failed to load config");
                    return false;
                }
                var url = Util.ExecuteCommand("git", $"remote get-url {GitRemote}", RootDir).Trim();
                if (!url.StartsWith("git@github.com:"))
                {
                    Log.LogError($"Remote URL of {GitRemote} must be a github's ssh url");
                    return false;
                }
                var repoParts = url.Split(':')[1].Split('.')[0].Split('/');
                var owner = repoParts[0];
                var repo = repoParts[1];
                var body = Util.ExecuteCommand("git", "cliff -l", RootDir);

                var client = new GitHubClient(new ProductHeaderValue("LibNanofi"));
                client.Credentials = new Credentials(config.GitHubToken);
                Log.LogMessage(MessageImportance.Normal, $"Creating release {Version}...");
                var release = client.Repository.Release.Create(owner, repo, new NewRelease($"v{Version}")
                {
                    Name = $"Version {Version}",
                    Body = body,
                    Draft = false,
                    Prerelease = false
                }).WaitAndReturn();
                Log.LogMessage(MessageImportance.Normal, $"Release {Version} created");
                Log.LogMessage(MessageImportance.Normal, "Uploading artifacts...");
                foreach (var item in Artifacts)
                {
                    var path = item.GetMetadata("FullPath");
                    var name = Path.GetFileName(path);
                    if (MimeTypes.TryGetMimeType(name, out string? contentType))
                    {
                        contentType = "application/octet-stream";
                    }
                    Log.LogMessage(MessageImportance.Normal, $"Uploading {name}...");
                    using (var stream = File.OpenRead(path))
                    {
                        _ = client.Repository.Release.UploadAsset(release, new ReleaseAssetUpload(name, contentType, stream, null)).WaitAndReturn();
                    }
                    Log.LogMessage(MessageImportance.Normal, $"{name} uploaded");
                }
            }
            catch (Exception e)
            {
                Log.LogErrorFromException(e);
            }
            return !Log.HasLoggedErrors;
        }
    }
}
