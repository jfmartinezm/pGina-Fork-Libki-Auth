using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using pGina.Shared.Settings;
using log4net;
using System.Diagnostics;
using System.Text.RegularExpressions;
using IniParser;
using IniParser.Model;

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
            m_settings.SetDefault("INIFilePath", "C:\\ProgramData\\Libki\\Libki Kiosk Management System.ini");
        }

        public static dynamic Store
        {
            get { return m_settings; }
        }

        public static int loadSettingsFromIni ()
        {
            string iniFilePath = Store.IniFilePath;
            if (File.Exists(iniFilePath))
            {
                var parser = new FileIniDataParser(new LibkiIniParser());
                // parser.Parser.Configuration.CommentString = ";";
                //parser.Parser.Configuration.AssigmentSpacer = " ";
                //parser.Parser.Configuration.CommentRegex = new Regex(@"(;?)(.*).*(?:;).*");
                IniData data = parser.ReadFile(iniFilePath);
                m_settings.SetSetting("ServerHost", data["server"]["host"]);
                m_settings.SetSetting("ServerPort", data["server"]["port"]);
                m_settings.SetSetting("ServerScheme", data["server"]["scheme"]);
                m_settings.SetSetting("NodeName", data["node"]["name"]);
                m_settings.SetSetting("NodeLocation", data["node"]["location"]);
                m_settings.SetSetting("NodeType", data["node"]["type"]);
                return 0;
            }
            else
            {
                return 1;
            }
        }
    }
}
