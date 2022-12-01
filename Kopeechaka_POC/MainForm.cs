using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Policy;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Reflection.Metadata.BlobBuilder;

namespace Kopeechaka_POC
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_LoadAsync(object sender, EventArgs e)
        {
            ////// token: be62e73382087beb2738085377f40208

            cmbServer.SelectedIndex = 0;
            cmbEndPoint.SelectedIndex = 0;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnExe_Click(object sender, EventArgs e)
        {
             string urlParameters="";
            string url = string.Empty;
            HTTPResponse? jsonResult = null;

            if (cmbEndPoint.SelectedIndex == 0)
                url = $@"https://api.kopeechka.store/user-balance?token={txtToken.Text.Trim()}&type=json&api=2.0";
            else if (cmbEndPoint.SelectedIndex == 1)
                url = $@"https://api.kopeechka.store/mailbox-reorder?site={txtSite.Text.Trim()}&email={txtEmail.Text.Trim()}&regex={txtRegx.Text.Trim()}&token={txtToken.Text.Trim()}&type=json&subject={txtSubject.Text.Trim()}&api=2.0";

            using var client = new HttpClient();
            client.BaseAddress = new Uri(url);
            // Add an Accept header for JSON format.
            client.DefaultRequestHeaders.Accept.Add(
               new MediaTypeWithQualityHeaderValue("application/json"));
            // Get data response
            var response = client.GetAsync(urlParameters).Result;
            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;

                //JsonSerializer.Deserialize<deweyData>(result);

                jsonResult = JsonConvert.DeserializeObject<HTTPResponse>(result);
                //// Parse the response body
                //var dataObjects = response.Content.ReadFromJsonAsync() .ReadAsAsync<IEnumerable<DataObject>>().Result;
                //foreach (var d in dataObjects)
                //{
                //    Console.WriteLine("{0}", d.Name);
                //}

                if (cmbEndPoint.SelectedIndex == 0)
                    lblOutPut.Text = jsonResult.balance != null ? jsonResult.balance : jsonResult.value;
                else
                    lblOutPut.Text = jsonResult.email!=null ? jsonResult.email : jsonResult.value;
            }
            else
            {
                //Console.WriteLine("{0} ({1})", (int)response.StatusCode,
                //              response.ReasonPhrase);

                lblOutPut.Text = jsonResult.value;
            }
        }
    }

    public class HTTPResponse
    {
        public string id { get; set; }
        public string status { get; set; }
        public string email { get; set; }
        public string value { get; set; }
        public string balance { get; set; }

    }
}
