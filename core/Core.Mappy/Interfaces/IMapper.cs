using Core.Mappy.Configuration;

namespace Core.Mappy.Interfaces;

public interface IMapper

{
    void CreateMap<TSource, TDestination>();

    void CreateMap<TSource, TDestination>(
        Action<MapperConfiguration<TSource, TDestination>> configure
    );

}
