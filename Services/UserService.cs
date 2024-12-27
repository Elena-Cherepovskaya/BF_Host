using System.Data;
using Newtonsoft.Json;
using Npgsql;

namespace BF_Host.Services
{
    public class UserService : BaseService
    {
        internal class UserData : BaseData
        {
            public string name = string.Empty;
            public string password = string.Empty;
            public string rule = "Client";
        }

        private const string create_SQL = "DO\r\n$do$\r\nBEGIN\r\n    IF (SELECT to_regclass('public.login') IS NOT NULL AS table_exists) THEN\r\n    ELSE\r\n        CREATE TABLE login\r\n        (\r\n            id BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,\r\n            name VARCHAR(50),\r\n            password VARCHAR(50),\r\n            rule VARCHAR(20)\r\n        );\r\n        INSERT INTO login VALUES (default, 'admin', 'password', 'Admin');\r\n        CREATE TABLE ingredient\r\n        (\r\n            id BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,\r\n            name VARCHAR(50)\r\n        );\r\n        CREATE TABLE storage_ingredient\r\n        (\r\n        id BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,\r\n\r\n        ingredient_id BIGINT,\r\n        FOREIGN KEY (ingredient_id) REFERENCES ingredient(id) ON DELETE CASCADE,\r\n\r\n        quantity INT\r\n        );\r\n        CREATE TABLE product\r\n        (\r\n        id BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,\r\n        name VARCHAR(50)\r\n        );\r\n\r\n        CREATE TABLE storage_product\r\n        (\r\n        id BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,\r\n\r\n        product_id BIGINT,\r\n        FOREIGN KEY (product_id) REFERENCES product(id) ON DELETE CASCADE,\r\n\r\n        quantity INT\r\n        );\r\n\r\n        CREATE TABLE recipe\r\n        (\r\n            id BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,\r\n\r\n            name VARCHAR(50),\r\n\r\n            product_id BIGINT,\r\n            FOREIGN KEY (product_id) REFERENCES product(id) ON DELETE CASCADE,\r\n\r\n            product_quantity INT\r\n        );\r\n\r\n        CREATE TABLE ingredient_for_recipe\r\n        (\r\n            id BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,\r\n\r\n            recipe_id BIGINT,\r\n            FOREIGN KEY (recipe_id) REFERENCES recipe(id) ON DELETE CASCADE,\r\n\r\n            ingredient_id BIGINT,\r\n            FOREIGN KEY (ingredient_id) REFERENCES ingredient(id) ON DELETE CASCADE,\r\n\r\n            ingredient_quantity INT\r\n        );\r\n\r\n    END IF;\r\nEND\r\n$do$;\r\n";

        private List<UserData> DataBase
        {
            get
            {
                List<UserData> data = [];

                try
                {
                    DataTable table = new();
                    //Открываем соединение.
                    nc.Open();
                    var sql = create_SQL + "SELECT * FROM login;";
                    var cmd = new NpgsqlCommand(sql, nc);
                    var reader = cmd.ExecuteReader();
                    table.Load(reader);
                    reader.Close();
                    nc.Close();
                    var json = JsonConvert.SerializeObject(table);
                    var result = JsonConvert.DeserializeObject<List<UserData>>(json);
                    if (result != null)
                        data.AddRange(result);
                }
                catch (Exception ex)
                {
                    //Код обработки ошибок
                }
                finally
                {
                    nc.Close();
                }
                return data;
            }
        }


        public List<User> TryLoginUser(UserLogin login)
        {
            List<User> data = [];
            int index = DataBase.FindIndex(user => user.name == login.Name && user.password == login.Password);
            if (index != -1)
            {
                User newData = new()
                {
                    Name = DataBase[index].name,
                    Rule = DataBase[index].rule
                };
                data.Add(newData);
            }
            return data;
        }

        public List<User> GetAllUser()
        {
            List<User> res = new();
            foreach (UserData data in DataBase)
            {
                User newData = new()
                {
                    Name = data.name,
                    Rule = data.rule
                };
                res.Add(newData);
            }
            return res;
        }

        public void AddUser(UserLogin user)
        {
            UserData data = new()
            {
                name = user.Name,
                password = user.Password,
                rule = user.Rule
            };

            try
            {
                nc.Open();
                var sql = "INSERT INTO login (name, password, rule) VALUES (@name, @password, @rule);\r\n";
                var cmd = new NpgsqlCommand(sql, nc);
                data.SetValues(cmd);
                cmd.ExecuteNonQuery();
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

        public void Delete(User user)
        {
            UserData data = new()
            {
                name = user.Name
            };

            try
            {
                nc.Open();
                var sql = "DELETE FROM login WHERE name = @name;";
                var cmd = new NpgsqlCommand(sql, nc);
                data.SetValues(cmd);
                cmd.ExecuteNonQuery();
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