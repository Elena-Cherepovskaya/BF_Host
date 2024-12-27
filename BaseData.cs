using Npgsql;

namespace BF_Host
{
    public class BaseData
    {
        public void SetValues(NpgsqlCommand cmd)
        {
            foreach (var field in GetType().GetProperties())
            {
                var name = field.Name;
                object value = field.GetValue(this);
                cmd.Parameters.AddWithValue(name, value);
            }

            foreach (var field in GetType().GetFields())
            {
                var name = field.Name;
                object value = field.GetValue(this);
                cmd.Parameters.AddWithValue(name, value);
            }
        }
    }
}