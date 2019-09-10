using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using StackExchange.Redis;
using StackExchange.Redis.Profiling;

namespace Senko.Framework
{
    internal class InvalidConnectionMultiplexer : IConnectionMultiplexer
    {
        public void Dispose()
        {
        }

        public void RegisterProfiler(Func<ProfilingSession> profilingSessionProvider)
        {
            throw new InvalidOperationException("No connection string given in the configuration");
        }

        public ServerCounters GetCounters()
        {
            throw new InvalidOperationException("No connection string given in the configuration");
        }

        public EndPoint[] GetEndPoints(bool configuredOnly = false)
        {
            throw new InvalidOperationException("No connection string given in the configuration");
        }

        public void Wait(Task task)
        {
            throw new InvalidOperationException("No connection string given in the configuration");
        }

        public T Wait<T>(Task<T> task)
        {
            throw new InvalidOperationException("No connection string given in the configuration");
        }

        public void WaitAll(params Task[] tasks)
        {
            throw new InvalidOperationException("No connection string given in the configuration");
        }

        public int HashSlot(RedisKey key)
        {
            throw new InvalidOperationException("No connection string given in the configuration");
        }

        public ISubscriber GetSubscriber(object asyncState = null)
        {
            throw new InvalidOperationException("No connection string given in the configuration");
        }

        public IDatabase GetDatabase(int db = -1, object asyncState = null)
        {
            throw new InvalidOperationException("No connection string given in the configuration");
        }

        public IServer GetServer(string host, int port, object asyncState = null)
        {
            throw new InvalidOperationException("No connection string given in the configuration");
        }

        public IServer GetServer(string hostAndPort, object asyncState = null)
        {
            throw new InvalidOperationException("No connection string given in the configuration");
        }

        public IServer GetServer(IPAddress host, int port)
        {
            throw new InvalidOperationException("No connection string given in the configuration");
        }

        public IServer GetServer(EndPoint endpoint, object asyncState = null)
        {
            throw new InvalidOperationException("No connection string given in the configuration");
        }

        public Task<bool> ConfigureAsync(TextWriter log = null)
        {
            throw new InvalidOperationException("No connection string given in the configuration");
        }

        public bool Configure(TextWriter log = null)
        {
            throw new InvalidOperationException("No connection string given in the configuration");
        }

        public string GetStatus()
        {
            throw new InvalidOperationException("No connection string given in the configuration");
        }

        public void GetStatus(TextWriter log)
        {
            throw new InvalidOperationException("No connection string given in the configuration");
        }

        public void Close(bool allowCommandsToComplete = true)
        {
            throw new InvalidOperationException("No connection string given in the configuration");
        }

        public Task CloseAsync(bool allowCommandsToComplete = true)
        {
            throw new InvalidOperationException("No connection string given in the configuration");
        }

        public string GetStormLog()
        {
            throw new InvalidOperationException("No connection string given in the configuration");
        }

        public void ResetStormLog()
        {
            throw new InvalidOperationException("No connection string given in the configuration");
        }

        public long PublishReconfigure(CommandFlags flags = CommandFlags.None)
        {
            throw new InvalidOperationException("No connection string given in the configuration");
        }

        public Task<long> PublishReconfigureAsync(CommandFlags flags = CommandFlags.None)
        {
            throw new InvalidOperationException("No connection string given in the configuration");
        }

        public int GetHashSlot(RedisKey key)
        {
            throw new InvalidOperationException("No connection string given in the configuration");
        }

        public void ExportConfiguration(Stream destination, ExportOptions options = ExportOptions.All)
        {
            throw new InvalidOperationException("No connection string given in the configuration");
        }

        public string ClientName => null;

        public string Configuration => null;

        public int TimeoutMilliseconds => 0;

        public long OperationCount => 0;

        public bool PreserveAsyncOrder { get; set; }

        public bool IsConnected => false;

        public bool IsConnecting => false;

        public bool IncludeDetailInExceptions { get; set; }

        public int StormLogThreshold { get; set; }

#pragma warning disable 67
        public event EventHandler<RedisErrorEventArgs> ErrorMessage;
        public event EventHandler<ConnectionFailedEventArgs> ConnectionFailed;
        public event EventHandler<InternalErrorEventArgs> InternalError;
        public event EventHandler<ConnectionFailedEventArgs> ConnectionRestored;
        public event EventHandler<EndPointEventArgs> ConfigurationChanged;
        public event EventHandler<EndPointEventArgs> ConfigurationChangedBroadcast;
        public event EventHandler<HashSlotMovedEventArgs> HashSlotMoved;
#pragma warning restore 67
    }
}