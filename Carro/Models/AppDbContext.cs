using Microsoft.EntityFrameworkCore;
using Carro.Models;

namespace Carro.Models
{
    public class AppDbContext : DbContext
    {

        public DbSet<Category> tblCategorias { get; set; }
        public DbSet<Product> tblProductos { get; set; }
        public DbSet<Usuario> tblUsuario { get; set; }
        public DbSet<Cliente> tblCliente { get; set; }
        public DbSet<ItemCarro> ItemsCarro { get; set; }
        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<DetallePedido> DetallePedido { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=CarroDocumentos;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False");
        }

        


    }
}
