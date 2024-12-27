using System.Data;
using Newtonsoft.Json;
using Npgsql;

namespace BF_Host.Services
{
    public class IngredientForRecipeService : BaseService
    {

        private const string SQL_add = "CREATE OR REPLACE PROCEDURE add_ingredient_to_recipe(_name VARCHAR(50), _ingredient_name VARCHAR(50), _quantity INT)\r\nLANGUAGE plpgsql AS $$\r\n    DECLARE _recipe_id BIGINT;\r\n            _ingredient_id BIGINT;\r\nBEGIN\r\n\r\n--     Ищем id рецепта с таким названием\r\n    SELECT id INTO _recipe_id\r\n    FROM recipe\r\n    WHERE name IN (_name);\r\n\r\n    IF (_recipe_id IS NOT NULL ) THEN\r\n        BEGIN\r\n-- Ищем id ингредиента c таким названием\r\n            SELECT id INTO _ingredient_id\r\n            FROM ingredient\r\n            WHERE name IN (_ingredient_name);\r\n\r\n            IF (_ingredient_id ISNULL) THEN\r\n                BEGIN\r\n                    INSERT INTO ingredient (name)\r\n                    VALUES (_ingredient_name);\r\n                END;\r\n            END IF;\r\n\r\n            SELECT id INTO _ingredient_id\r\n            FROM ingredient\r\n            WHERE name IN (_ingredient_name);\r\n\r\n\r\n            IF (SELECT EXISTS (SELECT 1\r\n                                FROM ingredient_for_recipe\r\n                                WHERE recipe_id = _recipe_id\r\n                                AND ingredient_id = _ingredient_id)) THEN\r\n                BEGIN\r\n                    UPDATE ingredient_for_recipe\r\n                    SET ingredient_quantity = _quantity\r\n                    WHERE recipe_id = _recipe_id AND ingredient_id = _ingredient_id;\r\n                END;\r\n            ELSE\r\n                BEGIN\r\n                    INSERT INTO ingredient_for_recipe(recipe_id, ingredient_id, ingredient_quantity)\r\n                    VALUES (_recipe_id, _ingredient_id, _quantity);\r\n                END;\r\n            END IF;\r\n        END;\r\n    END IF;\r\n    -- Если такого рецепта даже нет(не знаем количество продуктов, которые получатся)\r\nEND;\r\n$$;";
        private const string SQL_delete = "CREATE OR REPLACE PROCEDURE delete_ingredient_to_recipe(_name VARCHAR(50), _ingredient_name VARCHAR(50))\r\nLANGUAGE plpgsql AS $$\r\n    DECLARE _recipe_id BIGINT;\r\n            _ingredient_id BIGINT;\r\nBEGIN\r\n    SELECT id INTO _recipe_id\r\n    FROM recipe\r\n    WHERE name IN (_name);\r\n\r\n    SELECT id INTO _ingredient_id\r\n    FROM ingredient\r\n    WHERE name IN (_ingredient_name);\r\n\r\n    DELETE FROM ingredient_for_recipe WHERE _recipe_id = recipe_id AND _ingredient_id = ingredient_id;\r\nEND;\r\n$$;\r\n";

        private readonly List<string> initArray = [
            SQL_add, SQL_delete
        ];

        public IngredientForRecipeService()
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


        public List<IngredientForRecipe> Get(Recipe recipe)
        {
            List<IngredientForRecipe> data = [];

            try
            {
                DataTable table = new();
                //Открываем соединение.
                nc.Open();
                var sql = "SELECT recipe.name AS recipe_name, ingredient.name AS ingredient_name, ingredient_for_recipe.ingredient_quantity AS ingredient_quantity FROM ingredient, recipe, ingredient_for_recipe WHERE recipe.name = @name  AND recipe_id =  recipe.id AND ingredient_for_recipe.ingredient_id = ingredient.id;";
                var cmd = new NpgsqlCommand(sql, nc);
                recipe.SetValues(cmd);
                var reader = cmd.ExecuteReader();
                table.Load(reader);
                reader.Close();
                nc.Close();
                var json = JsonConvert.SerializeObject(table);
                var result = JsonConvert.DeserializeObject<List<IngredientForRecipe>>(json);
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

        private void Call(string sql, IngredientForRecipe data)
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

        public void Set(IngredientForRecipe data)
        {
            var sql = "CALL add_ingredient_to_recipe(@recipe_name, @ingredient_name, @ingredient_quantity);";
            Call(sql, data);
        }

        public void Add(IngredientForRecipe data)
        {
            Set(data);
        }

        public void Delete(IngredientForRecipe data)
        {
            var sql = "CALL delete_ingredient_to_recipe(@recipe_name, @ingredient_name);";
            Call(sql, data);
        }
    }
}