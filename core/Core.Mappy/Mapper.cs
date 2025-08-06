using System;
using System.Collections;
using System.Reflection;
using Core.Mappy.Configuration;
using Core.Mappy.Interfaces;

namespace Core.Mappy;

public class Mapper : IMapper
{
    private readonly Dictionary<(Type, Type), object> _Configurations = new();
    public void CreateMap<TSource, TDestination>()
    {
        throw new NotImplementedException();
    }

    public void CreateMap<TSource, TDestination>(
        Action<MapperConfiguration<TSource,
        TDestination>> configure
        )
    {
        var config = new MapperConfiguration<TSource, TDestination>();
        configure(config);

        var key = (typeof(TSource), typeof(TDestination));
        _Configurations[key] = config;

    }
    public TDestination Map<TDestination>(object source)
    {
        if (source is null) return default!;

        var sourceType = source.GetType();
        var destinationType = typeof(TDestination);

        if (IsCollectionType(destinationType))
        {
            return MappCollection<TDestination>(source);
        }

        var key = (sourceType, destinationType);
        var destination = Activator.CreateInstance<TDestination>();

        if (_Configurations.TryGetValue(key, out var config))
        {
            ApplyCustomMapping(source, destination, config);
        }
        else
        {
            ApplyAutoMapping(source, destination);
        }
        
        return destination!;

    }

    private void ApplyCustomMapping<TDestination>(object source, TDestination destination, object config)
    {
        var genericMethod = typeof(Mapper)
                .GetMethod(nameof(ApplyMappings), BindingFlags.NonPublic | BindingFlags.Instance)
                .MakeGenericMethod(source.GetType(), typeof(TDestination));
        genericMethod.Invoke(this, new[] { source, destination, config });
    }

    private TDestination MappCollection<TDestination>(object source)
    {
        var sourceType = source.GetType();
        var destinationType = typeof(TDestination);

        var sourceElementType = sourceType.IsArray ?
                sourceType.GetElementType() :
                sourceType.GetGenericArguments()[0];

        var destElementType = destinationType.IsGenericType ?
                destinationType.GetGenericArguments()[0] :
                destinationType.GetElementType();

        var sourceList = ((IEnumerable)source).Cast<object>().ToList();

        if (destElementType is null)
        {
            throw new InvalidOperationException("Destination element type cannot be null.");
        }

        var destList = (IList)Activator.CreateInstance(typeof(List<>)
                            .MakeGenericType(destElementType));

        if (sourceElementType is null)
        {
            throw new InvalidOperationException("Source element type cannot be null.");
        }

        var elementMappingKey = (sourceElementType, destElementType);
        var hasElementConfig = _Configurations
                    .TryGetValue(elementMappingKey, out var elementConfig);

        foreach (var item in sourceList)
        {
            var mappedItem = hasElementConfig
            ? MapWithConfig(item, destElementType, elementConfig!)
            : MapWithoutConfig(item, destElementType);

            destList.Add(mappedItem);
        }
        if (destinationType == typeof(List<>).MakeGenericType(destElementType) ||
            destinationType == typeof(IList<>).MakeGenericType(destElementType))
        {
            return (TDestination)destList;
        }

        if (destinationType == typeof(IEnumerable<>).MakeGenericType(destElementType))
        {
            return (TDestination)(IEnumerable)destList;
        }

        if (destinationType.IsArray)
        {
            var array = Array.CreateInstance(destElementType, destList.Count);
            destList.CopyTo(array, 0);
            return (TDestination)(object)array;
        }

        throw new NotSupportedException(
            $"Destination collection type {destinationType.Name} is not supported."
        );

    }

    private object MapWithConfig(object source, Type destElementType, object config)
    {
        var destination = Activator.CreateInstance(destElementType);
        var sourceType = source.GetType();

        var applyMappings = typeof(Mapper)
                .GetMethod(nameof(ApplyMappings), BindingFlags.NonPublic | BindingFlags.Instance)
                .MakeGenericMethod(sourceType, destElementType);

        applyMappings.Invoke(this, new[] { source, config });
        return destination!;
    }

    private void ApplyMappings<TSource, TDestination>(
        TSource source,
        TDestination destination,
        MapperConfiguration<TSource, TDestination> config
    )
    {
        if (source == null || destination == null) return;

        var mappings = config.GetMappings();
        foreach (var (propertyName, sourceFunc) in mappings)
        {
            var destProperty = typeof(TDestination).GetProperty(propertyName);
            if (destProperty?.CanWrite == true)
            {
                var value = sourceFunc(source);
                if (value != null)
                {
                    destProperty.SetValue(destination, value);
                }
            }
        }
        ApplyAutoMapping(source, destination);
    }

    private object MapWithoutConfig(object item, Type destinationType)
    {
        var destination = Activator.CreateInstance(destinationType);
        var genericAutoMapMethod = typeof(Mapper)
                .GetMethod(nameof(ApplyAutoMapping), BindingFlags.NonPublic | BindingFlags.Instance)
                .MakeGenericMethod(destinationType);
        genericAutoMapMethod.Invoke(this, new[] { item, destination });
        if (destination is null)
        {
            throw new InvalidOperationException("Mapping failed: destination object.");
        }
        return destination;
    }

    private void ApplyAutoMapping<TDestination>(object source, TDestination destination)
    {
        var sourceProps = source.GetType().GetProperties();
        var destProps = typeof(TDestination).GetProperties();

        foreach (var sourceProp in sourceProps)
        {
            var destProp = destProps.FirstOrDefault(p =>
            p.Name == sourceProp.Name &&
            (p.PropertyType == sourceProp.PropertyType ||
            sourceProp.PropertyType.IsAssignableTo(p.PropertyType)));
            if (destProp?.CanWrite == true)
            {
                var value = sourceProp.GetValue(source);
                if (value != null)
                {
                    destProp.SetValue(destination, value);
                }
            }
        }
    }

    private bool IsCollectionType(Type destinationType)
    {
        return destinationType.IsGenericType && (
            destinationType.GetGenericTypeDefinition() == typeof(List<>) ||
            destinationType.GetGenericTypeDefinition() == typeof(IEnumerable<>)
        );
    }
}
