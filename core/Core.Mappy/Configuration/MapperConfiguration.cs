using System;

namespace Core.Mappy.Configuration;

public class MapperConfiguration<TSource, TDestination>
{
    private readonly Dictionary<string, Func<TSource, object>> _mappings = new();
    internal Dictionary<string, Func<TSource, object>> GetMappings() => _mappings;


}
