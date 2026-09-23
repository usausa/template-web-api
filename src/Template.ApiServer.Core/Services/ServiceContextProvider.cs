namespace Template.ApiServer.Services;

public abstract class ServiceContextProvider
{
    public abstract ServiceContext Current { get; }
}
