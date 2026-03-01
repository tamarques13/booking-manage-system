using Microsoft.Extensions.Configuration;

public static class TestConfiguration
{
    public static IConfiguration Load()
    {
        return new ConfigurationBuilder()
        .SetBasePath(AppContext.BaseDirectory)
        .AddJsonFile("testConfiguration.json", optional: false)
        .Build();
    }
}