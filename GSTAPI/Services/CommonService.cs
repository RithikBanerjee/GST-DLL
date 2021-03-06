using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using GSTAPI.Helper;
using GSTAPI.Models;
using System.Collections.Specialized;

namespace GSTAPI.Services
{
    //class to call all common based api 
    public static class CommonService
    {
        //authentication request api 
        public static Response AuthRequest(string username, string password, string appKey)
        {
            string payload;
            try
            {
                payload = JsonConvert.SerializeObject(new JObject()
                {
                    { "action", "ACCESSTOKEN" },
                    { "username", username },
                    { "password", CipherHandler.EncryptData(password, appKey) },
                    { "app_key", CipherHandler.EncryptTextWithGSTNPublicKey(appKey) }
                });
            }
            catch (Exception)
            {
                return RequestHandler.ErrorResponse("GSP141", "Error encrypting payload");
            }
            var handler = new RequestHandler();
            var url = UrlHandler.Route(accessGroup.commonapi, version.v0_2, modName.authenticate);
            return handler.Post(url, "ACCESSTOKEN", payload);
        }
        //log out api 
        public static Response Logout(string username, string appKey, string authToken)
        {
            string payload;
            try
            {
                payload = JsonConvert.SerializeObject(new JObject()
                {
                    { "action", "LOGOUT" },
                    { "username", username },
                    { "authtoken", authToken },
                    { "app_key", CipherHandler.EncryptTextWithGSTNPublicKey(appKey) }
                });
            }
            catch (Exception)
            {
                return RequestHandler.ErrorResponse("GSP141", "Error encrypting payload");
            }
            var handler = new RequestHandler();
            handler.UserRequest.Header.Username = username;
            handler.UserRequest.Header.AuthToken = authToken;
            var url = UrlHandler.Route(accessGroup.commonapi, version.v0_2, modName.authenticate);
            return handler.Post(url, "LOGOUT", payload);
        }
        //search taxpayer api 
        public static Response SearchTaxpayer(string username, string authToken, string gstinToSearch)
        {
            var queryString = new NameValueCollection
            {
                { "action", "TP" },
                { "gstin", gstinToSearch }
            };

            var handler = new RequestHandler();
            handler.UserRequest.Header.Username = username;
            handler.UserRequest.Header.AuthToken = authToken;
            var url = UrlHandler.Route(accessGroup.commonapi, version.v0_2, modName.search);
            return handler.Get(url, queryString);
        }
        //track return status api 
        public static Response TrackReturnStatus(string username, string authToken, CipherKeys keys, string gstin, string returnPeriod, string returnType)
        {
            var queryString = new NameValueCollection
            {
                { "action", "RETTRACK" },
                { "gstin", gstin },
                { "fy", returnPeriod },
                { "type", returnType }
            };

            var handler = new RequestHandler();
            handler.UserRequest.Keys = keys;
            handler.UserRequest.Header.Username = username;
            handler.UserRequest.Header.AuthToken = authToken;
            var url = UrlHandler.Route(accessGroup.commonapi, version.v1_0, modName.returns);
            return handler.DecryptGetResponse(url, queryString);
        } 
    }
}
