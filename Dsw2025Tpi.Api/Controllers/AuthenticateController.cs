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
        try
        {
            var token= await _authenticateService.LoginAsync(request);
            return Ok(token);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }
    
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterModelRequest model)
    {
        try
        {
            var result = await _authenticateService.RegisterAsync(model);
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
