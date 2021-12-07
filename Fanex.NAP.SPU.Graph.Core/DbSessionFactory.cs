using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Fanex.NAP.SPU.Graph.Core
{
    public class DbSessionFactory
    {
        private static IDictionary<string, string> configs = new Dictionary<string, string>();

        private static IDictionary<string, GraphSetting> settings = new Dictionary<string, GraphSetting>();

        private static IDriverFactory driverFactory;

        private static IDictionary<Type, ICommandBuilder> builders = new Dictionary<Type, ICommandBuilder>();

        private static IDictionary<Type, dynamic> excutors = new Dictionary<Type, dynamic>();

        private static IDictionary<string, (Type type, dynamic driver)> drivers;

        public DbSessionFactory RegisterConfigs(params string[] keys)
        {
            foreach (var key in keys)
            {
                configs.Add(key, ConfigurationManager.AppSettings[key]);
            }

            return this;
        }

        public DbSessionFactory RegisterXmlFile(string dir, string fileName)
        {
            var url = Path.Combine(Utilities.ResolvePath(dir), fileName);

            ReadXmlConfig(url);

            return this;
        }

        public DbSessionFactory RegisterXmlFile(string dir)
        {
            var files = Directory.GetFiles(Utilities.ResolvePath(dir), "*.xml", SearchOption.AllDirectories);

            foreach (var file in files)
            {
                ReadXmlConfig(file);
            }

            return this;
        }

        private void ReadXmlConfig(string url)
        {
            var document = XDocument.Load(url);

            foreach (var setting in document.Descendants("GraphSetting"))
            {
                var key = setting.Attribute("Key").Value;
                var spName = setting.Attribute("SpName").Value;
                var db = setting.Attribute("DbName").Value;

                if (key is null)
                {
                    throw new InvalidOperationException($"There's no Key attribute for {setting}");
                }

                if (spName is null)
                {
                    throw new InvalidOperationException($"There's no SpName attribute for {setting}");
                }

                if (db is null)
                {
                    throw new InvalidOperationException($"There's no DbName attribute for {setting}");
                }

                settings.Add(setting.Attribute("Key").Value, new GraphSetting
                {
                    DbKey = db,
                    SpKey = key,
                    SpName = spName
                });
            }
        }

        public DbSessionFactory RegisterDriverFactory(IDriverFactory factory)
        {
            driverFactory = factory;

            return this;
        }

        public DbSessionFactory RegisterCommandBuilder(ICommandBuilder commandBuilder)
        {
            var t = commandBuilder.DriverType;

            if (builders.ContainsKey(t))
            {
                builders[t] = commandBuilder;
            }
            else
            {
                builders.Add(t, commandBuilder);
            }

            return this;
        }

        public DbSessionFactory RegisterCommandExecutor<TDriver>(ICommandExecutor<TDriver> commandExecutor)
        {
            var t = typeof(TDriver);

            if (excutors.ContainsKey(t))
            {
                excutors[t] = commandExecutor;
            }
            else
            {
                excutors.Add(t, commandExecutor);
            }

            return this;
        }

        public DbSessionFactory RegisterModules<TDriver>(IDriverFactory factory, ICommandBuilder commandBuilder, ICommandExecutor<TDriver> executor)
        {
            RegisterDriverFactory(factory);
            RegisterCommandBuilder(commandBuilder);
            RegisterCommandExecutor(executor);

            return this;
        }

        public void Run()
        {
            drivers = configs.ToDictionary(kv => kv.Key, kv => driverFactory.GetDriver(kv.Value));
            configs = null;
        }

        public static DbSessionFactory Init() => new DbSessionFactory();

        public static ExecutionResult<IEnumerable<T>> Execute<T>(string key, object parameters = null)
        {
            var setting = settings[key];

            var (type, driver) = drivers[setting.DbKey];
            var builder = builders[type];
            var executor = excutors[type];

            var cmd = builder.BuildCommand(setting);

            return (ExecutionResult<IEnumerable<T>>)executor.Execute<T>(driver, cmd, parameters);
        }

        public static ExecutionResult Execute(string key, object parameters = null)
        {
            var setting = settings[key];

            var (type, driver) = drivers[setting.DbKey];
            var builder = builders[type];
            var executor = excutors[type];

            var cmd = builder.BuildCommand(setting);

            return (ExecutionResult)executor.Execute(driver, cmd, parameters);
        }

        public static ExecutionResult<T> ExecuteSingle<T>(string key, object parameters = null)
        {
            var setting = settings[key];

            var (type, driver) = drivers[setting.DbKey];
            var builder = builders[type];
            var executor = excutors[type];

            var cmd = builder.BuildCommand(setting);

            return (ExecutionResult<T>)executor.ExecuteSingle<T>(driver, cmd, parameters);
        }

        public static IEnumerable<T> Query<T>(string key, object parameters = null) => Execute<T>(key, parameters).Data;

        public static T QuerySingle<T>(string key, object parameters = null) => ExecuteSingle<T>(key, parameters).Data;
    }
}