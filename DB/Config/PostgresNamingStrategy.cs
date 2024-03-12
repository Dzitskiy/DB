using NHibernate.Cfg;
using System;

namespace DB1.Config
{
    internal class PostgresNamingStrategy : INamingStrategy
    {
        public string ClassToTableName(string className)
        {
            return EntitiesToDbNamesConverter.Convert(className);
        }

        public string PropertyToColumnName(string propertyName)
        {
            return EntitiesToDbNamesConverter.Convert(propertyName);
        }

        public string TableName(string tableName)
        {
            return EntitiesToDbNamesConverter.Convert(tableName);
        }

        public string ColumnName(string columnName)
        {
            return EntitiesToDbNamesConverter.Convert(columnName);
        }

        public string PropertyToTableName(string className, string propertyName)
        {
            return propertyName;
        }

        public string LogicalColumnName(string columnName, string propertyName)
        {
            return String.IsNullOrWhiteSpace(columnName) ? propertyName : columnName;
        }
    }
}
