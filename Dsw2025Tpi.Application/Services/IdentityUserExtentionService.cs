using Dsw2025Tpi.Data;
using Dsw2025Tpi.Data.Identity;
using Dsw2025Tpi.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dsw2025Tpi.Application.Services;

public class IdentityUserExtentionService
{
    private AuthenticateContext _authenticateContext;
    private Dsw2025TpiContext _dsw2025TpiContext;
    public IdentityUserExtentionService(AuthenticateContext authContext, Dsw2025TpiContext dominioContext)
    {
        _authenticateContext = authContext ?? throw new ArgumentNullException(nameof(authContext));
        _dsw2025TpiContext = dominioContext ?? throw new ArgumentNullException(nameof(dominioContext));
    }
    /*
    public async Task<Customer> GetCustomerOfUser(IdentityUser userr)
    {
        
    }
    */

}
