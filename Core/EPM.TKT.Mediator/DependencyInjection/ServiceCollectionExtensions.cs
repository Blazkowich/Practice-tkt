using System.Reflection;
using EPM.TKT.Mediator.Abstractions;
using EPM.TKT.Mediator.Internal;
using EPM.TKT.Mediator.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace EPM.TKT.Mediator.DependencyInjection;

/// <summary>
/// Registers <see cref="ISender"/> and handler implementations found in the given assemblies:
/// <see cref="IRequestHandler{TRequest,TResponse}"/> (requests with a response) and
/// <see cref="IRequestHandler{TRequest}"/> (requests implementing <see cref="IRequest"/> with no response).
/// </summary>
public static class ServiceCollectionExtensions
{
    private static readonly Type RequestHandlerOpen = typeof(IRequestHandler<,>);
    private static readonly Type VoidRequestHandlerOpen = typeof(IRequestHandler<>);

    /// <summary>
    /// Scans <paramref name="assemblies"/> for concrete <c>IRequestHandler&lt;TRequest,TResponse&gt;</c> and
    /// <c>IRequestHandler&lt;TRequest&gt;</c> types, registers each handler in DI, and wires the sender.
    /// Each request type may have at most one handler (typed or void, not both).
    /// </summary>
    public static IServiceCollection AddMediator(
        this IServiceCollection services,
        params Assembly[] assemblies)
    {
        ArgumentNullException.ThrowIfNull(assemblies);
        if (assemblies.Length == 0)
        {
            throw new ArgumentException("At least one assembly is required.", nameof(assemblies));
        }

        var map = new Dictionary<Type, (Type HandlerType, Type RequestType, Type ResponseType)>();

        foreach (var assembly in assemblies)
        {
            if (assembly is null)
            {
                continue;
            }

            foreach (var type in assembly.GetTypes())
            {
                if (type is not { IsClass: true } || type.IsAbstract)
                {
                    continue;
                }

                foreach (var iface in type.GetInterfaces())
                {
                    if (!iface.IsGenericType || iface.GenericTypeArguments.Length != 2)
                    {
                        continue;
                    }

                    if (iface.GetGenericTypeDefinition() != RequestHandlerOpen)
                    {
                        continue;
                    }

                    var requestType = iface.GenericTypeArguments[0];
                    var responseType = iface.GenericTypeArguments[1];

                    if (!requestType.IsAssignableTo(typeof(IRequest<>).MakeGenericType(responseType)))
                    {
                        continue;
                    }

                    if (map.TryGetValue(requestType, out var existing))
                    {
                        throw new InvalidOperationException(
                            $"Duplicate handler for request '{requestType.FullName}': " +
                            $"'{existing.HandlerType.FullName}' and '{type.FullName}'.");
                    }

                    map[requestType] = (type, requestType, responseType);
                    services.TryAddTransient(type);
                }

                foreach (var iface in type.GetInterfaces())
                {
                    if (!iface.IsGenericType || iface.GenericTypeArguments.Length != 1)
                    {
                        continue;
                    }

                    if (iface.GetGenericTypeDefinition() != VoidRequestHandlerOpen)
                    {
                        continue;
                    }

                    var requestType = iface.GenericTypeArguments[0];
                    if (!requestType.IsAssignableTo(typeof(IRequest)))
                    {
                        continue;
                    }

                    if (map.TryGetValue(requestType, out var existing))
                    {
                        throw new InvalidOperationException(
                            $"Request '{requestType.FullName}' already has a handler " +
                            $"'{existing.HandlerType.FullName}'. Cannot add void handler '{type.FullName}'. " +
                            "Use either IRequest&lt;TResponse&gt; + IRequestHandler&lt;TRequest,TResponse&gt; " +
                            "or IRequest + IRequestHandler&lt;TRequest&gt;.");
                    }

                    map[requestType] = (type, requestType, typeof(void));
                    services.TryAddTransient(type);
                }
            }
        }

        var invokers = new Dictionary<Type, IRequestHandlerInvoker>();
        foreach (var (requestType, (handlerType, _, responseType)) in map)
        {
            IRequestHandlerInvoker invoker;
            if (responseType == typeof(void))
            {
                var invokerType = typeof(VoidRequestHandlerInvoker<>).MakeGenericType(requestType);
                invoker = (IRequestHandlerInvoker)ReflectionUnwrap.CreateInstance(invokerType, handlerType);
            }
            else
            {
                var invokerType = typeof(RequestHandlerInvoker<,>).MakeGenericType(requestType, responseType);
                invoker = (IRequestHandlerInvoker)ReflectionUnwrap.CreateInstance(invokerType, handlerType);
            }

            invokers[requestType] = invoker;
        }

        services.AddSingleton<IRequestHandlerProvider>(new RequestHandlerProvider(invokers));
        services.TryAddScoped<ISender, Sender>();

        return services;
    }

    /// <summary>
    /// Registers mediator using the assembly that contains <typeparamref name="TAnchor"/>.
    /// </summary>
    public static IServiceCollection AddMediator<TAnchor>(this IServiceCollection services)
    {
        return services.AddMediator(typeof(TAnchor).Assembly);
    }
}
