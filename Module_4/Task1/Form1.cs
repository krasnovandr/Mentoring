using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace Task1
{
    public partial class Form1 : Form
    {
        CancellationTokenSource cts;

        public Form1()
        {
            InitializeComponent();
        }

        private async void button1_Click(object sender, EventArgs e)
        {

            var progress = new Progress<double>();
            progress.ProgressChanged += Changed;

            var cancellationToken = new CancellationTokenSource();

            await DownloadFileAsync("http://www.dotpdn.com/files/Paint.NET.3.5.11.Install.zip", progress, cancellationToken.Token);
            resultsTextBox.Clear();

            // Instantiate the CancellationTokenSource.
            cts = new CancellationTokenSource();

            try
            {
                await AccessTheWebAsync(cts.Token);
                resultsTextBox.Text += Environment.NewLine + "Downloads complete.";
            }
            catch (OperationCanceledException)
            {
                resultsTextBox.Text += "Downloads canceled." + Environment.NewLine;
            }
            catch (Exception)
            {
                resultsTextBox.Text += Environment.NewLine + "Downloads failed." + Environment.NewLine;
            }

            cts = null;
        }

        private void Changed(object sender, EventArgs e)
        {
            
        }


        private void button2_Click(object sender, EventArgs e)
        {
            if (cts != null)
            {
                cts.Cancel();
            }
        }

        async Task AccessTheWebAsync(CancellationToken ct, IProgress<int> progress)
        {
            HttpClient client = new HttpClient();

            // Make a list of web addresses.
            List<string> urlList = SetUpURLList();

            var downloadTasks = new List<Task<int>>();
            foreach (var url in urlList)
            {
                downloadTasks.Add(ProcessURL(url, client, ct));
            }

            //int totalCount = imageList.Count;
            //int processCount = await Task.Run<int>(() =>
            //{
            //    int tempCount = 0;
            //    foreach (var image in imageList)
            //    {
            //        //await the processing and uploading logic here
            //        int processed = await UploadAndProcessAsync(image);
            //        if (progress != null)
            //        {
            //            progress.Report((tempCount * 100 / totalCount));
            //        }
            //        tempCount++;
            //    }

            //    return tempCount;
            //});
            //return processCount;

            // ***Add a loop to process the tasks one at a time until none remain.
            int totalCount = downloadTasks.Count;
            while (downloadTasks.Any())
            {
                    int tempCount = 0;
                // Identify the first task that completes.
                Task<int> firstFinishedTask = await Task.WhenAny(downloadTasks);

                // ***Remove the selected task from the list so that you don't
                // process it more than once.
                downloadTasks.Remove(firstFinishedTask);

                // Await the completed task.
                int length = await firstFinishedTask;
                progress.Report((tempCount * 100 / totalCount));
                resultsTextBox.Text += String.Format("Length of the download:  {1} {0}", length, Environment.NewLine);
            }
        }


        private List<string> SetUpURLList()
        {
            List<string> urls = new List<string> 
            { 
                "http://msdn.microsoft.com",
                "http://msdn.microsoft.com/library/windows/apps/br211380.aspx",
                "http://msdn.microsoft.com/en-us/library/hh290136.aspx",
                "http://msdn.microsoft.com/en-us/library/dd470362.aspx",
                "http://msdn.microsoft.com/en-us/library/aa578028.aspx",
                "http://msdn.microsoft.com/en-us/library/ms404677.aspx",
                "http://msdn.microsoft.com/en-us/library/ff730837.aspx"
            };
            return urls;
        }


        async Task<int> ProcessURL(string url, HttpClient client, CancellationToken ct)
        {
            // GetAsync returns a Task<HttpResponseMessage>. 
            HttpResponseMessage response = await client.GetAsync(url, ct);

            // Retrieve the website contents from the HttpResponseMessage.
            byte[] urlContents = await response.Content.ReadAsByteArrayAsync();

            return urlContents.Length;
        }

        private void progressBar1_Click(object sender, EventArgs e)
        {

        }
        public async Task DownloadFileAsync(string url, IProgress<double> progress, CancellationToken token)
        {
            var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, token);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(string.Format("The request returned with HTTP status code {0}", response.StatusCode));
            }

            var total = response.Content.Headers.ContentLength.HasValue ? response.Content.Headers.ContentLength.Value : -1L;
            var canReportProgress = total != -1 && progress != null;

            using (var stream = await response.Content.ReadAsStreamAsync())
            {
                var totalRead = 0L;
                var buffer = new byte[4096];
                var isMoreToRead = true;

                do
                {
                    token.ThrowIfCancellationRequested();

                    var read = await stream.ReadAsync(buffer, 0, buffer.Length, token);

                    if (read == 0)
                    {
                        isMoreToRead = false;
                    }
                    else
                    {
                        var data = new byte[read];
                        buffer.ToList().CopyTo(0, data, 0, read);

                        // TODO: put here the code to write the file to disk

                        totalRead += read;

                        if (canReportProgress)
                        {
                            progress.Report((totalRead * 1d) / (total * 1d) * 100);
                        }
                    }
                } while (isMoreToRead);
            }
        }


    }
}
