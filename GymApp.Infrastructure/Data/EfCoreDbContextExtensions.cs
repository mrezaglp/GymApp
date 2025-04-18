using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using GymApp.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;


public static class EfCoreDbContextExtensions
{
  private static string RegExReplace(this string source, string replace = "")
  {
           string pattern = @"[\\r|\\n|\\t]";

           // Specify your replace string value here.
           string replaceValue = replace;

            var outString = Regex.Replace(source, pattern, replaceValue);

    return outString;
  }

  public static void EnsureTables(
    this DbContext context,
    string script,
    List<CreateView> mustbeViewNames
  )
  {
    if (!string.IsNullOrEmpty(script))
    {
      try
      {
        var connection = context.Database.GetDbConnection();

        bool isConnectionClosed = connection.State == ConnectionState.Closed;

        if (isConnectionClosed)
        {
          connection.Open();
        }

        var existingTableNames = new List<string>();
        using (var command = connection.CreateCommand())
        {
          command.CommandText =
            "SELECT table_name from INFORMATION_SCHEMA.TABLES WHERE table_type = 'base table'";

          using (var reader = command.ExecuteReader())
          {
            while (reader.HasRows && reader.Read())
            {
              if (reader.VisibleFieldCount > 0)
                existingTableNames.Add(reader.GetString(0).ToLowerInvariant());
            }
          }
        }

        var split = script.Split(new[] { "CREATE TABLE " }, StringSplitOptions.RemoveEmptyEntries);
        foreach (string sql in split)
        {
          var tableName = sql.Substring(0, sql.IndexOf("(", StringComparison.OrdinalIgnoreCase));
          tableName = tableName.Split('.').Last();
          tableName = tableName.Trim().TrimStart('[').TrimEnd(']').ToLowerInvariant();

          if (existingTableNames.Contains(tableName))
          {
            continue;
          }

          try
          {
            string[] splitter = new string[] { "\r\nGO\r\n" };
            string[] commandTexts = (
              "CREATE TABLE " + sql.Substring(0, sql.LastIndexOf(";"))
            ).Split(splitter, StringSplitOptions.RemoveEmptyEntries);

            foreach (string commandText in commandTexts)
            {
              using (var createCommand = connection.CreateCommand())
              {
                createCommand.CommandText = commandText;
                createCommand.ExecuteNonQuery();
              }
            }
          }
          catch (Exception)
          {
            // Ignore
          }
        }

        var existingViewNames = new List<string>();
        using (var command = connection.CreateCommand())
        {
          command.CommandText =
            "SELECT table_name from INFORMATION_SCHEMA.TABLES WHERE table_type = 'view'";

          using (var reader = command.ExecuteReader())
          {
            while (reader.HasRows && reader.Read())
            {
              if (reader.VisibleFieldCount > 0)
                existingViewNames.Add(reader.GetString(0).ToLowerInvariant());
            }
          }
        }

        foreach (var x in mustbeViewNames)
        {
          if (existingViewNames.Contains(x.Name.ToLower()))
          {
            continue;
          }

          try
          {
            using (var createCommand = connection.CreateCommand())
            {
              createCommand.CommandText = x.Script;
              createCommand.ExecuteNonQuery();
            }
          }
          catch (Exception)
          {
            // Ignore
          }
        }

        if (isConnectionClosed)
        {
          connection.Close();
        }
                }
                catch (Exception)
                {
                   // Ignore
      }
    }
  }
}

public static class EfCoreBulkExtensions
{
#if NET7_0_OR_GREATER
  public static Expression<Func<SetPropertyCalls<T>, SetPropertyCalls<T>>>? ToBulkUpdate<T>(
    this BulkOperation<T> bulk
  )
    where T : class, IMuteEntity
  {
    if (!bulk.Bulks.Any())
      return null;
    Expression<Func<SetPropertyCalls<T>, SetPropertyCalls<T>>> executer = x =>
      x.SetProperty(bulk.Bulks.First().Getter, bulk.Bulks.First().Setter);

    foreach (var item in bulk.Bulks.Skip(1))
    {
      var previousExecuter = executer.Compile(); // Capture the current expression
      executer = x => previousExecuter.Invoke(x).SetProperty(item.Getter, item.Setter);
    }
    return executer!;
  }
#endif
}
