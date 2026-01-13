using MassTransit.EntityFrameworkCoreIntegration;
using Microsoft.EntityFrameworkCore;
using TripBooking.Saga.States;

namespace TripBooking.Saga.Persistence
{
    public class TripBookingSagaDbContext : SagaDbContext
    {
        public DbSet<TripBookingSagaState> TripBookingSagaStates => Set<TripBookingSagaState>();

        public TripBookingSagaDbContext(DbContextOptions<TripBookingSagaDbContext> options)
            : base(options)
        {
        }

        protected override IEnumerable<ISagaClassMap> Configurations
        {
            get { yield return new TripBookingSagaStateMap(); }
        }
    }
}
