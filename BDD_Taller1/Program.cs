using BDD_Taller1;
using System;
using System.Globalization;

namespace SistemaInventario
{
    internal class Program
    {
        static Inventario inventario = new Inventario();

        static void Main(string[] args)
        {
            // Cargar los datos guardados anteriormente
            try
            {
                inventario = GestorArchivos.Cargar();
                Console.WriteLine("Datos cargados correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al cargar datos: " + ex.Message);
                Console.WriteLine("No se iniciará el sistema para evitar pérdida de datos.");
                return;
            }

            int opcion;

            do
            {
                Console.WriteLine("\n================================");
                Console.WriteLine("     SISTEMA DE INVENTARIO");
                Console.WriteLine("================================");
                Console.WriteLine("1. Registrar producto");
                Console.WriteLine("2. Listar productos");
                Console.WriteLine("3. Buscar producto");
                Console.WriteLine("4. Actualizar producto");
                Console.WriteLine("5. Eliminar producto");
                Console.WriteLine("6. Registrar compra");
                Console.WriteLine("7. Registrar venta");
                Console.WriteLine("8. Consultar Kárdex");
                Console.WriteLine("9. Reporte de ventas");
                Console.WriteLine("0. Salir");
                Console.WriteLine("================================");

                opcion = LeerEntero("Seleccione una opción: ");

                try
                {
                    switch (opcion)
                    {
                        case 1:
                            RegistrarProducto();
                            break;

                        case 2:
                            inventario.ListarProductos();
                            break;

                        case 3:
                            BuscarProducto();
                            break;

                        case 4:
                            ActualizarProducto();
                            break;

                        case 5:
                            EliminarProducto();
                            break;

                        case 6:
                            RegistrarCompra();
                            break;

                        case 7:
                            RegistrarVenta();
                            break;

                        case 8:
                            ConsultarKardex();
                            break;

                        case 9:
                            ReporteVentas();
                            break;

                        case 0:
                            Console.WriteLine("Saliendo del sistema...");
                            break;

                        default:
                            Console.WriteLine("Opción no válida.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }

            } while (opcion != 0);
        }

        // ==========================================
        // 1. REGISTRAR PRODUCTO
        // ==========================================

        static void RegistrarProducto()
        {
            Console.WriteLine("\n--- REGISTRAR PRODUCTO ---");

            string codigo = LeerTexto("Código: ");
            string nombre = LeerTexto("Nombre: ");
            decimal precio = LeerDecimal("Precio base: ");

            Console.WriteLine("1. Perecible");
            Console.WriteLine("2. No perecible");

            int tipo = LeerEntero("Tipo de producto: ");

            Producto producto;

            if (tipo == 1)
            {
                DateTime fecha = LeerFecha(
                    "Fecha de caducidad (dd/MM/yyyy): ");

                producto = new ProductoPerecible(
                    codigo, nombre, precio, fecha);
            }
            else if (tipo == 2)
            {
                int garantia = LeerEntero(
                    "Meses de garantía: ");

                if (garantia < 0)
                {
                    Console.WriteLine("Garantía inválida.");
                    return;
                }

                producto = new ProductoNoPerecible(
                    codigo, nombre, precio, garantia);
            }
            else
            {
                Console.WriteLine("Tipo de producto inválido.");
                return;
            }

            inventario.AgregarProducto(producto);
            GestorArchivos.Guardar(inventario);

            Console.WriteLine("Producto registrado correctamente.");
        }

        // ==========================================
        // 3. BUSCAR PRODUCTO
        // ==========================================

        static void BuscarProducto()
        {
            Console.WriteLine("\n--- BUSCAR PRODUCTO ---");

            string codigo = LeerTexto("Código: ");

            Producto? producto = inventario.BuscarProducto(codigo);

            if (producto == null)
            {
                Console.WriteLine("Producto no encontrado.");
                return;
            }

            Console.WriteLine($"Código: {producto.Codigo}");
            Console.WriteLine($"Nombre: {producto.Nombre}");
            Console.WriteLine($"Precio base: ${producto.PrecioBase:F2}");
            Console.WriteLine($"Stock: {producto.Stock}");
            Console.WriteLine(
                $"Precio de venta: ${producto.CalcularPrecioVenta():F2}");

            if (producto is ProductoPerecible perecible)
            {
                Console.WriteLine("Tipo: Perecible");
                Console.WriteLine(
                    $"Caducidad: {perecible.FechaCaducidad:dd/MM/yyyy}");
            }
            else if (producto is ProductoNoPerecible noPerecible)
            {
                Console.WriteLine("Tipo: No perecible");
                Console.WriteLine(
                    $"Garantía: {noPerecible.MesesGarantia} meses");
            }
        }

        // ==========================================
        // 4. ACTUALIZAR PRODUCTO
        // ==========================================

        static void ActualizarProducto()
        {
            Console.WriteLine("\n--- ACTUALIZAR PRODUCTO ---");

            string codigo = LeerTexto("Código: ");

            Producto? producto = inventario.BuscarProducto(codigo);

            if (producto == null)
            {
                Console.WriteLine("Producto no encontrado.");
                return;
            }

            string nombre = LeerTexto("Nuevo nombre: ");
            decimal precio = LeerDecimal("Nuevo precio base: ");

            if (producto is ProductoPerecible)
            {
                DateTime fecha = LeerFecha(
                    "Nueva fecha de caducidad (dd/MM/yyyy): ");

                inventario.ActualizarProducto(
                    codigo, nombre, precio, fecha: fecha);
            }
            else if (producto is ProductoNoPerecible)
            {
                int garantia = LeerEntero(
                    "Nuevos meses de garantía: ");

                inventario.ActualizarProducto(
                    codigo, nombre, precio, garantia: garantia);
            }

            GestorArchivos.Guardar(inventario);

            Console.WriteLine("Producto actualizado correctamente.");
        }

        // ==========================================
        // 5. ELIMINAR PRODUCTO
        // ==========================================

        static void EliminarProducto()
        {
            Console.WriteLine("\n--- ELIMINAR PRODUCTO ---");

            string codigo = LeerTexto("Código: ");

            Producto? producto = inventario.BuscarProducto(codigo);

            if (producto == null)
            {
                Console.WriteLine("Producto no encontrado.");
                return;
            }

            Console.WriteLine($"Producto: {producto.Nombre}");

            string confirmar = LeerTexto(
                "¿Desea eliminar este producto? (S/N): ");

            if (confirmar.Equals("S",
                StringComparison.OrdinalIgnoreCase))
            {
                inventario.EliminarProducto(codigo);
                GestorArchivos.Guardar(inventario);

                Console.WriteLine("Producto eliminado.");
            }
            else
            {
                Console.WriteLine("Eliminación cancelada.");
            }
        }

        // ==========================================
        // 6. REGISTRAR COMPRA
        // ==========================================

        static void RegistrarCompra()
        {
            Console.WriteLine("\n--- REGISTRAR COMPRA ---");

            string codigo = LeerTexto("Código del producto: ");

            Producto? producto = inventario.BuscarProducto(codigo);

            if (producto == null)
            {
                Console.WriteLine("Producto no encontrado.");
                return;
            }

            Console.WriteLine($"Producto: {producto.Nombre}");
            Console.WriteLine($"Stock actual: {producto.Stock}");

            int cantidad = LeerEntero("Cantidad a comprar: ");
            decimal costo = LeerDecimal("Costo unitario: ");
            string proveedor = LeerTexto("Proveedor: ");

            inventario.RegistrarCompra(
                codigo, cantidad, costo, proveedor);

            GestorArchivos.Guardar(inventario);

            Console.WriteLine("Compra registrada correctamente.");
            Console.WriteLine(
                $"Nuevo stock: {producto.Stock}");
        }

        // ==========================================
        // 7. REGISTRAR VENTA
        // ==========================================

        static void RegistrarVenta()
        {
            Console.WriteLine("\n--- REGISTRAR VENTA ---");

            string codigo = LeerTexto("Código del producto: ");

            Producto? producto = inventario.BuscarProducto(codigo);

            if (producto == null)
            {
                Console.WriteLine("Producto no encontrado.");
                return;
            }

            Console.WriteLine($"Producto: {producto.Nombre}");
            Console.WriteLine($"Stock disponible: {producto.Stock}");
            Console.WriteLine(
                $"Precio de venta: ${producto.CalcularPrecioVenta():F2}");

            int cantidad = LeerEntero("Cantidad a vender: ");

            Console.Write("Cliente (Enter = Consumidor final): ");
            string cliente = Console.ReadLine() ?? "";

            decimal total = inventario.RegistrarVenta(
                codigo, cantidad, cliente);

            GestorArchivos.Guardar(inventario);

            Console.WriteLine("Venta registrada correctamente.");
            Console.WriteLine($"Total a cobrar: ${total:F2}");
            Console.WriteLine($"Stock restante: {producto.Stock}");
        }

        // ==========================================
        // 8. CONSULTAR KARDEX
        // ==========================================

        static void ConsultarKardex()
        {
            Console.WriteLine("\n--- KÁRDEX DE PRODUCTO ---");

            string codigo = LeerTexto("Código del producto: ");

            inventario.MostrarKardex(codigo);
        }

        // ==========================================
        // 9. REPORTE DE VENTAS
        // ==========================================

        static void ReporteVentas()
        {
            Console.WriteLine("\n--- REPORTE DE VENTAS ---");

            DateTime desde = LeerFecha(
                "Fecha inicial (dd/MM/yyyy): ");

            DateTime hasta = LeerFecha(
                "Fecha final (dd/MM/yyyy): ");

            inventario.ReporteVentas(desde, hasta);
        }

        // ==========================================
        // MÉTODOS AUXILIARES
        // ==========================================

        static string LeerTexto(string mensaje)
        {
            Console.Write(mensaje);
            return Console.ReadLine()?.Trim() ?? "";
        }

        static int LeerEntero(string mensaje)
        {
            int numero;

            while (true)
            {
                Console.Write(mensaje);

                if (int.TryParse(Console.ReadLine(), out numero))
                    return numero;

                Console.WriteLine("Ingrese un número entero válido.");
            }
        }

        static decimal LeerDecimal(string mensaje)
        {
            decimal numero;

            while (true)
            {
                Console.Write(mensaje);

                if (decimal.TryParse(
                    Console.ReadLine(),
                    NumberStyles.Number,
                    CultureInfo.CurrentCulture,
                    out numero))
                    return numero;

                Console.WriteLine("Ingrese un número decimal válido.");
            }
        }

        static DateTime LeerFecha(string mensaje)
        {
            DateTime fecha;

            while (true)
            {
                Console.Write(mensaje);

                if (DateTime.TryParseExact(
                    Console.ReadLine(),
                    "dd/MM/yyyy",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out fecha))
                    return fecha;

                Console.WriteLine(
                    "Fecha inválida. Use dd/MM/yyyy.");
            }
        }
    }
}