using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BDD_Taller1
{
    public class ProductoNoPerecible : Producto
    {
        public int MesesGarantia { get; set; }

        public ProductoNoPerecible(
            string codigo,
            string nombre,
            decimal precioBase,
            int mesesGarantia
        ) : base(codigo, nombre, precioBase)
        {
            MesesGarantia = mesesGarantia;
        }

        public override decimal CalcularPrecioVenta()
        {
            return PrecioBase * 1.15m;
        }
    }
}


