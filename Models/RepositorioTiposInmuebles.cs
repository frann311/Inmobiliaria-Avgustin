using Inmobiliaria_Avgustin.Models;
using MySql.Data.MySqlClient;

public class RepositorioTiposInmuebles : RepositorioBase, IRepositorioTiposImnuebles
{
    public RepositorioTiposInmuebles(IConfiguration configuration) : base(configuration)
    {
    }

    public int Alta(TiposInmuebles p)
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

    public int Modificacion(TiposInmuebles entidad)
    {
        throw new NotImplementedException();
    }

    public TiposInmuebles ObtenerPorId(int id)
    {
        var tipo = new TiposInmuebles();

        try
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @"SELECT Id, Nombre FROM tipoinmueble WHERE Id = @Id";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            tipo.Id = reader.GetInt32("Id");
                            tipo.Nombre = reader.GetString("Nombre");
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            throw new Exception("Error al obtener el tipo de inmueble por ID", ex);
        }

        return tipo;
    }

    public IList<TiposInmuebles> ObtenerTodos()
    {
        var tipos = new List<TiposInmuebles>();

        try
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = "SELECT Id, Nombre FROM tipoinmueble";

                using (var command = new MySqlCommand(sql, connection))
                {
                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var tipo = new TiposInmuebles
                            {
                                Id = reader.GetInt32("Id"),
                                Nombre = reader.GetString("Nombre")
                            };
                            tipos.Add(tipo);
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            throw new Exception("Error al obtener los tipos de inmuebles", ex);
        }

        return tipos;
    }
}