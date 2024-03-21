using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibNanofi
{
    [BepInPlugin(PluginInfo.GUID, PluginInfo.NAME, PluginInfo.VERSION)]
    public class Plugin: BaseUnityPlugin
    {
        internal static new ManualLogSource Logger;
        internal static new ConfigFile Config;

        void Awake()
        {
            Logger = base.Logger;
            Config = base.Config;
            Logger.LogInfo($"Plugin: GUID={PluginInfo.GUID} Version={PluginInfo.VERSION}");
        }
    }
}
