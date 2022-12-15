using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EdgesWebAPI.BackgroundService
{
    public class FileDeletionService : IHostedService, IDisposable
    {
        private readonly ILogger<FileDeletionService> _logger;
        private Timer? _timer = null;

        public FileDeletionService(ILogger<FileDeletionService> logger)
        {
            _logger = logger;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("File Deletion Service running.");

            _timer = new Timer(DoWork, null, TimeSpan.FromSeconds(5),
                TimeSpan.FromSeconds(30));

            return Task.CompletedTask;
        }

        private void DoWork(object? state)
        {
            _logger.LogInformation("File deletions starting.");
            foreach (var file in Directory.GetFiles("./files", "*", SearchOption.AllDirectories))
            {
                File.Delete(file);
                _logger.LogInformation($"File {Path.GetFileName(file)} deleted from files directory");
            }
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("File Deletion Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}