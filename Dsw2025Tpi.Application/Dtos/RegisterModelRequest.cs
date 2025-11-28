using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dsw2025Tpi.Application.Dtos;

public record RegisterModelRequest(
    string Username,
    string? Email,
    CustomerModel.CustomerModelRequest? Customer,
    string Password,
    string Role
);

public record RegisterModelResponse(
    Guid? CustomerId,
    string Username,
    string Role
);
