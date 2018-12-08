using System;
using Apache.Ignite.Core.Cache;

namespace Nop.Core.Caching
{
    public interface IIgniteConnectionWrapper : IDisposable
    {
        ICache<string, string> Cache { get; }
    }
}
