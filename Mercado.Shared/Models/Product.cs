using System.Collections.Generic;
using System.Threading.Tasks;

using Mercado.DataAccess;

namespace Mercado.Shared.Models;

public record class Product(int Id, string Name, long? Code, decimal Price) {
    public static async Task<(int row, int error)> Create(Product product) {
        Microsoft.Data.Sqlite.SqliteCommand cmd = DBC.I.DbConn.CreateCommand();
        cmd.CommandText = "INSERT INTO product (code, name, price) VALUES ($code, upper($name), $price);";
        cmd.Parameters.AddWithValue("$code", product.Code);
        cmd.Parameters.AddWithValue("$name", product.Name);
        cmd.Parameters.AddWithValue("$price", product.Price);

        try { return (await cmd.ExecuteNonQueryAsync(), 0); }
        catch (System.Data.Common.DbException e) { System.Console.WriteLine($"{e.ErrorCode}: {e.Message}"); return (-1, e.ErrorCode); }
    }
    public static async Task<(int row, int error)> Update(Product product) {
        Microsoft.Data.Sqlite.SqliteCommand cmd = DBC.I.DbConn.CreateCommand();
        cmd.CommandText = "UPDATE product SET code=$code, name=upper($name), price=$price WHERE id=$id;";
        cmd.Parameters.AddWithValue("$id", product.Id);
        cmd.Parameters.AddWithValue("$code", product.Code);
        cmd.Parameters.AddWithValue("$name", product.Name);
        cmd.Parameters.AddWithValue("$price", product.Price);

        try { return (await cmd.ExecuteNonQueryAsync(), 0); }
        catch (System.Data.Common.DbException e) { System.Console.WriteLine($"{e.ErrorCode}: {e.Message}"); return (-1, e.ErrorCode); }
    }
    public static async Task<(int row, int error)> Delete(int id) {
        Microsoft.Data.Sqlite.SqliteCommand cmd = DBC.I.DbConn.CreateCommand();
        cmd.CommandText = "DELETE FROM product WHERE id=$id;";
        cmd.Parameters.AddWithValue("$id", id);

        try { return (await cmd.ExecuteNonQueryAsync(), 0); }
        catch (System.Data.Common.DbException e) { System.Console.WriteLine($"{e.ErrorCode}: {e.Message}"); return (-1, e.ErrorCode); }
    }

    public override int GetHashCode() => Id.GetHashCode();
    bool System.IEquatable<Product>.Equals(Product product) {
        return this.Id == product.Id;
    }

    public static IEnumerable<Product> Read(uint lastId, uint limit) {
        Microsoft.Data.Sqlite.SqliteCommand cmd = DBC.I.DbConn.CreateCommand();
        cmd.CommandText = @"SELECT id, code, name, price FROM product
                            WHERE id > $lastId
                            ORDER BY id
                            LIMIT 0,$limit;";
        cmd.Parameters.AddWithValue("$lastId", lastId);
        cmd.Parameters.AddWithValue("$limit", limit);

        Microsoft.Data.Sqlite.SqliteDataReader reader = cmd.ExecuteReader();

        List<Product> products = [];
        while (reader.Read()) {
            products.Add(new Product(
                Id: reader.GetInt32(0),
                Code: reader.GetInt64(1),
                Name: reader.GetString(2),
                Price: reader.GetDecimal(3)
            ));
        }
        return products;
    }

    public static IEnumerable<Product> Search(long code) {
        Microsoft.Data.Sqlite.SqliteCommand cmd = DBC.I.DbConn.CreateCommand();
        cmd.CommandText = @"SELECT id, code, name, price FROM product
                            WHERE instr(code, $code) > 0
                            LIMIT 10;";
        cmd.Parameters.AddWithValue("$code", code);

        Microsoft.Data.Sqlite.SqliteDataReader reader = cmd.ExecuteReader();
        
        List<Product> products = [];
        while (reader.Read()) {
            products.Add(new Product(
                Id: reader.GetInt32(0),
                Code: reader.GetInt64(1),
                Name: reader.GetString(2),
                Price: reader.GetDecimal(3)
            ));
        }
        return products;
    }
    public static IEnumerable<Product> Search(string name, uint offset = 0, uint limit = 10) {
        Microsoft.Data.Sqlite.SqliteCommand cmd = DBC.I.DbConn.CreateCommand();
        cmd.CommandText = @"SELECT id, code, name, price FROM product
                            WHERE instr(name, upper($name)) > 0 AND id > $offset
                            ORDER BY id
                            LIMIT $limit;";
        cmd.Parameters.AddWithValue("$name", name);
        cmd.Parameters.AddWithValue("$offset", offset);
        cmd.Parameters.AddWithValue("$limit", limit);

        Microsoft.Data.Sqlite.SqliteDataReader reader = cmd.ExecuteReader();

        List<Product> products = [];
        while (reader.Read()) {
            products.Add(new Product(
                Id: reader.GetInt32(0),
                Code: reader.GetInt64(1),
                Name: reader.GetString(2),
                Price: reader.GetDecimal(3)
            ));
        }
        return products;
    }
}