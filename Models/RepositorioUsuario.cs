using MySql.Data.MySqlClient;

namespace Inmobiliaria_Avgustin.Models
{
    public class RepositorioUsuario : RepositorioBase, IRepositorioUsuario
    {
        public RepositorioUsuario(IConfiguration configuration) : base(configuration) { }

        public Usuario ObtenerPorEmail(string email)
        {
            Usuario u = null;
            using var conn = new MySqlConnection(connectionString);
            var sql = @"SELECT * FROM usuarios WHERE email = @email";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@email", email);
            conn.Open();
            var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                u = new Usuario
                {
                    Id = reader.GetInt32("id"),
                    Email = reader.GetString("email"),
                    PasswordHash = reader.GetString("password"),
                    Nombre = reader.GetString("nombre"),
                    Apellido = reader.GetString("apellido"),
                    Rol = reader.GetString("rol"),
                    AvatarUrl = reader.IsDBNull(reader.GetOrdinal("avatar_url")) ? null : reader.GetString("avatar_url"),
                    Creado_En = reader.GetDateTime("creado_en"),
                    Actualizado_En = reader.GetDateTime("actualizado_en")
                };
            }
            return u;
        }

        public Usuario ObtenerPorId(int id)
        {
            Usuario u = null;
            using var conn = new MySqlConnection(connectionString);
            var sql = @"SELECT * FROM usuarios WHERE id = @id";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", id);
            conn.Open();
            var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                u = new Usuario
                {
                    Id = reader.GetInt32("id"),
                    Email = reader.GetString("email"),
                    PasswordHash = reader.GetString("password"),
                    Nombre = reader.GetString("nombre"),
                    Apellido = reader.GetString("apellido"),
                    Rol = reader.GetString("rol"),
                    AvatarUrl = reader.IsDBNull(reader.GetOrdinal("avatar_url")) ? null : reader.GetString("avatar_url"),
                    Creado_En = reader.GetDateTime("creado_en"),
                    Actualizado_En = reader.GetDateTime("actualizado_en")
                };
            }
            return u;
        }

        public int Alta(Usuario u)
        {
            int res = -1;
            using var conn = new MySqlConnection(connectionString);
            var sql = @"INSERT INTO usuarios (email, password, nombre, apellido, rol, avatar_url, creado_en, actualizado_en)
                        VALUES (@email, @password, @nombre, @apellido, @rol, @avatar, NOW(), NOW());
                        SELECT LAST_INSERT_ID();";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@email", u.Email);
            cmd.Parameters.AddWithValue("@password", u.PasswordHash);
            cmd.Parameters.AddWithValue("@nombre", u.Nombre);
            cmd.Parameters.AddWithValue("@apellido", u.Apellido);
            cmd.Parameters.AddWithValue("@rol", u.Rol);
            cmd.Parameters.AddWithValue("@avatar", u.AvatarUrl ?? (object)DBNull.Value);
            conn.Open();
            res = Convert.ToInt32(cmd.ExecuteScalar()); // Devuelve el ID nuevo
            return res;
        }

        public IList<Usuario> ObtenerTodos()
        {
            IList<Usuario> lista = new List<Usuario>();
            using var conn = new MySqlConnection(connectionString);
            var sql = @"SELECT * FROM usuarios
            WHERE Rol != 'Administrador'  ";
            using var cmd = new MySqlCommand(sql, conn);
            conn.Open();
            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Usuario u = new Usuario
                {
                    Id = reader.GetInt32("id"),
                    Email = reader.GetString("email"),
                    PasswordHash = reader.GetString("password"),
                    Nombre = reader.GetString("nombre"),
                    Apellido = reader.GetString("apellido"),
                    Rol = reader.GetString("rol"),
                    AvatarUrl = reader.IsDBNull(reader.GetOrdinal("avatar_url")) ? null : reader.GetString("avatar_url"),
                    Creado_En = reader.GetDateTime("creado_en"),
                    Actualizado_En = reader.GetDateTime("actualizado_en")
                };
                lista.Add(u);
            }
            return lista;
        }

        public int Baja(int id)
        {
            int res = -1;
            using var conn = new MySqlConnection(connectionString);
            var sql = @"DELETE FROM usuarios WHERE id = @id";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", id);
            conn.Open();
            res = cmd.ExecuteNonQuery(); // Devuelve el número de filas afectadas
            return res;
        }

        public int Modificacion(Usuario entidad)
        {
            int res = -1;
            using var conn = new MySqlConnection(connectionString);
            var sql = @"UPDATE usuarios SET email = @email, password = @password, nombre = @nombre, apellido = @apellido, rol = @rol, avatar_url = @avatar, actualizado_en = NOW() WHERE id = @id";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", entidad.Id);
            cmd.Parameters.AddWithValue("@email", entidad.Email);
            cmd.Parameters.AddWithValue("@password", entidad.PasswordHash);
            cmd.Parameters.AddWithValue("@nombre", entidad.Nombre);
            cmd.Parameters.AddWithValue("@apellido", entidad.Apellido);
            cmd.Parameters.AddWithValue("@rol", entidad.Rol);
            cmd.Parameters.AddWithValue("@avatar", entidad.AvatarUrl ?? (object)DBNull.Value);
            conn.Open();
            res = cmd.ExecuteNonQuery(); // Devuelve el número de filas afectadas
            return res;
        }
    }
}
