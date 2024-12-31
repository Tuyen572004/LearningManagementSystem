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

    /// <summary>
    /// Configuration settings for Brevo email service.
    /// </summary>
    public class BrevoConfig
    {
        /// <summary>
        /// Gets or sets the API key for the email service.
        /// </summary>
        public string EMAIL_API_KEY { get; set; }

        /// <summary>
        /// Gets or sets the base URL for the email service.
        /// </summary>
        public string EMAIL_BASE_URL { get; set; }

        /// <summary>
        /// Gets or sets the endpoint for sending emails.
        /// </summary>
        public string EMAIL_ENDPOINT { get; set; }

        /// <summary>
        /// Gets or sets the sender email address.
        /// </summary>
        public string SENDER_EMAIL { get; set; }

        /// <summary>
        /// Gets or sets the sender name.
        /// </summary>
        public string SENDER_NAME { get; set; }
    }
    /// <summary>
    /// Service for sending emails using Brevo.
    /// </summary>
    public class BrevoEmailService : IEmailService
    {
        private readonly BrevoConfig _config;

        /// <summary>
        /// Initializes a new instance of the <see cref="BrevoEmailService"/> class.
        /// </summary>
        /// <param name="configService">The configuration service to retrieve Brevo settings.</param>
        public BrevoEmailService(IConfigService configService)
        {
            _config = configService.GetBrevoConfig();
        }

        /// <summary>
        /// Sends an email using the Brevo email service.
        /// </summary>
        /// <param name="emailRequest">The email request containing the email details.</param>
        /// <returns>A task that represents the asynchronous send operation.</returns>
        /// <exception cref="Exception">Thrown when the email service returns an unsuccessful status code.</exception>
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
