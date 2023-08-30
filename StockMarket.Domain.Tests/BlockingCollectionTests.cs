using System.Collections;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using Xunit.Abstractions;

namespace StockMarket.Domain.Tests;
public class BlockingCollectionTests
{
    private readonly ITestOutputHelper output;

    public BlockingCollectionTests(ITestOutputHelper output)
    {
        this.output = output;
    }

    private struct QueueItem
    {
        public int Data { get; private set; }
        public TaskCompletionSource<int> Completion { get; }

        public QueueItem(int data)
        {
            this.Data = data;
            Completion = new();
        }

        public void IncreaseData()
        {
            this.Data++;
        }
    }

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

    [Fact]
    public void BlockingCollection_With_TaskCompletionSource_Test()
    {
        //Arrange
        using BlockingCollection<QueueItem> queue = new();
        int sum = 0;

        //Act
        var producer = Task.Run(async () =>
        {
            for (int i = 0; i < 10; i++)
            {
                var item = new QueueItem(i);

                queue.Add(item);

                sum += await item.Completion.Task;
            }

            queue.CompleteAdding();
        });

        var consumer = Task.Run(() =>
        {
            foreach (var item in queue.GetConsumingEnumerable())
            {
                item.IncreaseData();
                item.Completion.SetResult(item.Data);
            }
        });

        Task.WaitAll(producer, consumer);

        //Assert
        Assert.Equal(55, sum);
    }
}
