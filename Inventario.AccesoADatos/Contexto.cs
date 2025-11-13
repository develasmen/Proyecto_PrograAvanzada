using Inventario.AccesoADatos.Modelos;
using System.Data.Entity;

namespace Inventario.AccesoADatos
{
    public class Contexto : DbContext
    {
        public Contexto() : base("name=Contexto")
        {
            Database.SetInitializer<Contexto>(null);
        }

        public DbSet<ProductoAD> Productos { get; set; }

        public DbSet<ClienteAd> Clientes { get; set; }

        public DbSet<CarritoAD> Carritos { get; set; }
        public DbSet<CuponDescuentoAD> CuponesDescuento { get; set; }
    }
}
