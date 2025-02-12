using System;
using System.DirectoryServices.AccountManagement;
using System.Diagnostics;

using log4net;

using pGina.Shared.Interfaces;
using pGina.Shared.Types;
using pGina.Shared.Settings;

namespace pGina.Plugin.LibkiAuth
{
    public class LibkiAuthPlugin : IPluginConfiguration, IPluginAuthentication
    {
        private static ILog m_logger = LogManager.GetLogger("LibkiAuth");
        public static Guid PluginUuid = new Guid("{123C9161-AD2B-4E2C-AFCD-A36BC2B763A7}");
        private string m_defaultDescription = "Plugin for Libki user authentication and authorization";
        private dynamic m_settings = null;

        #region Init-plugin

        public LibkiAuthPlugin()
        {
            using (Process me = Process.GetCurrentProcess())
            {
                m_settings = new pGinaDynamicSettings(PluginUuid);
                m_settings.SetDefault("ShowDescription", true);
                m_settings.SetDefault("Description", m_defaultDescription);
                m_logger.DebugFormat("Plugin initialized on {0} in PID: {1} Session: {2}", Environment.MachineName, me.Id, me.SessionId);
            }
        }

        public string Name
        {
            get { return "LibkiAuth"; }
        }

        public string Description
        {
            get { return m_settings.Description; }
        }

        public string Version
        {
            get
            {
                return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }

        public Guid Uuid
        {
            get { return PluginUuid; }
        }

        #endregion

        public void Starting() { }

        public void Stopping() { }

        public void Configure()
        {
            Configuration dialog = new Configuration();
            dialog.ShowDialog();
        }

        public BooleanResult AuthenticateUser(SessionProperties properties)
        {
            // this method shall say if our credentials are valid
            UserInformation userInfo = properties.GetTrackedSingle<UserInformation>();
            m_logger.DebugFormat("LibkiAuth: Authenticate user {0}", userInfo.Username);

            m_logger.DebugFormat("LibkiAuth: Parsing INI data from {0}", Settings.Store.INIFilePath);
            int iniDataParseError = Settings.loadSettingsFromIni();

            m_logger.DebugFormat("LibkiAuth: Parsing data result: {0}", iniDataParseError);
            m_logger.DebugFormat("LibkiAuth: Server settings: scheme {0}, host {1}, port {2}",
                Settings.Store.ServerScheme, Settings.Store.ServerHost, Settings.Store.ServerPort);
            m_logger.DebugFormat("LibkiAuth: Node settings: name {0}, location {1}, type {2}",
                Settings.Store.NodeName, Settings.Store.NodeLocation, Settings.Store.NodeType);

            BooleanResult result = LibkiClientAPI.registerNode();
            m_logger.DebugFormat("LibkiAuth: Node registration result: {0}, {1}", result.Success, result.Message);

            if (result.Success)
            {
                result = LibkiClientAPI.login(userInfo.Username, userInfo.Password);
                m_logger.DebugFormat("LibkiAuth: Authentication result: {0}, {1}", result.Success, result.Message);
            }

            return result;
        }

    }
}
