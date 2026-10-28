using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BDD_Taller1
{
    public class GestorArchivos
    {
        private const string Ruta = "inventario.json";

        public class ProductoDato
        {
            public string Tipo { get; set; } = "";
            public string Codigo { get; set; } = "";
            public string Nombre { get; set; } = "";
            public decimal PrecioBase { get; set; }
            public DateTime? FechaCaducidad { get; set; }
            public int? MesesGarantia { get; set; }
        }

        public class DatosInventario
        {
            public List<ProductoDato> Productos { get; set; } = new();
            public List<Movimiento> Movimientos { get; set; } = new();
        }

        public static void Guardar(Inventario inventario)
        {
            var datos = new DatosInventario();

            foreach (var p in inventario.Productos)
            {
                var dato = new ProductoDato
                {
                    Codigo = p.Codigo,
                    Nombre = p.Nombre,
                    PrecioBase = p.PrecioBase
                };

                if (p is ProductoPerecible perecible)
                {
                    dato.Tipo = "Perecible";
                    dato.FechaCaducidad = perecible.FechaCaducidad;
                }
                else if (p is ProductoNoPerecible noPerecible)
                {
                    dato.Tipo = "NoPerecible";
                    dato.MesesGarantia = noPerecible.MesesGarantia;
                }

                datos.Productos.Add(dato);
            }

            datos.Movimientos.AddRange(inventario.Movimientos);

            var opciones = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            string json = JsonSerializer.Serialize(datos, opciones);
            File.WriteAllText(Ruta, json);
            string rutaCompleta = Path.GetFullPath(Ruta);

        }

        public static Inventario Cargar()
        {
            var inventario = new Inventario();

            if (!File.Exists(Ruta))
                return inventario;

            string json = File.ReadAllText(Ruta);

            var datos = JsonSerializer.Deserialize<DatosInventario>(json)
                ?? throw new InvalidDataException("JSON inválido.");

            if (datos.Productos == null || datos.Movimientos == null)
                throw new InvalidDataException("Datos incompletos.");

            foreach (var d in datos.Productos)
            {
                Producto producto;

                if (d.Tipo == "Perecible")
                {
                    producto = new ProductoPerecible(
                        d.Codigo, d.Nombre, d.PrecioBase,
                        d.FechaCaducidad ??
                        throw new InvalidDataException("Falta caducidad."));
                }
                else if (d.Tipo == "NoPerecible")
                {
                    producto = new ProductoNoPerecible(
                        d.Codigo, d.Nombre, d.PrecioBase,
                        d.MesesGarantia ??
                        throw new InvalidDataException("Falta garantía."));
                }
                else
                {
                    throw new InvalidDataException(
                        "Tipo de producto desconocido.");
                }

                inventario.AgregarProducto(producto);
            }

            inventario.CargarMovimientos(datos.Movimientos);
            return inventario;
        }
    }
}
