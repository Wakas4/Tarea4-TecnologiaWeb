using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("cliente")]
public class Cliente
{
    [Key][Column("id_cliente")] public long IdCliente { get; set; }
    [Column("nombre"), MaxLength(100)]   public string Nombre { get; set; } = "";
    [Column("apellido"), MaxLength(100)] public string Apellido { get; set; } = "";
    [Column("ci"), MaxLength(30)]        public string Ci { get; set; } = "";
    [Column("correo"), MaxLength(150)]   public string Correo { get; set; } = "";
    [Column("telefono"), MaxLength(20)]  public string Telefono { get; set; } = "";
}