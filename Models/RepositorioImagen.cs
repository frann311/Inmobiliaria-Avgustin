using Inmobiliaria_Avgustin.Models;
using MySql.Data.MySqlClient;


public class RepositorioImagen : RepositorioBase, IRepositorioImagen
{

    public RepositorioImagen(IConfiguration configuration) : base(configuration)
    {
    }

    public int Alta(Imagen img)
    {
        var idGenerado = 0;
        try
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                var url = @"INSERT INTO Imagenes (InmuebleId, Url, EsPortada)
                 VALUES (@inmuebleId, @url, @EsPortada);";
                using (var cmd = new MySqlCommand(url, conn))
                {
                    // Parámetros para prevenir SQL Injection
                    cmd.Parameters.AddWithValue("@inmuebleId", img.InmuebleId);
                    cmd.Parameters.AddWithValue("@url", img.Url);
                    cmd.Parameters.AddWithValue("@EsPortada", img.EsPortada);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                    idGenerado = (int)cmd.LastInsertedId;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERROR en Alta: {ex}");
            throw;
        }
        return idGenerado;
    }

    public int Baja(int id)
    {
        try
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                Console.WriteLine("Eliminando imagen...2 id: " + id);
                var sql = "DELETE FROM Imagenes WHERE Id = @id";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    conn.Open();
                    return cmd.ExecuteNonQuery();
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERROR en Baja: {ex}");
            throw;
        }
    }

    public IList<Imagen> BuscarPorInmueble(int inmuebleId)
    {
        var lista = new List<Imagen>();
        using (var conn = new MySqlConnection(connectionString))
        {
            var sql = "SELECT * FROM Imagenes WHERE InmuebleId = @inmuebleId";
            using (var cmd = new MySqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@inmuebleId", inmuebleId);
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        lista.Add(new Imagen
                        {
                            Id = reader.GetInt32("Id"),
                            InmuebleId = reader.GetInt32("InmuebleId"),
                            Url = reader.GetString("Url"),
                            EsPortada = reader.GetBoolean("EsPortada")
                        });

                    }
                    return lista;
                }
            }
        }
    }

    public int Modificacion(Imagen entidad)
    {
        throw new NotImplementedException();
    }

    public Imagen ObtenerPorId(int id)
    {
        Imagen res = null;
        using (MySqlConnection conn = new MySqlConnection(connectionString))
        {
            string sql = @$"
					SELECT *
					FROM Imagenes
					WHERE id = @id";
            using (MySqlCommand comm = new MySqlCommand(sql, conn))
            {
                comm.Parameters.AddWithValue("@id", id);
                conn.Open();
                var reader = comm.ExecuteReader();
                if (reader.Read())
                {
                    res = new Imagen
                    {
                        InmuebleId = reader.GetInt32("InmuebleId"),
                        Url = reader.GetString("Url"),
                        Id = reader.GetInt32("Id"),
                        EsPortada = reader.GetBoolean("EsPortada")
                    };
                }
                conn.Close();
            }
        }
        Console.WriteLine("Obteniendo imagen... idInmueble: " + res.InmuebleId);
        return res;
    }

    public IList<Imagen> ObtenerTodos()
    {
        throw new NotImplementedException();
    }
    // Desmarca cualquier imagen que esté actualmente como portada en un inmueble
    public void QuitarPortadaActual(int inmuebleId)
    {
        using var conn = new MySqlConnection(connectionString);
        const string sql = @"
                UPDATE Imagenes
                SET EsPortada = 0
                WHERE InmuebleId = @inmuebleId";

        using var cmd = new MySqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@inmuebleId", inmuebleId);
        conn.Open();
        cmd.ExecuteNonQuery();
    }

    public void MarcarComoPortada(int id)
    {
        using var conn = new MySqlConnection(connectionString);
        const string sql = @"
                UPDATE Imagenes
                   SET EsPortada = 1
                 WHERE Id = @id";

        using var cmd = new MySqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@id", id);
        conn.Open();
        cmd.ExecuteNonQuery();
    }

    public int CantidadImagenes(int inmuebleId)
    {
        int cantidad = 0;
        using (var conn = new MySqlConnection(connectionString))
        {
            var sql = "SELECT COUNT(*) FROM Imagenes WHERE InmuebleId = @inmuebleId";
            using (var cmd = new MySqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@inmuebleId", inmuebleId);
                conn.Open();
                cantidad = Convert.ToInt32(cmd.ExecuteScalar());
            }
        }
        return cantidad;
    }
}

