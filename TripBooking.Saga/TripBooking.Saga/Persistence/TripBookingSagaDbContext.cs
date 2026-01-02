using MassTransit.EntityFrameworkCoreIntegration;
using Microsoft.EntityFrameworkCore;
using TripBooking.Saga.States;

namespace TripBooking.Saga.Persistence
{
    /// <summary>
    /// Database context for persisting trip booking saga state.
    /// </summary>
    public class TripBookingSagaDbContext : SagaDbContext
    {
        /// <summary>Trip booking saga state instances.</summary>
        public DbSet<TripBookingSagaState> TripBookingSagaStates { get; set; }

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
