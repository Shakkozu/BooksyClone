using BooksyClone.Contract.BusinessManagement;
using BooksyClone.Contract.Shared;
using BooksyClone.Domain.Availability.Storage;
using BooksyClone.Domain.Shared.Exceptions;
using Dapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace BooksyClone.Domain.BusinessManagement.FetchingBusinessConfiguration;

internal class GetBusinessConfiguration(DbConnectionFactory _dbConnectionFactory)
{
    private  class GetEmployeeServiceDao
    {
        public Guid Guid { get; set; }
        public Guid EmployeeId { get; set; }
        public Guid BusinessId { get; set; }
        public string Name { get; set; }
        public string MarkdownDescription { get; set; }
        public IEnumerable<int> GenericServiceVariantsIds { get; set; }
        public TimeSpan Duration { get; set; }
        public string Price { get; set; }
        public int Order { get; set; }
        public long CategoryId { get; set; }
    }

    internal async Task<BusinessServiceConfigurationDto?> HandleAsync(Guid businessUnitId, CancellationToken ct)
    {
        using var connection = _dbConnectionFactory.CreateConnection();
        connection.Open();
        
        var businessExists = await connection.ExecuteScalarAsync<bool>(
            "SELECT EXISTS(SELECT 1 FROM business_management.business_drafts WHERE guid = @businessUnitId)",
            new { businessUnitId });
        if (!businessExists)
            return null;

        const string query = @"
            SELECT
                guid AS Guid,
                employee_id AS EmployeeId,
                business_id AS BusinessId,
                name,
                markdown_description AS MarkdownDescription,
                generic_service_variants_ids AS GenericServiceVariantsIds,
                duration AS Duration,
                price AS Price,
                ""order"",
                category_id AS CategoryId
            FROM
                business_management.employee_services
            WHERE
                business_id = @businessUnitId
            ORDER BY ""order""";

        var services = await connection.QueryAsync<GetEmployeeServiceDao>(query, new { businessUnitId });
        

        return new BusinessServiceConfigurationDto
        {
            BusinessUnitId = businessUnitId,
            OfferedServices = services.Select(svc => new OfferedServiceDto
            {
                Guid = svc.Guid,
                EmployeeId = svc.EmployeeId,
                Name = svc.Name,
                MarkdownDescription = svc.MarkdownDescription,
                GenericServiceVariantsIds = svc.GenericServiceVariantsIds.Select(id => (long)id).ToList(),
                Duration = svc.Duration,
                Price =  Money.FromJson(svc.Price)!,
                Order = svc.Order,
                CategoryId = svc.CategoryId
            }).ToList()
        };
    }
}

internal static class Route
{
    internal static IEndpointRouteBuilder MapGetBusinessConfigurationEndpoint(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/api/v1/companies/{businessUnitId}/services-configuration", async (HttpContext context,
            Guid businessUnitId,
            BusinessManagementFacade facade,
            CancellationToken ct) =>
        {
            var result = await facade.GetBusinessConfigurationAsync(businessUnitId, ct);
            return result is null ?
                Results.NotFound() :
                Results.Ok(result);
        });

        return endpoints;
    }
}