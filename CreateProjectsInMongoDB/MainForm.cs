using System;
using System.Configuration;
using System.Drawing;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CreateProjectsInMongoDB.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CreateProjectsInMongoDB
{
    public partial class MainForm : Form
    {
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

        private void BtnLogout_Click(object sender, EventArgs e)
        {
             Logout();    
        }

        

        private void Logout()
        {
            token = null; // forget token

            logginIn = false;
            lblStatusText.Text = "Not Logged In";
            lblStatusText.ForeColor = Color.Red;
            btnLogout.Visible = false;
            newProjectToolStripMenuItem.Enabled = false;
            editDeleteProjectToolStripMenuItem.Enabled = false;
            btnLogin.Visible = true;
        }
        private void BtnLogin_Click(object sender, EventArgs e)
        {
            LoginForm loginForm = new LoginForm();

            DialogResult loginResult = loginForm.ShowDialog();

            if(loginResult == DialogResult.OK)
            {
                logginIn = true;
                lblStatusText.Text = "Logged In";
                lblStatusText.ForeColor = Color.Green;
                btnLogin.Visible = false;
                btnLogout.Visible = true;
                newProjectToolStripMenuItem.Enabled = true;
                editDeleteProjectToolStripMenuItem.Enabled = true;
            }
            else if (loginResult == DialogResult.None)
            {
                MessageBox.Show("Error loggin in");
            }
            
            
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (logginIn)
            {
                Logout();
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
