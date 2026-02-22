namespace Api.Models;

public class Usuario{
    public required int id { get; set; }
    public required string nome { get; set; }
    public required string email { get; set; }
    public required string senhaHash { get; set; } = string.Empty;

}