using EventSourcing.Services;

namespace EventSourcing.Features;

public abstract class BaseFeatureHandler
{
    public BaseFeatureHandler(ILogger logger, IAccountService accountService)
    {
        Logger = logger;
        AccountService = accountService;
    }

    public ILogger Logger { get; }
    public IAccountService AccountService { get; }
}
