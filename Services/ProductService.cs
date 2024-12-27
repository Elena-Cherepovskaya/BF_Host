using System.Data;
using Newtonsoft.Json;
using Npgsql;

namespace BF_Host.Services
{
    public class ProductService : BaseService
    {
        private List<Product> DataBase
        {
            get
            {
                List<Product> data = [];

                try
                {
                    DataTable table = new();
                    //Открываем соединение.
                    nc.Open();
                    var sql = "SELECT * FROM product;";
                    var cmd = new NpgsqlCommand(sql, nc);
                    var reader = cmd.ExecuteReader();
                    table.Load(reader);
                    reader.Close();
                    nc.Close();
                    var json = JsonConvert.SerializeObject(table);
                    var result = JsonConvert.DeserializeObject<List<Product>>(json);
                    if (result != null)
                        data.AddRange(result);
                }
                catch (Exception ex)
                {
                    //Код обработки ошибок
                }
                {
                    nc.Close();
                }
                return data;
            }
        }

        public List<Product> GetAll()
        {
            return DataBase;
        }

        public void Add(Product value)
        {
            try
            {
                nc.Open();
                var sql = "INSERT INTO product (name) VALUES (@name);\r\n";
                var cmd = new NpgsqlCommand(sql, nc);
                value.SetValues(cmd);
                cmd.ExecuteNonQuery();
                nc.Close();
            }
            catch (Exception ex)
            {
                //Код обработки ошибок
            }
            finally
            {
                nc.Close();
            }
        }

        public void Delete(Product data)
        {
            try
            {
                nc.Open();
                var sql = "DELETE FROM product WHERE name = @name;";
                var cmd = new NpgsqlCommand(sql, nc);
                data.SetValues(cmd);
                cmd.ExecuteNonQuery();
                nc.Close();
            }
            catch (Exception ex)
            {
                //Код обработки ошибок
            }
            finally
            {
                nc.Close();
            }

        }
    }
}