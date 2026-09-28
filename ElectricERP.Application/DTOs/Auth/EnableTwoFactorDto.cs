using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectricERP.Application.DTOs.Auth
{
    public class EnableTwoFactorDto
    {
        public string Code { get; set; } = string.Empty;
    }
}
