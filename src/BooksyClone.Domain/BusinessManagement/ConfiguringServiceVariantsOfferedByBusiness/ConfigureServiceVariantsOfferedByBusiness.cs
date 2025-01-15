using BooksyClone.Contract.BusinessManagement;
using BooksyClone.Contract.Shared;
using BooksyClone.Domain.Availability.Storage;
using Dapper;
using Newtonsoft.Json;

namespace BooksyClone.Domain.BusinessManagement.ConfiguringServiceVariantsOfferedByBusiness;

internal record EmployeeServiceDao(
    Guid Guid,
    Guid EmployeeId,
    Guid BusinessId,
    string Name,
    string MarkdownDescription,
    List<ulong> GenericServiceVariantsIds,
    TimeSpan Duration,
    Money Price,
    int Order,
    ulong CategoryId
);

internal class ConfigureServiceVariantsOfferedByBusiness(DbConnectionFactory _dbConnectionFactory)
{
    internal async Task<Result> HandleAsync(BusinessConfigurationDto businessConfigurationDto, CancellationToken ct)
    {
        using var connection = _dbConnectionFactory.CreateConnection();
        const string command = @"INSERT INTO business_management.employee_services (
        guid, employee_id, business_id, name, markdown_description,
        generic_service_variants_ids, duration, price, ""order"", category_id)
    VALUES (
        @Guid, @EmployeeId, @BusinessId, @Name, @MarkdownDescription,
        @GenericServiceVariantsIds, @Duration, @Price, @Order, @CategoryId)
    ON CONFLICT (employee_id, service_id) DO UPDATE
    SET name = EXCLUDED.name,
        markdown_description = EXCLUDED.markdown_description,
        generic_service_variants_ids = EXCLUDED.generic_service_variants_ids,
        duration = EXCLUDED.duration,
        price = EXCLUDED.price,
        ""order"" = EXCLUDED.""order"",
        category_id = EXCLUDED.category_id
";
        var daos = businessConfigurationDto.OfferedServices.Select(x => new EmployeeServiceDao(
            Guid.NewGuid(),
            x.EmployeeId,
            businessConfigurationDto.BusinessUnitId,
            x.Name,
            x.MarkdownDescription,
            x.GenericServiceVariantsIds,
            x.Duration,
            x.Price,
            x.Order,
            x.CategoryId
        )).ToList();
        
            
        connection.Open();
        using var transaction = connection.BeginTransaction();

        foreach (var dao in daos)
        {
            await connection.ExecuteAsync(command, new
            {
                dao.Guid,
                dao.EmployeeId,
                dao.BusinessId,
                dao.Name,
                dao.MarkdownDescription,
                GenericServiceVariantsIds = JsonConvert.SerializeObject(dao.GenericServiceVariantsIds),
                dao.Duration,
                Price = JsonConvert.SerializeObject(dao.Price),
                dao.Order,
                dao.CategoryId
            }, transaction);
        }


        transaction.Commit();
        return Result.Correct();
    }
}