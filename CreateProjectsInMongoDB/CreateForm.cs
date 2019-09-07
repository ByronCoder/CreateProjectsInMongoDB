using CreateProjectsInMongoDB.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CreateProjectsInMongoDB
{
    public partial class CreateForm : Form
    {
        private HttpClient _client;
        private string apiUri = ConfigurationManager.AppSettings["apiUri"];
        public CreateForm()
        {
            InitializeComponent();
            _client = new HttpClient();
        }

        private async void BtnSave_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtTitle.Text)
               && !string.IsNullOrEmpty(txtDesc.Text)
               && !string.IsNullOrEmpty(txtSource.Text)
               && !string.IsNullOrEmpty(txtDemo.Text))
            {
                await SaveProject();
              
            }
            else
            {
                MessageBox.Show("All information must be populated in the text fields");
            }
        }

        private async Task SaveProject()
        {
            Project proj = new Project
            {
                title = txtTitle.Text,
                description = txtDesc.Text,
                sourceLink = txtSource.Text,
                demoLink = txtDemo.Text
            };

            var json = JsonConvert.SerializeObject(proj);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, apiUri + "api/Projects");

            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", MainForm.token);
            requestMessage.Content = content;
            var projectsResponse = await _client.SendAsync(requestMessage);



            if (projectsResponse.IsSuccessStatusCode)
            {
                var projectsResponseString = await projectsResponse.Content.ReadAsStringAsync();
                var responseJson = JObject.Parse(projectsResponseString);
                DialogResult = DialogResult.OK;
            }
            else
            {
                MessageBox.Show("Error saving data: " + projectsResponse.StatusCode);
            }

        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}
