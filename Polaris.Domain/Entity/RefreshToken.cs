namespace Polaris.Domain.Entity;

public partial class RefreshToken
{
    public Guid Id { get; set; }

    public Guid AuthenticationId { get; set; }
    
    public bool Used { get; set; }

    public virtual Authentication AuthenticationNavigation { get; set; } = null!;
}