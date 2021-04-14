using Microsoft.SqlServer.XEvent.XELite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace XELiveStreamUI
{
    public partial class frmMain : Form
    {
        private int cnt = 0;

        public frmMain()
        {
            InitializeComponent();
        }

        private void btnClick_Click(object sender, EventArgs e)
        {
            //txtResult.Text = AppConfig.GetConnectionStringByName("mydb");


            OutputXELStream(AppConfig.GetConnectionStringByName("mydb"), AppConfig.GetAppConfig("sessionName"));

            
        }

        void OutputXELStream(string connectionString, string sessionName)
        {
            var cancellationTokenSource = new CancellationTokenSource();

            var xeStream = new XELiveEventStreamer(connectionString, sessionName);

            //Console.WriteLine("Press any key to stop listening...");
            //Task waitTask = Task.Run(() =>
            //{
            //    //this.backgroundWorker1.RunWorkerAsync();
            //    txtResult.Text += "Press any key to stop listening...";
            //    //Debug.WriteLine("wait");

            //    //Thread.Sleep(100000);

            //    cancellationTokenSource.Cancel();
            //});

            Task readTask = xeStream.ReadEventStream(() =>
            {
                txtCount.Text = cnt.ToString();                

                txtResult.Text += "Connected to session" + LongLine();
                return Task.CompletedTask;
            },
                xevent =>
                {
                    //Console.WriteLine(xevent);
                    //Console.WriteLine("");

                    cnt++;

                    txtCount.Text = cnt.ToString();
                    txtLastTime.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                    //txtResult.Text += xevent.ToString() + LongLine();
                    txtResult.AppendText(xevent.ToString() + LongLine());

                    //txtResult.SelectionStart = txtResult.Text.Length;
                    //txtResult.ScrollToCaret();

                    return Task.CompletedTask;
                },
                cancellationTokenSource.Token);


            try
            {
                //Task.WaitAny(waitTask, readTask);
                Task.WhenAll(readTask);
            } catch (TaskCanceledException)
            {
                //Debug.WriteLine(err.ToString());
            }

            if (readTask.IsFaulted)
            {
                //Console.Error.WriteLine("Failed with: {0}", readTask.Exception);
                //txtResult.Text += readTask.Exception.ToString() + Environment.NewLine;
                txtResult.AppendText(readTask.Exception.ToString() + Environment.NewLine);
            }
        }

        private string LongLine()
        {
            return Environment.NewLine + String.Concat(Enumerable.Repeat("-", 100)) + Environment.NewLine; 
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            cnt = 0;

            txtCount.Text = cnt.ToString();
            txtLastTime.Text = "";
            txtResult.Text = "";
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.txtResult.Text = "Press any key to stop listening...";
        }
    }
}
