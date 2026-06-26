using Domain.Entities;

namespace Domain.Factories;

/// <summary>
/// GOF Design Pattern: Factory Method (Creational)
/// 
/// Encapsulates the creation logic for Certification entities. Instead of
/// having the UseCase (or any other class) call "new Certification()" directly
/// and manually set every field, all creation paths go through this factory.
/// 
/// Why this solves a real problem in SkillVault:
/// Certifications can come from different sources with different defaults —
/// a certification registered manually by the user vs. one imported automatically
/// from a verified provider (e.g., an AWS Academy badge webhook in the future).
/// Each creation path needs consistent defaults and validation, but the calling
/// code (UseCase) should not need to know those details.
/// </summary>
public static class CertificationFactory
{
    /// <summary>
    /// Creates a certification manually registered by the user.
    /// Applies default validation and timestamps.
    /// </summary>
    public static Certification CreateManual(string provider, string title, DateTime completedDate, string? credentialUrl = null)
    {
        var certification = new Certification
        {
            Provider = provider,
            Title = title,
            CompletedDate = NormalizeToUtc(completedDate),
            CredentialUrl = credentialUrl,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        certification.Validate();
        return certification;
    }

    /// <summary>
    /// Creates a certification imported from a verified external provider
    /// (e.g., AWS Academy badge, Credly webhook). Automatically marks the
    /// credential as verified by requiring a non-empty CredentialUrl.
    /// </summary>
    public static Certification CreateFromVerifiedProvider(string provider, string title, DateTime completedDate, string credentialUrl)
    {
        if (string.IsNullOrWhiteSpace(credentialUrl))
            throw new ArgumentException("Verified provider certifications must include a credential URL", nameof(credentialUrl));

        var certification = new Certification
        {
            Provider = provider,
            Title = title,
            CompletedDate = NormalizeToUtc(completedDate),
            CredentialUrl = credentialUrl,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        certification.Validate();
        return certification;
    }

    private static DateTime NormalizeToUtc(DateTime value)
    {
        return value.Kind switch
        {
            DateTimeKind.Utc => value,
            DateTimeKind.Local => value.ToUniversalTime(),
            _ => DateTime.SpecifyKind(value, DateTimeKind.Utc)
        };
    }
}
