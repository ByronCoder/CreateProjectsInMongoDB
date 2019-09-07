using CreateProjectsInMongoDB.Models;
using Newtonsoft.Json;
using System;
using System.Configuration;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CreateProjectsInMongoDB
{
    public partial class LoginForm : Form
    {
        private readonly HttpClient _client;
        private string apiUri = ConfigurationManager.AppSettings["apiUri"];

        public LoginForm()
        {
            InitializeComponent();
            _client = new HttpClient();
        }

        private async Task<string> Login()
        {

            User user = new User
            {
                Username = txtUserName.Text,
                Password = txtPass.Text
            };

            var json = JsonConvert.SerializeObject(user);

            var response = await _client.PostAsync(apiUri + "/api/signin", new StringContent(json, Encoding.UTF8, "application/json"));
            var responseString = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {       
                DialogResult = DialogResult.OK;
                return responseString;
            }
            else
            {
                MessageBox.Show("Error logging in: " + responseString);
                DialogResult = DialogResult.None;
                return "Error logging in";
            }

        }

        private async void BtnLogin_Click(object sender, EventArgs e)
        {
            MainForm.token = await Login();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}
