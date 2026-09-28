using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectricERP.Application.Interfaces
{
    public interface IQrCodeService
    {
        string GenerateBase64(string content);
    }
}
