using BooksyClone.Contract.Shared;
using BooksyClone.Domain.Auth;
using BooksyClone.Domain.Availability.Storage;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BooksyClone.Domain.BusinessManagement.EmployeesManagement;
internal class AcceptEmployeeInvitationToJoiningBusiness(DbConnectionFactory dbConnectionFactory)
{
	private record InvitationDao
	{
        public Guid Guid { get; set; }
        public Guid BusinessId { get; set; }
        public Guid EmployeeId { get; set; }
    }

	internal Result AcceptInvitation(Guid userId, string email, string token)
	{
		using var connection = dbConnectionFactory.CreateConnection();
		connection.Open();
		using var transaction = connection.BeginTransaction();
		try
		{
			var selectInvitationSql = @"
				SELECT guid, business_id as BusinessId, employee_id as EmployeeId
				FROM business_management.employees_invitation
				WHERE email = @Email AND invitation_token = @Token AND valid_to > now()";
			var invitation = connection.QueryFirstOrDefault<InvitationDao>(selectInvitationSql, new { Email = email, Token = token }, transaction);
			if (invitation == null)
				return Result.ErrorResult("Invitation not found");

			var updateEmployeeSql = @"
				UPDATE business_management.employees
				SET user_id = @UserId
				WHERE guid = @EmployeeId;";
			connection.Execute(updateEmployeeSql, new { UserId = userId, EmployeeId = invitation.EmployeeId }, transaction);

			var useInvitationSql = @"
				UPDATE business_management.employees_invitation
				SET used_at = now()
				WHERE guid = @InvitationId;";
			connection.Execute(useInvitationSql, new { InvitationId = invitation.Guid }, transaction);

			transaction.Commit();
			return Result.Correct();
		}
		catch (Exception)
		{
			transaction.Rollback();
			return Result.ErrorResult("Internal error");
		}
	}

}
