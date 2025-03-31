
using MySql.Data.MySqlClient;

namespace Inmobiliaria_Avgustin.Models
{


    public class RepositorioInquilino : RepositorioBase, IRepositorioInquilino
    {
        public RepositorioInquilino(IConfiguration configuration) : base(configuration) { }

        public IList<Inquilino> ObtenerTodos()
        {
            var lista = new List<Inquilino>();

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
                        Telefono,
                        Trabajo,
                        Ingresos
                    FROM inquilinos";

                    using (var command = new MySqlCommand(sql, connection))
                    {
                        connection.Open();

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lista.Add(new Inquilino
                                {
                                    Id = reader.GetInt32("Id"),
                                    Nombre = reader.GetString("Nombre"),
                                    Apellido = reader.GetString("Apellido"),
                                    Dni = reader.GetString("Dni"),
                                    Email = reader.GetString("Email"),
                                    Telefono = reader.GetString("Telefono"),
                                    Trabajo = reader.GetString("Trabajo"),
                                    Ingresos = reader.GetInt32("Ingresos")
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

        public int Alta(Inquilino i)

        {
            int res = -1;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = @"
                            INSERT INTO Inquilinos 
                            (nombre, apellido, dni, email, telefono, trabajo, ingresos)
                             VALUES (@nombre, @apellido, @dni, @email, @telefono,@trabajo,@ingresos);
                             SELECT LAST_INSERT_ID();";

                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@nombre", i.Nombre);
                    command.Parameters.AddWithValue("@apellido", i.Apellido);
                    command.Parameters.AddWithValue("@dni", i.Dni);
                    command.Parameters.AddWithValue("@telefono", i.Telefono);
                    command.Parameters.AddWithValue("@email", i.Email);
                    command.Parameters.AddWithValue("@trabajo", i.Trabajo);
                    command.Parameters.AddWithValue("@ingresos", i.Ingresos);
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
                    string sql = "DELETE FROM Inquilinos WHERE Id = @id;";

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
        public int Modificacion(Inquilino Inquilino)
        {
            int res = -1;
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    string sql = @"
                            UPDATE Inquilinos 
                            SET nombre = @nombre, 
                                apellido = @apellido, 
                                dni = @dni, 
                                email = @email, 
                                telefono = @telefono
                            WHERE Id = @id;";

                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@id", Inquilino.Id);
                        command.Parameters.AddWithValue("@nombre", Inquilino.Nombre);
                        command.Parameters.AddWithValue("@apellido", Inquilino.Apellido);
                        command.Parameters.AddWithValue("@dni", Inquilino.Dni);
                        command.Parameters.AddWithValue("@telefono", Inquilino.Telefono);
                        command.Parameters.AddWithValue("@email", Inquilino.Email);

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
        public Inquilino ObtenerPorId(int id)
        {
            var Inquilino = new Inquilino();

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
                        Telefono,
                        Trabajo,
                        Ingresos
                    FROM Inquilinos WHERE Id = @id";

                    using (var command = new MySqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        connection.Open();

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                Inquilino = new Inquilino
                                {
                                    Id = reader.GetInt32("Id"),
                                    Nombre = reader.GetString("Nombre"),
                                    Apellido = reader.GetString("Apellido"),
                                    Dni = reader.GetString("Dni"),
                                    Email = reader.GetString("Email"),
                                    Telefono = reader.GetString("Telefono"),
                                    Trabajo = reader.GetString("Trabajo"),
                                    Ingresos = reader.GetInt32("Ingresos")
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
            return Inquilino;


        }



        // Implemetnacion de metodos (OTROS )
        public Inquilino ObtenerPorEmail(string email)
        {
            throw new NotImplementedException();
        }
        public IList<Inquilino> BuscarPorDni(string dni)
        {
            throw new NotImplementedException();
        }

        public IList<Inquilino> BuscarPorNombre(string nombre)
        {
            throw new NotImplementedException();
        }

    }

}