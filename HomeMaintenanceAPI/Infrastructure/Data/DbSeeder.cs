using System.Security.Cryptography;
using System.Text;
using HomeMaintenanceAPI.Domain.Entities;
using HomeMaintenanceAPI.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace HomeMaintenanceAPI.Infrastructure.Data
{
    public static class DbSeeder
    {
        public static async Task SeedRequiredDataAsync(AppDbContext context)
        {
            await SeedAdminAsync(context);
            await SeedSpecializationsAsync(context);
            await SeedSubscriptionPlansAsync(context);
        }

        public static async Task SeedDemoDataAsync(AppDbContext context)
        {
            await SeedRequiredDataAsync(context);

            var demoAlreadyExists = await context.Users
                .AnyAsync(u => u.Email == "demo.customer1@home.test");

            if (demoAlreadyExists)
                return;

            await using var transaction = await context.Database.BeginTransactionAsync();

            var admin = await context.Users.FirstAsync(u => u.Email == "admin@homemaintenance.com");

            var electricity = await context.Specializations.FirstAsync(s => s.Name == "Electricity");
            var plumbing = await context.Specializations.FirstAsync(s => s.Name == "Plumbing");
            var ac = await context.Specializations.FirstAsync(s => s.Name == "Air Conditioning");

            var monthlyPlan = await context.SubscriptionPlans.FirstAsync(p => p.Name == "Monthly Plan");
            var quarterlyPlan = await context.SubscriptionPlans.FirstAsync(p => p.Name == "Quarterly Plan");

            var customer1 = CreateUser(
                "Demo Customer 1",
                "demo.customer1@home.test",
                "0991000001",
                UserRole.User,
                "Password@123");

            var customer2 = CreateUser(
                "Demo Customer 2",
                "demo.customer2@home.test",
                "0991000002",
                UserRole.User,
                "Password@123");

            var providerUser1 = CreateUser(
                "Demo Provider Electrician",
                "demo.provider1@home.test",
                "0992000001",
                UserRole.User,
                "Password@123");

            var providerUser2 = CreateUser(
                "Demo Provider Plumber",
                "demo.provider2@home.test",
                "0992000002",
                UserRole.User,
                "Password@123");

            var providerUser3 = CreateUser(
                "Demo Provider AC",
                "demo.provider3@home.test",
                "0992000003",
                UserRole.User,
                "Password@123");

            await context.Users.AddRangeAsync(
                customer1,
                customer2,
                providerUser1,
                providerUser2,
                providerUser3);

            await context.SaveChangesAsync();

            var provider1 = new ProviderProfile
            {
                UserId = providerUser1.Id,
                SpecializationId = electricity.Id,
                Bio = "Demo electrician provider."
            };

            var provider2 = new ProviderProfile
            {
                UserId = providerUser2.Id,
                SpecializationId = plumbing.Id,
                Bio = "Demo plumbing provider."
            };

            var provider3 = new ProviderProfile
            {
                UserId = providerUser3.Id,
                SpecializationId = ac.Id,
                Bio = "Demo air conditioning provider."
            };

            await context.ProviderProfiles.AddRangeAsync(provider1, provider2, provider3);
            await context.SaveChangesAsync();

            var pendingRequest = new SubscriptionPaymentRequest
            {
                ProviderProfileId = provider3.Id,
                SubscriptionPlanId = monthlyPlan.Id,
                PaymentMethod = PaymentMethod.ShamCash,
                TransactionId = "DEMO-PENDING-001",
                ProofImageUrl = "/uploads/demo/subscription-proof-pending.jpg",
                Status = SubscriptionPaymentRequestStatus.Pending
            };

            var approvedRequest = new SubscriptionPaymentRequest
            {
                ProviderProfileId = provider1.Id,
                SubscriptionPlanId = monthlyPlan.Id,
                PaymentMethod = PaymentMethod.ShamCash,
                TransactionId = "DEMO-APPROVED-001",
                ProofImageUrl = "/uploads/demo/subscription-proof-approved.jpg",
                Status = SubscriptionPaymentRequestStatus.Approved,
                ReviewedAt = DateTime.UtcNow,
                ReviewedByAdminId = admin.Id,
                AdminNote = "Demo approved request."
            };

            var rejectedRequest = new SubscriptionPaymentRequest
            {
                ProviderProfileId = provider2.Id,
                SubscriptionPlanId = quarterlyPlan.Id,
                PaymentMethod = PaymentMethod.ShamCash,
                TransactionId = "DEMO-REJECTED-001",
                ProofImageUrl = "/uploads/demo/subscription-proof-rejected.jpg",
                Status = SubscriptionPaymentRequestStatus.Rejected,
                ReviewedAt = DateTime.UtcNow,
                ReviewedByAdminId = admin.Id,
                AdminNote = "Demo rejected request."
            };

            await context.SubscriptionPaymentRequests.AddRangeAsync(
                pendingRequest,
                approvedRequest,
                rejectedRequest);

            await context.SaveChangesAsync();

            var activeSubscription = new ProviderSubscription
            {
                ProviderProfileId = provider1.Id,
                SubscriptionPlanId = monthlyPlan.Id,
                SubscriptionPaymentRequestId = approvedRequest.Id,
                StartsAt = DateTime.UtcNow.AddDays(-2),
                EndsAt = DateTime.UtcNow.AddDays(28)
            };

            await context.ProviderSubscriptions.AddAsync(activeSubscription);
            await context.SaveChangesAsync();

            var waitingOrder = new Order
            {
                CustomerId = customer1.Id,
                SpecializationId = electricity.Id,
                Description = "Demo waiting order with pending offers.",
                Latitude = 33.5138000m,
                Longitude = 36.2765000m,
                AddressText = "Damascus - Demo waiting order",
                Status = OrderStatus.WaitingForOffers
            };

            var inspectionAcceptedOrder = new Order
            {
                CustomerId = customer1.Id,
                SpecializationId = electricity.Id,
                SelectedProviderProfileId = provider1.Id,
                Description = "Demo order accepted for inspection.",
                Latitude = 33.5150000m,
                Longitude = 36.2800000m,
                AddressText = "Damascus - Demo inspection order",
                Status = OrderStatus.InspectionAccepted
            };

            var inProgressOrder = new Order
            {
                CustomerId = customer1.Id,
                SpecializationId = plumbing.Id,
                SelectedProviderProfileId = provider2.Id,
                Description = "Demo order in progress.",
                Latitude = 33.5180000m,
                Longitude = 36.2850000m,
                AddressText = "Damascus - Demo in progress order",
                Status = OrderStatus.InProgress
            };

            var completionPendingOrder = new Order
            {
                CustomerId = customer2.Id,
                SpecializationId = ac.Id,
                SelectedProviderProfileId = provider3.Id,
                Description = "Demo order waiting for QR completion.",
                Latitude = 33.5200000m,
                Longitude = 36.2900000m,
                AddressText = "Damascus - Demo completion pending order",
                Status = OrderStatus.CompletionPending,
                CompletionTokenHash = "DEMO_HASH",
                CompletionTokenExpiresAt = DateTime.UtcNow.AddMinutes(10)
            };

            var completedOrder = new Order
            {
                CustomerId = customer2.Id,
                SpecializationId = electricity.Id,
                SelectedProviderProfileId = provider1.Id,
                Description = "Demo completed order.",
                Latitude = 33.5250000m,
                Longitude = 36.2950000m,
                AddressText = "Damascus - Demo completed order",
                Status = OrderStatus.Completed,
                CompletedAt = DateTime.UtcNow.AddDays(-1)
            };

            var cancelledOrder = new Order
            {
                CustomerId = customer2.Id,
                SpecializationId = plumbing.Id,
                Description = "Demo cancelled order.",
                Latitude = 33.5300000m,
                Longitude = 36.3000000m,
                AddressText = "Damascus - Demo cancelled order",
                Status = OrderStatus.Cancelled
            };

            var afterInspectionRejectedOrder = new Order
            {
                CustomerId = customer1.Id,
                SpecializationId = ac.Id,
                Description = "Demo order returned to waiting after inspection rejection.",
                Latitude = 33.5350000m,
                Longitude = 36.3050000m,
                AddressText = "Damascus - Demo rejected after inspection order",
                Status = OrderStatus.WaitingForOffers
            };

            await context.Orders.AddRangeAsync(
                waitingOrder,
                inspectionAcceptedOrder,
                inProgressOrder,
                completionPendingOrder,
                completedOrder,
                cancelledOrder,
                afterInspectionRejectedOrder);

            await context.SaveChangesAsync();

            var offers = new List<ProviderOffer>
            {
                new ProviderOffer
                {
                    OrderId = waitingOrder.Id,
                    ProviderProfileId = provider1.Id,
                    InspectionPrice = 25000,
                    Note = "Demo pending offer.",
                    ProviderLatitude = 33.5140000m,
                    ProviderLongitude = 36.2770000m,
                    Status = OfferStatus.Pending
                },
                new ProviderOffer
                {
                    OrderId = inspectionAcceptedOrder.Id,
                    ProviderProfileId = provider1.Id,
                    InspectionPrice = 30000,
                    Note = "Demo accepted for inspection offer.",
                    ProviderLatitude = 33.5155000m,
                    ProviderLongitude = 36.2805000m,
                    Status = OfferStatus.AcceptedForInspection
                },
                new ProviderOffer
                {
                    OrderId = inProgressOrder.Id,
                    ProviderProfileId = provider2.Id,
                    InspectionPrice = 35000,
                    Note = "Demo accepted for work offer.",
                    ProviderLatitude = 33.5185000m,
                    ProviderLongitude = 36.2855000m,
                    Status = OfferStatus.AcceptedForWork
                },
                new ProviderOffer
                {
                    OrderId = waitingOrder.Id,
                    ProviderProfileId = provider2.Id,
                    InspectionPrice = 28000,
                    Note = "Demo rejected offer.",
                    ProviderLatitude = 33.5160000m,
                    ProviderLongitude = 36.2820000m,
                    Status = OfferStatus.Rejected
                },
                new ProviderOffer
                {
                    OrderId = afterInspectionRejectedOrder.Id,
                    ProviderProfileId = provider3.Id,
                    InspectionPrice = 40000,
                    Note = "Demo rejected after inspection offer.",
                    ProviderLatitude = 33.5360000m,
                    ProviderLongitude = 36.3060000m,
                    Status = OfferStatus.RejectedAfterInspection
                },
                new ProviderOffer
                {
                    OrderId = inProgressOrder.Id,
                    ProviderProfileId = provider3.Id,
                    InspectionPrice = 38000,
                    Note = "Demo automatically rejected offer.",
                    ProviderLatitude = 33.5190000m,
                    ProviderLongitude = 36.2860000m,
                    Status = OfferStatus.RejectedAutomatically
                },
                new ProviderOffer
                {
                    OrderId = waitingOrder.Id,
                    ProviderProfileId = provider3.Id,
                    InspectionPrice = 26000,
                    Note = "Demo cancelled by provider offer.",
                    ProviderLatitude = 33.5170000m,
                    ProviderLongitude = 36.2830000m,
                    Status = OfferStatus.CancelledByProvider
                },
                new ProviderOffer
                {
                    OrderId = cancelledOrder.Id,
                    ProviderProfileId = provider2.Id,
                    InspectionPrice = 27000,
                    Note = "Demo cancelled due to order cancellation.",
                    ProviderLatitude = 33.5310000m,
                    ProviderLongitude = 36.3010000m,
                    Status = OfferStatus.CancelledDueToOrderCancellation
                }
            };

            await context.ProviderOffers.AddRangeAsync(offers);
            await context.SaveChangesAsync();

            var rating = new Rating
            {
                OrderId = completedOrder.Id,
                CustomerId = customer2.Id,
                ProviderProfileId = provider1.Id,
                Value = 5
            };

            await context.Ratings.AddAsync(rating);
            await context.SaveChangesAsync();

            var firstOffer = offers.First();

            var notifications = new List<Notification>
            {
                CreateNotification(customer1.Id, "New offer received", "Demo notification.", NotificationType.NewOfferReceived, waitingOrder.Id, firstOffer.Id, null),
                CreateNotification(provider1.UserId, "Offer accepted for inspection", "Demo notification.", NotificationType.OfferAcceptedForInspection, inspectionAcceptedOrder.Id, offers[1].Id, null),
                CreateNotification(provider3.UserId, "Offer rejected after inspection", "Demo notification.", NotificationType.OfferRejectedAfterInspection, afterInspectionRejectedOrder.Id, offers[4].Id, null),
                CreateNotification(provider2.UserId, "Offer accepted for work", "Demo notification.", NotificationType.OfferAcceptedForWork, inProgressOrder.Id, offers[2].Id, null),
                CreateNotification(provider3.UserId, "Order completion pending", "Demo notification.", NotificationType.OrderCompletionPending, completionPendingOrder.Id, null, null),
                CreateNotification(customer2.Id, "Order completed", "Demo notification.", NotificationType.OrderCompleted, completedOrder.Id, null, null),
                CreateNotification(provider1.UserId, "Subscription approved", "Demo notification.", NotificationType.SubscriptionApproved, null, null, approvedRequest.Id),
                CreateNotification(provider2.UserId, "Subscription rejected", "Demo notification.", NotificationType.SubscriptionRejected, null, null, rejectedRequest.Id),
                CreateNotification(provider2.UserId, "Order cancelled", "Demo notification.", NotificationType.OrderCancelled, cancelledOrder.Id, offers[7].Id, null),
                CreateNotification(provider2.UserId, "Offer rejected", "Demo notification.", NotificationType.OfferRejected, waitingOrder.Id, offers[3].Id, null),
                CreateNotification(provider3.UserId, "Offer rejected automatically", "Demo notification.", NotificationType.OfferRejectedAutomatically, inProgressOrder.Id, offers[5].Id, null),
                CreateNotification(customer1.Id, "Offer cancelled by provider", "Demo notification.", NotificationType.OfferCancelledByProvider, waitingOrder.Id, offers[6].Id, null),
                CreateNotification(provider1.UserId, "Rating received", "Demo notification.", NotificationType.RatingReceived, completedOrder.Id, null, null)
            };

            notifications[0].IsRead = true;
            notifications[0].ReadAt = DateTime.UtcNow;

            await context.Notifications.AddRangeAsync(notifications);
            await context.SaveChangesAsync();

            await transaction.CommitAsync();
        }

        private static async Task SeedAdminAsync(AppDbContext context)
        {
            var adminEmail = "admin@homemaintenance.com";

            var exists = await context.Users.AnyAsync(u => u.Email == adminEmail);

            if (exists)
                return;

            var admin = CreateUser(
                "System Admin",
                adminEmail,
                "0999999999",
                UserRole.Admin,
                "Admin@12345");

            await context.Users.AddAsync(admin);
            await context.SaveChangesAsync();
        }

        private static async Task SeedSpecializationsAsync(AppDbContext context)
        {
            var specializations = new List<Specialization>
            {
                new Specialization
                {
                    Name = "Electricity",
                    Description = "Electrical maintenance and repairs.",
                    IsActive = true
                },
                new Specialization
                {
                    Name = "Plumbing",
                    Description = "Water leaks, pipes, sinks, and bathrooms.",
                    IsActive = true
                },
                new Specialization
                {
                    Name = "Air Conditioning",
                    Description = "AC installation, cleaning, and repair.",
                    IsActive = true
                },
                new Specialization
                {
                    Name = "Carpentry",
                    Description = "Wood repair, doors, furniture, and installation.",
                    IsActive = true
                },
                new Specialization
                {
                    Name = "Painting",
                    Description = "Home wall painting and finishing.",
                    IsActive = true
                },
                new Specialization
                {
                    Name = "Home Appliances",
                    Description = "Appliance repair and maintenance.",
                    IsActive = true
                },
                new Specialization
                {
                    Name = "Cleaning",
                    Description = "Home cleaning services.",
                    IsActive = true
                }
            };

            foreach (var specialization in specializations)
            {
                var exists = await context.Specializations
                    .AnyAsync(s => s.Name == specialization.Name);

                if (!exists)
                {
                    await context.Specializations.AddAsync(specialization);
                }
            }

            await context.SaveChangesAsync();
        }

        private static async Task SeedSubscriptionPlansAsync(AppDbContext context)
        {
            var plans = new List<SubscriptionPlan>
            {
                new SubscriptionPlan
                {
                    Name = "Monthly Plan",
                    Price = 50000,
                    DurationInDays = 30,
                    IsActive = true
                },
                new SubscriptionPlan
                {
                    Name = "Quarterly Plan",
                    Price = 135000,
                    DurationInDays = 90,
                    IsActive = true
                },
                new SubscriptionPlan
                {
                    Name = "Yearly Plan",
                    Price = 480000,
                    DurationInDays = 365,
                    IsActive = true
                }
            };

            foreach (var plan in plans)
            {
                var exists = await context.SubscriptionPlans
                    .AnyAsync(p => p.Name == plan.Name);

                if (!exists)
                {
                    await context.SubscriptionPlans.AddAsync(plan);
                }
            }

            await context.SaveChangesAsync();
        }

        private static User CreateUser(
            string fullName,
            string email,
            string phoneNumber,
            UserRole role,
            string password)
        {
            var salt = GenerateSalt();

            return new User
            {
                FullName = fullName,
                Email = email,
                PhoneNumber = phoneNumber,
                Role = role,
                PasswordSalt = salt,
                PasswordHash = ComputeHash(password, salt),
                IsEmailVerified = true,
                CreatedAt = DateTime.UtcNow
            };
        }

        private static Notification CreateNotification(
            int userId,
            string title,
            string message,
            NotificationType type,
            int? relatedOrderId,
            int? relatedOfferId,
            int? relatedSubscriptionPaymentRequestId)
        {
            return new Notification
            {
                UserId = userId,
                Title = title,
                Message = message,
                Type = type,
                RelatedOrderId = relatedOrderId,
                RelatedOfferId = relatedOfferId,
                RelatedSubscriptionPaymentRequestId = relatedSubscriptionPaymentRequestId,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };
        }

        private static string GenerateSalt()
        {
            var saltBytes = RandomNumberGenerator.GetBytes(16);
            return Convert.ToBase64String(saltBytes);
        }

        private static string ComputeHash(string value, string salt)
        {
            using var sha256 = SHA256.Create();

            var combined = value + salt;
            var bytes = Encoding.UTF8.GetBytes(combined);
            var hash = sha256.ComputeHash(bytes);

            return Convert.ToBase64String(hash);
        }
    }
}