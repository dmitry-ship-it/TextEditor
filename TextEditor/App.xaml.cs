using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Serialization;
using TextEditor.FileHandling;
using TextEditor.Resources;

namespace TextEditor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application, IDisposable
    {
        private readonly ServiceProvider _serviceProvider;

        public App()
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            _serviceProvider = services.BuildServiceProvider();
        }

        private static void ConfigureServices(ServiceCollection services)
        {
            CreateRequiredFiles();

            services.AddSingleton<IFileHandler, FileHandler>();

            services.AddSingleton<MainWindow>();
            services.AddSingleton<SettingsWindow>();
            services.AddSingleton(JsonSerializer.Deserialize<GeneralData>(File.ReadAllText(GeneralData.FilePath))!);
        }

        private static void CreateRequiredFiles()
        {
            GeneralData.CreateFile();
        }

        private void OnStartup(object sender, StartupEventArgs e)
        {
            var mainWindow = _serviceProvider.GetService<MainWindow>()
                ?? throw new ArgumentException("Service not found.");

            mainWindow.Show();
        }

        #region Dispose

        private bool _disposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Protected implementation of Dispose pattern.
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _serviceProvider.Dispose();
                }

                _disposed = true;
            }
        }

        #endregion
    }
}
