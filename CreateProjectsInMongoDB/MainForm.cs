using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using Auth0.OidcClient;
using CreateProjectsInMongoDB.Models;
using IdentityModel.OidcClient;
using IdentityModel.OidcClient.Browser;
using MongoDB.Driver;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CreateProjectsInMongoDB
{
    public partial class MainForm : Form
    {
        private Auth0Client client;
        private readonly HttpClient _client;
        private string token;
        private bool logginIn;

        public MainForm( )
        {
            InitializeComponent();
            _client = new HttpClient();

        }

        private async void MainForm_Load(object sender, EventArgs e)
        {
            logginIn = false;
            btnSave.Enabled = false;
            btnLogin.Visible = true;
            btnLogout.Visible = false;
           
            await PopulateDataGridView();
        }

        

        private async Task PopulateDataGridView()
        {
            string apiUrl = "https://localhost:44312/api/Projects";

            var response = await _client.GetAsync(apiUrl);
            var projectsResponseString = await response.Content.ReadAsStringAsync();
            var projectsResponseJson = JArray.Parse(projectsResponseString);
            dgvProjects.DataSource = projectsResponseJson;
        }

        private async Task SaveProject()
        {
  
            string apiUrl = "https://localhost:44312/api/Projects";
            Project proj = new Project
            {
                title = txtTitle.Text,
                description = txtDesc.Text,
                sourceLink = txtSource.Text,
                demoLink = txtDemo.Text
            };

            var json = JsonConvert.SerializeObject(proj);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, apiUrl);

            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            requestMessage.Content = content;
            var projectsResponse = await _client.SendAsync(requestMessage);

           

            if (projectsResponse.IsSuccessStatusCode)
            {
                var projectsResponseString = await projectsResponse.Content.ReadAsStringAsync();
                var responseJson = JObject.Parse(projectsResponseString);
                
            }
            else
            {
                MessageBox.Show("Error: " + projectsResponse.StatusCode);
            }

        }

        private async void BtnSave_Click(object sender, EventArgs e)
        {
            if(!string.IsNullOrEmpty(txtTitle.Text) 
                && !string.IsNullOrEmpty(txtDesc.Text)
                && !string.IsNullOrEmpty(txtSource.Text)
                && !string.IsNullOrEmpty(txtDemo.Text)) {


                await SaveProject();
                await PopulateDataGridView();
            }
            else
            {
                MessageBox.Show("All information must be populated in the text fields");
            }


        }

        private async void BtnLogout_Click(object sender, EventArgs e)
        {

            await Logout();
          
        }

        private void Login(LoginResult login)
        {
            if(login.IsError)
            {
                MessageBox.Show(login.Error);
                return;
            }

            logginIn = true;
            token = login.AccessToken;

            btnSave.Enabled = true;
            btnLogin.Visible = false;
            btnLogout.Visible = true;
            
        }

        private async Task Logout()
        {
            BrowserResultType browserResult = await client.LogoutAsync();

            if (browserResult != BrowserResultType.Success)
            {
                MessageBox.Show(browserResult.ToString());
                return;
            }

            logginIn = false;
            btnSave.Enabled = false;
            btnLogout.Visible = false;
            btnLogin.Visible = true;
        }
        private async void BtnLogin_Click(object sender, EventArgs e)
        {
            string domain = ConfigurationManager.AppSettings["Auth0:Domain"];
            string clientId = ConfigurationManager.AppSettings["Auth0:ClientId"];

            client = new Auth0Client(new Auth0ClientOptions
            {
                Domain = domain,
                ClientId = clientId
            });


            var extraParameters = new Dictionary<string, string>();


            extraParameters.Add("audience", "https://byroncoder.github.io/ProjectPortfolio/");
            Login(await client.LoginAsync(extraParameters: extraParameters));
        }

        private async void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (logginIn)
            {
                await Logout();
            }
        }
    }
 }
