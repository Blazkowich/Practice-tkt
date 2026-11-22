using System.Reflection;
using System.Runtime.ExceptionServices;

namespace EPM.TKT.Mediator.Internal;

internal static class ReflectionUnwrap
{
    /// <summary>
    /// <see cref="Activator.CreateInstance(Type, object?[])"/> can wrap failures in <see cref="TargetInvocationException"/>.
    /// </summary>
    public static object CreateInstance(Type type, params object?[]? args)
    {
        try
        {
            return Activator.CreateInstance(type, args)!;
        }
        catch (TargetInvocationException ex) when (ex.InnerException is not null)
        {
            ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
            throw;
        }
    }
}
