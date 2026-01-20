using System;
using InvestmentApp.Domain;
using Microsoft.AspNetCore.Identity;

namespace InvestmentApp.Infrastructure;

public class ApplicationUser : IdentityUser<Guid>
{
    // Optional link to domain Customer if you want to associate an identity user with a Customer record.
    public Guid? CustomerId { get; set; }
    public Customer? Customer { get; set; }

    // Add any application-specific properties here, e.g. DisplayName, TimeZone, etc.
}
