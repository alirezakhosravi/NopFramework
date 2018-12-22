using System;
using Apache.Ignite.Core;
using Apache.Ignite.Core.Cache;
using Apache.Ignite.Core.Cache.Configuration;
using Apache.Ignite.Core.Configuration;
using Apache.Ignite.Core.Discovery.Tcp;
using Apache.Ignite.Core.Discovery.Tcp.Static;
using Apache.Ignite.Core.Events;
using Raveshmand.Cache.Ignite;
using Nop.Core.Configuration;

namespace Nop.Core.Caching
{
    public class IgniteConnectionWrapper : IIgniteConnectionWrapper
    {
        private readonly NopConfig _config;

        public ICache<string, string> Cache { get; private set; }
        private IIgnite _ignite;

        #region Ctor

        public IgniteConnectionWrapper(NopConfig config)
        {
            this._config = config;
        }

        #endregion

        private void Connection(NopConfig config, string cacheName)
        {
            IgniteConfig igniteConfig = new IgniteConfig
            {
                Configuration = new IgniteConfiguration
                {
                    DiscoverySpi = new TcpDiscoverySpi
                    {
                        IpFinder = new TcpDiscoveryStaticIpFinder
                        {
                            Endpoints = config.IgniteCachingConnectionString.Split(','),
                        },
                        SocketTimeout = TimeSpan.FromSeconds(0.3)
                    },
                    IncludedEventTypes = EventType.CacheAll
                }
            };

            if (config.PersistenceEnabledToIgnite)
            {
                igniteConfig.Configuration.DataStorageConfiguration = new DataStorageConfiguration
                {
                    DefaultDataRegionConfiguration = new DataRegionConfiguration
                    {
                        Name = "defaultRegion",
                        PersistenceEnabled = true
                    },
                    DataRegionConfigurations = new[]
                    {
                        new DataRegionConfiguration
                        {
                            // Persistence is off by default.
                            Name = "inMemoryRegion"
                        }
                    }
                };
                igniteConfig.Configuration.CacheConfiguration = new[]
                {
                    new CacheConfiguration
                    {
                        // Default data region has persistence enabled.
                        Name = "persistentCache"
                    },
                    new CacheConfiguration
                    {
                        Name = "inMemoryOnlyCache",
                        DataRegionName = "inMemoryRegion"
                    }
                };

                igniteConfig.SetActive = true;
            }

            Connection(igniteConfig, cacheName);
        }

        private void Connection(IgniteConfig config, string cacheName)
        {
            if(_ignite == null)
            {
                this._ignite = Ignition.Start(config.Configuration);
            }

            if(config.SetActive)
            {
                this._ignite.GetCluster().SetActive(true);
            }

            if(this.Cache == null && this._ignite.GetCluster().IsActive())
            {
                this.Cache = this._ignite.GetOrCreateCache<string, string>(cacheName);
            }
        }

        public void Dispose()
        {
            this._ignite?.Dispose();
        }
    }
}