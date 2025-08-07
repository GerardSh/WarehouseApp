using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using WarehouseApp.Data.Models;

using static WarehouseApp.Common.Constants.EntityConstants.AdminRequest;

namespace WarehouseApp.Data.Configuration
{
    internal class AdminRequestConfiguration : IEntityTypeConfiguration<AdminRequest>
    {
        public void Configure(EntityTypeBuilder<AdminRequest> entity)
        {
            entity
                .HasKey(ar => ar.Id);

            entity
                .Property(ar => ar.Reason)
                .IsRequired()
                .HasMaxLength(ReasonMaxLength);

            entity
                .Property(ar => ar.RequestedAt)
                .IsRequired();

            entity
                .Property(ar => ar.Status)
                .HasConversion<int>()
                .IsRequired();

            entity
                .HasOne(ar => ar.User)
                .WithMany()
                .HasForeignKey(ar => ar.UserId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
