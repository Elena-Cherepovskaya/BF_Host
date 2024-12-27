using System.Data;
using Newtonsoft.Json;
using Npgsql;

namespace BF_Host.Services
{
    public class StorageProductService : BaseService
    {
        private const string SQL_get_from_product = "CREATE OR REPLACE FUNCTION get_product_id(_name VARCHAR(50)) RETURNS BIGINT AS $$\r\n    DECLARE _id BIGINT;\r\nBEGIN\r\n    SELECT id INTO _id\r\n    FROM product\r\n    WHERE name IN (_name);\r\n\r\n    RETURN _id;\r\nEND;\r\n$$ LANGUAGE plpgsql;";
        private const string SQL_get_from_storage = "CREATE OR REPLACE FUNCTION get_product_id_from_storage(_name VARCHAR(50)) RETURNS BIGINT AS $$\r\n    DECLARE _id BIGINT;\r\n            _product_id BIGINT;\r\nBEGIN\r\n    _id = get_product_id(_name);\r\n\r\n    IF (_id ISNULL) THEN\r\n        RETURN NULL;\r\n    END IF;\r\n\r\n    _product_id = _id;\r\n\r\n    SELECT product_id INTO _id\r\n    FROM storage_product\r\n    WHERE product_id IN (_product_id);\r\n\r\n    RETURN _id;\r\nEND;\r\n$$ LANGUAGE plpgsql;";

        private const string SQL_set = "CREATE OR REPLACE PROCEDURE set_product_to_storage(_name VARCHAR(50), _quantity INT)\r\nLANGUAGE plpgsql AS $$\r\n    DECLARE _id_in_product BIGINT;\r\n            _id_in_storage BIGINT;\r\nBEGIN\r\n    _id_in_product = get_product_id(_name);\r\n\r\n    IF (_id_in_product ISNULL) THEN\r\n        INSERT INTO product (name) VALUES (_name);\r\n\r\n        _id_in_product = get_product_id(_name);\r\n    END IF;\r\n\r\n    _id_in_storage = get_product_id_from_storage(_name);\r\n\r\n    IF (_id_in_storage ISNULL) THEN\r\n        BEGIN\r\n\r\n            INSERT INTO storage_product (product_id, quantity) VALUES (_id_in_product, _quantity);\r\n        END;\r\n    ELSE\r\n        BEGIN\r\n            UPDATE storage_product\r\n            SET quantity = _quantity\r\n            WHERE product_id = _id_in_product;\r\n        END;\r\n    END IF;\r\nEND;\r\n$$;\r\n";
        private const string SQL_add = "CREATE OR REPLACE PROCEDURE add_product_to_storage(_name VARCHAR(50), _quantity INT)\r\nLANGUAGE plpgsql AS $$\r\n    DECLARE _id_in_product BIGINT;\r\n            _id_in_storage BIGINT;\r\nBEGIN\r\n    _id_in_product = get_product_id(_name);\r\n\r\n    IF (_id_in_product ISNULL) THEN\r\n        INSERT INTO product (name) VALUES (_name);\r\n\r\n        _id_in_product = get_product_id(_name);\r\n    END IF;\r\n\r\n    _id_in_storage = get_product_id_from_storage(_name);\r\n\r\n    IF (_id_in_product ISNULL) THEN\r\n        BEGIN\r\n            INSERT INTO storage_product (product_id, quantity) VALUES (_id_in_product, _quantity);\r\n        END;\r\n    ELSE\r\n        BEGIN\r\n            UPDATE storage_product\r\n            SET quantity = (quantity + _quantity)\r\n            WHERE product_id = _id_in_product;\r\n        END;\r\n    END IF;\r\nEND;\r\n$$;\r\n";
        private const string SQL_delete = "CREATE OR REPLACE PROCEDURE delete_product_to_storage(_name VARCHAR(50), _quantity INT)\r\nLANGUAGE plpgsql AS $$\r\n    DECLARE _id BIGINT;\r\nBEGIN\r\n    SELECT id INTO _id\r\n    FROM product\r\n    WHERE name IN (_name);\r\n\r\n    IF (_id IS NOT NULL) THEN\r\n        BEGIN\r\n            UPDATE storage_product\r\n            SET quantity = (quantity - _quantity)\r\n            WHERE id = _id;\r\n        END;\r\n    END IF;\r\nEND;\r\n$$;";

        private readonly List<string> initArray = [
            SQL_get_from_product, SQL_get_from_storage,
        SQL_set, SQL_add, SQL_delete];

        private List<StorageProduct> DataBase
        {
            get
            {
                List<StorageProduct> data = [];

                try
                {
                    DataTable table = new();
                    //Открываем соединение.
                    nc.Open();
                    var sql = "SELECT storage_product.id, product.name, quantity FROM storage_product, product WHERE storage_product.product_id = product.id";
                    var cmd = new NpgsqlCommand(sql, nc);
                    var reader = cmd.ExecuteReader();
                    table.Load(reader);
                    reader.Close();
                    nc.Close();
                    var json = JsonConvert.SerializeObject(table);
                    var result = JsonConvert.DeserializeObject<List<StorageProduct>>(json);
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

        public StorageProductService()
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
                    //Код обработки ошибок
                }
                finally
                {
                    nc.Close();
                }
            }
        }


        public List<StorageProduct> GetAll()
        {
            return DataBase;
        }

        private void Call(string function_name, StorageProduct data)
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

        public void Set(StorageProduct data)
        {
            Call("set_product_to_storage", data);
        }

        public void Add(StorageProduct data)
        {
            Call("add_product_to_storage", data);
        }

        public void Delete(StorageProduct data)
        {
            Call("delete_product_to_storage", data);
        }
    }
}