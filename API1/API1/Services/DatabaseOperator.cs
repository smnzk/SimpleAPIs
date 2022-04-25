using APBD5.Models;
using System.Data.SqlClient;

namespace APBD5.Services
{
    public class DatabaseOperator : IDatabaseOperator
    {
        public async Task<Information> Post(Entry entry)
        {
            string connectionString = "Data Source=localhost;Initial Catalog=APBD;Integrated Security=True";

            using (SqlConnection con = new SqlConnection(connectionString))
            {


                await con.OpenAsync();

                bool isThereId = await CheckIfId(con, entry);

                bool isThereWarehouse = await CheckIfWarehouse(con, entry);

                if(isThereId == false || isThereWarehouse == false || entry.Amount <= 0)
                {
                    return new Information("Nie ma takiego produktu lub hurtowni", false);
                }

                if(await CheckIfOrder(con, entry) == false)
                {
                    return new Information("Nie ma takiego zamówienia", false); 
                }
                
                if(await CheckIfOrderDate(con, entry) == false)
                {
                    return new Information("Błąd w dacie zamówienia", false); 
                }

                int orderId = await GetIdOrder(con, entry);

                if(await CheckIfOrderInWarehouse(con, orderId) == true)
                {
                    return new Information("Produkt już jest w hurtowni", false); 
                }

                string time = DateTime.Now.ToString("yyyy-MM-dd h:mm:ss tt");
                SqlCommand cmd = new SqlCommand("UPDATE Ordr SET FullfilledAt = @time WHERE IdOrder = @id");
                cmd.Parameters.AddWithValue("@time", time);
                cmd.Parameters.AddWithValue("@id", orderId);

                await cmd.ExecuteNonQueryAsync();

                cmd = new SqlCommand("INSERT INTO Product_Warehouse VALUES (@idProductWarehouse, @idWarehouse, @idProduct, @idOrder, @amount, @price, @createdAt)", con);
                if (await CheckIfProductWarehouseId(con)) {
                    cmd.Parameters.AddWithValue("@idProductWarehouse", await GetMaxProductWarehouseId(con) + 1);
                } else
                {
                    cmd.Parameters.AddWithValue("@idProductWarehouse", 1);
                }
                cmd.Parameters.AddWithValue("@idWarehouse", entry.IdWarehouse);
                cmd.Parameters.AddWithValue("@idProduct", entry.IdProduct);
                cmd.Parameters.AddWithValue("@idOrder", orderId);
                cmd.Parameters.AddWithValue("@amount", entry.Amount);
                cmd.Parameters.AddWithValue("@price", await GetPrice(con, entry) * entry.Amount);
                cmd.Parameters.AddWithValue("@createdAt", DateTime.Now.ToString("yyyy-MM-dd h:mm:ss tt"));

                return new Information("Proces zrealizowany pomyślnie", true);

            }

        }

        public async Task<Information> PostWithProcedure(Entry entry)
        {
            string connectionString = "Data Source=localhost;Initial Catalog=APBD;Integrated Security=True";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                await con.OpenAsync();
                SqlCommand com = new SqlCommand("AddProductToWarehouse");
                com.Connection = con;
                com.CommandType = System.Data.CommandType.StoredProcedure;

                await com.ExecuteNonQueryAsync();

                return new Information("Proces zrealizowany pomyślnie", true);

            }
        }

        public async Task<bool> CheckIfId(SqlConnection con, Entry entry)
        {
            SqlCommand cmd = new SqlCommand("SELECT 1 FROM Product WHERE IdProduct = @id", con);
            cmd.Parameters.AddWithValue("@id", entry.IdProduct);

            SqlDataReader reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return true;
            } else
            {
                return false;
            }
        }


        public async Task<bool> CheckIfWarehouse(SqlConnection con, Entry entry)
        {
            SqlCommand cmd = new SqlCommand("SELECT 1 FROM Warehouse WHERE IdWarehouse = @id", con);
            cmd.Parameters.AddWithValue("@id", entry.IdWarehouse);

            SqlDataReader reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> CheckIfOrder(SqlConnection con, Entry entry)
        {
            SqlCommand cmd = new SqlCommand("SELECT 1 FROM Ordr WHERE IdProduct = @id AND Amount = @amount", con);
            cmd.Parameters.AddWithValue("@id", entry.IdProduct);
            cmd.Parameters.AddWithValue("@amount", entry.Amount);

            SqlDataReader reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return true;
            } else
            {
                return false; 
            }
        }

        public async Task<bool> CheckIfOrderDate(SqlConnection con, Entry entry)
        {
            SqlCommand cmd = new SqlCommand("SELECT 1 FROM Ordr WHERE IdProduct = @id AND Amount = @amount", con);
            cmd.Parameters.AddWithValue("@id", entry.IdProduct);
            cmd.Parameters.AddWithValue("@amount", entry.Amount);

            SqlDataReader reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                if (DateTime.Parse(reader["Name"].ToString()) < entry.CreatedAt){
                    return true;    
                }
            }           
            return false;   
        }

        public async Task<int> GetIdOrder(SqlConnection con, Entry entry)
        {
            SqlCommand cmd = new SqlCommand("SELECT IdOrder FROM Ordr WHERE IdProduct = @id AND Amount = @amount", con);
            cmd.Parameters.AddWithValue("@id", entry.IdProduct);
            cmd.Parameters.AddWithValue("@amount", entry.Amount);

            SqlDataReader reader = await cmd.ExecuteReaderAsync();

            int id = 0;
            while (await reader.ReadAsync())
            {
                id = int.Parse(reader["IdOrder"].ToString());
                break;
            }
            return id;
        }

        public async Task<bool> CheckIfOrderInWarehouse(SqlConnection con, int orderId)
        {
            SqlCommand cmd = new SqlCommand("SELECT 1 FROM Product_Warehouse WHERE IdOrder = @id", con);
            cmd.Parameters.AddWithValue("@id", orderId);

            SqlDataReader reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return true;
            } else
            {
                return false;
            }
        }

        public async Task<bool> CheckIfProductWarehouseId(SqlConnection con)
        {
            SqlCommand cmd = new SqlCommand("SELECT 1 FROM Product_Warehouse", con);

            SqlDataReader reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return true;
            } else
            {
                return false;
            }
        }

        public async Task<int> GetMaxProductWarehouseId(SqlConnection con)
        {
            SqlCommand cmd = new SqlCommand("SELECT MAX(IdProductWarehouse) FROM Product_Warehouse", con);

            SqlDataReader reader = await cmd.ExecuteReaderAsync();

            int id = 0;
            while (await reader.ReadAsync())
            {
                id = int.Parse(reader["IdProductWarehouse"].ToString());
                break;
            }
            return id;
        }

        public async Task<double> GetPrice(SqlConnection con, Entry entry)
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM Product WHERE IdProduct = @id", con);
            cmd.Parameters.AddWithValue("@id", entry.IdProduct);

            SqlDataReader reader = await cmd.ExecuteReaderAsync();

            double price = 0;   
            while (await reader.ReadAsync())
            {
                price = double.Parse(reader["Price"].ToString());
                break;
            }
            return price;
        }

    }
}
