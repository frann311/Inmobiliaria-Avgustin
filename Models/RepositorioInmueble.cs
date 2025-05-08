using Inmobiliaria_Avgustin.Models;
using MySql.Data.MySqlClient;

public class RepositorioInmueble : RepositorioBase, IRepositorioInmueble
{

    public RepositorioInmueble(IConfiguration configuration) : base(configuration)
    {
    }


    public IList<Inmueble> ObtenerTodosFilter(
        bool? disponible,
        int? propietarioId,
        DateTime? fechaDesde,
        DateTime? fechaHasta,
        int? TipoId,
        int? UsoId,
        int page = 1,
        int pageSize = 10)
    {
        Console.WriteLine("UsoId: " + UsoId);
        Console.WriteLine("TipoId: " + TipoId);
        var lista = new List<Inmueble>();
        using var conn = new MySqlConnection(connectionString);

        // 1) SELECT + JOINS
        var sql = @"
            SELECT 
                i.Id, i.Direccion, i.Ambientes, i.Precio, i.Disponible,
                i.id_propietario, i.id_uso_inmueble, i.id_tipo_inmueble,
                i.coordenadas, i.superficie,
                p.Nombre, p.Apellido, p.Dni,
                u.nombre AS Uso,
                t.nombre AS Tipo
            FROM inmuebles i
            JOIN propietarios p      ON i.id_propietario    = p.Id
            JOIN usoinmueble u       ON i.id_uso_inmueble   = u.id
            JOIN tipoinmueble t      ON i.id_tipo_inmueble  = t.id
            WHERE 1 = 1
            ";

        // 2) Filtros simples
        if (disponible.HasValue)
            sql += "  AND i.Disponible       = @disponible\n";
        if (propietarioId.HasValue)
            sql += "  AND i.id_propietario   = @propietarioId\n";

        // 3) Filtro de disponibilidad entre fechas: excluyo los inmuebles con
        //    contratos que se solapan con el rango indicado.
        if (fechaDesde.HasValue && fechaHasta.HasValue)
        {
            sql += @"
              AND NOT EXISTS (
                  SELECT 1
                    FROM contratos c
                   WHERE c.id_inmueble = i.Id
                     AND (
                          (c.Fecha_Rescision IS NULL
                           AND c.Fecha_Inicio <= @fechaHasta
                           AND c.Fecha_Fin    >= @fechaDesde)
                       OR (c.Fecha_Rescision IS NOT NULL
                           AND c.Fecha_Inicio    <= @fechaHasta
                           AND c.Fecha_Rescision >= @fechaDesde)
                     )
              ) 
            ";


        }

        if (TipoId.HasValue)
        {
            sql += @"AND i.id_tipo_inmueble = @TipoId ";

        }
        if (UsoId.HasValue)
        {
            sql += @"AND i.id_uso_inmueble = @UsoId ";
        }


        // 4) Orden y paginación (sin punto y coma al final)
        sql += " ORDER BY i.Direccion\n";
        sql += " LIMIT @offset, @pageSize\n";

        using var cmd = new MySqlCommand(sql, conn);

        // 5) Parámetros dinámicos
        if (disponible.HasValue)
            cmd.Parameters.AddWithValue("@disponible", disponible.Value);
        if (propietarioId.HasValue)
            cmd.Parameters.AddWithValue("@propietarioId", propietarioId.Value);
        if (fechaDesde.HasValue && fechaHasta.HasValue)
        {
            cmd.Parameters.AddWithValue("@fechaDesde", fechaDesde.Value.Date);
            cmd.Parameters.AddWithValue("@fechaHasta", fechaHasta.Value.Date);
        }
        if (TipoId.HasValue)
            cmd.Parameters.AddWithValue("@TipoId", TipoId);
        if (UsoId.HasValue)
            cmd.Parameters.AddWithValue("@UsoId", UsoId);


        // 6) Parámetros de paginación
        int offset = (page - 1) * pageSize;
        cmd.Parameters.AddWithValue("@offset", offset);
        cmd.Parameters.AddWithValue("@pageSize", pageSize);

        conn.Open();
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            lista.Add(new Inmueble
            {
                Id = reader.GetInt32("Id"),
                Direccion = reader.GetString("Direccion"),
                Ambientes = reader.GetInt32("Ambientes"),
                Precio = reader.GetDouble("Precio"),
                Disponible = reader.GetBoolean("Disponible"),
                Propietario = new Propietario
                {
                    Id = reader.GetInt32("id_propietario"),
                    Nombre = reader.GetString("Nombre"),
                    Apellido = reader.GetString("Apellido"),
                    Dni = reader.GetString("Dni")
                },
                Uso = new UsosInmuebles
                {
                    Id = reader.GetInt32("id_uso_inmueble"),
                    Nombre = reader.GetString("Uso")
                },
                Tipo = new TiposInmuebles
                {
                    Id = reader.GetInt32("id_tipo_inmueble"),
                    Nombre = reader.GetString("Tipo")
                },
                Coordenadas = reader.GetInt32("coordenadas"),
                Superficie = reader.GetInt32("superficie")
            });
        }
        return lista;
    }






    public IList<Inmueble> ObtenerTodos()
    {
        var lista = new List<Inmueble>();

        try
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @"SELECT 
                    i.Id, 
                    i.Direccion, 
                    i.Ambientes, 
                    i.Precio, 
                    i.Disponible,
                    i.id_propietario,
                    i.id_uso_inmueble,
                    i.id_tipo_inmueble,
                    i.coordenadas,
                    i.superficie,
                    p.Nombre,
                    p.Apellido,
                    p.Dni,
                    u.nombre AS Uso,
                    t.nombre AS Tipo
                FROM inmuebles i JOIN propietarios p ON i.id_propietario = p.Id
                JOIN usoinmueble u ON i.id_uso_inmueble = u.id
                JOIN tipoinmueble t ON i.id_tipo_inmueble = t.id;";

                using (var command = new MySqlCommand(sql, connection))
                {
                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new Inmueble
                            {
                                Id = reader.GetInt32("Id"),
                                Direccion = reader.GetString("Direccion"),
                                Ambientes = reader.GetInt32("Ambientes"),
                                Precio = reader.GetDouble("Precio"),
                                Disponible = reader.GetBoolean("Disponible"),
                                Coordenadas = reader.GetInt32("coordenadas"),
                                Superficie = reader.GetInt32("superficie"),
                                PropietarioId = reader.GetInt32("id_propietario"),
                                UsoId = reader.GetInt32("id_uso_inmueble"),
                                TipoId = reader.GetInt32("id_tipo_inmueble"),
                                Uso = new UsosInmuebles
                                {
                                    Id = reader.GetInt32("id_uso_inmueble"),
                                    Nombre = reader.GetString("Uso")
                                },
                                Tipo = new TiposInmuebles
                                {
                                    Id = reader.GetInt32("id_tipo_inmueble"),
                                    Nombre = reader.GetString("Tipo")
                                },
                                Propietario = new Propietario
                                {
                                    Id = reader.GetInt32("id_propietario"),
                                    Nombre = reader.GetString("Nombre"),
                                    Apellido = reader.GetString("Apellido"),
                                    Dni = reader.GetString("Dni")
                                }
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

    public Inmueble ObtenerPorId(int id)
    {
        var inmueble = new Inmueble();

        try
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @"SELECT 
                    i.Id, 
                    i.Direccion, 
                    i.Ambientes, 
                    i.Precio, 
                    i.Disponible,
                    i.id_propietario,
                    i.id_uso_inmueble,
                    i.id_tipo_inmueble,
                    i.coordenadas,
                    i.superficie,
                    p.Nombre,
                    p.Apellido,
                    p.Dni,
                    u.nombre AS Uso,
                    t.nombre AS Tipo
                FROM inmuebles i JOIN propietarios p ON i.id_propietario = p.Id
                JOIN usoinmueble u ON i.id_uso_inmueble = u.id
                JOIN tipoinmueble t ON i.id_tipo_inmueble = t.id 
                    WHERE i.Id = @id;
                    ";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            inmueble = new Inmueble
                            {
                                Id = reader.GetInt32("Id"),
                                Direccion = reader.GetString("Direccion"),
                                Ambientes = reader.GetInt32("Ambientes"),
                                Precio = reader.GetDouble("Precio"),
                                Disponible = reader.GetBoolean("Disponible"),
                                Coordenadas = reader.GetInt32("coordenadas"),
                                Superficie = reader.GetInt32("superficie"),
                                PropietarioId = reader.GetInt32("id_propietario"),
                                UsoId = reader.GetInt32("id_uso_inmueble"),
                                TipoId = reader.GetInt32("id_tipo_inmueble"),
                                Uso = new UsosInmuebles
                                {
                                    Id = reader.GetInt32("id_uso_inmueble"),
                                    Nombre = reader.GetString("Uso")
                                },
                                Tipo = new TiposInmuebles
                                {
                                    Id = reader.GetInt32("id_tipo_inmueble"),
                                    Nombre = reader.GetString("Tipo")
                                },
                                Propietario = new Propietario
                                {
                                    Id = reader.GetInt32("id_propietario"),
                                    Nombre = reader.GetString("Nombre"),
                                    Apellido = reader.GetString("Apellido"),
                                    Dni = reader.GetString("Dni")
                                }
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

        return inmueble;
    }

    public int Alta(Inmueble p)
    {
        int id = 0;
        try
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @"INSERT INTO inmuebles 
                        (Direccion, Ambientes, coordenadas, superficie, Precio, Disponible, id_propietario, id_uso_inmueble, id_tipo_inmueble) 
                        VALUES 
                        (@Direccion, @Ambientes, @Coordenadas, @Superficie, @Precio, @Disponible, @PropietarioId, @UsoId, @TipoId);
                        SELECT LAST_INSERT_ID();";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Direccion", p.Direccion);
                    command.Parameters.AddWithValue("@Ambientes", p.Ambientes);
                    command.Parameters.AddWithValue("@Coordenadas", p.Coordenadas);
                    command.Parameters.AddWithValue("@Superficie", p.Superficie);
                    command.Parameters.AddWithValue("@Precio", p.Precio);
                    command.Parameters.AddWithValue("@Disponible", p.Disponible);
                    command.Parameters.AddWithValue("@PropietarioId", p.PropietarioId);
                    command.Parameters.AddWithValue("@UsoId", p.UsoId);
                    command.Parameters.AddWithValue("@TipoId", p.TipoId);

                    connection.Open();
                    id = Convert.ToInt32(command.ExecuteScalar());
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERROR en Alta: {ex}");
            throw;
        }
        return id;
    }

    public int Baja(int id)
    {
        int filasAfectadas = 0;
        try
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @"DELETE FROM inmuebles WHERE Id = @id";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    filasAfectadas = command.ExecuteNonQuery();
                }
            }
        }
        catch (Exception ex)
        {
            // Registra el error para diagnóstico
            Console.WriteLine($"ERROR en Baja: {ex.ToString()}");
            throw; // Re-lanza la excepción para ver detalles
        }

        return filasAfectadas;
    }

    public int Modificacion(Inmueble entidad)
    {
        int filasAfectadas = 0;
        try
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @"UPDATE inmuebles SET 
                        Direccion = @Direccion, 
                        id_uso_inmueble = @UsoId, 
                        id_tipo_inmueble = @TipoId,  
                        Ambientes = @Ambientes, 
                        coordenadas = @Coordenadas,
                        superficie = @Superficie,
                        Precio = @Precio, 
                        Disponible = @Disponible, 
                        id_propietario = @PropietarioId
                        WHERE Id = @Id";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Direccion", entidad.Direccion);
                    command.Parameters.AddWithValue("@UsoId", entidad.UsoId);
                    command.Parameters.AddWithValue("@TipoId", entidad.TipoId);
                    command.Parameters.AddWithValue("@Ambientes", entidad.Ambientes);
                    command.Parameters.AddWithValue("@Coordenadas", entidad.Coordenadas);
                    command.Parameters.AddWithValue("@Superficie", entidad.Superficie);
                    command.Parameters.AddWithValue("@Precio", entidad.Precio);
                    command.Parameters.AddWithValue("@Disponible", entidad.Disponible);
                    command.Parameters.AddWithValue("@PropietarioId", entidad.PropietarioId);
                    command.Parameters.AddWithValue("@Id", entidad.Id);

                    connection.Open();
                    filasAfectadas = command.ExecuteNonQuery();
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERROR en Modificacion: {ex}");
            throw;
        }
        return filasAfectadas;
    }


    public IEnumerable<Inmueble> ObtenerPorPropietario(int idPropietario)
    {
        var lista = new List<Inmueble>();

        try
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @"SELECT 
                    i.Id, 
                    i.Direccion, 
                    i.Ambientes, 
                    i.Precio, 
                    i.Disponible,
                    i.id_propietario,
                    i.id_uso_inmueble,
                    i.id_tipo_inmueble,
                    i.coordenadas,
                    i.superficie,
                    p.Nombre,
                    p.Apellido,
                    p.Dni,
                    u.nombre AS Uso,
                    t.nombre AS Tipo
                FROM inmuebles i JOIN propietarios p ON i.id_propietario = p.Id
                JOIN usoinmueble u ON i.id_uso_inmueble = u.id
                JOIN tipoinmueble t ON i.id_tipo_inmueble = t.id 
                WHERE p.Id = @idPropietario;";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@idPropietario", idPropietario);
                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new Inmueble
                            {
                                Id = reader.GetInt32("Id"),
                                Direccion = reader.GetString("Direccion"),
                                Ambientes = reader.GetInt32("Ambientes"),
                                Precio = reader.GetDouble("Precio"),
                                Disponible = reader.GetBoolean("Disponible"),
                                Coordenadas = reader.GetInt32("coordenadas"),
                                Superficie = reader.GetInt32("superficie"),
                                PropietarioId = reader.GetInt32("id_propietario"),
                                UsoId = reader.GetInt32("id_uso_inmueble"),
                                TipoId = reader.GetInt32("id_tipo_inmueble"),
                                Uso = new UsosInmuebles
                                {
                                    Id = reader.GetInt32("id_uso_inmueble"),
                                    Nombre = reader.GetString("Uso")
                                },
                                Tipo = new TiposInmuebles
                                {
                                    Id = reader.GetInt32("id_tipo_inmueble"),
                                    Nombre = reader.GetString("Tipo")
                                },
                                Propietario = new Propietario
                                {
                                    Id = reader.GetInt32("id_propietario"),
                                    Nombre = reader.GetString("Nombre"),
                                    Apellido = reader.GetString("Apellido"),
                                    Dni = reader.GetString("Dni")
                                }
                            });
                        }
                    }
                }
            }

        }
        catch (Exception ex)
        {
            // Registra el error para diagnóstico
            Console.WriteLine($"ERROR en ObtenerPorPropietario: {ex.ToString()}");
            throw; // Re-lanza la excepción para ver detalles
        }

        return lista;

    }

    public IEnumerable<Inmueble> ObtenerPorTipo(string tipo)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Inmueble> ObtenerPorPrecio(decimal precioMinimo, decimal precioMaximo)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Inmueble> ObtenerPorUbicacion(string ubicacion)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Inmueble> ObtenerPorFecha(DateTime fechaDesde, DateTime fechaHasta)
    {
        throw new NotImplementedException();
    }

    public IList<Inmueble> BuscarPorNombre(string nombre)
    {
        List<Inmueble> res = new List<Inmueble>();
        Inmueble i = null;
        nombre = "%" + nombre + "%";
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            string sql = @"SELECT Id, Direccion, Precio
					FROM Inmuebles
					WHERE Direccion LIKE @nombre ";
            using (MySqlCommand command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@nombre", nombre);

                connection.Open();
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    i = new Inmueble
                    {
                        Id = reader.GetInt32(nameof(Inmueble.Id)),
                        Direccion = reader.GetString("Direccion"),
                        Precio = reader.GetDouble("Precio"),
                    };
                    res.Add(i);
                }
                connection.Close();
            }
        }
        return res;
    }

    public int ContarInmueblesFilter(bool? disponible, int? propietarioId, DateTime? fechaDesde, DateTime? fechaHasta, int? TipoId, int? UsoId)
    {
        int count = 0;
        using var conn = new MySqlConnection(connectionString);
        var sql = @"
            SELECT COUNT(*) 
            FROM inmuebles i
            JOIN propietarios p ON i.id_propietario = p.Id
            JOIN usoinmueble u ON i.id_uso_inmueble = u.id
            JOIN tipoinmueble t ON i.id_tipo_inmueble = t.id
            WHERE 1 = 1
            ";

        if (disponible.HasValue)
            sql += "  AND i.Disponible       = @disponible\n";
        if (propietarioId.HasValue)
            sql += "  AND i.id_propietario   = @propietarioId\n";
        if (TipoId.HasValue)
            sql += @"AND i.id_tipo_inmueble = @TipoId ";
        if (UsoId.HasValue)
            sql += @"AND i.id_uso_inmueble = @UsoId ";

        using var cmd = new MySqlCommand(sql, conn);

        if (disponible.HasValue)
            cmd.Parameters.AddWithValue("@disponible", disponible.Value);
        if (propietarioId.HasValue)
            cmd.Parameters.AddWithValue("@propietarioId", propietarioId.Value);
        if (TipoId.HasValue)
            cmd.Parameters.AddWithValue("@TipoId", TipoId);
        if (UsoId.HasValue)
            cmd.Parameters.AddWithValue("@UsoId", UsoId);


        conn.Open();
        count = Convert.ToInt32(cmd.ExecuteScalar());
        return count;
    }
}






// extras



