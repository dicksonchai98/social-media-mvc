using System.Text;
using Microsoft.EntityFrameworkCore;

namespace SocialMediaApp.Web.Data;

public static class DatabaseScriptRunner
{
    public static void RunDevelopmentScripts(AppDbContext db, string contentRootPath)
    {
        var scripts = new[]
        {
            "stored-procedures.sql",
            "seed.sql",
            "sample-data.sql"
        };

        var dbScriptsPath = Path.Combine(contentRootPath, "DB");
        foreach (var script in scripts)
        {
            var filePath = Path.Combine(dbScriptsPath, script);
            if (!File.Exists(filePath))
            {
                continue;
            }

            var sql = File.ReadAllText(filePath);
            foreach (var batch in SplitSqlBatches(sql))
            {
                if (!string.IsNullOrWhiteSpace(batch))
                {
                    db.Database.ExecuteSqlRaw(batch);
                }
            }
        }
    }

    private static IEnumerable<string> SplitSqlBatches(string sql)
    {
        var sb = new StringBuilder();
        using var reader = new StringReader(sql);
        string? line;
        while ((line = reader.ReadLine()) is not null)
        {
            if (line.Trim().Equals("GO", StringComparison.OrdinalIgnoreCase))
            {
                if (sb.Length > 0)
                {
                    yield return sb.ToString();
                    sb.Clear();
                }

                continue;
            }

            sb.AppendLine(line);
        }

        if (sb.Length > 0)
        {
            yield return sb.ToString();
        }
    }
}
