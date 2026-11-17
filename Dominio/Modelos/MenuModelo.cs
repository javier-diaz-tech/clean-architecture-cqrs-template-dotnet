namespace Dominio.Modelos
{
    public class MenuModelo
    {
        public string Nombre { get; set; }
        public string Url { get; set; }
        public string Icono { get; set; }
        public string Nivel { get; set; }
        public List<MenuModelo> SubMenu { get; set; }

    }
}
