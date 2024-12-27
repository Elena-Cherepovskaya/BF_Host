using System.Data;
using Newtonsoft.Json;
using Npgsql;

namespace BF_Host.Services
{
    public class IngredientService : BaseService
    {
        private List<Ingredient> DataBase
        {
            get
            {
                List<Ingredient> data = [];

                try
                {
                    DataTable table = new();

                    nc.Open();
                    var sql = "SELECT * FROM ingredient;";
                    var cmd = new NpgsqlCommand(sql, nc);
                    var reader = cmd.ExecuteReader();

                    table.Load(reader);
                    reader.Close();
                    nc.Close();

                    var json = JsonConvert.SerializeObject(table);
                    var result = JsonConvert.DeserializeObject<List<Ingredient>>(json);
                    if (result != null)
                        data.AddRange(result);
                }
                catch (Exception ex)
                {

                }
                {
                    nc.Close();
                }
                return data;
            }
        }

        public List<Ingredient> GetAll()
        {
            return DataBase;
        }

        public void Add(Ingredient value)
        {
            try
            {
                nc.Open();
                var sql = "INSERT INTO ingredient (name) VALUES (@name);\r\n";
                var cmd = new NpgsqlCommand(sql, nc);
                value.SetValues(cmd);
                cmd.ExecuteNonQuery();
                nc.Close();
            }
            catch (Exception ex)
            {
            }
            finally
            {
                nc.Close();
            }
        }

        public void Delete(Ingredient data)
        {
            try
            {
                nc.Open();
                var sql = "DELETE FROM ingredient WHERE name = @name;";
                var cmd = new NpgsqlCommand(sql, nc);
                data.SetValues(cmd);
                cmd.ExecuteNonQuery();
                nc.Close();
            }
            catch (Exception ex)
            {
            }
            finally
            {
                nc.Close();
            }

        }
    }
}