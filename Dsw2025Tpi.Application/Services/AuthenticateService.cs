using Dsw2025Tpi.Application.Dtos;
using Dsw2025Tpi.Application.Exceptions;
using Dsw2025Tpi.Application.Interfaces;
using Dsw2025Tpi.Application.Validation;
using Dsw2025Tpi.Data.Identity;
using Dsw2025Tpi.Domain.Entities;
using Dsw2025Tpi.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dsw2025Tpi.Application.Services;

public class AuthenticateService : IAuthenticateService
{
    private readonly IRepository _repository;
    private readonly UserManager<IdentityUserExtension> _userManager;
    public AuthenticateService(IRepository repository, UserManager<IdentityUserExtension> userManager)
    {
        _repository = repository;
        _userManager = userManager;
    }
    public Task<IdentityResult> LoginAsync(LoginModel modedl)
    {
        throw new NotImplementedException();
    }

    public async Task<IdentityResult> RegisterAsync(RegisterModel model)
    {
        CustomerValidator.Validate(model.Customer);
        var existmail = await _repository.First<Customer>(c => c.Email == model.Customer.Email);
        var existPhoneNumber = await _repository.First<Customer>(c => c.PhoneNumber == model.Customer.PhoneNumber);
        if (existmail !=null) throw new DuplicatedEntityException($"Un cliente ya fue registrado con el EMAIL: {model.Customer.Email}");
        if (existPhoneNumber !=null) throw new DuplicatedEntityException($"Un cliente ya fue registrado el numero de telefono: {model.Customer.PhoneNumber}");
        var customer = new Customer(model.Customer.Name, model.Customer.Email, model.Customer.PhoneNumber);
        
        var user = new IdentityUserExtension { CustomerId = customer.Id, UserName = model.Customer.Email, Email = model.Customer.Email };
        await _repository.Add(customer);
        var result = await _userManager.CreateAsync(user, model.Password);

        return result;
    }
}
