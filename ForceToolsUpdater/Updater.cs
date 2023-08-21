using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Threading;
using System.Windows;

namespace ForceToolsUpdater
{
    public class Updater
    {
        private string updateTempFolderPath;
        private string zipFilePath;
        private string ForceToolsExePath;
        private MainWindow mainWindow;

        public Updater(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
            updateTempFolderPath = AppDomain.CurrentDomain.BaseDirectory + @"/Temp/LatestUpdate";
            zipFilePath = updateTempFolderPath + @"/LatestRelease.zip";
            ForceToolsExePath = AppDomain.CurrentDomain.BaseDirectory + @"/ForceTools.exe";
            CreateTempFolder();
            mainWindow.MessageLabel.Content = "Стартиране на обновяването...";
        }

        private void CreateTempFolder()
        {
            if (Directory.Exists(updateTempFolderPath) == false) Directory.CreateDirectory(updateTempFolderPath);
        }

        public void UpdateFiles()
        {
            try
            {
                WebClient webClient = new WebClient();
                webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(DownloadCompletedCallback);
                mainWindow.MessageLabel.Content = "Изтегляне на файлове...";
                webClient.DownloadFileAsync(new Uri("https://github.com/ValentinStoqnov/ForceToolsUpdateFiles/raw/main/LatestRelease.zip"), zipFilePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Проблем при изтеглянето на обновлението!: {ex}");
            }
        }

        private void DownloadCompletedCallback(object sender, AsyncCompletedEventArgs e)
        {
            try
            {
                mainWindow.MessageLabel.Content = "Обновяване на версия...";
                File.Delete(ForceToolsExePath);
                ZipFile.ExtractToDirectory(zipFilePath, AppDomain.CurrentDomain.BaseDirectory);
                Directory.Delete(updateTempFolderPath,true);
                mainWindow.MessageLabel.Content = "Обновяването е приключено!";
                Process.Start(ForceToolsExePath);
                mainWindow.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Проблем при инсталирането на обновлението!: {ex}");
            }
        }
    }
}
