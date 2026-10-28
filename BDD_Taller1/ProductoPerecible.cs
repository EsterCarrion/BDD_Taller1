using BDD_Taller1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BDD_Taller1
{
   
}
public class ProductoPerecible : Producto
{
    public DateTime FechaCaducidad { get; set; }

    public ProductoPerecible(
        string codigo,
        string nombre,
        decimal precioBase,
        DateTime fechaCaducidad
    ) : base(codigo, nombre, precioBase)
    {
        FechaCaducidad = fechaCaducidad;
    }

    public override decimal CalcularPrecioVenta()
    {
        int dias = (FechaCaducidad.Date - DateTime.Today).Days;

        if (dias >= 0 && dias <= 7)
        {
            return PrecioBase * 0.70m;
        }

        return PrecioBase;
    }
}