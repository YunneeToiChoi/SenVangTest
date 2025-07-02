using OrderManagement.Application.Interfaces;
using Xunit;

namespace OrderManagement.Tests;

public class QuickTest
{
    [Fact]
    public void ICacheService_Interface_ShouldExist()
    {
        // Arrange & Act
        var type = typeof(ICacheService);
        
        // Assert
        Assert.NotNull(type);
        Assert.True(type.IsInterface);
    }
} 