using Microsoft.Data.Sqlite;

namespace Mercado.DataAccess;

public class DBC {
    private static DBC s_instance;
    public static DBC I {
        get {
            s_instance ??= new DBC();
            return s_instance;
        }
    }

    public SqliteConnection? DbConn { get; private set; }

    public DBC() {
        DbConn = new SqliteConnection("Data Source=mercado.sqlite;");
        DbConn.Open();

        SqliteCommand command = DbConn.CreateCommand();
        command.CommandText = @"
            CREATE TABLE IF NOT EXISTS product (
                id      INTEGER PRIMARY KEY,
                name    VARCHAR NOT NULL UNIQUE,
                code    INTEGER,
                price   REAL NOT NULL,
                UNIQUE (name, code)
            );
        ";
        command.ExecuteNonQuery();

        command = DbConn.CreateCommand();
        command.CommandText = "INSERT OR IGNORE INTO product (id, name, code, price) VALUES (1, 'OTRO', 1, 1);";
        command.ExecuteNonQuery();

        command = DbConn.CreateCommand();
        command.CommandText =  @"
            CREATE INDEX IF NOT EXISTS idx_code ON product (code);
            CREATE INDEX IF NOT EXISTS idx_name ON product (name);
        ";
        command.ExecuteNonQuery();
    }

    ~DBC() {
        DbConn?.Close();
        DbConn?.Dispose();
    }
}