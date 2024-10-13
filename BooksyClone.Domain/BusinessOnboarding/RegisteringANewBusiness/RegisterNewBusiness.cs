using BooksyClone.Domain.BusinessOnboarding.Model;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;

namespace BooksyClone.Domain.BusinessOnboarding.RegisteringANewBusiness;

public static class RegisterNewBusinessRoute
{
    public static IEndpointRouteBuilder MapRegisterNewBusinessEndpoint(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/api/v1/business", async (
            HttpContext context,
            HttpRequest request,
            ILogger<RegisterNewBusinessRequest> logger,
            CancellationToken ct,
            OnboardingFacade facade) =>
        {
            var payload = await request.ReadFormAsync(ct);
            var contentBody = new RegisterNewBusinessRequest
            {
                CorrelationId = Guid.Parse(payload["CorrelationId"].Single()),
                BusinessName = payload["BusinessName"].Single(),
                BusinessType = Enum.Parse<BusinessType>(payload["BusinessType"].Single()),
                BusinessNIP = payload["BusinessNIP"].Single(),
                BusinessAddress = payload["BusinessAddress"].Single(),
                BusinessPhoneNumber = payload["BusinessPhoneNumber"].Single(),
                BusinessEmail = payload["BusinessEmail"].Single(),
                UserId = Guid.Parse(payload["UserId"].Single()),
                UserFullName = payload["UserFullName"].Single(),
                UserIdNumber = payload["UserIdNumber"].Single(),
                UserEmail = payload["UserEmail"].Single(),
                UserPhoneNumber = payload["UserPhoneNumber"].Single(),
                LegalConsent = bool.Parse(payload["LegalConsent"].Single()),
                LegalConsentContent = payload["LegalConsentContent"].Single(),
                BusinessProofDocument = payload.Files["BusinessProofDocument"],
                UserIdentityDocument = payload.Files["UserIdentityDocument"]
            };

            var validationErrors = contentBody.GetValidationErrors();
            if(validationErrors.Any())
                return Results.BadRequest(validationErrors);

            var result = await facade.RegisterNewBusinessDraftAsync(BusinessDraft.From(contentBody), ct);

            return Results.Created($"/api/v1/business/{result}", result);
        });

        return endpoints;
    }
}
