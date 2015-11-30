using System.Collections.Generic;
using System.Net;
using System.Linq;
using System.Web;
using System.Collections.Specialized;
using System.Text;
using System;
using Newtonsoft.Json.Linq;

namespace cbConsole
{
    public class CoinbaseConsumer
    {
        const string authorizeUrl = @"https://www.coinbase.com/oauth/authorize";
        const string accessTokenUrl = @"https://api.coinbase.com/oauth/token";

        private string _accessToken;
        private string _refreshToken;

        public string ClientId { get; set; }
        public string ClientSecret { get; set; }

        public CoinbaseConsumer(string clientId, string clientSecret)
        {
            ClientId = clientId;
            ClientSecret = clientSecret;
        }

        public string BeginAuth()
        {
            var requestArgs = new Dictionary<string, string>();
            requestArgs.Add("response_type", "code");
            requestArgs.Add("client_id", ClientId);
            requestArgs.Add("scope", "wallet:accounts:read");   // modify as needed

            var array = (from key in requestArgs.Keys
                         select string.Format("{0}={1}", HttpUtility.UrlEncode(key), HttpUtility.UrlEncode(requestArgs[key])))
                .ToArray();
            return authorizeUrl + "/?" + string.Join("&", array);
        }

        public string CompleteAuth(string verifier)
        {
            JObject o;
            using (var client = new WebClient())
            {
                var responseBytes =
                    client.UploadValues(accessTokenUrl, new NameValueCollection()
                    {
                        {"grant_type", "authorization_code" },
                        {"code", verifier },
                        {"client_id", ClientId },
                        {"client_secret", ClientSecret },
                        {"redirect_uri", @"https://loqu8.com" },
                    });
                
                
                // e.g., { "access_token":"xb7b51c9743595cf3111b10d275c8f3c6be20fcfad73c40aea83a12bf661b4d6","token_type":"bearer","expires_in":7200,"refresh_token":"xc1102ea910804c11b0e17a4a925e08ec655a095a6756de67aa886c6f621e890","scope":"wallet:accounts:read"}
                var json = Encoding.UTF8.GetString(responseBytes);
                o = JObject.Parse(json);
            }

            _accessToken = (string)o.SelectToken("access_token");
            _refreshToken = (string)o.SelectToken("refresh_token");

            return _accessToken;
        }

        public WebRequest PrepareAuthorizedRequest(string endpoint)
        {
            var webRequest = HttpWebRequest.Create(endpoint);
            webRequest.Headers["Authorization"] = "Bearer " + _accessToken;
            return webRequest;
        }
    }
}
