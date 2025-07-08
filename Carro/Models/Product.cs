namespace Carro.Models
{
    public class Product
    {

        public int ProductId { get; set; }
        public string Nombre { get; set; }
        public int Precio { get; set; }
        public int Stock { get; set; }
        public string UrlImagen { get; set; }


        public Category? Category { get; set; }
        public int CategoryId { get; set; }
    }
}
