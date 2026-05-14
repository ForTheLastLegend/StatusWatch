using StatusWatch.Services;

namespace StatusWatch.Endpoints;

// Minimal API : tous les endpoints lecture seule, regroupes sous /api.
// Pas d'ecriture exposee : le CRUD reste sur les pages Razor admin avec authentification.
public static class StatusEndpoints
{
    public static void MapStatusEndpoints(this WebApplication app)
    {
        var api = app.MapGroup("/api").WithTags("StatusWatch");

        // resume global : nombre de services par statut + nombre d'incidents actifs
        api.MapGet("/status", (ServiceService services, IncidentService incidents) =>
        {
            var allServices = services.GetAll();
            var byStatut = allServices
                .GroupBy(s => s.Statut)
                .ToDictionary(g => g.Key, g => g.Count());

            return Results.Ok(new
            {
                services_total = allServices.Count,
                services_par_statut = byStatut,
                incidents_actifs = incidents.GetActive().Count
            });
        })
        .WithName("GetStatus")
        .WithSummary("Resume global de la plateforme");

        api.MapGet("/services", (ServiceService services) =>
            Results.Ok(services.GetAll()))
            .WithName("GetServices")
            .WithSummary("Liste des services");

        api.MapGet("/services/{id:int}", (int id, ServiceService services) =>
        {
            var service = services.GetById(id);
            return service is null ? Results.NotFound() : Results.Ok(service);
        })
        .WithName("GetServiceById")
        .WithSummary("Detail d'un service");

        api.MapGet("/services/{id:int}/incidents", (int id, ServiceService services, IncidentService incidents) =>
        {
            // 404 si le service n'existe pas (sinon liste vide ambigue)
            if (services.GetById(id) is null)
            {
                return Results.NotFound();
            }
            return Results.Ok(incidents.GetByService(id));
        })
        .WithName("GetServiceIncidents")
        .WithSummary("Incidents d'un service");

        api.MapGet("/incidents", (IncidentService incidents) =>
            Results.Ok(incidents.GetAll()))
            .WithName("GetIncidents")
            .WithSummary("Historique complet des incidents");
    }
}
