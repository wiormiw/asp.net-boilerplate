namespace Application.Common.Interfaces;

public interface IAppDbContext
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}