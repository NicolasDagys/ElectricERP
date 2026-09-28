using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectricERP.Infrastructure.Entities
{
    public partial class ApplicationUser : IdentityUser
    {
        public bool Activo { get; set; } = true;

        public virtual ICollection<Movimiento> Movimientos { get; set; } = new List<Movimiento>();

        public virtual ICollection<SolicitudAjuste> SolicitudAjusteIdEmpleadoSolicitanteNavigations { get; set; } = new List<SolicitudAjuste>();

        public virtual ICollection<SolicitudAjuste> SolicitudAjusteIdSupervisorResolutorNavigations { get; set; } = new List<SolicitudAjuste>();

        public virtual ICollection<Transferencia> TransferenciaIdAdministradorAutorizadorNavigations { get; set; } = new List<Transferencia>();

        public virtual ICollection<Transferencia> TransferenciaIdSupervisorSolicitanteNavigations { get; set; } = new List<Transferencia>();

        public virtual ICollection<Transferencia> TransferenciaIdTransportistaNavigations { get; set; } = new List<Transferencia>();

        public virtual ICollection<UbicacionGps> UbicacionGps { get; set; } = new List<UbicacionGps>();
        public string? RefreshTokenHash { get; set; }

        public DateTime? RefreshTokenExpiryTime { get; set; }
    }
}
