using Inmobiliaria_Avgustin.Models;
using MySql.Data.MySqlClient;

public class RepositorioUsosInmuebles : RepositorioBase, IRepositorioUsosInmuebles
{
    public RepositorioUsosInmuebles(IConfiguration configuration) : base(configuration)
    {
    }

    public int Alta(UsosInmuebles p)
    {
        throw new NotImplementedException();
    }

    public int Baja(int id)
    {
        throw new NotImplementedException();
    }

    public IList<Inquilino> BuscarPorNombre(string nombre)
    {
        throw new NotImplementedException();
    }

    public int Modificacion(UsosInmuebles entidad)
    {
        throw new NotImplementedException();
    }

    public UsosInmuebles ObtenerPorId(int id)
    {
        var uso = new UsosInmuebles();

        try
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @"SELECT Id, Nombre FROM usoinmueble WHERE Id = @Id";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            uso.Id = reader.GetInt32("Id");
                            uso.Nombre = reader.GetString("Nombre");
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            throw new Exception("Error al obtener el uso de inmueble por ID", ex);
        }

        return uso;
    }

    public IList<UsosInmuebles> ObtenerTodos()
    {
        var usos = new List<UsosInmuebles>();

        try
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = "SELECT Id, Nombre FROM usoinmueble";

                using (var command = new MySqlCommand(sql, connection))
                {
                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var uso = new UsosInmuebles
                            {
                                Id = reader.GetInt32("Id"),
                                Nombre = reader.GetString("Nombre")
                            };
                            usos.Add(uso);
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            throw new Exception("Error al obtener todos los usos de inmuebles", ex);
        }

        return usos;
    }
}

