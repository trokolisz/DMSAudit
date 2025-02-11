using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using DMSAudit.ApiService.Data;
using DMSAudit.ApiService.Models;

namespace DMSAudit.ApiService.Endpoints;

public static class CriteriaEndpoints{
    public static void MapCriteriaEndpoints(this IEndpointRouteBuilder app ){
        app.MapGet("/criterias", async (DmsDbContext db, CancellationToken ct) =>
            await db.Criterias
                .Select(c => new CriteriaOnly
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    Group = c.Group,
                })
                .ToListAsync(ct))
        .WithName("GetCriterias")
        .WithDescription("Retrieves all Criterias without related entities")
        .Produces<List<CriteriaOnly>>(StatusCodes.Status200OK);
    
        app.MapGet("/criterias/{id}", async (int id, int? year, int? month, DmsDbContext db, CancellationToken ct) =>
        {
            var currentDate = DateTime.Now;
            var targetYear = year ?? currentDate.Year;
            var targetMonth = month ?? currentDate.Month;

            var criteria = await db.Criterias
                .Include(c => c.CriteriaStates.Where(cs =>
                    cs.Year == targetYear && 
                    cs.Month == targetMonth))
                .Include(c => c.Levels)
                    .ThenInclude(l => l.LevelStates.Where(ls =>
                        ls.Year == targetYear &&
                        ls.Month == targetMonth))
                .FirstOrDefaultAsync(c => c.Id == id, ct);

            if (criteria == null)
                return Results.NotFound();

            return Results.Ok(criteria);
        })
        .WithName("GetCriteriaById")
        .WithDescription("Retrieves a specific Criteria by ID with its CriteriaStates and LevelStates for the given year and month (defaults to current date)")
        .Produces<Criteria>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);
       
    
        app.MapPost("/criterias", async (CriteriaCreate criteriaCreate, DmsDbContext db, CancellationToken ct) =>
        {
            var criteria = new Criteria
            {
                Name = criteriaCreate.Name,
                Description = criteriaCreate.Description,
                Group = criteriaCreate.Group,
                Levels = []
            };

            // Create levels 0-4 for this criteria
            for (short i = 0; i <= 4; i++)
            {
                var level = new Level
                {
                    Level_ = i,
                    Description = criteriaCreate.LevelDescriptions[i],
                    Criteria = criteria
                };
                criteria.Levels.Add(level);
            }

            db.Criterias.Add(criteria);
            await db.SaveChangesAsync(ct);

            return Results.Created($"/criterias/{criteria.Id}", criteria);
        })
        .WithName("CreateCriteria")
        .WithDescription("Creates a new Criteria with 5 levels (0-4)")
        .Produces<Criteria>(StatusCodes.Status201Created)
        .RequireAuthorization();

   
    }
}