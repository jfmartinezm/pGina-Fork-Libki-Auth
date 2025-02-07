using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using pGina.Shared.Settings;
using log4net;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace pGina.Plugin.LibkiAuth
{
    class Settings
    {
        private static dynamic m_settings = new pGinaDynamicSettings(LibkiAuthPlugin.PluginUuid);
        private static ILog m_logger = LogManager.GetLogger("LibkiAuthSettings");

        static Settings()
        {
            // Set default values for settings (if not already set)  
            m_settings = new pGinaDynamicSettings(LibkiAuthPlugin.PluginUuid);
            m_settings.SetDefault("ServerHost", "localhost");
            m_settings.SetDefault("ServerPort", 3000);
            m_settings.SetDefault("ServerScheme", "http");
            m_settings.SetDefault("NodeName", System.Environment.GetEnvironmentVariable("COMPUTERNAME"));
            m_settings.SetDefault("NodeLocation", "");
            m_settings.SetDefault("NodeType", "");
        }

        public static dynamic Store
        {
            get { return m_settings; }
        }
    }
}
