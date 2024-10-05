using System.Globalization;

namespace BooksyClone.Domain.BusinessOnboarding.RegisteringANewBusiness;

internal static class LegalConsents
{
    static LegalConsents()
    {
        CurrentTrueInformationConsent = new LegalConsent
        {
            Content = "Oświadczam że wprowadzone przeze mnie dane są poprawne i zgodne z stanem faktycznym.",
            Name = "TrueInformationConsent",
            ValidFrom = DateTime.ParseExact("2024-10-01", "yyyy-MM-dd", CultureInfo.InvariantCulture),
            ValidTo = null,
            Version = 1
        };
    }

    public static LegalConsent CurrentTrueInformationConsent { get; }
}

internal class LegalConsent
{
    public DateTime ValidFrom { get; set; }
    public DateTime? ValidTo { get; set; }
    public string Name { get; set; }
    public string Content { get; set; }
    public int Version { get; set; }
}
