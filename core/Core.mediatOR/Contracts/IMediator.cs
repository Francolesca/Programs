namespace Core.mediatOR.Contracts;

public interface IMediator
{

    Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken);
}
