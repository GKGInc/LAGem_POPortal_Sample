using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.Configuration;
using DevExpress.Xpo;
using Microsoft.Extensions.Configuration;

namespace MasterDetail.Code
{
    public class ConnectionHelper
    {
        //static Type[] persistentTypes = new Type[]
        //{
        //    typeof(WebApiAccountItem)
        //};

        static Type[] persistentTypes = new Type[] { };
        public IConfiguration Configuration { get; }

        public static Type[] GetPersistentTypes()
        {
            Type[] copy = new Type[persistentTypes.Length];
            Array.Copy(persistentTypes, copy, persistentTypes.Length);
            return copy;
        }

        //<add name="ConnectionString" connectionString="Data Source=CTCF\SQLEXPRESS;Initial Catalog=CTCEnterprise;User id=sa;Password=gkg#52Inc;Pooling=false" />
        //public static string ConnectionString => ConfigurationManager.ConnectionStrings["CTCEnterpriseConnectionString"].ConnectionString;

        //public static string ConnectionString => Configuration.GetConnectionString("DefaultConnection"); // Did not seem to work
        public static string ConnectionString => GetConnectionString();
        public static string GetConnectionString()
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build();

            string sqlConnection = configuration.GetConnectionString("DefaultConnection");

            return sqlConnection;
        }

        public static void Connect(DevExpress.Xpo.DB.AutoCreateOption autoCreateOption, bool threadSafe = false)
        {
            if (threadSafe)
            {
                var provider = XpoDefault.GetConnectionProvider(ConnectionString, autoCreateOption);
                var dictionary = new DevExpress.Xpo.Metadata.ReflectionDictionary();
                dictionary.GetDataStoreSchema(persistentTypes);
                XpoDefault.DataLayer = new ThreadSafeDataLayer(dictionary, provider);
            }
            else
            {
                XpoDefault.DataLayer = XpoDefault.GetDataLayer(ConnectionString, autoCreateOption);
            }
            XpoDefault.Session = null;
        }

        public static DevExpress.Xpo.DB.IDataStore GetConnectionProvider(DevExpress.Xpo.DB.AutoCreateOption autoCreateOption)
        {
            return XpoDefault.GetConnectionProvider(ConnectionString, autoCreateOption);
        }
        public static DevExpress.Xpo.DB.IDataStore GetConnectionProvider(DevExpress.Xpo.DB.AutoCreateOption autoCreateOption, out IDisposable[] objectsToDisposeOnDisconnect)
        {
            return XpoDefault.GetConnectionProvider(ConnectionString, autoCreateOption, out objectsToDisposeOnDisconnect);
        }
        public static IDataLayer GetDataLayer(DevExpress.Xpo.DB.AutoCreateOption autoCreateOption)
        {
            return XpoDefault.GetDataLayer(ConnectionString, autoCreateOption);
        }
    }
}
