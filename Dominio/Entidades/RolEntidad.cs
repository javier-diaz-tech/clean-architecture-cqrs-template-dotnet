namespace Dominio.Entidades
{
    public class RolEntidad
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public bool Activo { get; set; } = true;

        public ICollection<RolMenuEntidad> Menus { get; set; }
            = new List<RolMenuEntidad>();
    }
}
