using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using pGina.Shared.Types;
using pGina.Plugin.LibkiAuth;


namespace TestingExe
{
    class Program
    {
        static void Main(string[] args)
        {
            SessionProperties properties = new SessionProperties(new Guid("12345678-1234-1234-1234-123412341234"));
            UserInformation userInfo = new UserInformation();
            userInfo.Username = "01";
            userInfo.Password = "01";
            properties.AddTrackedSingle<UserInformation>(userInfo);

            PluginImpl plugin = new PluginImpl();

            var authResult = plugin.AuthenticateUser(properties);
            Debug.Assert(authResult.Success == true, authResult.Message);

            /*
            var gatewayResult = plugin.AuthenticatedUserGateway(properties);
            Debug.Assert(gatewayResult.Success == true, gatewayResult.Message);
            */

            System.Console.Write("DONE");
        }
    }
}
