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
        private bool logginIn;
        private string apiUri = ConfigurationManager.AppSettings["apiUri"];

        public static string token;

        public MainForm( )
        {
            InitializeComponent();
            _client = new HttpClient();

        }

        private async void MainForm_Load(object sender, EventArgs e)
        {
            logginIn = false;
            btnLogin.Visible = true;
            btnLogout.Visible = false;
           
            await PopulateDataGridView();
        }

        

        private async Task PopulateDataGridView()
        {       
            var response = await _client.GetAsync(apiUri + "api/Projects");
            var projectsResponseString = await response.Content.ReadAsStringAsync();
            var projectsResponseJson = JArray.Parse(projectsResponseString);
            dgvProjects.DataSource = projectsResponseJson;

            
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
            lblStatusText.Text = "Logged In";
            lblStatusText.ForeColor = Color.Green;
            token = login.AccessToken;
            
            btnLogin.Visible = false;
            btnLogout.Visible = true;
            newProjectToolStripMenuItem.Enabled = true;
            editDeleteProjectToolStripMenuItem.Enabled = true;
            
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
            lblStatusText.Text = "Not Logged In";
            lblStatusText.ForeColor = Color.Red;
            btnLogout.Visible = false;
            newProjectToolStripMenuItem.Enabled = false;
            editDeleteProjectToolStripMenuItem.Enabled = false;
            btnLogin.Visible = true;
        }
        private async void BtnLogin_Click(object sender, EventArgs e)
        {
            string domain = ConfigurationManager.AppSettings["Auth0:Domain"];
            string clientId = ConfigurationManager.AppSettings["Auth0:ClientId"];
            string audience = ConfigurationManager.AppSettings["Auth0:Audience"];

            client = new Auth0Client(new Auth0ClientOptions
            {
                Domain = domain,
                ClientId = clientId
            });


            var extraParameters = new Dictionary<string, string>();


            extraParameters.Add("audience", audience);
            Login(await client.LoginAsync(extraParameters: extraParameters));
        }

        private async void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (logginIn)
            {
                await Logout();
            }
        }

        private async void NewProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CreateForm cfrm = new CreateForm();
            cfrm.ShowDialog();

            if(cfrm.DialogResult == DialogResult.OK)
            {
                await PopulateDataGridView();
            }
        }
        private async void EditDeleteProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EditDeleteForm edfrm = new EditDeleteForm();
            edfrm.ShowDialog();

            if(edfrm.DialogResult == DialogResult.OK)
            {
                await PopulateDataGridView();
            }
        }
        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        
    }
 }
