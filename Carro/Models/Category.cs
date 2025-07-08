namespace Carro.Models
{
    public class Category
    {
        public int CategoryId { get; set; }
        public string Descripcion { get; set; }



        public List<Product>? ListaProductos { get; set; }
    }
}
