using System;
using System.Threading.Tasks;
using System.Windows.Forms;

public class Form1 : Form
{
    private Client client;

    private TextBox txtCity;
    private TextBox txtResponse;
    private Button btnTime, btnDate, btnEur, btnBtc;

    public Form1()
    {
        InitializeComponent();
        client = new Client();
    }

    private async void Form1_Load(object sender, EventArgs e)
    {
        bool connected = await client.ConnectAsync();

        if (!connected)
        {
            MessageBox.Show("Не удалось подключиться к серверу. Запустите сервер и попробуйте снова.");
            Close();
        }
    }

    private async void btnTime_Click(object sender, EventArgs e) => await SendRequestAndShowResponseAsync("time");
    private async void btnDate_Click(object sender, EventArgs e) => await SendRequestAndShowResponseAsync("date");
    private async void btnEur_Click(object sender, EventArgs e) => await SendRequestAndShowResponseAsync("eur");
    private async void btnBtc_Click(object sender, EventArgs e) => await SendRequestAndShowResponseAsync("btc");

    private async Task SendRequestAndShowResponseAsync(string request)
    {
        try
        {
            string response = await client.SendRequestAsync(request);
            txtResponse.AppendText(response + Environment.NewLine);
        }
        catch (Exception ex)
        {
            txtResponse.AppendText("Ошибка: " + ex.Message + Environment.NewLine);
        }
    }

    private void Form1_FormClosing(object sender, FormClosingEventArgs e)
    {
        client.Dispose();
    }

    private void InitializeComponent()
    {
        this.Text = "Клиент";
        this.Width = 600;
        this.Height = 450;
        this.StartPosition = FormStartPosition.CenterScreen;

        btnTime = new Button() { Text = "Время", Left = 10, Top = 50, Width = 110 };
        btnTime.Click += btnTime_Click;
        this.Controls.Add(btnTime);

        btnDate = new Button() { Text = "Дата", Left = 130, Top = 50, Width = 110 };
        btnDate.Click += btnDate_Click;
        this.Controls.Add(btnDate);


        btnEur = new Button() { Text = "Курс евро", Left = 260, Top = 50, Width = 110 };
        btnEur.Click += btnEur_Click;
        this.Controls.Add(btnEur);

        btnBtc = new Button() { Text = "Курс биткоина", Left = 390, Top = 50, Width = 110 };
        btnBtc.Click += btnBtc_Click;
        this.Controls.Add(btnBtc);

        txtResponse = new TextBox()
        {
            Multiline = true,
            ScrollBars = ScrollBars.Vertical,
            Left = 10,
            Top = 90,
            Width = this.ClientSize.Width - 20,
            Height = this.ClientSize.Height - 100,
            Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
            ReadOnly = true,
        };
        this.Controls.Add(txtResponse);

        this.Load += Form1_Load;
        this.FormClosing += Form1_FormClosing;
    }
}
