
using MySql.Data.MySqlClient;

namespace Inmobiliaria_Avgustin.Models
{

    public class RepositorioPropietario : RepositorioBase, IRepositorioPropietario
    {
        public RepositorioPropietario(IConfiguration configuration) : base(configuration) { }


        public bool ProbarConexion()
        {
            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    Console.WriteLine("¡Conexión exitosa!");
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al conectar: {ex.Message}");
                return false;
            }
        }
        public IList<Propietario> ObtenerTodos()
        {
            var lista = new List<Propietario>();

            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    var sql = @"SELECT 
                        Id, 
                        Nombre, 
                        Apellido, 
                        Dni, 
                        Email, 
                        Telefono
                    FROM Propietarios";

                    using (var command = new MySqlCommand(sql, connection))
                    {
                        connection.Open();

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lista.Add(new Propietario
                                {
                                    Id = reader.GetInt32("Id"),
                                    Nombre = reader.GetString("Nombre"),
                                    Apellido = reader.GetString("Apellido"),
                                    Dni = reader.GetString("Dni"),
                                    Email = reader.GetString("Email"),
                                    Telefono = reader.GetString("Telefono")
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Registra el error para diagnóstico
                Console.WriteLine($"ERROR en ObtenerTodos: {ex.ToString()}");
                throw; // Re-lanza la excepción para ver detalles
            }

            return lista;
        }

        // Implemetnacion de metodos (Alta)
        public int Alta(Propietario p)

        {
            int res = -1;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = @"
                            INSERT INTO Propietarios 
                            (nombre, apellido, dni, email, telefono)
                             VALUES (@nombre, @apellido, @dni, @email, @telefono);
                             SELECT LAST_INSERT_ID();";

                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@nombre", p.Nombre);
                    command.Parameters.AddWithValue("@apellido", p.Apellido);
                    command.Parameters.AddWithValue("@dni", p.Dni);
                    command.Parameters.AddWithValue("@telefono", p.Telefono);
                    command.Parameters.AddWithValue("@email", p.Email);
                    // Abre la conexión y ejecuta el comando

                    connection.Open();
                    res = Convert.ToInt32(command.ExecuteScalar());
                }

            }
            return res;

        }
        // Implemetnacion de metodos (Baja)
        public int Baja(int id)
        {
            int res = -1;
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    string sql = "DELETE FROM Propietarios WHERE Id = @id;";

                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        // Abre la conexión y ejecuta el comando
                        connection.Open();
                        res = Convert.ToInt32(command.ExecuteNonQuery());
                    }

                }
                return res;

            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR en ObtenerPorId: {ex.ToString()}");
                throw;
            }
        }

        // Implemetnacion de metodos (Modificacion)
        public int Modificacion(Propietario propietario)
        {
            int res = -1;
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    string sql = @"
                            UPDATE Propietarios 
                            SET nombre = @nombre, 
                                apellido = @apellido, 
                                dni = @dni, 
                                email = @email, 
                                telefono = @telefono
                            WHERE Id = @id;";

                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@id", propietario.Id);
                        command.Parameters.AddWithValue("@nombre", propietario.Nombre);
                        command.Parameters.AddWithValue("@apellido", propietario.Apellido);
                        command.Parameters.AddWithValue("@dni", propietario.Dni);
                        command.Parameters.AddWithValue("@telefono", propietario.Telefono);
                        command.Parameters.AddWithValue("@email", propietario.Email);

                        // Abre la conexión y ejecuta el comando
                        connection.Open();
                        res = Convert.ToInt32(command.ExecuteNonQuery());
                    }

                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR en ObtenerPorId: {ex.ToString()}");
                throw;

            }
            return res;
        }


        // Implemetnacion de metodos (ObtenerPorId)
        public Propietario ObtenerPorId(int id)
        {
            var propietario = new Propietario();

            try
            {

                using (var connection = new MySqlConnection(connectionString))
                {
                    var sql = @"SELECT 
                        Id, 
                        Nombre, 
                        Apellido, 
                        Dni, 
                        Email, 
                        Telefono
                    FROM Propietarios WHERE Id = @id";

                    using (var command = new MySqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        connection.Open();

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                propietario = new Propietario
                                {
                                    Id = reader.GetInt32("Id"),
                                    Nombre = reader.GetString("Nombre"),
                                    Apellido = reader.GetString("Apellido"),
                                    Dni = reader.GetString("Dni"),
                                    Email = reader.GetString("Email"),
                                    Telefono = reader.GetString("Telefono")
                                };
                            }
                        }

                    }
                }

            }
            catch (Exception ex)
            {
                // Registra el error para diagnóstico
                Console.WriteLine($"ERROR en ObtenerPorId: {ex.ToString()}");
                throw; // Re-lanza la excepción para ver detalles
            }
            return propietario;


        }









        // Implemetnacion de metodos (OTROS )
        public Propietario ObtenerPorEmail(string email)
        {
            throw new NotImplementedException();
        }
        public IList<Propietario> BuscarPorDni(string dni)
        {
            throw new NotImplementedException();
        }

        public IList<Propietario> BuscarPorNombre(string nombre)
        {
            List<Propietario> res = new List<Propietario>();
            Propietario p = null;
            nombre = "%" + nombre + "%";
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = @"SELECT Id, Nombre, Apellido, Dni, Telefono, Email 
					FROM Propietarios
					WHERE Nombre LIKE @nombre OR Apellido LIKE @nombre";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@nombre", nombre);

                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        p = new Propietario
                        {
                            Id = reader.GetInt32(nameof(Propietario.Id)),
                            Nombre = reader.GetString("Nombre"),
                            Apellido = reader.GetString("Apellido"),
                            Dni = reader.GetString("Dni"),
                            Telefono = reader.GetString("Telefono"),
                            Email = reader.GetString("Email"),
                        };
                        res.Add(p);
                    }
                    connection.Close();
                }
            }
            return res;
        }

        public int contarPropietarios()
        {
            int res = 0;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = "SELECT COUNT(*) FROM Propietarios;";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    connection.Open();
                    res = Convert.ToInt32(command.ExecuteScalar());
                }
            }
            return res;
        }

        public IList<Propietario> ObtenerTodosPaginado(int page = 1, int pageSize = 10)
        {

            var lista = new List<Propietario>();
            int offset = (page - 1) * pageSize;

            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    var sql = @"SELECT 
                        Id, 
                        Nombre, 
                        Apellido, 
                        Dni, 
                        Email, 
                        Telefono
                    FROM Propietarios
                    ORDER BY Apellido
                    LIMIT @pageSize OFFSET @offset";

                    using (var command = new MySqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@pageSize", pageSize);
                        command.Parameters.AddWithValue("@offset", offset);
                        connection.Open();

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lista.Add(new Propietario
                                {
                                    Id = reader.GetInt32("Id"),
                                    Nombre = reader.GetString("Nombre"),
                                    Apellido = reader.GetString("Apellido"),
                                    Dni = reader.GetString("Dni"),
                                    Email = reader.GetString("Email"),
                                    Telefono = reader.GetString("Telefono")
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Registra el error para diagnóstico
                Console.WriteLine($"ERROR en ObtenerTodosPaginado: {ex.ToString()}");
                throw; // Re-lanza la excepción para ver detalles
            }

            return lista;
        }
    }
}