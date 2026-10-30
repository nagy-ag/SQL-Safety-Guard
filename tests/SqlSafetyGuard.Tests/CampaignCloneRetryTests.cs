using System.Reflection;
using Microsoft.Data.SqlClient;
using SqlSafetyGuard.ServerHarness;

namespace SqlSafetyGuard.Tests;

public class CampaignCloneRetryTests
{
    [Fact]
    public void SuccessfulCloneDoesNotRetry()
    {
        var attempts = new List<bool>();
        var set = CampaignWorld.CloneWithRetry(fresh => { attempts.Add(fresh); return "set"; });
        Assert.Equal("set", set);
        Assert.Equal([false], attempts);
    }

    [Theory]
    [InlineData(596)]
    [InlineData(0, 596)]
    public void KillStateRetriesOnceWithFreshConnections(params int[] errors)
    {
        var attempts = new List<bool>();
        var failure = SqlFailure(errors);
        var set = CampaignWorld.CloneWithRetry(fresh =>
        {
            attempts.Add(fresh);
            if (!fresh) throw failure;
            return "fresh-set";
        });
        Assert.Equal("fresh-set", set);
        Assert.Equal([false, true], attempts);
    }

    [Fact]
    public void SecondKillStatePropagatesWithoutThirdAttempt()
    {
        var attempts = new List<bool>();
        var failure = SqlFailure(596);
        Assert.Same(failure, Assert.Throws<SqlException>(() => CampaignWorld.CloneWithRetry<string>(fresh =>
        {
            attempts.Add(fresh);
            throw failure;
        })));
        Assert.Equal([false, true], attempts);
    }

    [Theory]
    [InlineData(-2)]
    [InlineData(1205)]
    [InlineData(18456)]
    public void OtherSqlFailuresDoNotRetry(int number)
    {
        var attempts = 0;
        var failure = SqlFailure(number);
        Assert.Same(failure, Assert.Throws<SqlException>(() => CampaignWorld.CloneWithRetry<string>(fresh =>
        {
            attempts++;
            throw failure;
        })));
        Assert.Equal(1, attempts);
    }

    [Fact]
    public void MessageAloneDoesNotRetryNonSqlFailures()
    {
        var attempts = 0;
        var failure = new InvalidOperationException("Cannot continue the execution because the session is in the kill state");
        Assert.Same(failure, Assert.Throws<InvalidOperationException>(() => CampaignWorld.CloneWithRetry<string>(fresh =>
        {
            attempts++;
            throw failure;
        })));
        Assert.Equal(1, attempts);
    }

    [Fact]
    public void KillStateAfterCloneReturnedDoesNotRepeatCloneOrCase()
    {
        var attempts = 0;
        var executions = 0;
        var failure = SqlFailure(596);
        Assert.Same(failure, Assert.Throws<SqlException>((Action)(() =>
        {
            CampaignWorld.CloneWithRetry(fresh => { attempts++; return "set"; });
            executions++;
            throw failure;
        })));
        Assert.Equal(1, attempts);
        Assert.Equal(1, executions);
    }

    private static SqlException SqlFailure(params int[] numbers)
    {
        var errors = (SqlErrorCollection)Activator.CreateInstance(typeof(SqlErrorCollection), nonPublic: true)!;
        var constructor = typeof(SqlError).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)
            .First(candidate => candidate.GetParameters().Length == 9);
        foreach (var number in numbers)
        {
            var error = (SqlError)constructor.Invoke([number, (byte)1, (byte)21, "test-server", "test failure", "", 0, 0, null]);
            typeof(SqlErrorCollection).GetMethod("Add", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(errors, [error]);
        }
        return (SqlException)typeof(SqlException).GetMethod("CreateException", BindingFlags.Static | BindingFlags.NonPublic,
            null, [typeof(SqlErrorCollection), typeof(string)], null)!.Invoke(null, [errors, "17.0"])!;
    }
}
