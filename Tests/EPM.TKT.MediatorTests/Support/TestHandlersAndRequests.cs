using EPM.TKT.Mediator.Abstractions;

namespace EPM.TKT.MediatorTests.Support;

/// <summary>Anchor for <c>AddMediator&lt;T&gt;</c> — same assembly as test handlers.</summary>
public sealed class MediatorTestAssemblyAnchor { }

public sealed record EchoRequest(string Message) : IRequest<string>;

public sealed class EchoHandler : IRequestHandler<EchoRequest, string>
{
    public Task<string> HandleAsync(EchoRequest request, CancellationToken cancellationToken) =>
        Task.FromResult(request.Message);
}

public sealed record SumRequest(int A, int B) : IRequest<int>;

public sealed class SumHandler : IRequestHandler<SumRequest, int>
{
    public Task<int> HandleAsync(SumRequest request, CancellationToken cancellationToken) =>
        Task.FromResult(request.A + request.B);
}

public sealed record AlphaRequest : IRequest<int>;

public sealed record BetaRequest : IRequest<int>;

/// <summary>Single class implementing two typed request handlers.</summary>
public sealed class DualTypedHandler : IRequestHandler<AlphaRequest, int>, IRequestHandler<BetaRequest, int>
{
    public Task<int> HandleAsync(AlphaRequest request, CancellationToken cancellationToken) => Task.FromResult(10);

    public Task<int> HandleAsync(BetaRequest request, CancellationToken cancellationToken) => Task.FromResult(20);
}

public sealed record LogCommand(string Payload) : IRequest;

public sealed class LogCommandHandler : IRequestHandler<LogCommand>
{
    internal static int InvocationCount;

    public Task HandleAsync(LogCommand request, CancellationToken cancellationToken)
    {
        Interlocked.Increment(ref InvocationCount);
        return Task.CompletedTask;
    }
}

public sealed record ExplodingRequest : IRequest<string>;

public sealed class ExplodingHandler : IRequestHandler<ExplodingRequest, string>
{
    public Task<string> HandleAsync(ExplodingRequest request, CancellationToken cancellationToken) =>
        throw new InvalidOperationException("handler failure");
}

/// <summary>Request type with no handler registered in this assembly.</summary>
public sealed record UnhandledRequest : IRequest<string>;
