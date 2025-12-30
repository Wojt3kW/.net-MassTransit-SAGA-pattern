namespace Insurance.Contracts.DTOs;

/// <summary>
/// Response DTO for paginated list of insurance policies.
/// </summary>
public record InsurancePolicyListResponse(
    IReadOnlyList<InsurancePolicyResponse> Items,
    int TotalCount,
    int Page,
    int PageSize);
