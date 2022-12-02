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
using System.Text.RegularExpressions;
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
            string url = string.Empty;
            HTTPResponse? jsonResult = null;

            if (cmbEndPoint.SelectedIndex == 0)
                url = $"https://api.kopeechka.store/user-balance?token={txtToken.Text.Trim()}&type=json&api=2.0";
            else if (cmbEndPoint.SelectedIndex == 1)
                url = $"https://api.kopeechka.store/mailbox-reorder?site={txtSite.Text.Trim()}&email={txtEmail.Text.Trim()}&regex={txtRegx.Text.Trim()}&token={txtToken.Text.Trim()}&type=json&subject={txtSubject.Text.Trim()}&api=2.0";

            var response = GetRequest(url);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                jsonResult = JsonConvert.DeserializeObject<HTTPResponse>(result);

                if (cmbEndPoint.SelectedIndex == 0)
                    lblOutPut.Text = jsonResult.balance != null ? jsonResult.balance : jsonResult.value;
                else
                {
                    url = $"http://api.kopeechka.store/mailbox-get-message?full=1&id={jsonResult.id}&token={txtToken.Text.Trim()}&type=json&api=2.0";
                    response = GetRequest(url);
                    if (response.IsSuccessStatusCode)
                    {
                        result = response.Content.ReadAsStringAsync().Result;

                        jsonResult = JsonConvert.DeserializeObject<HTTPResponse>(result);

                        lblOutPut.Text = jsonResult.fullmessage != null ? $"OPT: {RegExOTP(jsonResult.fullmessage)}" : $"Message: {jsonResult.value}";
                    }
                }
            }
            else
            {
                lblOutPut.Text = jsonResult.value;
            }
        }
        private HttpResponseMessage GetRequest(string url)
        {
            string urlParameters = "";

            using var client = new HttpClient();
            client.BaseAddress = new Uri(url);
            // Add an Accept header for JSON format.
            client.DefaultRequestHeaders.Accept.Add(
               new MediaTypeWithQualityHeaderValue("application/json"));
            // Get data response
            var response = client.GetAsync(urlParameters).Result;

            return response;
        }
        private string RegExOTP(string data)
        {
            //string data = File.ReadAllText("c:\\new9.html");
            data = data.Replace("\\\"font-weight:bold;", "\"font-weight:bold;");
            // Extract text between specific HTML tag
            /////(?:<p.*?style=.font-weight:bold;.*?>)(.*?)(?:</p>)
            Regex extractHTMLRegex = new Regex("(?:<p.*?style=.font-weight:bold;.*?>)([\\s]+.*?[\\s]+)(?:</p>)");

            Match match = extractHTMLRegex.Match(data);
            if (match.Success)
            {
                string code = match.Groups[1].Captures[0].Value;
                return code.Trim();
            }

            return "** Not Found **";
        }
    }

    public class HTTPResponse
    {
        public string id { get; set; }
        public string status { get; set; }
        public string mail { get; set; }
        public string value { get; set; }
        public string balance { get; set; }
        public string fullmessage { get; set; }

    }
}
