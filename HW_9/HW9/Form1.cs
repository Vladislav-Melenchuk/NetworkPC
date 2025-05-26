using System;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Text.Json;
using System.Windows.Forms;

namespace HW9
{
    public partial class Form1 : Form
    {
        private string movieHtml = "";

        public Form1()
        {
            InitializeComponent();
        }

        private async void btnSearch_Click(object sender, EventArgs e)
        {
            string title = txtMovieTitle.Text.Trim();
            if (string.IsNullOrEmpty(title))
            {
                MessageBox.Show("Please enter a movie title.");
                return;
            }

            string apiKey = "9bb30534"; 
            string url = $"http://www.omdbapi.com/?t={Uri.EscapeDataString(title)}&apikey={apiKey}";

            using var client = new HttpClient();

            try
            {
                string response = await client.GetStringAsync(url);
                using var doc = JsonDocument.Parse(response);
                var root = doc.RootElement;

                if (root.GetProperty("Response").GetString() == "True")
                {
                    string movieTitle = root.GetProperty("Title").GetString();
                    string year = root.GetProperty("Year").GetString();
                    string genre = root.GetProperty("Genre").GetString();
                    string director = root.GetProperty("Director").GetString();
                    string plot = root.GetProperty("Plot").GetString();

                    movieHtml = $@"
                    <html><body style='font-family:Segoe UI;'>
                        <h2>{movieTitle} ({year})</h2>
                        <p><b>Genre:</b> {genre}</p>
                        <p><b>Director:</b> {director}</p>
                        <p><b>Plot:</b> {plot}</p>
                    </body></html>";

                    webResult.DocumentText = movieHtml;
                }
                else
                {
                    MessageBox.Show("Movie not found.");
                    webResult.DocumentText = "";
                    movieHtml = "";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error" + ex.Message);
            }
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(movieHtml))
            {
                MessageBox.Show("Error");
                return;
            }

            try
            {
                var senderEmail = "v.melenchuk03@gmail.com";
                var senderPassword = "************"; 
                var receiverEmail = "sunmeatrich@gmail.com";

                var mail = new MailMessage();
                mail.From = new MailAddress(senderEmail);
                mail.To.Add(receiverEmail);
                mail.Subject = "Movie Info Report";
                mail.Body = movieHtml;
                mail.IsBodyHtml = true;

                var smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential(senderEmail, senderPassword),
                    EnableSsl = true
                };

                smtpClient.Send(mail);
                MessageBox.Show("Sent successfully!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to send: " + ex.Message);
            }
        }
    }
}
