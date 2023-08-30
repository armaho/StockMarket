using System.Collections.Concurrent;

namespace StockMarket.Domain.Tests;
public class BlockingCollectionTests
{
    [Fact]
    public void BlockingCollection_Add_And_Take_Test()
    {
        //Arrange
        using var queue = new BlockingCollection<int>();
        int sum = 0;

        //Act
        var producer = Task.Run(() =>
        {
            for (int i = 0; i < 10; i++)
            {
                queue.Add(i);
            }

            queue.CompleteAdding();
        });

        var consumer = Task.Run(() =>
        {
            foreach (var i in queue.GetConsumingEnumerable())
            {
                sum += i;
            }
        });

        Task.WaitAll(producer, consumer);

        //Assert
        Assert.Equal(45, sum);
    }
}
