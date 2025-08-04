using Dsw2025Tpi.Application.Dtos;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dsw2025Tpi.Application.Interfaces;

public interface IAuthenticateService
{
    Task<RegisterModelResponse> RegisterAsync(RegisterModelRequest model);
    Task<LoginModelResponse> LoginAsync(LoginModelRequest model);
}
