using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibNanofi.Build.Tasks
{
    public class CreateGitTag : Task
    {
        [Required]
        public string RootDir { get; set; } = "";
        [Required]
        public string Name { get; set; } = "";

        public bool Push { get; set; } = false;
        public string Remote { get; set; } = "origin";

        public override bool Execute()
        {
            try
            {
                Util.ExecuteCommand("git", $"tag {Name}", RootDir);
                if (Push)
                {
                    Util.ExecuteCommand("git", $"push {Remote} {Name}", RootDir);
                }
            }catch(Exception e)
            {
                Log.LogErrorFromException(e);
            }
            return !Log.HasLoggedErrors;
        }
    }
}
