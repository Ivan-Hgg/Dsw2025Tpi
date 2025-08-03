using Dsw2025Tpi.Application.Dtos;
using Dsw2025Tpi.Application.Exceptions;
using Dsw2025Tpi.Application.Interfaces;
//using Dsw2025Tpi.Application.Services;
using Dsw2025Tpi.Application.Validation;
using Dsw2025Tpi.Data.Identity;
using Dsw2025Tpi.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace Dsw2025Tpi.Api.Controllers;
[ApiController]
[Route("api/auth")]
public class AuthenticateController : ControllerBase
{
    private readonly UserManager<IdentityUserExtension> _userManager;
    private readonly SignInManager<IdentityUserExtension> _signInManager;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IAuthenticateService _authenticateService;

    public AuthenticateController(UserManager<IdentityUserExtension> userManager,
        SignInManager<IdentityUserExtension> signInManager,
        IJwtTokenService jwtTokenService, IAuthenticateService authenticateService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtTokenService = jwtTokenService;
        _authenticateService = authenticateService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel request)
    {
        
        var user = await _userManager.FindByNameAsync(request.Username);
        if (user == null)
        {
            return Unauthorized("Usuario o contraseña incorrectos");
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
        if (!result.Succeeded)
        {
            return Unauthorized("Usuario o contraseña incorrectos");
        }
        var token = _jwtTokenService.GenerateToken(request.Username, request.role);
        return Ok(new { token }); 
        /*
        var role = string.Empty;
        if (request.Username == "admin" && request.Password == "admin123")
        {
            role = "Administrador";
        }
        else if (request.Username == "user" && request.Password == "user123")
        {
            role = "Usuario";
        }
        else
        {
            return Unauthorized("Usuario o contraseña incorrectos");
        }
        var token = _jwtTokenService.GenerateToken(request.Username, role);
        return Ok(new { token });*/
    }
    
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterModel model)
    {
        try
        {
            var result = await _authenticateService.RegisterAsync(model);
            if (!result.Succeeded) return BadRequest(result.Errors);
            return StatusCode(201, result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (DuplicatedEntityException ex)
        {
            return Conflict(ex.Message);
        }
    }
}
