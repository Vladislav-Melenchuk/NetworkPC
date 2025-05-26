namespace HW9
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.TextBox txtMovieTitle;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.WebBrowser webResult;
        private System.Windows.Forms.Label lblMovieTitle;
        private System.Windows.Forms.Button btnSend;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.txtMovieTitle = new System.Windows.Forms.TextBox();
            this.btnSearch = new System.Windows.Forms.Button();
            this.webResult = new System.Windows.Forms.WebBrowser();
            this.lblMovieTitle = new System.Windows.Forms.Label();
            this.btnSend = new System.Windows.Forms.Button();
            this.SuspendLayout();

            // lblMovieTitle
            this.lblMovieTitle.AutoSize = true;
            this.lblMovieTitle.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblMovieTitle.Location = new System.Drawing.Point(20, 20);
            this.lblMovieTitle.Text = "Enter the name of the movie:";

            // txtMovieTitle
            this.txtMovieTitle.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtMovieTitle.Location = new System.Drawing.Point(200, 17);
            this.txtMovieTitle.Size = new System.Drawing.Size(300, 25);

            // btnSearch
            this.btnSearch.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnSearch.Location = new System.Drawing.Point(510, 16);
            this.btnSearch.Size = new System.Drawing.Size(100, 28);
            this.btnSearch.Text = "Search";
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);

            // webResult
            this.webResult.Location = new System.Drawing.Point(20, 60);
            this.webResult.MinimumSize = new System.Drawing.Size(20, 20);
            this.webResult.Size = new System.Drawing.Size(590, 300);

            // btnSend
            this.btnSend.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnSend.Location = new System.Drawing.Point(20, 370);
            this.btnSend.Size = new System.Drawing.Size(200, 35);
            this.btnSend.Text = "Send to Email";
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);

            // Form1
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.ClientSize = new System.Drawing.Size(640, 420);
            this.Controls.Add(this.btnSend);
            this.Controls.Add(this.webResult);
            this.Controls.Add(this.btnSearch);
            this.Controls.Add(this.txtMovieTitle);
            this.Controls.Add(this.lblMovieTitle);
            this.Name = "Form1";
            this.Text = "Movie Info";
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
