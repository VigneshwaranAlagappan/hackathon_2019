using Bounty.DataClasses;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Bounty.Data
{
    public class QueryBuilder
    {
        private static Regex regexName = new Regex("^[a-zA-Z_]*$");

        public static string IsExistingEmailQuery(string email) => "Select * from [User] where Email = '" + email + "'And IsActive= 'true'";

        public static string UpdateRowInTable(string tableName, Dictionary<string, object> updateColumns, List<ConditionColumn> whereConditionColumns)
        {
            var queryString = new StringBuilder();
            queryString.Append("UPDATE ");
            if (!string.IsNullOrWhiteSpace(tableName))
            {
                tableName = tableName.Trim();

                if (!regexName.IsMatch(tableName))
                {
                    throw new ArgumentException("Table name should not contain special characters");
                }

                if (tableName.Contains(" "))
                {
                    throw new ArgumentException("Table Name has whitespace");
                }

                queryString.Append("[" + tableName + "]");
                queryString.Append(" SET ");
                var counter = 0;
                foreach (var value in updateColumns)
                {
                    if (string.IsNullOrWhiteSpace(value.Key))
                    {
                        throw new ArgumentNullException("updateColumns", "The column name should not be null");
                    }

                    if (value.Key.Trim().Contains(" "))
                    {
                        throw new ArgumentException("Column Name has whitespace");
                    }

                    if (!regexName.IsMatch(value.Key))
                    {
                        throw new ArgumentException("Column name should not contain special characters");
                    }

                    queryString.Append("[" + value.Key + "]=");
                    queryString.Append(GetData(value.Value));
                    if (counter != updateColumns.Keys.Count - 1)
                    {
                        queryString.Append(",");
                    }
                    counter++;
                }
            }
            else
            {
                throw new ArgumentNullException("tableName", "The table name should not be null");
            }

            return ApplyWhereClause(queryString.ToString(), whereConditionColumns);
        }

        public static string ApplyWhereClause(string query, List<ConditionColumn> whereConditionColumns)
        {
            var queryString = new StringBuilder();

            if (!string.IsNullOrWhiteSpace(query))
            {
                queryString.Append(query);
                if (whereConditionColumns != null && whereConditionColumns.Count > 0)
                {
                    queryString.Append("  WHERE  ");
                    for (var i = 0; i < whereConditionColumns.Count; i++)
                    {
                        if (whereConditionColumns[i].Condition == Condition.IN
                            || whereConditionColumns[i].Condition == Condition.NOTIN)
                        {
                            if (whereConditionColumns[i].LogicalOperator != LogicalOperator.None && i != 0)
                            {
                                queryString.Append(" " + whereConditionColumns[i].LogicalOperator);
                            }

                            queryString.Append(
                                (!string.IsNullOrWhiteSpace(whereConditionColumns[i].TableName)
                                     ? " [" + whereConditionColumns[i].TableName + "]." : " ") + "["
                                + whereConditionColumns[i].ColumnName + "]");
                            queryString.Append(" " + GetConditionOperator(whereConditionColumns[i].Condition));
                            queryString.Append("(");
                            if (whereConditionColumns[i].Values != null)
                            {
                                for (var j = 0; j < whereConditionColumns[i].Values.Count; j++)
                                {
                                    if (j != 0)
                                    {
                                        queryString.Append(",");
                                    }

                                    queryString.Append(GetData(whereConditionColumns[i].Values[j]));
                                }
                            }

                            queryString.Append(")");
                        }
                        else
                        {
                            if (whereConditionColumns[i].LogicalOperator != LogicalOperator.None && i != 0)
                            {
                                queryString.Append(" " + whereConditionColumns[i].LogicalOperator);
                            }

                            if (!string.IsNullOrWhiteSpace(whereConditionColumns[i].TableName))
                            {
                                whereConditionColumns[i].TableName = whereConditionColumns[i].TableName.Trim();
                                if (!regexName.IsMatch(whereConditionColumns[i].TableName))
                                {
                                    throw new ArgumentException("Table name should not contain special characters");
                                }

                                if (whereConditionColumns[i].TableName.Contains(" "))
                                {
                                    throw new ArgumentException("Table Name has whitespace");
                                }
                            }

                            queryString.Append(
                                (!string.IsNullOrWhiteSpace(whereConditionColumns[i].TableName)
                                     ? " [" + whereConditionColumns[i].TableName + "]." : " ") + "["
                                + whereConditionColumns[i].ColumnName + "]");
                            queryString.Append(GetConditionOperator(whereConditionColumns[i].Condition));
                            if (whereConditionColumns[i].Condition == Condition.LIKE)
                            {
                                queryString.Append(
                                    whereConditionColumns[i].Value == DBNull.Value
                                        ? "Null"
                                        : (IsNumber(whereConditionColumns[i].Value)
                                               ? whereConditionColumns[i].Value
                                               : whereConditionColumns[i].Value == null
                                                     ? "%%" : "'%" + whereConditionColumns[i].Value + "%'"));
                            }
                            else if (whereConditionColumns[i].Condition == Condition.IS)
                            {
                                queryString.Append(
                                    whereConditionColumns[i].Value == DBNull.Value
                                        ? "Null"
                                        : (IsNumber(whereConditionColumns[i].Value)
                                               ? whereConditionColumns[i].Value
                                               : whereConditionColumns[i].Value == null
                                                     ? "NULL" : "'" + whereConditionColumns[i].Value + "'"));
                            }
                            else
                            {
                                queryString.Append(GetData(whereConditionColumns[i].Value));
                            }
                        }
                    }
                }
            }

            return queryString.ToString();
        }

        public static string AddToTable(string tableName, Dictionary<string, object> values)
        {
            if (string.IsNullOrWhiteSpace(tableName))
            {
                throw new ArgumentNullException("tableName", "Table Name is null");
            }

            tableName = tableName.Trim();

            if (!regexName.IsMatch(tableName))
            {
                throw new ArgumentException("Table name should not contain special characters");
            }

            if (tableName.Contains(" "))
            {
                throw new ArgumentException("Table Name has whitespace");
            }

            var queryString = new StringBuilder();
            queryString.Append("INSERT INTO ");
            queryString.Append("[" + tableName.Replace("'", string.Empty) + "] (");
            var columnValues = new StringBuilder();
            var counter = 0;

            if (values == null || values.Count <= 0)
            {
                throw new ArgumentNullException("values", "The Values should not be null");
            }

            foreach (var value in values)
            {
                if (string.IsNullOrWhiteSpace(value.Key))
                {
                    throw new ArgumentNullException("value", "The key field should not be null");
                }

                if (value.Key.Trim().Contains(" "))
                {
                    throw new ArgumentException("Column Name has whitespace");
                }

                if (!regexName.IsMatch(value.Key))
                {
                    throw new ArgumentException("Column name should not contain special characters");
                }

                queryString.Append("[" + value.Key + "]");
                columnValues.Append(GetData(value.Value));
                if (counter != values.Keys.Count - 1)
                {
                    queryString.Append(",");
                    columnValues.Append(",");
                }

                counter++;
            }

            queryString.Append(")");
           

            queryString.Append(" VALUES (");
            queryString.Append(columnValues);
            queryString.Append(")");
            return queryString.ToString();
        }

        public static object GetData(object value)
        {
            object data;

            if (IsNumber(value))
            {
                data = value;
            }
            else if (value == DBNull.Value)
            {
                data = "NULL";
            }
            else if (value is DateTime)
            {
                data = "CONVERT(datetime,'" + ((DateTime)value).ToString("yyyyMMdd HH:mm:ss", new CultureInfo("en-us"))
                       + "',112)";
            }
            else
            {
                data = value == null || string.IsNullOrWhiteSpace(value.ToString())
                           ? "N'" + value + "'" : "N'" + value.ToString().Replace("'", "''") + "'";
            }

            return data;
        }

        public static bool IsNumber(object value)
        {
            if (value == null)
            {
                return false;
            }

            return value is double || value is int || value is short || value is long || value is decimal;
        }


        /// <summary>
        ///     Returns mathematical operator for the given condition
        /// </summary>
        /// <param name="condition">Conditions Enum</param>
        /// <returns>Mathematical operator as string</returns>
        private static string GetConditionOperator(Condition condition)
        {
            switch (condition)
            {
                case Condition.Equals:
                    return "=";

                case Condition.GreaterThan:
                    return ">";

                case Condition.GreaterThanOrEquals:
                    return ">=";

                case Condition.LessThan:
                    return "<";

                case Condition.LessThanOrEquals:
                    return "<=";

                case Condition.NotEquals:
                    return "!=";

                case Condition.IS:
                    return " IS ";

                case Condition.IN:
                    return " IN ";

                case Condition.LIKE:
                    return " LIKE ";

                case Condition.NOTIN:
                    return " NOT IN ";

                default:
                    return string.Empty;
            }
        }

        public string SelectAllRecordsFromTable(string tableName)
        {
            return "SELECT * FROM [" + tableName.Replace("'", string.Empty) + "]";
        }

        public string SelectAllRecordsFromTable(string tableName, List<ConditionColumn> whereConditionColumns)
        {
            return ApplyWhereClause(SelectAllRecordsFromTable(tableName), whereConditionColumns);
        }
    }
}