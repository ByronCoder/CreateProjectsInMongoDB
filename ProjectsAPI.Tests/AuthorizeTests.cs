using System;
using System.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Xunit;
using ProjectsAPI;
using System.Text;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using ProjectsAPI.Models;
using Newtonsoft.Json;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Amazon;

namespace API.Tests
{
    public class AuthorizeTests
    {

        private readonly HttpClient _client;
        private IConfiguration _configuration;
        private readonly TestServer _server;
    
        private readonly RegionEndpoint _region = RegionEndpoint.USEast1;


        public AuthorizeTests()
        {
            

            _configuration = new ConfigurationBuilder()
                .SetBasePath(Path.GetFullPath(@"../../.."))
                .AddJsonFile($"appsettings.Development.json", optional: false)
                .AddEnvironmentVariables()
                .Build();

            _server = new TestServer(new WebHostBuilder()
                .UseStartup<Startup>()
                .UseConfiguration(_configuration));
            
            _client = _server.CreateClient();
        }

        [Fact]
        public async Task UnAuthorizedAccess()
        {
            Project proj = new Project
            {
                Title = "Test Project",
                Description = "This is a test Project",
                SourceLink = "testtest",
                DemoLink = "testtestest"
            };

            var json = JsonConvert.SerializeObject(proj);

            var responsePost = await _client.PostAsync("/api/Projects", new StringContent(json, Encoding.UTF8, "application/json"));
            var responsePut = await _client.PutAsync("/api/Projects/123", new StringContent(json, Encoding.UTF8, "application/json"));
            var responseDelete = await _client.DeleteAsync("/api/Projects/123");
            Assert.Equal(HttpStatusCode.Unauthorized, responsePost.StatusCode);
            Assert.Equal(HttpStatusCode.Unauthorized, responsePut.StatusCode);
            Assert.Equal(HttpStatusCode.Unauthorized, responseDelete.StatusCode);
        }

        public async Task<string> GetToken()
        {
           

            User user = new User
            {
                    Username = _configuration["AWS:TestUserName"],
                    Password = _configuration["AWS:TestUserPass"]
            };

            var cognito = new AmazonCognitoIdentityProviderClient(_region);

            var request = new AdminInitiateAuthRequest
            {
                UserPoolId = _configuration["AWS:UserPoolId"],
                ClientId = _configuration["AWS:Authority"],
                AuthFlow = AuthFlowType.ADMIN_NO_SRP_AUTH
            };

            request.AuthParameters.Add("USERNAME", user.Username);
            request.AuthParameters.Add("PASSWORD", user.Password);

            var response = await cognito.AdminInitiateAuthAsync(request);

            return (response.AuthenticationResult.IdToken);
        }

        [Fact]
        public async Task TestGetToken()
        {

            User user = new User
            {
                Username = _configuration["AWS:TestUserName"],
                Password = _configuration["AWS:TestUserPass"]

            };

            var cognito = new AmazonCognitoIdentityProviderClient(_region);

            var request = new AdminInitiateAuthRequest
            {
                UserPoolId = _configuration["AWS:UserPoolId"],
                ClientId = _configuration["AWS:Authority"],
                AuthFlow = AuthFlowType.ADMIN_NO_SRP_AUTH
            };

            request.AuthParameters.Add("USERNAME", user.Username);
            request.AuthParameters.Add("PASSWORD", user.Password);

            var response = await cognito.AdminInitiateAuthAsync(request);

            Assert.Equal(HttpStatusCode.OK, response.HttpStatusCode);

            Assert.NotNull(response.AuthenticationResult.IdToken);

        }
        [Fact]
        public async Task GetProjects()
        {
            var response = await _client.GetAsync("api/Projects");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var projectsResponseString = await response.Content.ReadAsStringAsync();
            var projectsResponseJson = JArray.Parse(projectsResponseString);
            Assert.True(projectsResponseJson.Count > 0);
        }

        [Fact]
        public async Task CreateProject()
        {
            var token = await GetToken();

            Project proj = new Project
            {
                Id = "6d4f0e07fd9ca95b145ee40b",
                Title = "Test Project",
                Description = "This is a test Project",
                SourceLink = "testtest",
                DemoLink = "testtestest"
            };

            var json = JsonConvert.SerializeObject(proj);
           

            _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
            var response = await _client.PostAsync("/api/Projects", new StringContent(json, Encoding.UTF8, "application/json"));

            

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            if(response.IsSuccessStatusCode)
            {
                var projectsResponseString = await response.Content.ReadAsStringAsync();
                var responseJson = JObject.Parse(projectsResponseString);
                Assert.NotNull((string)responseJson["title"]);
                Assert.Equal(proj.Title, (string)responseJson["title"]);
            }

        }

        [Fact] 
        public async Task UpdateProject()
        {
            var token = await GetToken();

            Project proj = new Project
            {
                Id = "6d4f0e07fd9ca95b145ee40b",
                Title = "Test Project Edited",
                Description = "This is a test Project",
                SourceLink = "testtest",
                DemoLink = "testtestest"
            };

            var json = JsonConvert.SerializeObject(proj);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var requestMessage = new HttpRequestMessage(HttpMethod.Put, "/api/Projects/6d4f0e07fd9ca95b145ee40b");

            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            requestMessage.Content = content;
            var projectsResponse = await _client.SendAsync(requestMessage);

            Assert.True(projectsResponse.IsSuccessStatusCode);

            if (projectsResponse.IsSuccessStatusCode)
            {
                var request = new HttpRequestMessage(HttpMethod.Get, "/api/Projects/6d4f0e07fd9ca95b145ee40b");
                var requestResponse = await _client.SendAsync(request);
                var requestString = await requestResponse.Content.ReadAsStringAsync();
                var responseJson = JObject.Parse(requestString);
                Assert.NotNull((string)responseJson["title"]);
                Assert.Equal(proj.Title, (string)responseJson["title"]);
            }


        }

        [Fact]
        public async Task DeleteProject()
        {
            var token = await GetToken();

            var requestMessage = new HttpRequestMessage(HttpMethod.Delete, "/api/Projects/6d4f0e07fd9ca95b145ee40b");
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var projectsResponse = await _client.SendAsync(requestMessage);

            Assert.True(projectsResponse.IsSuccessStatusCode);

            if(projectsResponse.IsSuccessStatusCode)
            {
                var request = new HttpRequestMessage(HttpMethod.Get, "/api/Projects/6d4f0e07fd9ca95b145ee40b");
                var requestResponse = await _client.SendAsync(request);
                Assert.Equal(HttpStatusCode.NotFound, requestResponse.StatusCode);
            }

        }


    }

   
}
