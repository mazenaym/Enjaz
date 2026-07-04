using Enjaz.Maps.Application.Maps;
using Enjaz.Maps.Domain.Maps;
using Enjaz.Maps.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Enjaz.Maps.Infrastructure.Maps;

public sealed class MapsRepository(MapsDbContext dbContext, IConfiguration configuration) : IMapsRepository, ITechnicianLocationLookupService
{
    private readonly string connectionString = configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");

    public async Task<ServiceZone?> GetServiceZoneAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await dbContext.ServiceZones.FirstOrDefaultAsync(zone => zone.Id == id, cancellationToken);
    }

    public async Task<ServiceZone?> GetServiceZoneBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        return await dbContext.ServiceZones.FirstOrDefaultAsync(zone => zone.Slug == slug, cancellationToken);
    }

    public async Task<ServiceZone?> GetContainingActiveServiceZoneAsync(decimal latitude, decimal longitude, CancellationToken cancellationToken = default)
    {
        return await dbContext.ServiceZones
            .FromSqlInterpolated($"""
                SELECT *
                FROM maps.service_zones
                WHERE is_active = TRUE
                  AND ST_Covers(polygon, ST_SetSRID(ST_MakePoint({longitude}, {latitude}), 4326))
                ORDER BY created_at_utc
                LIMIT 1
                """)
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<ServiceZone>> GetActiveServiceZonesAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.ServiceZones
            .AsNoTracking()
            .Where(zone => zone.IsActive)
            .OrderBy(zone => zone.City)
            .ThenBy(zone => zone.Area)
            .ThenBy(zone => zone.NameEn ?? zone.NameAr)
            .ToArrayAsync(cancellationToken);
    }

    public async Task AddServiceZoneAsync(ServiceZone serviceZone, CancellationToken cancellationToken = default)
    {
        await dbContext.ServiceZones.AddAsync(serviceZone, cancellationToken);
    }

    public async Task<TechnicianLocationSnapshot?> GetTechnicianLocationSnapshotAsync(Guid technicianId, CancellationToken cancellationToken = default)
    {
        return await dbContext.TechnicianLocationSnapshots.FirstOrDefaultAsync(snapshot => snapshot.TechnicianId == technicianId, cancellationToken);
    }

    public async Task<TechnicianLocationLookupResponse?> GetLatestLocationAsync(Guid technicianId, CancellationToken cancellationToken = default)
    {
        return await dbContext.TechnicianLocationSnapshots
            .AsNoTracking()
            .Where(snapshot => snapshot.TechnicianId == technicianId)
            .Select(snapshot => new TechnicianLocationLookupResponse(snapshot.TechnicianId, snapshot.Latitude, snapshot.Longitude, snapshot.UpdatedAtUtc))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task AddTechnicianLocationSnapshotAsync(TechnicianLocationSnapshot snapshot, CancellationToken cancellationToken = default)
    {
        await dbContext.TechnicianLocationSnapshots.AddAsync(snapshot, cancellationToken);
    }

    public async Task AddTechnicianLocationHistoryAsync(TechnicianLocationHistory history, CancellationToken cancellationToken = default)
    {
        await dbContext.TechnicianLocationHistory.AddAsync(history, cancellationToken);
    }

    public async Task<IReadOnlyCollection<NearbyTechnicianResponse>> GetNearbyTechniciansAsync(decimal latitude, decimal longitude, Guid serviceId, int radiusMeters, CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT
                p.id AS technician_id,
                p.full_name,
                p.profile_image_url,
                p.average_rating,
                p.total_reviews,
                ST_Distance(
                    s.location::geography,
                    ST_SetSRID(ST_MakePoint(@longitude, @latitude), 4326)::geography
                ) AS distance_meters,
                p.availability_status,
                sk.skill_level
            FROM maps.technician_location_snapshots AS s
            INNER JOIN technicians.technician_profiles AS p ON p.id = s.technician_id
            INNER JOIN technicians.technician_skills AS sk ON sk.technician_id = p.id
            WHERE p.status = 'Approved'
              AND p.availability_status = 'Online'
              AND sk.service_id = @service_id
              AND ST_DWithin(
                    s.location::geography,
                    ST_SetSRID(ST_MakePoint(@longitude, @latitude), 4326)::geography,
                    @radius_meters
                  )
            ORDER BY distance_meters ASC
            """;

        var technicians = new List<NearbyTechnicianResponse>();
        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken);
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("latitude", latitude);
        command.Parameters.AddWithValue("longitude", longitude);
        command.Parameters.AddWithValue("service_id", serviceId);
        command.Parameters.AddWithValue("radius_meters", radiusMeters);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            technicians.Add(new NearbyTechnicianResponse(
                reader.GetGuid(reader.GetOrdinal("technician_id")),
                reader.GetString(reader.GetOrdinal("full_name")),
                reader.IsDBNull(reader.GetOrdinal("profile_image_url")) ? null : reader.GetString(reader.GetOrdinal("profile_image_url")),
                reader.GetDecimal(reader.GetOrdinal("average_rating")),
                reader.GetInt32(reader.GetOrdinal("total_reviews")),
                Convert.ToDecimal(reader.GetDouble(reader.GetOrdinal("distance_meters"))),
                reader.GetString(reader.GetOrdinal("availability_status")),
                reader.IsDBNull(reader.GetOrdinal("skill_level")) ? null : reader.GetString(reader.GetOrdinal("skill_level"))));
        }

        return technicians;
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
