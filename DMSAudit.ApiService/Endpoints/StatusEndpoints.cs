using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using DMSAudit.ApiService.Data;
using DMSAudit.ApiService.Models;

namespace DMSAudit.ApiService.Endpoints;

public static class StatusEndpoints{
    public static void MapStatusEndpoints(this IEndpointRouteBuilder app ){
    app.MapPost("/criteria-state/{id}", async (int id, int? year, int? month, CriteriaState criteriaState, DmsDbContext db, CancellationToken ct) =>
        {
            if (criteriaState == null)
                return Results.BadRequest("Criteria state cannot be null");

            var currentDate = DateTime.Now;
            var targetYear = year ?? currentDate.Year;
            var targetMonth = month ?? currentDate.Month;
            
            var criteria = await db.Criterias.FindAsync([id], ct);
            if (criteria == null)
                return Results.NotFound();

            // Calculate previous month
            var prevMonth = targetMonth == 1 ? 12 : targetMonth - 1;
            var prevYear = targetMonth == 1 ? targetYear - 1 : targetYear;

            // Try to get previous month's state
            var prevState = await db.CriteriaStates
                .FirstOrDefaultAsync(cs => 
                    cs.CriteriaId == id && 
                    cs.Year == prevYear && 
                    cs.Month == prevMonth, 
                    ct);

            var newState = new CriteriaState
            {
                CriteriaId = id,
                Year = (short)targetYear,
                Month = (byte)targetMonth,
                CurrentLvl = prevState?.CurrentLvl ?? 0,
                Comment = prevState?.Comment
            };

            db.CriteriaStates.Add(newState);
            await db.SaveChangesAsync(ct);

            return Results.Created($"/criteria-status/{id}/{targetYear}/{targetMonth}", newState);
        })
        .WithName("CreateCriteriaStatus") 
        .WithDescription("Creates a new status entry for a criteria for the specified year and month")
        .RequireAuthorization()
        .Produces<CriteriaState>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status404NotFound);
        
        app.MapPut("/criteria-state/{id}/{year}/{month}/level", async (int id, int year, int month, short newLevel, DmsDbContext db, HttpContext context, CancellationToken ct) =>
        {
            var state = await db.CriteriaStates
                .FirstOrDefaultAsync(cs => 
                    cs.CriteriaId == id && 
                    cs.Year == year && 
                    cs.Month == month, 
                    ct);

            if (state == null)
                return Results.NotFound();

            if (state.Closed)
                return Results.BadRequest("Cannot modify a closed state");

            state.CurrentLvl = newLevel;
            state.ModifiedDate = DateTime.UtcNow;
            state.ModifiedBy = context.User?.Identity?.Name ?? "unknown";

            await db.SaveChangesAsync(ct);
            return Results.Ok(state);
        })
        .WithName("UpdateCriteriaLevel")
        .Produces<CriteriaState>(StatusCodes.Status200OK)
        .RequireAuthorization()
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status400BadRequest);

        app.MapPut("/criteria-state/{id}/{year}/{month}/comment", async (int id, int year, int month, string newComment, DmsDbContext db, HttpContext context, CancellationToken ct) =>
        {
            var state = await db.CriteriaStates
                .FirstOrDefaultAsync(cs => 
                    cs.CriteriaId == id && 
                    cs.Year == year && 
                    cs.Month == month, 
                    ct);

            if (state == null)
                return Results.NotFound();

            if (state.Closed)
                return Results.BadRequest("Cannot modify a closed state");

            state.Comment = newComment;
            state.ModifiedDate = DateTime.UtcNow;
            state.ModifiedBy = context.User?.Identity?.Name ?? "unknown";

            await db.SaveChangesAsync(ct);
            return Results.Ok(state);
        })
        .WithName("UpdateCriteriaComment")
        .Produces<CriteriaState>(StatusCodes.Status200OK)
        .RequireAuthorization()
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status400BadRequest);

        app.MapPut("/criteria-state/{id}/{year}/{month}/close", async (int id, int year, int month, string? closingComment, DmsDbContext db, HttpContext context, CancellationToken ct) =>
        {
            var state = await db.CriteriaStates
                .FirstOrDefaultAsync(cs => 
                    cs.CriteriaId == id && 
                    cs.Year == year && 
                    cs.Month == month, 
                    ct);

            if (state == null)
                return Results.NotFound();

            if (state.Closed)
                return Results.BadRequest("State is already closed");

            var currentUser = context.User?.Identity?.Name ?? "unknown";
            var currentTime = DateTime.UtcNow;

            state.Closed = true;
            state.ClosedDate = currentTime;
            state.ClosedBy = currentUser;
            state.ClosingComment = closingComment;
            
            // Also update the general modification tracking
            state.ModifiedDate = currentTime;
            state.ModifiedBy = currentUser;

            await db.SaveChangesAsync(ct);
            return Results.Ok(state);
        })
        .WithName("CloseCriteriaState")
        .Produces<CriteriaState>(StatusCodes.Status200OK)
        .RequireAuthorization()
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status400BadRequest);
    }
}