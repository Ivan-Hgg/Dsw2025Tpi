using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Dsw2025Tpi.Application.Exceptions;

public class DataInsertException(string message) : Exception(message)
{
}
