using System;
using System.Linq;
using System.Threading.Tasks;
using GymApp.Core.Common;
using GymApp.Core.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GymApp.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        private readonly IMediator _mediator;

        public DbSet<Gym> Gyms { get; set; }
        public DbSet<Locker> Lockers { get; set; }
        public DbSet<GymSession> GymSessions { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Membership> Memberships { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options, IMediator mediator)
            : base(options)
        {
            _mediator = mediator;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(IEntity<string>).IsAssignableFrom(entityType.ClrType))
                {
                    var method = typeof(AppDbContext)
                        .GetMethod(nameof(SetSoftDeleteFilter), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!
                        .MakeGenericMethod(entityType.ClrType);

                    method.Invoke(null, new object[] { modelBuilder });
                }
            }

            modelBuilder.Entity<Gym>()
                .HasMany(g => g.Lockers)
                .WithOne(l => l.Gym)
                .HasForeignKey(l => l.GymId);

            modelBuilder.Entity<Gym>()
                .HasMany(g => g.ActiveSessions)
                .WithOne(s => s.Gym)
                .HasForeignKey(s => s.GymId);

            modelBuilder.Entity<Gym>()
                .HasMany(g => g.Members)
                .WithMany(u => u.Gyms);

            modelBuilder.Entity<Gym>()
                .HasMany(g => g.Memberships)
                .WithOne(m => m.Gym)
                .HasForeignKey(m => m.GymId);

            modelBuilder.Entity<User>()
                .HasMany(u => u.Sessions)
                .WithOne(s => s.User)
                .HasForeignKey(s => s.UserId);

            modelBuilder.Entity<User>()
                .HasOne(u => u.ActiveSession)
                .WithOne()
                .HasForeignKey<GymSession>("UserId");

            modelBuilder.Entity<User>()
                .HasOne(u => u.Locker)
                .WithOne(l => l.OccupiedByUser)
                .HasForeignKey<Locker>("OccupiedByUserId");

            modelBuilder.Entity<User>()
                .HasMany(u => u.Memberships)
                .WithOne(m => m.User)
                .HasForeignKey(m => m.UserId);

            modelBuilder.Entity<GymSession>()
                .HasOne(s => s.Locker)
                .WithOne()
                .HasForeignKey<GymSession>("LockerId");

            modelBuilder.Entity<Locker>()
                .Property(l => l.Id)
                .HasDefaultValueSql("NEWID()");

            modelBuilder.Entity<GymSession>()
                .Property(s => s.Id)
                .HasDefaultValueSql("NEWID()");

            modelBuilder.Entity<User>()
                .Property(u => u.Id)
                .HasDefaultValueSql("NEWID()");

            modelBuilder.Entity<Membership>()
                .Property(m => m.Id)
                .HasDefaultValueSql("NEWID()");

            base.OnModelCreating(modelBuilder);
        }

        private static void SetSoftDeleteFilter<TEntity>(ModelBuilder builder) where TEntity : class, IEntity<string>
        {
            builder.Entity<TEntity>().HasQueryFilter(e => !e.IsDeleted);
        }

        public override int SaveChanges()
        {
            ApplyAuditInfo();
            DispatchDomainEvents();
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ApplyAuditInfo();
            await DispatchDomainEventsAsync(cancellationToken);
            return await base.SaveChangesAsync(cancellationToken);
        }

        private void ApplyAuditInfo()
        {
            var now = DateTime.UtcNow;

            foreach (var entry in ChangeTracker.Entries<IEntity<string>>())
            {
                if (entry.State == EntityState.Added)
                {
                    if (string.IsNullOrWhiteSpace(entry.Entity.Id as string))
                        entry.Entity.Id = Guid.NewGuid().ToString();

                    entry.Entity.CreatedAt = now;
                }

                if (entry.State == EntityState.Modified || entry.State == EntityState.Added)
                {
                    entry.Entity.ModifiedAt = now;
                }
            }
        }

        private void DispatchDomainEvents()
        {
            var domainEvents = ChangeTracker
                .Entries<IEntity<string>>()
                .Where(e => e.Entity.DomainEvents.Any())
                .SelectMany(e => e.Entity.DomainEvents)
                .ToList();

            foreach (var domainEvent in domainEvents)
            {
                _mediator.Publish(domainEvent).GetAwaiter().GetResult();
            }

            foreach (var entry in ChangeTracker.Entries<IEntity<string>>())
            {
                entry.Entity.ClearDomainEvents();
            }
        }

        private async Task DispatchDomainEventsAsync(CancellationToken cancellationToken)
        {
            var domainEvents = ChangeTracker
                .Entries<IEntity<string>>()
                .Where(e => e.Entity.DomainEvents.Any())
                .SelectMany(e => e.Entity.DomainEvents)
                .ToList();

            foreach (var domainEvent in domainEvents)
            {
                await _mediator.Publish(domainEvent, cancellationToken);
            }

            foreach (var entry in ChangeTracker.Entries<IEntity<string>>())
            {
                entry.Entity.ClearDomainEvents();
            }
        }
    }
}