using Dsw2025Tpi.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dsw2025Tpi.Data.Identity;

public class IdentityUserExtension : IdentityUser
{
    public Guid CustomerId { get; set; } // Fk
    public Customer? Customer { get; set; } // Navigation property
}
