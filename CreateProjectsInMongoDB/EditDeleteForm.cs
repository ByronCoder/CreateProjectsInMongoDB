using CreateProjectsInMongoDB.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CreateProjectsInMongoDB
{
    public partial class EditDeleteForm : Form
    {
        private HttpClient _client;
        private string apiUri = ConfigurationManager.AppSettings["apiUri"];
        public EditDeleteForm()
        {
            InitializeComponent();
            _client = new HttpClient();
        }

        private async void EditDeleteForm_Load(object sender, EventArgs e)
        {
            await PopulateComboBox();
        }

        private async Task PopulateComboBox()
        {
            var response = await _client.GetAsync(apiUri + "api/Projects");
            var projectsResponseString = await response.Content.ReadAsStringAsync();
            var projectsResponseJson = JArray.Parse(projectsResponseString);
            cboProjects.DataSource = projectsResponseJson;
            cboProjects.DisplayMember = "title";
            cboProjects.ValueMember = "id";
            
        }

        private async void BtnLoad_Click(object sender, EventArgs e)
        {
            var response = await _client.GetAsync(apiUri + "api/Projects/" + cboProjects.SelectedValue);

            if (response.IsSuccessStatusCode)
            {
                var projectsResponseString = await response.Content.ReadAsStringAsync();
                var projectsResponseJson = JObject.Parse(projectsResponseString);

                txtTitle.Text = projectsResponseJson["title"].ToString();
                txtDesc.Text = projectsResponseJson["description"].ToString();
                txtSource.Text = projectsResponseJson["sourceLink"].ToString();
                txtDemo.Text = projectsResponseJson["demoLink"].ToString();

                txtTitle.Enabled = true;
                txtDesc.Enabled = true;
                txtSource.Enabled = true;
                txtDemo.Enabled = true;
                btnSave.Enabled = true;
                btnDelete.Enabled = true;
            }
            else
            {
                MessageBox.Show("Error loading data: " + response.StatusCode);
            }
           

            

           
        }

        private async void BtnSave_Click(object sender, EventArgs e)
        {
            Project proj = new Project
            {
                id = cboProjects.SelectedValue.ToString(),
                title = txtTitle.Text,
                description = txtDesc.Text,
                sourceLink = txtSource.Text,
                demoLink = txtDemo.Text
            };

            var json = JsonConvert.SerializeObject(proj);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var requestMessage = new HttpRequestMessage(HttpMethod.Put, apiUri + "api/Projects/" + cboProjects.SelectedValue);

            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", MainForm.token);
            requestMessage.Content = content;
            var projectsResponse = await _client.SendAsync(requestMessage);

            if(projectsResponse.IsSuccessStatusCode)
            {
                DialogResult = DialogResult.OK;
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private async void BtnDelete_Click(object sender, EventArgs e)
        {
            DialogResult dlr = MessageBox.Show("Are you sure you want to DELETE the " + txtTitle.Text + " project? This action can NOT be undone.", "Delete Project", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if(dlr == DialogResult.Yes)
            {
                var requestMessage = new HttpRequestMessage(HttpMethod.Delete, apiUri + "api/Projects/" + cboProjects.SelectedValue);
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", MainForm.token);
                var projectsResponse = await _client.SendAsync(requestMessage);

                if (projectsResponse.IsSuccessStatusCode)
                {
                    DialogResult = DialogResult.OK;
                }
            }
           
        }

        private void CboProjects_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtTitle.Text = "";
            txtDesc.Text = "";
            txtSource.Text = "";
            txtDemo.Text = "";

            txtTitle.Enabled = false;
            txtDesc.Enabled = false;
            txtSource.Enabled = false;
            txtDemo.Enabled = false;
            btnSave.Enabled = false;
            btnDelete.Enabled = false;
        }
    }
}
