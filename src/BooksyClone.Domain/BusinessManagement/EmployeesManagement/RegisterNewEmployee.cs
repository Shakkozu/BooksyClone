using BooksyClone.Contract.BusinessManagement;
using BooksyClone.Domain.Availability.Storage;
using Dapper;

namespace BooksyClone.Domain.BusinessManagement.EmployeesManagement;

internal class RegisterNewEmployee(DbConnectionFactory _dbConnectionFactory)
{
	internal async Task<RegistrationToken> HandleAsync(RegisterNewEmployeeRequest registerNewEmployeeRequest, CancellationToken ct)
	{
		using var connection = _dbConnectionFactory.CreateConnection();
		connection.Open();
		using var transaction = connection.BeginTransaction();
		try
		{
			var insertEmployeeSql = @"
                      INSERT INTO business_management.employees (guid, user_id, business_id, registered_at, active_from, birth_date, active_to, first_name, last_name, email, phone_number)
                      VALUES (@EmployeeId, NULL, @BusinessUnitId, @RegisteredAt, @ActiveFrom, @BirthDate, @ActiveTo, @FirstName, @LastName, @Email, @PhoneNumber);";

			var parameters = new
			{
				registerNewEmployeeRequest.EmployeeId,
				registerNewEmployeeRequest.BusinessUnitId,
				RegisteredAt = DateTime.UtcNow,
				ActiveFrom = registerNewEmployeeRequest.ValidFrom,
				registerNewEmployeeRequest.BirthDate,
				ActiveTo = registerNewEmployeeRequest.ValidTo,
				registerNewEmployeeRequest.FirstName,
				registerNewEmployeeRequest.LastName,
				registerNewEmployeeRequest.Email,
				registerNewEmployeeRequest.PhoneNumber
			};

			await connection.ExecuteAsync(insertEmployeeSql, parameters, transaction);

			var registrationToken = RegistrationToken.CreateNew();
			var insertInvitationSql = @"
                      INSERT INTO business_management.employees_invitation (guid, business_id, employee_id, email, created_at, valid_to, invitation_token)
                      VALUES (@InvitationGuid, @BusinessUnitId, @EmployeeId, @Email, @CreatedAt, @ValidTo, @InvitationToken);";

			var invitationParameters = new
			{
				InvitationGuid = Guid.NewGuid(),
				registerNewEmployeeRequest.BusinessUnitId,
				registerNewEmployeeRequest.EmployeeId,
				registerNewEmployeeRequest.Email,
				CreatedAt = DateTime.UtcNow,
				ValidTo = registrationToken.ValidTo,
				InvitationToken = registrationToken.Token
			};

			await connection.ExecuteAsync(insertInvitationSql, invitationParameters, transaction);

			transaction.Commit();

			return registrationToken;
		}
		catch (Exception)
		{
			transaction.Rollback();
			throw;
		}
	}
}