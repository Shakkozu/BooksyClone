using BooksyClone.Domain.BusinessOnboarding.Model;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace BooksyClone.Domain.BusinessOnboarding.FetchingBusinessCreationApplication;

internal static class FetchBusinessDraftEndpoint
{
    internal static IEndpointRouteBuilder MapFetchBusinessDraftEndpoint(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/api/v1/business/{businessDraftGuid}", async (
            [FromRoute] string businessDraftGuid,
            CancellationToken ct,
            OnboardingFacade facade
            ) =>
        {
            var result = await facade.FindById(Guid.Parse(businessDraftGuid), ct);
            if (result == null)
                return Results.NotFound();

            return Results.Ok(result);
        });

        return endpoints;
    }
}

public record FetchBusinessDraftStateResponse
{
    public required string BusinessName { get; set; }
    public required string BusinessType { get; set; }
    public required string BusinessNIP { get; set; }
    public required string BusinessAddress { get; set; }
    public required string BusinessPhoneNumber { get; set; }
    public required string BusinessEmail { get; set; }

    public Guid UserId { get; set; }
    public required string UserFullName { get; set; }
    public required string UserIdNumber { get; set; }
    public required string UserEmail { get; set; }
    public required string UserPhoneNumber { get; set; }

    public FileDocument BusinessProofDocument { get; set; }
    public FileDocument UserIdentityDocument { get; set; }

    public bool LegalConsent { get; set; }
    public required string LegalConsentContent { get; set; }

}