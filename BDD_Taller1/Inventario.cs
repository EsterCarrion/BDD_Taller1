using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BDD_Taller1
{
    public class Inventario
    {
        private readonly List<Producto> productos = new();
        private readonly List<Movimiento> movimientos = new();

        public IReadOnlyList<Producto> Productos =>
            productos.AsReadOnly();

        public IReadOnlyList<Movimiento> Movimientos =>
            movimientos.AsReadOnly();

        // CREATE
        public void AgregarProducto(Producto producto)
        {
            if (producto == null)
                throw new ArgumentNullException(nameof(producto));

            if (string.IsNullOrWhiteSpace(producto.Codigo))
                throw new ArgumentException("Código obligatorio.");

            if (BuscarProducto(producto.Codigo) != null)
                throw new InvalidOperationException("El código ya existe.");

            if (string.IsNullOrWhiteSpace(producto.Nombre) ||
                producto.PrecioBase < 0)
                throw new ArgumentException("Datos inválidos.");

            productos.Add(producto);
        }

        // READ
        public Producto? BuscarProducto(string codigo)
        {
            return productos.FirstOrDefault(p =>
                p.Codigo.Equals(codigo,
                    StringComparison.OrdinalIgnoreCase));
        }

        public void ListarProductos()
        {
            foreach (var p in productos)
            {
                Console.WriteLine(
                    $"{p.Codigo} | {p.Nombre} | " +
                    $"Stock: {p.Stock} | " +
                    $"Precio: ${p.CalcularPrecioVenta():F2}");
            }
        }

        // UPDATE
        public void ActualizarProducto(
            string codigo, string nombre, decimal precio,
            DateTime? fecha = null, int? garantia = null)
        {
            var p = ObtenerProducto(codigo);

            if (string.IsNullOrWhiteSpace(nombre) || precio < 0)
                throw new ArgumentException("Datos inválidos.");

            if (p is ProductoPerecible && fecha == null)
                throw new ArgumentException("Falta la fecha.");

            if (p is ProductoNoPerecible &&
                (garantia == null || garantia < 0))
                throw new ArgumentException("Garantía inválida.");

            p.Nombre = nombre;
            p.PrecioBase = precio;

            if (p is ProductoPerecible perecible)
                perecible.FechaCaducidad = fecha!.Value;

            if (p is ProductoNoPerecible noPerecible)
                noPerecible.MesesGarantia = garantia!.Value;
        }

        // DELETE
        public void EliminarProducto(string codigo)
        {
            var p = ObtenerProducto(codigo);

            if (movimientos.Any(m =>
                m.CodigoProducto.Equals(codigo,
                    StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException(
                    "No se puede eliminar un producto con historial.");

            productos.Remove(p);
        }

        private Producto ObtenerProducto(string codigo)
        {
            return BuscarProducto(codigo) ??
                throw new InvalidOperationException(
                    "Producto no encontrado.");
        }

        // COMPRA
        public void RegistrarCompra(
            string codigo, int cantidad,
            decimal costoUnitario, string proveedor)
        {
            var p = ObtenerProducto(codigo);

            if (cantidad <= 0 || costoUnitario < 0 ||
                string.IsNullOrWhiteSpace(proveedor))
                throw new ArgumentException("Datos de compra inválidos.");

            decimal total = checked(cantidad * costoUnitario);
            p.AumentarStock(cantidad);

            movimientos.Add(new Movimiento(
                DateTime.Now, p.Codigo, "Compra",
                cantidad, costoUnitario, proveedor,
                total, p.Stock));
        }

        // VENTA
        public decimal RegistrarVenta(
            string codigo, int cantidad, string cliente)
        {
            var p = ObtenerProducto(codigo);

            if (cantidad <= 0)
                throw new ArgumentException("Cantidad inválida.");

            if (p is ProductoPerecible perecible &&
                perecible.FechaCaducidad.Date < DateTime.Today)
                throw new InvalidOperationException(
                    "No se puede vender un producto caducado.");

            decimal precio = p.CalcularPrecioVenta();
            decimal total = checked(precio * cantidad);

            p.DisminuirStock(cantidad);

            movimientos.Add(new Movimiento(
                DateTime.Now, p.Codigo, "Venta",
                cantidad, precio,
                string.IsNullOrWhiteSpace(cliente)
                    ? "Consumidor final" : cliente,
                total, p.Stock));

            return total;
        }

        // KARDEX
        public void MostrarKardex(string codigo)
        {
            ObtenerProducto(codigo);

            foreach (var m in movimientos.Where(m =>
                m.CodigoProducto.Equals(codigo,
                    StringComparison.OrdinalIgnoreCase)))
            {
                Console.WriteLine(
                    $"{m.Fecha:g} | {m.Tipo} | " +
                    $"Cantidad: {m.Cantidad} | " +
                    $"Precio: ${m.PrecioUnitario:F2} | " +
                    $"Saldo: {m.Saldo}");
            }
        }

        // REPORTE DE VENTAS
        public void ReporteVentas(DateTime desde, DateTime hasta)
        {
            if (desde.Date > hasta.Date)
                throw new ArgumentException("Fechas incorrectas.");

            var ventas = movimientos.Where(m =>
                m.Tipo == "Venta" &&
                m.Fecha.Date >= desde.Date &&
                m.Fecha.Date <= hasta.Date).ToList();

            Console.WriteLine(
                $"Total vendido: ${ventas.Sum(m => m.Total):F2}");

            var top3 = ventas
                .GroupBy(m => m.CodigoProducto)
                .Select(g => new
                {
                    Codigo = g.Key,
                    Cantidad = g.Sum(m => m.Cantidad)
                })
                .OrderByDescending(x => x.Cantidad)
                .Take(3);

            Console.WriteLine("Productos más vendidos:");

            foreach (var item in top3)
            {
                var p = BuscarProducto(item.Codigo);
                Console.WriteLine(
                    $"{item.Codigo} - {p?.Nombre}: " +
                    $"{item.Cantidad} unidades");
            }
        }

        // RECONSTRUIR HISTORIAL AL CARGAR JSON
        public void CargarMovimientos(IEnumerable<Movimiento> historial)
        {
            if (movimientos.Count > 0 ||
                productos.Any(p => p.Stock != 0))
                throw new InvalidOperationException(
                    "El inventario ya tiene movimientos.");

            foreach (var m in historial)
            {
                var p = ObtenerProducto(m.CodigoProducto);

                if (m.Tipo == "Compra")
                    p.AumentarStock(m.Cantidad);
                else if (m.Tipo == "Venta")
                    p.DisminuirStock(m.Cantidad);
                else
                    throw new InvalidOperationException(
                        "Tipo de movimiento desconocido.");

                if (p.Stock != m.Saldo)
                    throw new InvalidOperationException(
                        "Inconsistencia en el Kárdex.");

                movimientos.Add(m);
            }
        }
    }
}
