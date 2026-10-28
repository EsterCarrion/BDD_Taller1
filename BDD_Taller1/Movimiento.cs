using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BDD_Taller1
{
    public record Movimiento(
         DateTime Fecha,
         string CodigoProducto,
         string Tipo,
         int Cantidad,
         decimal PrecioUnitario,
         string Tercero,
         decimal Total,
         int Saldo
     );
}
