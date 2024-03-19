using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using IOPath = System.IO.Path;
using System.Net;
using System.IO;
using System.Windows.Forms;

namespace TechProgram4
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ProcessFolderButton_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                DialogResult result = dialog.ShowDialog();

                if (result == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(dialog.SelectedPath))
                {
                    string[] files = Directory.GetFiles(dialog.SelectedPath);

                    if (files.Length == 0)
                    {
                        System.Windows.MessageBox.Show("Выбранная папка не содержит файлов.");
                        return;
                    }

                    bool useHttps = HttpsCheckBox.IsChecked ?? false;

                    ProcessFiles(files, useHttps);

                    System.Windows.MessageBox.Show("Файлы обработаны успешно.");
                }
            }
        }

        private void ProcessFiles(string[] filePaths, bool useHttps)
        {
            int fileCount = filePaths.Length;
            int processedCount = 0;

            for (int i = 0; i < fileCount; i++)
            {
                string originalPath = filePaths[i];

                string processedPath = ProcessFilePath(originalPath, useHttps, i + 1);

                string directory = IOPath.GetDirectoryName(originalPath);
                string newFileName = IOPath.Combine(directory, processedPath);
                File.Move(originalPath, newFileName);

                processedCount++;
                OutputTextBox.AppendText($"Обработан файл {processedCount} из {fileCount}: {processedPath}{Environment.NewLine}");
            }
        }

        private string ProcessFilePath(string path, bool useHttps, int index)
        {
            string processedPath = ReplaceLocalLink(path, useHttps);

            processedPath = AddFileIndex(processedPath, index);

            processedPath = RemoveCharacters(processedPath);

            return processedPath;
        }

        private string ReplaceLocalLink(string path, bool useHttps)
        {
            if (path.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                path.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                return path;
            }

            string protocol = useHttps ? "https://" : "http://";
            return protocol + IOPath.GetFileName(path);
        }

        private string AddFileIndex(string path, int index)
        {
            string extension = IOPath.GetExtension(path);
            string fileNameWithoutExtension = IOPath.GetFileNameWithoutExtension(path);

            string indexString = index < 10 ? $"0{index}" : index.ToString();

            return $"{fileNameWithoutExtension}_{indexString}{extension}";
        }

        private string RemoveCharacters(string path)
        {
            return Regex.Replace(path, "[#!_]", "");
        }

    }
}
