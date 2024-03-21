using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace LibNanofi.Build.Tasks
{
    public class MakeBumpedVersionCommit : Task
    {
        [Required]
        public string RootDir { get; set; }
        public string ChangelogFile { get; set; } = "CHANGELOG.md";
        [Output]
        public string NextVersion { get; set; }

        public override bool Execute()
        {
            Log.LogMessage(MessageImportance.High, "Making bumped version commit...");
            try
            {
                var nextVersion = Util.ExecuteCommand("git", "cliff --bumped-version", RootDir).Trim();
                Util.ExecuteCommand("git", $"cliff --bump -o {ChangelogFile}", RootDir);
                Util.ExecuteCommand("git", $"add {ChangelogFile}", RootDir);
                Util.ExecuteCommand("git", $"commit -m \"chore(release): update {ChangelogFile} for {nextVersion}\"", RootDir);
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
