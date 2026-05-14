using System.Diagnostics;
using StatusWatch.Services;

namespace StatusWatch.BackgroundServices;

public class PingBackgroundService : BackgroundService
{
    private static readonly TimeSpan Interval = TimeSpan.FromMinutes(5);
    private static readonly TimeSpan HttpTimeout = TimeSpan.FromSeconds(10);

    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IHttpClientFactory _httpFactory;
    private readonly ILogger<PingBackgroundService> _logger;

    public PingBackgroundService(
        IServiceScopeFactory scopeFactory,
        IHttpClientFactory httpFactory,
        ILogger<PingBackgroundService> logger)
    {
        _scopeFactory = scopeFactory;
        _httpFactory = httpFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken token)
    {
        _logger.LogInformation("PingBackgroundService demarre (intervalle {Min} min)", Interval.TotalMinutes);

        while (!token.IsCancellationRequested)
        {
            try
            {
                await PingAllServices(token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur durant le cycle de ping");
            }

            try
            {
                await Task.Delay(Interval, token);
            }
            catch (TaskCanceledException)
            {
                break;
            }
        }
    }

    private async Task PingAllServices(CancellationToken token)
    {
        using var scope = _scopeFactory.CreateScope();
        var services = scope.ServiceProvider.GetRequiredService<ServiceService>();
        var pings = scope.ServiceProvider.GetRequiredService<PingService>();

        var http = _httpFactory.CreateClient();
        http.Timeout = HttpTimeout;

        foreach (var s in services.GetAll())
        {
            if (string.IsNullOrWhiteSpace(s.Url))
            {
                continue;
            }

            var sw = Stopwatch.StartNew();
            string statut;
            try
            {
                var resp = await http.GetAsync(s.Url, token);
                statut = resp.IsSuccessStatusCode ? "up" : "down";
            }
            catch (OperationCanceledException) when (token.IsCancellationRequested)
            {
                // arret de l'app, on sort proprement
                throw;
            }
            catch (Exception)
            {
                statut = "down";
            }
            sw.Stop();
            int latence = (int)sw.ElapsedMilliseconds;

            pings.Log(s.Id, statut, latence);
            pings.UpdateServiceStatusFromPing(s.Id, statut);

            _logger.LogInformation("Ping service {ServiceId} ({Nom}) : {Statut} en {Latence}ms",
                s.Id, s.Nom, statut, latence);
        }
    }
}
