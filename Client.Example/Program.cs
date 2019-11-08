using IdentityModel.Client;
using IdentityServer4.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Client.Example
{
    class Program
    {
        private static async Task Main()
        {
            await MyProgram();
            Console.ReadLine();
        }

        public static async Task MyProgram()
        {
            // discover endpoints from metadata
            var client = new HttpClient();
            var disco = await client.GetDiscoveryDocumentAsync("http://localhost:5005");
            if (disco.IsError)
            {
                Console.WriteLine(disco.Error);
                return;
            }

            Console.WriteLine("Request AccessToken:\n");
            // request token
            var tokenResponse = await client.RequestPasswordTokenAsync(new PasswordTokenRequest
            {
                Address = disco.TokenEndpoint,
                GrantType = "password",

                ClientId = "ro.client",
                ClientSecret = "ClientSecret",
                Scope = "MyFundsApi offline_access",

                UserName = "Admin",
                Password = "Admin1"
            });
            if (tokenResponse.IsError)
            {
                Console.WriteLine(tokenResponse.Error);
                return;
            }
            Console.WriteLine(tokenResponse.Json);
            Console.WriteLine("\n\n");


            Console.WriteLine("Refresh Token:\n");
            // refresh token
            var refreshTokenResponse = await client.RequestRefreshTokenAsync(new RefreshTokenRequest
            {
                Address = disco.TokenEndpoint,
                GrantType = "refresh_token",

                ClientId = "ro.client",
                ClientSecret = "ClientSecret",
                Scope = "MyFundsApi offline_access",

                RefreshToken = tokenResponse.RefreshToken
            });
            if (refreshTokenResponse.IsError)
            {
                Console.WriteLine(refreshTokenResponse.Error);
                return;
            }
            Console.WriteLine(refreshTokenResponse.Json);
            Console.WriteLine("\n\n");


            Console.WriteLine("Call Api:\n");
            // call api
            var apiClient = new HttpClient();
            apiClient.SetBearerToken(tokenResponse.AccessToken);

            var response = await apiClient.GetAsync("http://localhost:5000/api/user/GetMe");
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine(response.StatusCode);
            }
            else
            {
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine(JObject.Parse(content));
            }
        }
    }
}
