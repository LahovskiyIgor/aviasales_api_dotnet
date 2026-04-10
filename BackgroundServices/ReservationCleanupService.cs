using AirlineAPI.Interfaces.Services;
using AirlineAPI.Services;

namespace AirlineAPI.BackgroundServices
{
    public class ReservationCleanupService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ReservationCleanupService> _logger;
        private readonly TimeSpan _cleanupInterval = TimeSpan.FromMinutes(1);

        public ReservationCleanupService(
            IServiceProvider serviceProvider,
            ILogger<ReservationCleanupService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Сервис очистки просроченных резервирований запущен");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(_cleanupInterval, stoppingToken);
                    await CleanupExpiredReservations();
                }
                catch (OperationCanceledException)
                {
                    _logger.LogInformation("Сервис очистки просроченных резервирований остановлен");
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Ошибка при очистке просроченных резервирований");
                }
            }
        }

        private async Task CleanupExpiredReservations()
        {
            using var scope = _serviceProvider.CreateScope();
            var ticketService = scope.ServiceProvider.GetRequiredService<ITicketService>();

            _logger.LogInformation("Начало проверки просроченных резервирований");
            
            await ticketService.CancelExpiredReservationsAsync();
            
            _logger.LogInformation("Проверка просроченных резервирований завершена");
        }

        /// <summary>
        /// Принудительная очистка expired резервирований (для отладки)
        /// </summary>
        public async Task ForceCleanupAsync()
        {
            await CleanupExpiredReservations();
        }
    }
}
