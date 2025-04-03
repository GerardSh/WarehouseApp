using WarehouseApp.Data.Models;
using static WarehouseApp.Common.Constants.EntityConstants.Client;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WarehouseApp.Data.Configuration
{
    internal class ClientConfiguration : IEntityTypeConfiguration<Client>
    {
        public void Configure(EntityTypeBuilder<Client> entity)
        {
            entity
                .HasKey(c => c.Id);

            entity
                .Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(NameMaxLength);

            entity
                .Property(c => c.Address)
                .IsRequired()
                .HasMaxLength(AddressMaxLength);

            entity
                .Property(c => c.PhoneNumber)
                .IsRequired(false)
                .HasMaxLength(PhoneNumberMaxLength);

            entity
                .Property(c => c.Email)
                .IsRequired(false)
                .HasMaxLength(EmailMaxLength);

            entity
                .HasData(SeedClients());
        }

        private List<Client> SeedClients()
        {
            return new List<Client>
             {
                new Client
                {
                    Id = Guid.Parse("e3b0c442-98fc-1c14-9afb-f4c8996fb924"),
                    Name = "Tech Solutions Ltd.",
                    Email = "info@techsolutions.com",
                    Address = "123 Silicon Valley Blvd, San Jose, USA",
                    PhoneNumber = "+1-408-555-0123"
                },
                new Client
                {
                    Id = Guid.Parse("3b12c292-b084-491b-9b3d-3e01b7ff2eaf"),
                    Name = "Innovative Electronics Corp.",
                    Email = "contact@innovelectronics.com",
                    Address = "456 Tech Park, San Francisco, USA",
                    PhoneNumber = "+1-415-555-0456"
                },
                new Client
                {
                    Id = Guid.Parse("f56d3c8b-92a4-4d71-82c7-6f4b9e2c1f0d"),
                    Name = "Global IT Traders",
                    Email = "support@globalittraders.com",
                    Address = "789 Technology Ave, London, UK",
                    PhoneNumber = "+44-20-7946-0301"
                },
                new Client
                {
                    Id = Guid.Parse("8a3f9b62-1e24-46f1-bc95-71f25a4c0e9a"),
                    Name = "SmartTech Healthcare Solutions",
                    Email = "sales@smarttechhealth.com",
                    Address = "101 Digital Health Park, Berlin, Germany",
                    PhoneNumber = "+49-30-5557-0224"
                },
                new Client
                {
                    Id = Guid.Parse("ac5b9f38-712d-456b-849f-b7a3e49c5d7e"),
                    Name = "EduTech Innovations",
                    Email = "info@edutechinnovations.com",
                    Address = "67 Learning Lane, Toronto, Canada",
                    PhoneNumber = "+1-416-555-0789"
                },
                new Client
                {
                    Id = Guid.Parse("d9f1b25c-84e6-4c1a-92b5-3a5e8c0f7b19"),
                    Name = "Retail Tech Solutions",
                    Email = "orders@retailtechsolutions.com",
                    Address = "55 E-commerce Plaza, Paris, France",
                    PhoneNumber = "+33-1-5557-0145"
                },
                new Client
                {
                    Id = Guid.Parse("b3e2c5a1-7d84-4f6b-91c2-5a7f3b8e9d4f"),
                    Name = "AutoTech Systems",
                    Email = "service@autotechsystems.com",
                    Address = "88 Auto Innovations Blvd, Detroit, USA",
                    PhoneNumber = "+1-313-555-0912"
                },
                new Client
                {
                    Id = Guid.Parse("a1c4b9e5-7f28-48c1-b62d-4f3a5d7e92b8"),
                    Name = "FinTech Innovations",
                    Email = "hello@fintechinnov.com",
                    Address = "22 Wall Street, New York, USA",
                    PhoneNumber = "+1-646-555-0876"
                },
                new Client
                {
                    Id = Guid.Parse("f8c3a5d7-21b4-4e9c-86f2-7d5b3a9e1c4b"),
                    Name = "AgroTech Solutions",
                    Email = "contact@agrotechsolutions.com",
                    Address = "77 Green Field Ave, Sydney, Australia",
                    PhoneNumber = "+61-2-5557-0332"
                },
                new Client
                {
                    Id = Guid.Parse("b2d9f1c5-84e7-4c1b-92a6-3a5e7c0f8b29"),
                    Name = "Construction Tech Masters",
                    Email = "projects@constructtechmasters.com",
                    Address = "99 Builder's Road, Dubai, UAE",
                    PhoneNumber = "+971-4-555-0198"
                }
             };
        }
    }
}
