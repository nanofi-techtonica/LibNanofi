using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Xml.Linq;

namespace LibNanofi.Build.Tasks
{
    public class MakeBumpedVersionCommit : Task
    {
        [Required]
        public string RootDir { get; set; } = "";
        public string ChangelogFile { get; set; } = "CHANGELOG.md";

        public bool Push { get; set; } = false;
        public string Remote { get; set; } = "origin";

        [Output]
        public string NextVersion { get; set; } = "0.1.0";

        public override bool Execute()
        {
            Log.LogMessage(MessageImportance.High, "Making bumped version commit...");
            try
            {
                var nextVersion = Util.ExecuteCommand("git", "cliff --bumped-version", RootDir).Trim();
                Util.ExecuteCommand("git", $"cliff --bump -o {ChangelogFile}", RootDir);
                Util.ExecuteCommand("git", $"add {ChangelogFile}", RootDir);
                Util.ExecuteCommand("git", $"commit -m \"chore(release): update {ChangelogFile} for version {nextVersion}\"", RootDir);
                if (Push)
                {
                    var branch = Util.ExecuteCommand("git", "rev-parse --abbrev-ref HEAD", RootDir).Trim();
                    Util.ExecuteCommand("git", $"push {Remote} {branch}", RootDir);
                }
                NextVersion = nextVersion;
            }
            catch (Exception e)
            {
                Log.LogErrorFromException(e);
            }
            return !Log.HasLoggedErrors;
        }
    }
}
