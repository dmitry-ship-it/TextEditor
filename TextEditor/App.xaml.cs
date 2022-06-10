using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Text.Json;
using System.Windows;
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

            // create DI container
            _serviceProvider = services.BuildServiceProvider();
        }

        private static void ConfigureServices(ServiceCollection services)
        {
            // create seetings file if it not exists
            CreateRequiredFiles();

            // DI
            services.AddSingleton<MainWindow>();
            services.AddSingleton(JsonSerializer.Deserialize<EditorSettings>(File.ReadAllText(EditorSettings.FilePath))!);
        }

        private static void CreateRequiredFiles()
        {
            EditorSettings.CreateFile();
        }

        /// <summary>
        /// Creates <c>MainWindow</c> by using DI container.
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        private void OnStartup(object sender, StartupEventArgs e)
        {
            var mainWindow = _serviceProvider.GetService<MainWindow>()
                ?? throw new ArgumentException("Service not found.");

            mainWindow.Show();
        }

        /// <summary>
        /// Explicitly dispose service provider.
        /// </summary>
        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            Dispose();
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
