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
            userInfo.Username = "L12345678";
            userInfo.Password = "1234";
            properties.AddTrackedSingle<UserInformation>(userInfo);

            LibkiAuthPlugin plugin = new LibkiAuthPlugin();

            System.Console.WriteLine("Authenticating User");
            var authResult = plugin.AuthenticateUser(properties);
            System.Console.WriteLine("Authentication result: " + authResult.Success + ", " + authResult.Message);

            /*
            var gatewayResult = plugin.AuthenticatedUserGateway(properties);
            Debug.Assert(gatewayResult.Success == true, gatewayResult.Message);
            */

            System.Console.Write("DONE");
        }
    }
}
