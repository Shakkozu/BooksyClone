using BooksyClone.Contract.BusinessManagement;
using BooksyClone.Contract.Shared;
using BooksyClone.Domain.Availability.Storage;
using Dapper;
using Newtonsoft.Json;

namespace BooksyClone.Domain.BusinessManagement.ConfiguringServiceVariantsOfferedByBusiness;

internal record EmployeeServiceDao
{
    public EmployeeServiceDao()
    {     }

    public EmployeeServiceDao(Guid Guid,
        Guid EmployeeId,
        Guid BusinessId,
        string Name,
        string MarkdownDescription,
        List<long> GenericServiceVariantsIds,
        TimeSpan Duration,
        Money Price,
        int Order,
        long CategoryId)
    {
        this.Guid = Guid;
        this.EmployeeId = EmployeeId;
        this.BusinessId = BusinessId;
        this.Name = Name;
        this.MarkdownDescription = MarkdownDescription;
        this.GenericServiceVariantsIds = GenericServiceVariantsIds;
        this.Duration = Duration;
        this.Price = Price;
        this.Order = Order;
        this.CategoryId = CategoryId;
    }

    public Guid Guid { get; set; }
    public Guid EmployeeId { get; set; }
    public Guid BusinessId { get; set; }
    public string Name { get; set; }
    public string MarkdownDescription { get; set; }
    public List<long> GenericServiceVariantsIds { get; set; }
    public TimeSpan Duration { get; set; }
    public Money Price { get; set; }
    public int Order { get; set; }
    public long CategoryId { get; set; }

}

internal class ConfigureServiceVariantsOfferedByBusiness(DbConnectionFactory _dbConnectionFactory)
{
    internal async Task<Result> HandleAsync(BusinessServiceConfigurationDto businessServiceConfigurationDto, CancellationToken ct)
    {
        using var connection = _dbConnectionFactory.CreateConnection();
        const string command = @"INSERT INTO business_management.employee_services (
        guid, employee_id, business_id, name, markdown_description,
        generic_service_variants_ids, duration, price, ""order"", category_id)
    VALUES (
        @Guid, @EmployeeId, @BusinessId, @Name, @MarkdownDescription,
        @GenericServiceVariantsIds, @Duration, @Price::jsonb, @Order, @CategoryId)
    ON CONFLICT (guid) DO UPDATE
    SET name = EXCLUDED.name,
        markdown_description = EXCLUDED.markdown_description,
        generic_service_variants_ids = EXCLUDED.generic_service_variants_ids,
        duration = EXCLUDED.duration,
        price = EXCLUDED.price,
        ""order"" = EXCLUDED.""order"",
        category_id = EXCLUDED.category_id
";
        var daos = businessServiceConfigurationDto.OfferedServices.Select(x => new EmployeeServiceDao(
            x.Guid,
            x.EmployeeId,
            businessServiceConfigurationDto.BusinessUnitId,
            x.Name,
            x.MarkdownDescription,
            x.GenericServiceVariantsIds.Select(serviceVariantId => (long)serviceVariantId).ToList(),
            x.Duration,
            x.Price,
            x.Order,
            (long)x.CategoryId
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
                dao.GenericServiceVariantsIds,
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