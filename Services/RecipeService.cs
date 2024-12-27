using System.Data;
using Newtonsoft.Json;
using Npgsql;

namespace BF_Host.Services
{
    public class RecipeService : BaseService
    {
        private const string SQL_add = "CREATE OR REPLACE PROCEDURE add_recipe(_name VARCHAR(50), _product_name VARCHAR(50), _quantity INT)\r\nLANGUAGE plpgsql AS $$\r\n    DECLARE _product_id BIGINT;\r\nBEGIN\r\n\r\n    SELECT product_id INTO _product_id\r\n    FROM recipe\r\n    WHERE name IN (_name);\r\n\r\n    IF (_product_id ISNULL ) THEN\r\n        BEGIN\r\n            SELECT id INTO _product_id\r\n            FROM product\r\n            WHERE name IN (_product_name);\r\n\r\n            IF (_product_id ISNULL ) THEN\r\n                INSERT INTO product (name)\r\n                VALUES (_product_name);\r\n            END IF;\r\n\r\n            SELECT id INTO _product_id\r\n            FROM product\r\n            WHERE name IN (_product_name);\r\n\r\n            INSERT INTO recipe (name, product_id, product_quantity)\r\n            VALUES (_name, _product_id, _quantity);\r\n        END;\r\n    END IF;\r\nEND;$$;";
        private const string SQL_delete = "CREATE OR REPLACE PROCEDURE delete_recipe(_name VARCHAR(50))\r\nLANGUAGE plpgsql AS $$\r\n    DECLARE _id BIGINT;\r\nBEGIN\r\n    SELECT id INTO _id\r\n    FROM recipe\r\n    WHERE name IN (_name);\r\n\r\n    IF (_id IS NOT NULL) THEN\r\n        DELETE FROM recipe WHERE id IN (_id);\r\n    END IF;\r\nEND;\r\n$$;\r\n";
        private const string SQL_use = "CREATE OR REPLACE FUNCTION produce_product(_recipe_name VARCHAR(50), _quantity INT) RETURNS INT AS $$\r\n    DECLARE _product_id BIGINT;\r\n            _recipe_id BIGINT;\r\n            _product_name VARCHAR(50);\r\n            _i RECORD;\r\nBEGIN\r\n\r\n    SELECT recipe.id INTO _recipe_id\r\n    FROM recipe\r\n    WHERE name IN (_recipe_name);\r\n\r\n\r\n    CREATE  TEMP TABLE IF NOT EXISTS temp_table AS\r\n    SELECT ingredient_for_recipe.ingredient_id AS ingredient_id,\r\n           ingredient_for_recipe.ingredient_quantity AS quantity_for_recipe,\r\n           storage_ingredient.quantity AS quantity_on_storage\r\n    FROM ingredient_for_recipe, storage_ingredient\r\n    WHERE ingredient_for_recipe.recipe_id = _recipe_id\r\n    AND storage_ingredient.ingredient_id = ingredient_for_recipe.ingredient_id;\r\n\r\n    FOR _i IN SELECT * FROM temp_table LOOP\r\n        IF (_i.quantity_on_storage < _i.quantity_for_recipe * _quantity) THEN\r\n            RETURN 0;\r\n        END IF;\r\n    END LOOP;\r\n\r\n    FOR _i IN SELECT * FROM temp_table LOOP\r\n        UPDATE storage_ingredient\r\n        SET quantity = (quantity - _i.quantity_for_recipe * _quantity)\r\n        WHERE _i.ingredient_id = ingredient_id;\r\n    END LOOP;\r\n\r\n    SELECT recipe.product_id INTO _product_id\r\n    FROM recipe\r\n    WHERE name IN (_recipe_name);\r\n\r\n    SELECT name INTO _product_name\r\n    FROM product\r\n    WHERE id IN (_product_id);\r\n\r\n    FOR _i IN SELECT * FROM  public.recipe LOOP\r\n        UPDATE storage_product\r\n        SET quantity = (quantity + _i.product_quantity * _quantity)\r\n        WHERE _i.product_id = _product_id;\r\n    END LOOP;\r\n\r\n    RETURN 1;\r\nEND;\r\n$$ LANGUAGE plpgsql;";

        private readonly List<string> initArray = [SQL_add, SQL_delete, SQL_use];

        private List<Recipe> DataBase
        {
            get
            {
                List<Recipe> data = [];

                try
                {
                    DataTable table = new();
                    //Открываем соединение.
                    nc.Open();
                    var sql = "SELECT recipe.id, recipe.name, product.name AS product_name, recipe.product_quantity FROM recipe, product WHERE recipe.product_id = product.id;";
                    var cmd = new NpgsqlCommand(sql, nc);
                    var reader = cmd.ExecuteReader();
                    table.Load(reader);
                    reader.Close();
                    nc.Close();
                    var json = JsonConvert.SerializeObject(table);
                    var result = JsonConvert.DeserializeObject<List<Recipe>>(json);
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

        public RecipeService()
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

        public List<Recipe> GetAll()
        {
            return DataBase;
        }


        private void Call(string sql, Recipe data)
        {
            try
            {
                nc.Open();
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

        private bool GetResult(string sql, Recipe data)
        {
            bool res = false;
            try
            {
                nc.Open();
                var cmd = new NpgsqlCommand(sql, nc);
                data.SetValues(cmd);
                var obj = cmd.ExecuteScalar() as int?;
                if (obj != null)
                    res = obj.Value != 0;
            }
            catch (Exception ex)
            {
                //Код обработки ошибок
            }
            finally
            {
                nc.Close();
            }
            return res;
        }

        public void Add(Recipe data)
        {
            Call("CALL add_recipe (@name, @product_name, @product_quantity);", data);
        }

        public void Delete(Recipe data)
        {
            Call("CALL delete_recipe (@name)", data);
        }

        public bool Use(Recipe data)
        {
            return GetResult("SELECT produce_product(@name, 1)", data);
        }
    }
}