using System.Linq;
using Transformalize.Libs.Dapper;

namespace Transformalize.Main.Providers.MySql
{
    public class MySqlEntityExists : IEntityExists {

        private const string SQL = @"
            SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
            SELECT TABLE_NAME
            FROM information_schema.tables
            WHERE TABLE_NAME = @name 
            LIMIT 1;
            COMMIT;
        ";

        public bool Exists(AbstractConnection connection, string schema, string name) {
            using (var cn = connection.GetConnection()) {
                cn.Open();
                var table = cn.Query<string>(SQL, new { name }).DefaultIfEmpty(string.Empty).FirstOrDefault();
                return !string.IsNullOrEmpty(table);
            }
        }

    }
}