using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Task1
{
    public partial class Form1 : Form
    {
        readonly List<string> _urlList = new List<string>();
        private CancellationTokenSource _cancellationToken;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var button = new Button();
            dataGridView1.Rows.Add(textBox1.Text, button);
        }

        private void dataGridView1_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {
            var senderGrid = (DataGridView)sender;

            if (senderGrid.Columns[e.ColumnIndex] is DataGridViewButtonColumn &&
                e.RowIndex >= 0)
            {
                senderGrid.Rows.RemoveAt(e.RowIndex);
            }
        }

        private async void button2_Click_1(object sender, EventArgs e)
        {
            _cancellationToken = new CancellationTokenSource();
            richTextBox1.Text += "Downloads started." + Environment.NewLine;
            dataGridView1.ClearSelection();
           
            MapGridColumnToTheList();
            var progress = new Progress<double>();
            progress.ProgressChanged += progress_ProgressChanged;

            try
            {
                await Download(_cancellationToken.Token, progress);

            }
            catch (OperationCanceledException)
            {
                richTextBox1.Text += Environment.NewLine + "Downloads canceled." + Environment.NewLine;
            }
            catch (Exception)
            {
                richTextBox1.Text += Environment.NewLine + "Downloads failed." + Environment.NewLine;
            }

            _cancellationToken = null;
        }

        private void MapGridColumnToTheList()
        {
            _urlList.Clear();
            foreach (DataGridViewRow dr in dataGridView1.Rows)
            {
                foreach (var cell in dr.Cells.OfType<DataGridViewTextBoxCell>())
                {
                    _urlList.Add(cell.Value.ToString());
                }
            }
        }

        private async Task Download(CancellationToken token, IProgress<double> progress)
        {

            var client = new HttpClient();
          
            var downloadTasksQuery = _urlList.Select((t, i) => DownloadFileAsync(t, null, token, client, i)).ToList();

            int finishedCount = default(int);
            while (downloadTasksQuery.Any())
            {
                if (token.IsCancellationRequested)
                {
                    token.ThrowIfCancellationRequested();
                }
                Task<Result> firstFinishedTask = await Task.WhenAny(downloadTasksQuery);

                downloadTasksQuery.Remove(firstFinishedTask);

                var result = await firstFinishedTask;

                finishedCount++;
                dataGridView1.Rows[result.DwonloadNumber].Selected = true;
                progress.Report((finishedCount * 1d) / (_urlList.Count * 1d) * 100);

                richTextBox1.Text += string.Format("{0}Length of the download:  {1}", Environment.NewLine, result.Length);
            }
        }

        void progress_ProgressChanged(object sender, double e)
        {
            progressBar1.Value = (int)e;
        }


        private void button3_Click(object sender, EventArgs e)
        {
            if (_cancellationToken != null)
            {
                _cancellationToken.Cancel();
            }

            progressBar1.Value = progressBar1.Minimum;
        }

        public async Task<Result> DownloadFileAsync(string url, IProgress<double> progress, CancellationToken token, HttpClient client, int number)
        {


            var response = await client.GetAsync(url, token);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(string.Format("The request returned with HTTP status code {0}", response.StatusCode));
            }

            if (token.IsCancellationRequested)
            {
                token.ThrowIfCancellationRequested();
            }

            byte[] urlContents = await response.Content.ReadAsByteArrayAsync();

            return new Result
            {
                Length = urlContents.Length,
                DwonloadNumber = number
            };
        }
    }

    public class Result
    {
        public int Length { get; set; }
        public int DwonloadNumber { get; set; }
    }
}
