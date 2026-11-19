namespace Dominio.Entidades
{
    public class MenuEntidad
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string Icono { get; set; } = string.Empty;
        public string Nivel { get; set; } = "1";
        public int? MenuPadreId { get; set; }
        public bool Activo { get; set; } = true;

        public MenuEntidad? MenuPadre { get; set; }
        public ICollection<MenuEntidad> SubMenus { get; set; }
            = new List<MenuEntidad>();

        public ICollection<RolMenuEntidad> RolesMenu { get; set; }
            = new List<RolMenuEntidad>();
    }
}
