﻿using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibNanofi.Build
{
    public sealed class Config
    {
        public string GameRoot { get; set; }
        public string BepinExRoot { get; set; }

        public static Config Load(string path)
        {
            return new ConfigurationBuilder()
                .AddJsonFile(path)
                .Build()
                .Get<Config>();
        }
    }
}
