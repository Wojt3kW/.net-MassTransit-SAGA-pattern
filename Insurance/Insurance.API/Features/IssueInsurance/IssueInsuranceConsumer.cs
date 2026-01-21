using Insurance.Application.Abstractions;
using Insurance.Contracts.Events;
using Insurance.Domain.Entities;
using MassTransit;
using IssueInsuranceCommand = Insurance.Contracts.Commands.IssueInsurance;

namespace Insurance.API.Features.IssueInsurance;

/// <summary>
/// Consumer that handles insurance policy issuance commands.
/// </summary>
public class IssueInsuranceConsumer : IConsumer<IssueInsuranceCommand>
{
    private readonly IInsurancePolicyRepository _repository;

    public IssueInsuranceConsumer(IInsurancePolicyRepository repository)
    {
        _repository = repository;
    }

    public async Task Consume(ConsumeContext<IssueInsuranceCommand> context)
    {
        var command = context.Message;

        // SIMULATION: If customer name contains "FAIL" - simulate insurance failure
        if (command.CustomerName.Contains("FAIL"))
        {
            await context.Publish(new InsuranceIssueFailed(
                command.CorrelationId,
                command.TripId,
                "Simulated: Customer not eligible for insurance"));
            return;
        }

        // SIMULATION: If customer name contains "TIMEOUT" - simulate timeout
        if (command.CustomerName.Contains("TIMEOUT"))
        {
            await Task.Delay(TimeSpan.FromSeconds(65), context.CancellationToken);
        }

        var premium = CalculatePremium(command.TripTotalValue);

        var policy = new InsurancePolicy
        {
            Id = Guid.NewGuid(),
            TripId = command.TripId,
            CustomerId = command.CustomerId,
            CustomerName = command.CustomerName,
            CustomerEmail = command.CustomerEmail,
            PolicyNumber = GeneratePolicyNumber(),
            CoverageStartDate = command.CoverageStartDate,
            CoverageEndDate = command.CoverageEndDate,
            OutboundFlightReservationId = command.OutboundFlightReservationId,
            ReturnFlightReservationId = command.ReturnFlightReservationId,
            HotelReservationId = command.HotelReservationId,
            TripTotalValue = command.TripTotalValue,
            Premium = premium,
            Status = InsurancePolicyStatus.Issued,
            CreatedAt = DateTime.UtcNow,
            IssuedAt = DateTime.UtcNow
        };

        await _repository.AddAsync(policy, context.CancellationToken);

        await context.Publish(new InsuranceIssued(
            command.CorrelationId,
            command.TripId,
            policy.Id,
            policy.PolicyNumber!,
            policy.CoverageStartDate,
            policy.CoverageEndDate,
            policy.Premium));
    }

    private static string GeneratePolicyNumber() => $"INS-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..6].ToUpper()}";

    private static decimal CalculatePremium(decimal tripValue) => tripValue * 0.05m; // 5% of trip value
}
