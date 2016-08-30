using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace directoryTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            FileSystemWatcher watcher = new FileSystemWatcher();
            watcher.Path = System.IO.Path.GetDirectoryName(Application.ExecutablePath) + "\\files";

            watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
               | NotifyFilters.FileName | NotifyFilters.DirectoryName;

            watcher.Changed += new FileSystemEventHandler(OnChanged);
            watcher.Created += new FileSystemEventHandler(OnChanged);
            watcher.Deleted += new FileSystemEventHandler(OnChanged);
            watcher.Renamed += new RenamedEventHandler(OnRenamed);

            watcher.IncludeSubdirectories = true;
            watcher.EnableRaisingEvents = true;
        }

        private void OnChanged(object source, FileSystemEventArgs e)
        {
            if (e.ChangeType == WatcherChangeTypes.Created)
            {
                FileAttributes attr;

                try
                {
                    attr = File.GetAttributes(e.FullPath);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    attr = FileAttributes.NoScrubData;
                }
                
                if (attr.HasFlag(FileAttributes.NoScrubData))
                {

                }
                else if (attr.HasFlag(FileAttributes.Directory))
                {

                    try
                    {
                        string relPath = (e.FullPath).Replace((Path.GetDirectoryName(Application.ExecutablePath) + "\\files\\"), "").Replace("\\", "|");

                        string url = "http://192.168.1.119:8080/newfolder/" + relPath;

                        //richTextBox1.Text = richTextBox1.Text + url + "\n";

                        MessageBox.Show(url);

                        //MessageBox.Show(relPath);

                        //MessageBox.Show(relPath + " vs. " + e.Name);

                        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

                        HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                        Stream resStream = response.GetResponseStream();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                        OnChanged(source, e);
                    }

                    
                }
                    
                else
                {
                    try
                    {
                        using (WebClient client = new WebClient())
                        {
                            string relPath = (e.FullPath).Replace((Path.GetDirectoryName(Application.ExecutablePath) + "\\files\\"), "").Replace("\\", "|");
                            byte[] response = client.UploadFile("http://192.168.1.119:8080/create/" + relPath, e.FullPath);
                            //richTextBox1.Text = richTextBox1.Text + "http://192.168.1.119:8080/create/" + relPath + " with file: " + e.FullPath + "\n";
                            string result = Encoding.UTF8.GetString(response);
                            Console.WriteLine(response);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                        OnChanged(source, e);
                    }
                }
                    
               
                
            }
            else if (e.ChangeType == WatcherChangeTypes.Changed)
            {
                FileAttributes attr;

                try
                {
                    attr = File.GetAttributes(e.FullPath);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    attr = FileAttributes.NoScrubData;
                }

                if (!attr.HasFlag(FileAttributes.Directory))
                {
                    try
                    {
                        using (WebClient client = new WebClient())
                        {
                            string relPath = (e.FullPath).Replace((Path.GetDirectoryName(Application.ExecutablePath) + "\\files\\"), "").Replace("\\", "|");
                            byte[] response = client.UploadFile("http://192.168.1.119:8080/create/" + relPath, e.FullPath);
                            //richTextBox1.Text = richTextBox1.Text + "http://192.168.1.119:8080/create/" + relPath + " with file: " + e.FullPath + "\n";
                            string result = Encoding.UTF8.GetString(response);
                            Console.WriteLine(response);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                        OnChanged(source, e);
                    }
                }
            }
            else if (e.ChangeType == WatcherChangeTypes.Deleted)
            {
                if (!e.FullPath.ToString().Contains("."))
                {

                    try
                    {
                        string relPath = (e.FullPath).Replace((Path.GetDirectoryName(Application.ExecutablePath) + "\\files\\"), "").Replace("\\", "|");

                        string url = "http://192.168.1.119:8080/deletedir/" + relPath;

                        //richTextBox1.Text = richTextBox1.Text + url + "\n";

                        //MessageBox.Show(url);

                        //MessageBox.Show(relPath);

                        //MessageBox.Show(relPath + " vs. " + e.Name);

                        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

                        HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                        Stream resStream = response.GetResponseStream();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                        OnChanged(source, e);
                    }


                }

                else
                {
                    try
                    {
                        string relPath = (e.FullPath).Replace((Path.GetDirectoryName(Application.ExecutablePath) + "\\files\\"), "").Replace("\\", "|");

                        string url = "http://192.168.1.119:8080/delete/" + relPath;

                        //richTextBox1.Text = richTextBox1.Text + url + "\n";

                        //MessageBox.Show(url);

                        //MessageBox.Show(relPath);

                        //MessageBox.Show(relPath + " vs. " + e.Name);

                        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

                        HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                        Stream resStream = response.GetResponseStream();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                        OnChanged(source, e);
                    }
                }
            }
        }

        private void OnRenamed(object source, RenamedEventArgs e)
        {
            try
            {
                string relPathOld = (e.OldFullPath).Replace((Path.GetDirectoryName(Application.ExecutablePath) + "\\files\\"), "").Replace("\\", "|");
                string relPathNew = (e.FullPath).Replace((Path.GetDirectoryName(Application.ExecutablePath) + "\\files\\"), "").Replace("\\", "|");

                string url = "http://192.168.1.119:8080/rename/" + relPathOld + "/" + relPathNew;

                //richTextBox1.Text = richTextBox1.Text + url + "\n";

                //MessageBox.Show(url);

                //MessageBox.Show(relPathOld + "****" + relPathNew);

                //MessageBox.Show(relPath + " vs. " + e.Name);

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                Stream resStream = response.GetResponseStream();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                OnChanged(source, e);
            }
        }
    }
}
