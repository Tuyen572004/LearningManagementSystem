using LearningManagementSystem.Services.ConfigService;
using LearningManagementSystem.Services.EmailService;
using LearningManagementSystem.Services.EmailService.Model;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LearningManagementSystem.Services
{

    public class BrevoConfig
    {
        public string EMAIL_API_KEY { get; set; }
        public string EMAIL_BASE_URL { get; set; }
        public string EMAIL_ENDPOINT { get; set; }
        public string SENDER_EMAIL { get; set; }
        public string SENDER_NAME { get; set; }
    }
    public class BrevoEmailService : IEmailService
    {
        private readonly BrevoConfig _config;


        public BrevoEmailService(IConfigService configService)
        {
            _config = configService.GetBrevoConfig();
        }

        public async Task sendEmail(EmailRequest emailRequest)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("api-key", _config.EMAIL_API_KEY);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            string json = JsonConvert.SerializeObject(emailRequest);
            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync($"{_config.EMAIL_BASE_URL}{_config.EMAIL_ENDPOINT}", content);

            if (!response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                throw new Exception($"Error sending email: {response.StatusCode} - {responseBody}");
            }
        }

    }
}
