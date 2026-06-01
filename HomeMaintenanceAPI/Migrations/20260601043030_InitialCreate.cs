using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeMaintenanceAPI.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Specializations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Specializations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SubscriptionPlans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    DurationInDays = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscriptionPlans", x => x.Id);
                    table.CheckConstraint("CK_SubscriptionPlan_Duration", "[DurationInDays] > 0");
                    table.CheckConstraint("CK_SubscriptionPlan_Price", "[Price] >= 0");
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PasswordSalt = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<int>(type: "int", nullable: false),
                    IsEmailVerified = table.Column<bool>(type: "bit", nullable: false),
                    EmailVerificationCodeHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmailVerificationCodeExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RefreshToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RefreshTokenExpiryTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProviderProfiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    SpecializationId = table.Column<int>(type: "int", nullable: false),
                    Bio = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProviderProfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProviderProfiles_Specializations_SpecializationId",
                        column: x => x.SpecializationId,
                        principalTable: "Specializations",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ProviderProfiles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    SpecializationId = table.Column<int>(type: "int", nullable: false),
                    SelectedProviderProfileId = table.Column<int>(type: "int", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Latitude = table.Column<decimal>(type: "decimal(10,7)", precision: 10, scale: 7, nullable: false),
                    Longitude = table.Column<decimal>(type: "decimal(10,7)", precision: 10, scale: 7, nullable: false),
                    AddressText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CompletionTokenHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompletionTokenExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    table.CheckConstraint("CK_Order_Latitude", "[Latitude] BETWEEN -90 AND 90");
                    table.CheckConstraint("CK_Order_Longitude", "[Longitude] BETWEEN -180 AND 180");
                    table.ForeignKey(
                        name: "FK_Orders_ProviderProfiles_SelectedProviderProfileId",
                        column: x => x.SelectedProviderProfileId,
                        principalTable: "ProviderProfiles",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Orders_Specializations_SpecializationId",
                        column: x => x.SpecializationId,
                        principalTable: "Specializations",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Orders_Users_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SubscriptionPaymentRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProviderProfileId = table.Column<int>(type: "int", nullable: false),
                    SubscriptionPlanId = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PaymentMethod = table.Column<int>(type: "int", nullable: false),
                    TransactionId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProofImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReviewedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedByAdminId = table.Column<int>(type: "int", nullable: true),
                    AdminNote = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscriptionPaymentRequests", x => x.Id);
                    table.CheckConstraint("CK_SubscriptionPaymentRequest_Amount", "[Amount] >= 0");
                    table.ForeignKey(
                        name: "FK_SubscriptionPaymentRequests_ProviderProfiles_ProviderProfileId",
                        column: x => x.ProviderProfileId,
                        principalTable: "ProviderProfiles",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SubscriptionPaymentRequests_SubscriptionPlans_SubscriptionPlanId",
                        column: x => x.SubscriptionPlanId,
                        principalTable: "SubscriptionPlans",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SubscriptionPaymentRequests_Users_ReviewedByAdminId",
                        column: x => x.ReviewedByAdminId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ProviderOffers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    ProviderProfileId = table.Column<int>(type: "int", nullable: false),
                    InspectionPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProviderLatitude = table.Column<decimal>(type: "decimal(10,7)", precision: 10, scale: 7, nullable: false),
                    ProviderLongitude = table.Column<decimal>(type: "decimal(10,7)", precision: 10, scale: 7, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProviderOffers", x => x.Id);
                    table.CheckConstraint("CK_ProviderOffer_InspectionPrice", "[InspectionPrice] >= 0");
                    table.CheckConstraint("CK_ProviderOffer_Latitude", "[ProviderLatitude] BETWEEN -90 AND 90");
                    table.CheckConstraint("CK_ProviderOffer_Longitude", "[ProviderLongitude] BETWEEN -180 AND 180");
                    table.ForeignKey(
                        name: "FK_ProviderOffers_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ProviderOffers_ProviderProfiles_ProviderProfileId",
                        column: x => x.ProviderProfileId,
                        principalTable: "ProviderProfiles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Ratings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    ProviderProfileId = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ratings", x => x.Id);
                    table.CheckConstraint("CK_Rating_Value", "[Value] BETWEEN 1 AND 5");
                    table.ForeignKey(
                        name: "FK_Ratings_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Ratings_ProviderProfiles_ProviderProfileId",
                        column: x => x.ProviderProfileId,
                        principalTable: "ProviderProfiles",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Ratings_Users_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ProviderSubscriptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProviderProfileId = table.Column<int>(type: "int", nullable: false),
                    SubscriptionPlanId = table.Column<int>(type: "int", nullable: false),
                    SubscriptionPaymentRequestId = table.Column<int>(type: "int", nullable: false),
                    StartsAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndsAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProviderSubscriptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProviderSubscriptions_ProviderProfiles_ProviderProfileId",
                        column: x => x.ProviderProfileId,
                        principalTable: "ProviderProfiles",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ProviderSubscriptions_SubscriptionPaymentRequests_SubscriptionPaymentRequestId",
                        column: x => x.SubscriptionPaymentRequestId,
                        principalTable: "SubscriptionPaymentRequests",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ProviderSubscriptions_SubscriptionPlans_SubscriptionPlanId",
                        column: x => x.SubscriptionPlanId,
                        principalTable: "SubscriptionPlans",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReadAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RelatedOrderId = table.Column<int>(type: "int", nullable: true),
                    RelatedOfferId = table.Column<int>(type: "int", nullable: true),
                    RelatedSubscriptionPaymentRequestId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notifications_Orders_RelatedOrderId",
                        column: x => x.RelatedOrderId,
                        principalTable: "Orders",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Notifications_ProviderOffers_RelatedOfferId",
                        column: x => x.RelatedOfferId,
                        principalTable: "ProviderOffers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Notifications_SubscriptionPaymentRequests_RelatedSubscriptionPaymentRequestId",
                        column: x => x.RelatedSubscriptionPaymentRequestId,
                        principalTable: "SubscriptionPaymentRequests",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Notifications_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_RelatedOfferId",
                table: "Notifications",
                column: "RelatedOfferId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_RelatedOrderId",
                table: "Notifications",
                column: "RelatedOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_RelatedSubscriptionPaymentRequestId",
                table: "Notifications",
                column: "RelatedSubscriptionPaymentRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserId",
                table: "Notifications",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CustomerId",
                table: "Orders",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_SelectedProviderProfileId",
                table: "Orders",
                column: "SelectedProviderProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_SpecializationId",
                table: "Orders",
                column: "SpecializationId");

            migrationBuilder.CreateIndex(
                name: "IX_ProviderOffers_OrderId_ProviderProfileId",
                table: "ProviderOffers",
                columns: new[] { "OrderId", "ProviderProfileId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProviderOffers_ProviderProfileId",
                table: "ProviderOffers",
                column: "ProviderProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_ProviderProfiles_SpecializationId",
                table: "ProviderProfiles",
                column: "SpecializationId");

            migrationBuilder.CreateIndex(
                name: "IX_ProviderProfiles_UserId",
                table: "ProviderProfiles",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProviderSubscriptions_ProviderProfileId",
                table: "ProviderSubscriptions",
                column: "ProviderProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_ProviderSubscriptions_SubscriptionPaymentRequestId",
                table: "ProviderSubscriptions",
                column: "SubscriptionPaymentRequestId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProviderSubscriptions_SubscriptionPlanId",
                table: "ProviderSubscriptions",
                column: "SubscriptionPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_Ratings_CustomerId",
                table: "Ratings",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Ratings_OrderId",
                table: "Ratings",
                column: "OrderId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ratings_ProviderProfileId",
                table: "Ratings",
                column: "ProviderProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionPaymentRequests_ProviderProfileId",
                table: "SubscriptionPaymentRequests",
                column: "ProviderProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionPaymentRequests_ReviewedByAdminId",
                table: "SubscriptionPaymentRequests",
                column: "ReviewedByAdminId");

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionPaymentRequests_SubscriptionPlanId",
                table: "SubscriptionPaymentRequests",
                column: "SubscriptionPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionPaymentRequests_TransactionId",
                table: "SubscriptionPaymentRequests",
                column: "TransactionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_PhoneNumber",
                table: "Users",
                column: "PhoneNumber",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "ProviderSubscriptions");

            migrationBuilder.DropTable(
                name: "Ratings");

            migrationBuilder.DropTable(
                name: "ProviderOffers");

            migrationBuilder.DropTable(
                name: "SubscriptionPaymentRequests");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "SubscriptionPlans");

            migrationBuilder.DropTable(
                name: "ProviderProfiles");

            migrationBuilder.DropTable(
                name: "Specializations");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
