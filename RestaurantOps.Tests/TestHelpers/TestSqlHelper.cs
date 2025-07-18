using System.Data;
using Microsoft.Data.SqlClient;
using RestaurantOps.Tests.TestHelpers;

namespace RestaurantOps.Tests.TestHelpers;

public static class TestSqlHelper
{
    public static DataTable ExecuteDataTable(string sql, params SqlParameter[] parameters)
    {
        // Simple in-memory implementation for testing
        // In a real scenario, you might use a more sophisticated SQL parser
        
        var result = new DataTable();
        
        if (sql.Contains("FROM Employees"))
        {
            var employeesTable = TestDatabase.GetTable("Employees");
            result = FilterTable(employeesTable, sql, parameters);
        }
        else if (sql.Contains("FROM MenuItems"))
        {
            var menuItemsTable = TestDatabase.GetTable("MenuItems");
            result = FilterTable(menuItemsTable, sql, parameters);
        }
        else if (sql.Contains("FROM Orders"))
        {
            var ordersTable = TestDatabase.GetTable("Orders");
            result = FilterTable(ordersTable, sql, parameters);
        }
        else if (sql.Contains("FROM Tables"))
        {
            var tablesTable = TestDatabase.GetTable("Tables");
            result = FilterTable(tablesTable, sql, parameters);
        }

        return result;
    }

    public static int ExecuteNonQuery(string sql, params SqlParameter[] parameters)
    {
        // Simple implementation for INSERT/UPDATE/DELETE operations
        if (sql.ToUpper().StartsWith("INSERT"))
        {
            return HandleInsert(sql, parameters);
        }
        else if (sql.ToUpper().StartsWith("UPDATE"))
        {
            return HandleUpdate(sql, parameters);
        }
        else if (sql.ToUpper().StartsWith("DELETE"))
        {
            return HandleDelete(sql, parameters);
        }

        return 0;
    }

    private static DataTable FilterTable(DataTable sourceTable, string sql, SqlParameter[] parameters)
    {
        var result = sourceTable.Clone();
        
        // Simple filtering logic - in practice you'd want a proper SQL parser
        if (sql.Contains("WHERE"))
        {
            if (parameters.Length > 0)
            {
                var idParam = parameters.FirstOrDefault(p => p.ParameterName == "@id");
                if (idParam != null)
                {
                    var rows = sourceTable.Select($"{sourceTable.Columns[0].ColumnName} = {idParam.Value}");
                    foreach (var row in rows)
                    {
                        result.ImportRow(row);
                    }
                    return result;
                }
            }
            
            if (sql.Contains("IsActive = 1"))
            {
                var rows = sourceTable.Select("IsActive = true");
                foreach (var row in rows)
                {
                    result.ImportRow(row);
                }
                return result;
            }
        }
        
        // Return all rows if no specific filter
        foreach (DataRow row in sourceTable.Rows)
        {
            result.ImportRow(row);
        }
        
        return result;
    }

    private static int HandleInsert(string sql, SqlParameter[] parameters)
    {
        // Extract table name and handle insert
        var tableName = ExtractTableName(sql, "INTO");
        var table = TestDatabase.GetTable(tableName);
        
        if (table.Rows.Count == 0) return 1; // Simplified for now
        
        var newRow = table.NewRow();
        
        // Map parameters to columns - simplified mapping
        foreach (var param in parameters)
        {
            var columnName = MapParameterToColumn(param.ParameterName);
            if (table.Columns.Contains(columnName))
            {
                newRow[columnName] = param.Value ?? DBNull.Value;
            }
        }
        
        // Auto-generate ID if it's an identity column
        if (table.Columns[0].ColumnName.EndsWith("Id"))
        {
            var maxId = table.Rows.Count == 0 ? 0 : table.AsEnumerable().Max(r => r.Field<int>(0));
            newRow[0] = maxId + 1;
        }
        
        table.Rows.Add(newRow);
        return 1;
    }

    private static int HandleUpdate(string sql, SqlParameter[] parameters)
    {
        var tableName = ExtractTableName(sql, "UPDATE");
        var table = TestDatabase.GetTable(tableName);
        
        var idParam = parameters.FirstOrDefault(p => p.ParameterName == "@id");
        if (idParam != null)
        {
            var rows = table.Select($"{table.Columns[0].ColumnName} = {idParam.Value}");
            foreach (var row in rows)
            {
                foreach (var param in parameters.Where(p => p.ParameterName != "@id"))
                {
                    var columnName = MapParameterToColumn(param.ParameterName);
                    if (table.Columns.Contains(columnName))
                    {
                        row[columnName] = param.Value ?? DBNull.Value;
                    }
                }
            }
            return rows.Length;
        }
        
        return 0;
    }

    private static int HandleDelete(string sql, SqlParameter[] parameters)
    {
        var tableName = ExtractTableName(sql, "FROM");
        var table = TestDatabase.GetTable(tableName);
        
        var idParam = parameters.FirstOrDefault(p => p.ParameterName == "@id");
        if (idParam != null)
        {
            var rows = table.Select($"{table.Columns[0].ColumnName} = {idParam.Value}");
            foreach (var row in rows)
            {
                row.Delete();
            }
            table.AcceptChanges();
            return rows.Length;
        }
        
        return 0;
    }

    private static string ExtractTableName(string sql, string keyword)
    {
        var keywordIndex = sql.IndexOf(keyword, StringComparison.OrdinalIgnoreCase);
        if (keywordIndex == -1) return string.Empty;
        
        var startIndex = keywordIndex + keyword.Length;
        var sqlSubstring = sql.Substring(startIndex).Trim();
        var words = sqlSubstring.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        
        return words.Length > 0 ? words[0] : string.Empty;
    }

    private static string MapParameterToColumn(string parameterName)
    {
        return parameterName switch
        {
            "@fn" => "FirstName",
            "@ln" => "LastName",
            "@role" => "Role",
            "@hd" => "HireDate",
            "@act" => "IsActive",
            "@id" => "EmployeeId",
            _ => parameterName.TrimStart('@')
        };
    }
}