namespace Dominio.Entidades
{
    public class RolMenuEntidad
    {
        public int RolId { get; set; }
        public int MenuId { get; set; }
        public bool PuedeVer { get; set; } = true;

        public RolEntidad Rol { get; set; } = null!;
        public MenuEntidad Menu { get; set; } = null!;
    }
}
