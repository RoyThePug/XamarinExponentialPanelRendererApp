namespace ExpPanelRenderer.Domain.Tests.Base;

public abstract class BaseTest
{
    [SetUp]
    public void Setup()
    {
        ServiceCollection.Initialize();
    }
}