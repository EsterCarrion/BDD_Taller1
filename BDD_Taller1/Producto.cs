using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BDD_Taller1
{

    public abstract class Producto
    {
        public string Codigo { get; private set; }
        public string Nombre { get; set; }
        public decimal PrecioBase { get; set; }
        public int Stock { get; private set; }

        public Producto(string codigo, string nombre, decimal precioBase)
        {
            Codigo = codigo;
            Nombre = nombre;
            PrecioBase = precioBase;
            Stock = 0;
        }
        internal void AumentarStock(int cantidad)
        {
            if (cantidad <= 0)
                throw new ArgumentException("La cantidad debe ser positiva.");

            Stock = checked(Stock + cantidad);
        }

        internal void DisminuirStock(int cantidad)
        {
            if (cantidad <= 0)
                throw new ArgumentException("La cantidad debe ser positiva.");

            if (cantidad > Stock)
                throw new InvalidOperationException("Stock insuficiente.");

            Stock -= cantidad;
        }

        public abstract decimal CalcularPrecioVenta();


    }
}
