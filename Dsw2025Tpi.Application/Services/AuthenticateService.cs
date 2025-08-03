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
    private readonly SignInManager<IdentityUserExtension> _signInManager;
    private readonly IJwtTokenService _jwtTokenService;

    public AuthenticateService(IRepository repository, UserManager<IdentityUserExtension> userManager
        , SignInManager<IdentityUserExtension> signInManager, IJwtTokenService jwtTokenService)
    {
        _repository = repository;
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtTokenService = jwtTokenService;
    }
    public async Task<LoginModelResponse> LoginAsync(LoginModelRequest model)
    {
        string rol=model.role;
        if(string.IsNullOrWhiteSpace(model.Username) || string.IsNullOrWhiteSpace(model.Password))
            throw new ArgumentException("El nombre de usuario y la contraseña son obligatorios");
        if(string.IsNullOrWhiteSpace(model.role)) rol="Cliente";

        var user = await _userManager.FindByNameAsync(model.Username);
        if (user == null) throw new UnauthorizedAccessException("Usuario o contraseña incorrectos");

        var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
        if (!result.Succeeded) throw new UnauthorizedAccessException("Usuario o contraseña incorrectos");
        var token = _jwtTokenService.GenerateToken(model.Username, rol);
        return new LoginModelResponse(token); 
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
