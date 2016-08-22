using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace Task1
{
    public partial class Form1 : Form
    {
        CancellationTokenSource cancellationToken;
        private HttpClient client = new HttpClient();
        private int row = 20;
        private ProgressBar bar;
        private List<Task> loads = new List<Task>();
        public Form1()
        {
            InitializeComponent();
            Form.CheckForIllegalCrossThreadCalls = false;
        }

        private async void button1_Click(object sender, EventArgs e)
        {
                Task task1 = Task.Factory.StartNew(() => {
                    var progress = new Progress<double>();
                    progress.ProgressChanged += Changed;
                    cancellationToken = new CancellationTokenSource();
                    var button = new Button();
                    bar = new ProgressBar();
                    var box = new TextBox();
                    button.Location = new Point(461, row);
                    button.Text = "Stop";
                    bar.Location = new Point(344, row);
                    box.Location = new Point(7, row);
                    box.Text = textBox1.Text;
                    button.Click += button_Click;
                    groupBox1.Controls.Add(button);
                    groupBox1.Controls.Add(bar);
                    groupBox1.Controls.Add(box);
                    row += 30;
                    DownloadFileAsync("http://www.dotpdn.com/files/Paint.NET.3.5.11.Install.zip", progress,
                        cancellationToken.Token);
            });


            //Task.WaitAll(loads.ToArray());


            //cancellationToken = new CancellationTokenSource();
            //try
            //{
            //    await DownloadFileAsync("http://www.dotpdn.com/files/Paint.NET.3.5.11.Install.zip", progress, cancellationToken.Token);
            //}
            //catch (OperationCanceledException ex)
            //{
            //    //resultsTextBox.Clear();
            //}

        }

        void button_Click(object sender, EventArgs e)
        {
            if (cancellationToken != null)
            {
                cancellationToken.Cancel();
            }
        }

        private void Changed(object sender, double d)
        {
            bar.Value = (int)d;
        }


        private void button2_Click(object sender, EventArgs e)
        {
            if (cancellationToken != null)
            {
                cancellationToken.Cancel();
            }
        }

        public async Task DownloadFileAsync(string url, IProgress<double> progress, CancellationToken token)
        {
            var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, token);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(string.Format("The request returned with HTTP status code {0}", response.StatusCode));
            }

            if (token.IsCancellationRequested)
            {
                token.ThrowIfCancellationRequested();
            }

            var total = response.Content.Headers.ContentLength.HasValue ? response.Content.Headers.ContentLength.Value : -1L;
            var canReportProgress = total != -1 && progress != null;

            using (var stream = await response.Content.ReadAsStreamAsync())
            {
                var totalRead = 0L;
                var buffer = new byte[4096];
                int read;
                while ((read = await stream.ReadAsync(buffer, 0, buffer.Length, token)) != 0)
                {
                    // Was cancellation already requested? 

                    if (token.IsCancellationRequested)
                    {
                        token.ThrowIfCancellationRequested();
                    }

                    totalRead += read;
                    if (canReportProgress)
                    {
                        progress.Report((totalRead * 1d) / (total * 1d) * 100);
                    }
                }
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        //private void button2_Click_1(object sender, EventArgs e)
        //{
        //    Task.WaitAll(loads.ToArray());
        //}
    }
}
