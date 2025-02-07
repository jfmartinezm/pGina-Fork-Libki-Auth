using System;
using System.Collections.Generic;
using System.Net;
using System.IO;

using pGina.Shared.Types;
using log4net;
using System.Text;
using Newtonsoft.Json.Linq;

namespace pGina.Plugin.LibkiAuth
{
    public class LibkiClientAPI
    {
        private static ILog m_logger = LogManager.GetLogger("LibkiClientAPI");
        private static int requestTimeout = 10000;
        private static string apiBaseURL = "/api/client/v1_0";

        static LibkiClientAPI()
        {
        }

        public static BooleanResult sendAPIRequest(string URL, string data)
        {
            try
            {
                // Create request 
                WebRequest request = WebRequest.Create(URL);
                request.Timeout = requestTimeout;
                if (data != null)
                {
                    request.Method = "POST";

                    // POST data must be converted to a byte array.
                    byte[] byteArray = Encoding.UTF8.GetBytes(data);
                    request.ContentType = "application/x-www-form-urlencoded";
                    request.ContentLength = byteArray.Length;
                    // Send request
                    using (Stream dataStream = request.GetRequestStream())
                    {
                        dataStream.Write(byteArray, 0, byteArray.Length);
                    }
                }

                // Get the response.
                using (WebResponse response = request.GetResponse())
                {
                    using (Stream dataStream = response.GetResponseStream())
                    {
                        // Open the stream using a StreamReader for easy access.
                        using (StreamReader reader = new StreamReader(dataStream))
                        {
                            // Read the content.
                            string responseFromServer = reader.ReadToEnd();
                            // Display the content.
                            m_logger.DebugFormat("Response: {0}", responseFromServer);

                            return new BooleanResult() { Success = true, Message = responseFromServer };
                        }
                    }
                }
            }

            catch (WebException webx)
            {
                string errorString;

                using (HttpWebResponse res = (HttpWebResponse)webx.Response)
                {
                    if (res != null)
                    {
                        using (StreamReader resReader = new StreamReader(res.GetResponseStream()))
                        {
                            string responseBody = resReader.ReadLine();
                            if (responseBody.Length > 0)
                            {
                                errorString = responseBody;
                            }
                            else
                            {
                                errorString = webx.Message;
                            }
                        }
                    }
                    else
                    {
                        errorString = webx.Message;
                    }
                }

                m_logger.ErrorFormat("WebException sending API request: {0}", errorString);
                return new BooleanResult() { Success = false, Message = "sendApiRequest: " + errorString };
            }

            catch (Exception e)
            {
                // very bad scenario
                m_logger.ErrorFormat("Generic Exception sending API request: {0}", "sendApiRequest: " + e.StackTrace);
                return new BooleanResult() { Success = false, Message = "sendApiRequest: " +  e.Message };
            }

        }

        public static BooleanResult registerNode()
        {
            string apiURI = Settings.Store.ServerScheme + "://" +
                Settings.Store.ServerHost + ":" +
                Settings.Store.ServerPort + apiBaseURL;

            m_logger.DebugFormat("LibkiClientAPI: registerNode API URI {0}", apiURI);

            string apiParameters = "action=register_node" +
                "&node_name=" + Settings.Store.NodeName +
                "&location=" + Settings.Store.NodeLocation +
                "&type=" + Settings.Store.NodeType; 

            m_logger.DebugFormat("LibkiClientAPI: registerNode API parameters {0}", apiParameters);

            BooleanResult apiResult = sendAPIRequest(apiURI + "?" + apiParameters, null);

            if (apiResult.Success)
            {
                var registered = JObject.Parse(apiResult.Message)["registered"];
                if (registered == null)
                {
                    // Registered property not found, check error condition
                    var error = JObject.Parse(apiResult.Message)["error"];
                    if (error == null)
                    {
                        // Bad response format, should not happen
                        throw new Exception("Bad response format: " + apiResult.Message);
                    }
                    else
                    {
                        return new BooleanResult() { Success = false, Message = "Node not registered: " + error.ToString() };
                    }
                }
                else
                {
                    if (!registered.ToString().Equals("1"))
                    {
                        // Registered property exists, but it is not 1 (unsupported case)
                        throw new Exception("Registration error: " + registered.ToString());
                    }
                    return new BooleanResult() { Success = true };
                }
            }
            else 
            {
                return new BooleanResult() { Success = false, Message = "LibkiClientAPI: error: " + apiResult.Message};
            }
        }

        public static BooleanResult login(string username, string password)
        {
            string apiURI = Settings.Store.ServerScheme + "://" +
                Settings.Store.ServerHost + ":" +
                Settings.Store.ServerPort + apiBaseURL;

            m_logger.DebugFormat("LibkiClientAPI: login API URI {0}", apiURI);

            string apiParameters = "action=login" +
                "&node=" + Settings.Store.NodeName +
                "&location=" + Settings.Store.NodeLocation +
                "&type=" + Settings.Store.NodeType;

            string loginData = "username=" + username + "&password=" + password;

            m_logger.DebugFormat("LibkiClientAPI: login API parameters username={0}&password=XXXXXX", username);

            BooleanResult apiResult = sendAPIRequest(apiURI + "?" + apiParameters, loginData);

            if (apiResult.Success)
            {
                var authenticated = JObject.Parse(apiResult.Message)["authenticated"];
                if (authenticated == null)
                {
                    // Authenticated property not found, check error condition
                    var error = JObject.Parse(apiResult.Message)["error"];
                    if (error == null)
                    {
                        // Bad response format, should not happen
                        throw new Exception("Bad response format: " + apiResult.Message);
                    }
                    else
                    {
                        return new BooleanResult() { Success = false, Message = "User not authenticated: " + error.ToString() };
                    }
                }
                else
                {
                    if (!authenticated.ToString().Equals("1"))
                    {
                        // Authenticated property exists, but it is not 1 (unsupported case)
                        throw new Exception("Registration error: " + authenticated.ToString());
                    }
                    return new BooleanResult() { Success = true };
                }
            }
            else
            {
                return new BooleanResult() { Success = false, Message = "LibkiClientAPI: error: " + apiResult.Message };
            }
        }
    }
}
