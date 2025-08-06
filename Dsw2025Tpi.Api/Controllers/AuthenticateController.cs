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
        private readonly IAuthenticateService _authenticateService;

    public AuthenticateController(IAuthenticateService authenticateService)
    {
        _authenticateService = authenticateService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginModelRequest request)
    {
        var token= await _authenticateService.LoginAsync(request);
        return Ok(token);
    }
    
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterModelRequest model)
    {
        var result = await _authenticateService.RegisterAsync(model);
        return StatusCode(201, result);
    }
}
