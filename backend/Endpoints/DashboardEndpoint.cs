using backend.Models;
using backend.Repositories;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Shared.Contracts;

namespace backend.Endpoints;

public static class DashboardEndpoint
{
    public static void MapDashboard(this WebApplication app)
    { 
        // Collection
        app.MapGet("/api/dashboards", async (IDashboardRepository repo) =>
        {
            var all = await repo.GetAllWithWidgetsAsync();
            return Results.Ok(all);
        }).WithTags("Dashboards");

        // Single by id
        app.MapGet("/api/dashboards/{id:int}", async (int id, IDashboardRepository repo) =>
        {
            var dash = await repo.GetByIdWithWidgetsAsync(id);
            return dash is null ? Results.NotFound() : Results.Ok(dash);
        }).WithTags("Dashboards");

        // Create
        app.MapPost("/api/dashboards", async (Dashboard dashboard, IDashboardRepository repo) =>
        {
            await repo.AddAsync(dashboard);
            await repo.SaveChangesAsync();
            return Results.Created($"/api/dashboards/{dashboard.Id}", dashboard);
        }).WithTags("Dashboards");

        // Update
        app.MapPut("/api/dashboards/{id:int}", async (int id, Dashboard updated, IDashboardRepository repo) =>
        {
            var existing = await repo.GetByIdAsync(id);
            if (existing is null) return Results.NotFound();

            existing.Name = updated.Name;
            existing.Description = updated.Description;
            existing.UpdatedAt = DateTime.UtcNow;

            await repo.UpdateAsync(existing);
            await repo.SaveChangesAsync();
            return Results.NoContent();
        }).WithTags("Dashboards");

        // Delete
        app.MapDelete("/api/dashboards/{id:int}", async (int id, IDashboardRepository repo) =>
        {
            var existing = await repo.GetByIdAsync(id);
            if (existing is null) return Results.NotFound();

            await repo.DeleteAsync(id);
            await repo.SaveChangesAsync();
            return Results.NoContent();
        }).WithTags("Dashboards");
    }
}
