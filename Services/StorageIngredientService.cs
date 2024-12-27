using System.Data;
using Newtonsoft.Json;
using Npgsql;

namespace BF_Host.Services
{
    public class StorageIngredientService : BaseService
    {
        private const string SQL_get_from_ingredient = "CREATE OR REPLACE FUNCTION get_ingredient_id(_name VARCHAR(50)) RETURNS BIGINT AS $$\r\n    DECLARE _id BIGINT;\r\nBEGIN\r\n    SELECT id INTO _id\r\n    FROM ingredient\r\n    WHERE name IN (_name);\r\n\r\n    RETURN _id;\r\nEND;\r\n$$ LANGUAGE plpgsql";
        private const string SQL_get_from_storage = "CREATE OR REPLACE FUNCTION get_ingredient_id_from_storage(_name VARCHAR(50)) RETURNS BIGINT AS $$\r\n    DECLARE _id BIGINT;\r\n            _ingredient_id BIGINT;\r\nBEGIN\r\n    _id = get_ingredient_id(_name);\r\n\r\n    IF (_id ISNULL) THEN\r\n        RETURN NULL;\r\n    END IF;\r\n\r\n    _ingredient_id = _id;\r\n\r\n    SELECT ingredient_id INTO _id\r\n    FROM storage_ingredient\r\n    WHERE ingredient_id IN (_ingredient_id);\r\n\r\n    RETURN _id;\r\nEND;\r\n$$ LANGUAGE plpgsql";

        private const string SQL_set = "CREATE OR REPLACE PROCEDURE set_ingredient_to_storage(_name VARCHAR(50), _quantity INT)\r\nLANGUAGE plpgsql AS $$\r\n    DECLARE _id_in_ingredient BIGINT;\r\n            _id_in_storage BIGINT;\r\nBEGIN\r\n    _id_in_ingredient = get_ingredient_id(_name);\r\n\r\n    IF (_id_in_ingredient ISNULL) THEN\r\n        INSERT INTO ingredient (name) VALUES (_name);\r\n\r\n        _id_in_ingredient = get_ingredient_id(_name);\r\n    END IF;\r\n\r\n    _id_in_storage = get_ingredient_id_from_storage(_name);\r\n\r\n    IF (_id_in_storage ISNULL) THEN\r\n        BEGIN\r\n\r\n            INSERT INTO storage_ingredient (ingredient_id, quantity) VALUES (_id_in_ingredient, _quantity);\r\n        END;\r\n    ELSE\r\n        BEGIN\r\n            UPDATE storage_ingredient\r\n            SET quantity = _quantity\r\n            WHERE ingredient_id = _id_in_ingredient;\r\n        END;\r\n    END IF;\r\nEND;\r\n$$;";
        private const string SQL_add = "CREATE OR REPLACE PROCEDURE add_ingredient_to_storage(_name VARCHAR(50), _quantity INT)\r\nLANGUAGE plpgsql AS $$\r\n    DECLARE _id_in_ingredient BIGINT;\r\n            _id_in_storage BIGINT;\r\nBEGIN\r\n    _id_in_ingredient = get_ingredient_id(_name);\r\n\r\n    IF (_id_in_ingredient ISNULL) THEN\r\n        INSERT INTO ingredient (name) VALUES (_name);\r\n\r\n        _id_in_ingredient = get_ingredient_id(_name);\r\n    END IF;\r\n\r\n    _id_in_storage = get_ingredient_id_from_storage(_name);\r\n\r\n    IF (_id_in_storage ISNULL) THEN\r\n        BEGIN\r\n            INSERT INTO storage_ingredient (ingredient_id, quantity) VALUES (_id_in_ingredient, _quantity);\r\n        END;\r\n    ELSE\r\n        BEGIN\r\n            UPDATE storage_ingredient\r\n            SET quantity = (quantity + _quantity)\r\n            WHERE ingredient_id = _id_in_ingredient;\r\n        END;\r\n    END IF;\r\nEND;\r\n$$;";
        private const string SQL_delete = "CREATE OR REPLACE PROCEDURE delete_ingredient_to_storage(_name VARCHAR(50), _quantity INT)\r\nLANGUAGE plpgsql AS $$\r\n    DECLARE _id BIGINT;\r\nBEGIN\r\n    SELECT id INTO _id\r\n    FROM ingredient\r\n    WHERE name IN (_name);\r\n\r\n    IF (_id IS NOT NULL) THEN\r\n        BEGIN\r\n            UPDATE storage_ingredient\r\n            SET quantity = (quantity - _quantity)\r\n            WHERE id = _id;\r\n        END;\r\n    END IF;\r\nEND;\r\n$$;";

        private readonly List<string> initArray = [
            SQL_get_from_ingredient, SQL_get_from_storage,
        SQL_set, SQL_add, SQL_delete];

        private List<StorageIngredient> DataBase
        {
            get
            {
                List<StorageIngredient> data = [];

                try
                {
                    DataTable table = new();
                    //Открываем соединение.
                    nc.Open();
                    var sql = "SELECT storage_ingredient.id, ingredient.name, quantity FROM storage_ingredient, ingredient WHERE storage_ingredient.ingredient_id = ingredient.id";
                    var cmd = new NpgsqlCommand(sql, nc);
                    var reader = cmd.ExecuteReader();
                    table.Load(reader);
                    reader.Close();
                    nc.Close();
                    var json = JsonConvert.SerializeObject(table);
                    var result = JsonConvert.DeserializeObject<List<StorageIngredient>>(json);
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
        public StorageIngredientService()
        {
            foreach (var initString in initArray)
            {
                try
                {
                    nc.Open();
                    var cmd = new NpgsqlCommand(initString, nc);
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    int a = 0;
                    //Код обработки ошибок
                }
                finally
                {
                    nc.Close();
                }
            }
        }

        public List<StorageIngredient> GetAll()
        {
            return DataBase;
        }

        private void Call(string function_name, StorageIngredient data)
        {
            try
            {
                nc.Open();
                var sql = "CALL " + function_name + "(@name, @quantity);";
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
        public void Set(StorageIngredient data)
        {
            Call("set_ingredient_to_storage", data);
        }

        public void Add(StorageIngredient data)
        {
            Call("add_ingredient_to_storage", data);
        }

        public void Delete(StorageIngredient data)
        {
            Call("delete_ingredient_to_storage", data);
        }
    }
}