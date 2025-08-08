using System.Collections;
using System.Reflection;
using Core.Mappy.Configuration;
using Core.Mappy.Interfaces;

namespace Core.Mappy;

public class Mapper : IMapper
{
    private readonly Dictionary<(Type, Type), object> _configurations = new();

    public void CreateMap<TSource, TDestination>()
    {
        _configurations[(typeof(TSource), typeof(TDestination))]
             = new MapperConfiguration<TSource, TDestination>();
    }

    public void CreateMap<TSource, TDestination>(Action<MapperConfiguration<TSource, TDestination>> configure)
    {
        // Create configuration
        var config = new MapperConfiguration<TSource, TDestination>();

        // Apply configuration
        configure(config);

        // Create key
        var key = (typeof(TSource), typeof(TDestination));

        // Store configuration
        _configurations[key] = config;
    }

    public TDestination Map<TDestination>(object source)
    {
        if (source is null)
            return default!;

        var sourceType = source.GetType();
        var destinationType = typeof(TDestination);

        // Handle collections
        if (IsCollectionType(destinationType))
        {
            return MapCollection<TDestination>(source);
        }

        var key = (sourceType, destinationType);
        var destination = Activator.CreateInstance<TDestination>();

        if (_configurations.TryGetValue(key, out var config))
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
            .GetMethod(nameof(ApplyMappings), BindingFlags.NonPublic | BindingFlags.Instance)!
            .MakeGenericMethod(source.GetType(), typeof(TDestination));

        genericMethod.Invoke(this, new[] { source, destination, config });
    }


    private TDestination MapCollection<TDestination>(object source)
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
            .MakeGenericType(destElementType))!;

        // Obtenemos la configuración para el mapeo de elementos individuales
        if (sourceElementType is null)
        {
            throw new InvalidOperationException("Source element type cannot be null.");
        }

        var elementMappingKey = (sourceElementType, destElementType);
        var hasElementConfig = _configurations
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
            $"Destination collection type {destinationType.Name} is not supported.");
    }



    private object MapWithConfig(object source, Type destinationType, object config)
    {
        var destination = Activator.CreateInstance(destinationType);
        var sourceType = source.GetType();

        var applyMappings = typeof(Mapper)
            .GetMethod(nameof(ApplyMappings), BindingFlags.NonPublic | BindingFlags.Instance)!
            .MakeGenericMethod(sourceType, destinationType);

        applyMappings.Invoke(this, new[] { source, destination, config });
        return destination!;
    }


    private void ApplyMappings<TSource, TDestination>(
       TSource source,
       TDestination destination,
       MapperConfiguration<TSource, TDestination> config)
    {
        if (source == null || destination == null) return;

        // Apply custom mappings first
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

        // Apply auto mapping for remaining properties
        ApplyAutoMapping(source, destination);
    }


    private object MapWithoutConfig(object source, Type destinationType)
    {
        var destination = Activator.CreateInstance(destinationType);
        var genericAutoMapMethod = typeof(Mapper)
            .GetMethod(nameof(ApplyAutoMapping), BindingFlags.NonPublic | BindingFlags.Instance)!
            .MakeGenericMethod(destinationType);

        genericAutoMapMethod.Invoke(this, new[] { source, destination });

        if (destination == null)
        {
            throw new InvalidOperationException("Mapping failed: destination object is null.");
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



    private bool IsCollectionType(Type type)
    {
        return type.IsGenericType && (
            type.GetGenericTypeDefinition() == typeof(List<>) ||
            type.GetGenericTypeDefinition() == typeof(IEnumerable<>));
    }


}
