using BooksyClone.Contract.BusinessManagement;
using BooksyClone.Contract.Shared;
using BooksyClone.Domain.Availability.Storage;
using BooksyClone.Infrastructure.EmailSending;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace BooksyClone.Domain.BusinessManagement.EmployeesManagement;
internal class RegisterNewEmployee(IEmailSender _emailSender, DbConnectionFactory _dbConnectionFactory)
{
	internal async Task<Result> HandleAsync(RegisterNewEmployeeRequest registerNewEmployeeRequest, CancellationToken ct)
	{
		throw new NotImplementedException();
		//using var connection = _dbConnectionFactory.CreateConnection();
  //      using var transaction = connection.BeginTransaction();
		//try
  //      {
  //          // Insert new employee data into employees table
  //          var insertEmployeeSql = @"
  //              INSERT INTO business_management.employees (guid, user_id, business_id, registered_at, active_from, birth_date, active_to, first_name, last_name, email, phone_number)
  //              VALUES (@EmployeeId, NULL, @BusinessUnitId, @RegisteredAt, @ActiveFrom, @BirthDate, @ActiveTo, @FirstName, @LastName, @Email, @PhoneNumber);";

  //          var parameters = new
  //          {
  //              registerNewEmployeeRequest.EmployeeId,
  //              registerNewEmployeeRequest.BusinessUnitId,
  //              RegisteredAt = DateTime.UtcNow,
  //              ActiveFrom = registerNewEmployeeRequest.ValidFrom,
  //              registerNewEmployeeRequest.BirthDate,
  //              ActiveTo = registerNewEmployeeRequest.ValidTo,
  //              registerNewEmployeeRequest.FirstName,
  //              registerNewEmployeeRequest.LastName,
  //              registerNewEmployeeRequest.Email,
  //              registerNewEmployeeRequest.PhoneNumber
  //          };

  //          await connection.ExecuteAsync(insertEmployeeSql, parameters, transaction);

  //          // Generate an invitation token
  //          var invitationToken = Guid.NewGuid().ToString();

  //          // Insert employee invitation
  //          var insertInvitationSql = @"
  //              INSERT INTO business_management.employees_invitation (guid, business_id, employee_id, email, created_at, valid_to, invitation_token)
  //              VALUES (@InvitationGuid, @BusinessUnitId, @EmployeeId, @Email, @CreatedAt, @ValidTo, @InvitationToken);";

  //          var invitationParameters = new
  //          {
  //              InvitationGuid = Guid.NewGuid(),
  //              registerNewEmployeeRequest.BusinessUnitId,
  //              registerNewEmployeeRequest.EmployeeId,
  //              registerNewEmployeeRequest.Email,
  //              CreatedAt = DateTime.UtcNow,
  //              ValidTo = DateTime.UtcNow.AddHours(72),
  //              InvitationToken = invitationToken
  //          };

  //          await connection.ExecuteAsync(insertInvitationSql, invitationParameters, transaction);

  //          // Commit the transaction
  //          transaction.Commit();

  //          // Send the invitation email
  //          var subject = "You are invited to join our business";
  //          var message = $"Please use the following token to accept the invitation: {invitationToken}";
  //          _emailSender.SendEmail(registerNewEmployeeRequest.Email, subject, message);

  //          return Result.Correct();
  //      }
  //      catch (Exception ex)
  //      {
  //          // Rollback the transaction in case of an error
  //          transaction.Rollback();
  //          return Result.Failure(ex.Message);
  //      }
		
		


		


	}
}
