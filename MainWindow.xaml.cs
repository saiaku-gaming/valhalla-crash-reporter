using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Windows;
using System.Windows.Input;

namespace ValhallaCrashReporter
{
    public partial class MainWindow : Window
    {
        readonly string FilePath;
        readonly string Version;
        readonly string ZipFilePath;
        readonly HttpClient httpClient = new HttpClient();

        public MainWindow(string[] args)
        {
            InitializeComponent();
            SubmitButton.IsEnabled = false;
            CancelButton.IsEnabled = false;
            FilePath = args[0];
            ZipFilePath = FilePath + ".zip";
            CrashPath.Content = FilePath;
            Version = ExtractVersion();
            
            File.WriteAllText(FilePath + @"\version.txt", Version);
            CleanupZipFile();
            ZipFile.CreateFromDirectory(FilePath, ZipFilePath);
            SubmitButton.IsEnabled = true;
            CancelButton.IsEnabled = true;
        }

        private void CleanupZipFile()
        {
            if (File.Exists(ZipFilePath))
            {
                File.Delete(ZipFilePath);
            }
        }

        private string ExtractVersion()
        {
            string ConfigFilePath = FilePath + @"\..\..\..\Config\DefaultGame.ini";
            if (!File.Exists(ConfigFilePath))
            {
                return "NO_CONFIG_FILE_FOUND";
            }

            foreach (string Line in File.ReadAllLines(ConfigFilePath))
            {
                if (Line.Contains("GameVersion"))
                {
                    string[] VersionLine = Line.Split('=');
                    if (VersionLine.Length > 1)
                    {
                        return VersionLine[1].Replace("\"", "");
                    }
                }
            }
            return "NO_VERSION_FOUND_IN_CONFIG";
        }

        private void SubmitClick(object sender, RoutedEventArgs e)
        {
            SubmitButton.IsEnabled = false;
            CancelButton.IsEnabled = false;
            UploadFiles();
            CleanupZipFile();
            Application.Current.Shutdown();
        }

        private void CancelClick(object sender, RoutedEventArgs e)
        {
            CleanupZipFile();
            Application.Current.Shutdown();
        }

        private void UploadFiles()
        {
            using (var content =
                new MultipartFormDataContent("Upload----" + DateTime.Now.ToString(CultureInfo.InvariantCulture)))
            {
                FileStream FileStream = File.OpenRead(ZipFilePath);
                content.Add(new StreamContent(FileStream), "file", Path.GetFileName(ZipFilePath));
                content.Add(new StringContent(Version), "version");
                content.Add(new StringContent(Description.Text), "description");
                httpClient.PostAsync(BuildInfo.CrashUrl, content).Wait();
                Console.WriteLine("finish upload");
            }
        }

        private void CrashPath_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Process.Start(FilePath);
        }
    }
}
