using HomeMaintenanceAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HomeMaintenanceAPI.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<ProviderProfile> ProviderProfiles => Set<ProviderProfile>();
        public DbSet<Specialization> Specializations => Set<Specialization>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<ProviderOffer> ProviderOffers => Set<ProviderOffer>();
        public DbSet<Rating> Ratings => Set<Rating>();
        public DbSet<SubscriptionPlan> SubscriptionPlans => Set<SubscriptionPlan>();
        public DbSet<SubscriptionPaymentRequest> SubscriptionPaymentRequests => Set<SubscriptionPaymentRequest>();
        public DbSet<ProviderSubscription> ProviderSubscriptions => Set<ProviderSubscription>();
        public DbSet<Notification> Notifications => Set<Notification>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.PhoneNumber)
                .IsUnique();

            modelBuilder.Entity<User>()
                .Property(u => u.ProfileImageUrl)
                .HasMaxLength(500);

            // ProviderProfile: one User -> zero/one ProviderProfile
            modelBuilder.Entity<ProviderProfile>()
                .HasIndex(p => p.UserId)
                .IsUnique();

            modelBuilder.Entity<ProviderProfile>()
                .HasOne(p => p.User)
                .WithOne(u => u.ProviderProfile)
                .HasForeignKey<ProviderProfile>(p => p.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ProviderProfile>()
                .HasOne(p => p.Specialization)
                .WithMany(s => s.ProviderProfiles)
                .HasForeignKey(p => p.SpecializationId)
                .OnDelete(DeleteBehavior.NoAction);

            // Order
            modelBuilder.Entity<Order>()
                .Property(o => o.Latitude)
                .HasPrecision(10, 7);

            modelBuilder.Entity<Order>()
                .Property(o => o.Longitude)
                .HasPrecision(10, 7);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.Customer)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.CustomerId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.Specialization)
                .WithMany(s => s.Orders)
                .HasForeignKey(o => o.SpecializationId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.SelectedProviderProfile)
                .WithMany(p => p.SelectedOrders)
                .HasForeignKey(o => o.SelectedProviderProfileId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Order>()
                .ToTable(t =>
                {
                    t.HasCheckConstraint("CK_Order_Latitude", "[Latitude] BETWEEN -90 AND 90");
                    t.HasCheckConstraint("CK_Order_Longitude", "[Longitude] BETWEEN -180 AND 180");
                });

            // ProviderOffer
            modelBuilder.Entity<ProviderOffer>()
                .HasIndex(o => new { o.OrderId, o.ProviderProfileId })
                .IsUnique();

            modelBuilder.Entity<ProviderOffer>()
                .Property(o => o.InspectionPrice)
                .HasPrecision(18, 2);

            modelBuilder.Entity<ProviderOffer>()
                .Property(o => o.ProviderLatitude)
                .HasPrecision(10, 7);

            modelBuilder.Entity<ProviderOffer>()
                .Property(o => o.ProviderLongitude)
                .HasPrecision(10, 7);

            modelBuilder.Entity<ProviderOffer>()
                .HasOne(o => o.Order)
                .WithMany(order => order.Offers)
                .HasForeignKey(o => o.OrderId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ProviderOffer>()
                .HasOne(o => o.ProviderProfile)
                .WithMany(p => p.Offers)
                .HasForeignKey(o => o.ProviderProfileId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ProviderOffer>()
                .ToTable(t =>
                {
                    t.HasCheckConstraint("CK_ProviderOffer_InspectionPrice", "[InspectionPrice] >= 0");
                    t.HasCheckConstraint("CK_ProviderOffer_Latitude", "[ProviderLatitude] BETWEEN -90 AND 90");
                    t.HasCheckConstraint("CK_ProviderOffer_Longitude", "[ProviderLongitude] BETWEEN -180 AND 180");
                });

            // Rating
            modelBuilder.Entity<Rating>()
                .HasIndex(r => r.OrderId)
                .IsUnique();

            modelBuilder.Entity<Rating>()
                .HasOne(r => r.Order)
                .WithOne(o => o.Rating)
                .HasForeignKey<Rating>(r => r.OrderId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Rating>()
                .HasOne(r => r.Customer)
                .WithMany(u => u.Ratings)
                .HasForeignKey(r => r.CustomerId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Rating>()
                .HasOne(r => r.ProviderProfile)
                .WithMany(p => p.Ratings)
                .HasForeignKey(r => r.ProviderProfileId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Rating>()
                .ToTable(t =>
                {
                    t.HasCheckConstraint("CK_Rating_Value", "[Value] BETWEEN 1 AND 5");
                });

            // SubscriptionPlan
            modelBuilder.Entity<SubscriptionPlan>()
                .Property(p => p.Price)
                .HasPrecision(18, 2);

            modelBuilder.Entity<SubscriptionPlan>()
                .ToTable(t =>
                {
                    t.HasCheckConstraint("CK_SubscriptionPlan_Price", "[Price] >= 0");
                    t.HasCheckConstraint("CK_SubscriptionPlan_Duration", "[DurationInDays] > 0");
                });

            // SubscriptionPaymentRequest
            modelBuilder.Entity<SubscriptionPaymentRequest>()
                .HasIndex(r => r.TransactionId)
                .IsUnique();

            //modelBuilder.Entity<SubscriptionPaymentRequest>()
            //    .Property(r => r.Amount)
            //    .HasPrecision(18, 2);

            modelBuilder.Entity<SubscriptionPaymentRequest>()
                .HasOne(r => r.ProviderProfile)
                .WithMany(p => p.SubscriptionPaymentRequests)
                .HasForeignKey(r => r.ProviderProfileId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<SubscriptionPaymentRequest>()
                .HasOne(r => r.SubscriptionPlan)
                .WithMany(p => p.SubscriptionPaymentRequests)
                .HasForeignKey(r => r.SubscriptionPlanId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<SubscriptionPaymentRequest>()
                .HasOne(r => r.ReviewedByAdmin)
                .WithMany(u => u.ReviewedSubscriptionPaymentRequests)
                .HasForeignKey(r => r.ReviewedByAdminId)
                .OnDelete(DeleteBehavior.NoAction);

            //modelBuilder.Entity<SubscriptionPaymentRequest>()
            //    .ToTable(t =>
            //    {
            //        t.HasCheckConstraint("CK_SubscriptionPaymentRequest_Amount", "[Amount] >= 0");
            //    });

            modelBuilder.Entity<SubscriptionPaymentRequest>()
                .Property(r => r.ProofImageUrl)
                .HasMaxLength(500);


            // ProviderSubscription
            modelBuilder.Entity<ProviderSubscription>()
                .HasIndex(s => s.SubscriptionPaymentRequestId)
                .IsUnique();

            modelBuilder.Entity<ProviderSubscription>()
                .HasOne(s => s.ProviderProfile)
                .WithMany(p => p.Subscriptions)
                .HasForeignKey(s => s.ProviderProfileId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ProviderSubscription>()
                .HasOne(s => s.SubscriptionPlan)
                .WithMany(p => p.ProviderSubscriptions)
                .HasForeignKey(s => s.SubscriptionPlanId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ProviderSubscription>()
                .HasOne(s => s.SubscriptionPaymentRequest)
                .WithOne(r => r.ProviderSubscription)
                .HasForeignKey<ProviderSubscription>(s => s.SubscriptionPaymentRequestId)
                .OnDelete(DeleteBehavior.NoAction);

            // Notification
            modelBuilder.Entity<Notification>()
                .HasOne(n => n.User)
                .WithMany(u => u.Notifications)
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Notification>()
                .HasOne(n => n.RelatedOrder)
                .WithMany(o => o.Notifications)
                .HasForeignKey(n => n.RelatedOrderId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Notification>()
                .HasOne(n => n.RelatedOffer)
                .WithMany(o => o.Notifications)
                .HasForeignKey(n => n.RelatedOfferId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Notification>()
                .HasOne(n => n.RelatedSubscriptionPaymentRequest)
                .WithMany(r => r.Notifications)
                .HasForeignKey(n => n.RelatedSubscriptionPaymentRequestId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
