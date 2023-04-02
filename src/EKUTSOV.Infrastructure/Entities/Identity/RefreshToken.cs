namespace EKUTSOV.Infrastructure.Entities;

public class RefreshToken
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string Token { get; set; }

    public DateTime ExpiredDate { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime? RevokedDate { get; set; }

    public string ReplacedByToken { get; set; }

    public string RevokedReason { get; set; }

    public virtual User User { get; set; } = new();
}