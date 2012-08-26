using System;
using System.Linq;
using System.IO;
using System.Reflection;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.Mapping.ByCode;
using NHibernate.Tool.hbm2ddl;

namespace NHPad.Testbed
{
    public static class Configurator
    {
        //TODO: Refactor this path!
        static string clientPath = @"C:\Users\m.casamento.RECUPERA\Documents\GitHub\NHPad\NHPad.Testbed\bin\x86\Debug\";
        public static ISessionFactory GetSessionFactory()
        {
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
            var config = new Configuration();
            //config.SessionFactory().Integrate.Using<SQLiteDialect>().Connected.Using("Data source=nhtest.sqlite").AutoQuoteKeywords();
            config.SessionFactory().Integrate.Using<SQLiteDialect>().Connected.Using(String.Format("Data source={0}", Path.Combine(clientPath, "nhtest.sqlite"))).AutoQuoteKeywords();
            var mapper = new ConventionModelMapper();
            Map(mapper);
            config.AddDeserializedMapping(mapper.CompileMappingForAllExplicitlyAddedEntities(), "Mappings");
            SchemaMetadataUpdater.QuoteTableAndColumns(config);
            new SchemaUpdate(config).Execute(false, true);
            return config.BuildSessionFactory();
        }

        static System.Reflection.Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            //TODO: that's rather rough, and I suppose a better approach exists 
            string assemblyFileName = args.Name.Split(',')[0];
            if (File.Exists(assemblyFileName))
                return Assembly.LoadFrom(assemblyFileName);
            var filesFound = Directory.EnumerateFiles(clientPath, assemblyFileName + ".dll", SearchOption.AllDirectories);
            //TODO: Handle procecessor architecture (x86/x64)
            if (filesFound.Count() > 0)
                return Assembly.LoadFrom(filesFound.First());
            return null;
        }

        static void Map(ConventionModelMapper mapper)
        {
            mapper.Class<Blog>(cm => { });
            mapper.Class<Post>(cm => { });
        }
    }
}