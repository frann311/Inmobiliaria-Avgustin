using Inmobiliaria_Avgustin.Models;
using MySql.Data.MySqlClient;


public class RepositorioContrato : RepositorioBase, IRepositorioContrato
{
    public RepositorioContrato(IConfiguration configuration) : base(configuration)
    {
    }



    public IList<Contrato> ObtenerContratosFilter(
    DateTime? fechaDesde,
    DateTime? fechaHasta,
    int? diasVencimiento,
    int? idInmuebleSeleccionado,
    int page = 1,
    int pageSize = 10)
    {
        var lista = new List<Contrato>();
        using var conn = new MySqlConnection(connectionString);

        // 1) Base de la consulta con JOIN al inquilino
        var sql = @"
SELECT
    c.id,
    c.id_inmueble      AS Id_Inmueble,
    c.id_inquilino     AS Id_Inquilino,
    c.monto            AS Monto,
    c.fecha_inicio     AS Fecha_Inicio,
    c.fecha_fin        AS Fecha_Fin,
    c.fecha_rescision  AS Fecha_Rescision,
    c.multa            AS Multa,
    c.creado_en        AS Creado_En,
    c.actualizado_en   AS Actualizado_En,
    i.nombre           AS InqNombre,
    i.apellido         AS InqApellido
FROM contratos c
JOIN inquilinos i ON c.id_inquilino = i.id
WHERE 1 = 1
";

        // 2) Si llega rango de fechas, filtro por solapamiento con ese rango
        if (fechaDesde.HasValue && fechaHasta.HasValue)
        {
            sql += @"
  AND (
      c.fecha_inicio <= @fechaHasta
      AND COALESCE(c.fecha_rescision, c.fecha_fin) >= @fechaDesde
  )";
        }

        if (diasVencimiento.HasValue)
        {
            sql += @"
  AND (
      COALESCE(c.fecha_rescision, c.fecha_fin)
      BETWEEN CURDATE() AND DATE_ADD(CURDATE(), INTERVAL @diasVencimiento DAY)
  )";
        }
        if (idInmuebleSeleccionado.HasValue && idInmuebleSeleccionado.Value > 0)
        {
            sql += @"
  AND c.id_inmueble = @idInmuebleSeleccionado
";
        }

        // 4) Orden y paginación
        sql += @"
ORDER BY c.fecha_inicio DESC
LIMIT @offset, @pageSize
";

        // Console.WriteLine(sql);
        using var cmd = new MySqlCommand(sql, conn);

        // 5) Agrego solo los parámetros que correspondan
        if (fechaDesde.HasValue && fechaHasta.HasValue)
        {
            cmd.Parameters.AddWithValue("@fechaDesde", fechaDesde.Value.Date);
            cmd.Parameters.AddWithValue("@fechaHasta", fechaHasta.Value.Date);
        }
        if (diasVencimiento.HasValue)
        {
            cmd.Parameters.AddWithValue("@diasVencimiento", diasVencimiento.Value);
        }
        if (idInmuebleSeleccionado.HasValue && idInmuebleSeleccionado.Value > 0)
        {
            cmd.Parameters.AddWithValue("@idInmuebleSeleccionado", idInmuebleSeleccionado.Value);
        }

        // Parámetros de paginación
        int offset = (page - 1) * pageSize;
        cmd.Parameters.AddWithValue("@offset", offset);
        cmd.Parameters.AddWithValue("@pageSize", pageSize);

        conn.Open();
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            // Console.WriteLine($"ID: {reader.GetInt32("id")}, Inquilino: {reader.GetString("InqNombre")} {reader.GetString("InqApellido")}");
            lista.Add(new Contrato
            {
                Id = reader.GetInt32("id"),
                Id_Inmueble = reader.GetInt32("Id_Inmueble"),
                Id_Inquilino = reader.GetInt32("Id_Inquilino"),
                Monto = reader.GetDecimal("Monto"),
                Fecha_Inicio = reader.GetDateTime("Fecha_Inicio"),
                Fecha_Fin = reader.GetDateTime("Fecha_Fin"),
                Fecha_Rescision = reader.IsDBNull(reader.GetOrdinal("Fecha_Rescision"))
                                  ? (DateTime?)null
                                  : reader.GetDateTime("Fecha_Rescision"),
                Multa = reader.IsDBNull(reader.GetOrdinal("Multa"))
                                  ? (decimal?)null
                                  : reader.GetDecimal("Multa"),
                Creado_En = reader.GetDateTime("Creado_En"),
                Actualizado_En = reader.GetDateTime("Actualizado_En"),
                Inquilino = new Inquilino
                {
                    Nombre = reader.GetString("InqNombre"),
                    Apellido = reader.GetString("InqApellido")
                }
            });
        }

        return lista;
    }


    public IList<Contrato> ObtenerTodos()
    {
        var lista = new List<Contrato>();

        try
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @"
                SELECT 
                    c.id,
                    c.id_inmueble,
                    c.id_inquilino,
                    c.monto,
                    c.fecha_inicio,
                    c.fecha_fin,
                    c.fecha_rescision,
                    c.multa,
                    c.creado_en,
                    c.actualizado_en,
                    i.nombre,
                    i.apellido
                FROM contratos c
                JOIN inquilinos i ON c.id_inquilino = i.id;
            ";

                using (var command = new MySqlCommand(sql, connection))
                {
                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new Contrato
                            {
                                Id = reader.GetInt32("id"),
                                Id_Inmueble = reader.GetInt32("id_inmueble"),
                                Id_Inquilino = reader.GetInt32("id_inquilino"),
                                Monto = reader.GetDecimal("monto"),
                                Fecha_Inicio = reader.GetDateTime("fecha_inicio"),
                                Fecha_Fin = reader.GetDateTime("fecha_fin"),
                                Fecha_Rescision = reader.IsDBNull(reader.GetOrdinal("fecha_rescision")) ? null : reader.GetDateTime("fecha_rescision"),
                                Multa = reader.IsDBNull(reader.GetOrdinal("multa")) ? null : reader.GetDecimal("multa"),
                                Creado_En = reader.GetDateTime("creado_en"),
                                Actualizado_En = reader.GetDateTime("actualizado_en"),
                                Inquilino = new Inquilino
                                {
                                    Nombre = reader.GetString("nombre"),
                                    Apellido = reader.GetString("apellido")
                                }
                            });
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERROR en ObtenerTodos: {ex}");
            throw;
        }

        return lista;
    }


    public Contrato ObtenerPorId(int id)
    {
        Contrato contrato = null;

        try
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @"
                    SELECT 
                        contratos.id,
                        contratos.id_inmueble,
                        contratos.id_inquilino,
                        contratos.monto,
                        contratos.fecha_inicio,
                        contratos.fecha_fin,
                        contratos.fecha_rescision,
                        contratos.multa,
                        contratos.creado_en,
                        contratos.actualizado_en,
                        i.nombre,
                        i.apellido,
                        im.Direccion,
                        u.nombre AS nombre_usuario_creador,
                        u.apellido AS apellido_usuario_creador,
                        u.id AS id_usuario_creador,
                        uf.id                 AS UsuarioFinalizadorId,
                        uf.nombre             AS UsuarioFinalizadorNombre,
                        uf.apellido           AS UsuarioFinalizadorApellido
                    FROM contratos
                    JOIN inquilinos i ON contratos.id_inquilino = i.id
                    Join inmuebles im ON contratos.id_inmueble = im.id
                    Join Usuarios u ON contratos.usuario_creador_id = u.id
                    LEFT JOIN usuarios uf ON contratos.usuario_finalizador_id = uf.id
                    
                    WHERE contratos.id = @id;
                    
            ";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            contrato = new Contrato
                            {
                                Id = reader.GetInt32("id"),
                                Id_Inmueble = reader.GetInt32("id_inmueble"),
                                Id_Inquilino = reader.GetInt32("id_inquilino"),
                                Monto = reader.GetDecimal("monto"),
                                Fecha_Inicio = reader.GetDateTime("fecha_inicio"),
                                Fecha_Fin = reader.GetDateTime("fecha_fin"),
                                Fecha_Rescision = reader.IsDBNull(reader.GetOrdinal("fecha_rescision")) ? null : reader.GetDateTime("fecha_rescision"),
                                Multa = reader.IsDBNull(reader.GetOrdinal("multa")) ? null : reader.GetDecimal("multa"),
                                Creado_En = reader.GetDateTime("creado_en"),
                                Actualizado_En = reader.GetDateTime("actualizado_en"),
                                Inquilino = new Inquilino
                                {
                                    Nombre = reader.GetString("nombre"),
                                    Apellido = reader.GetString("apellido")
                                },
                                Inmueble = new Inmueble
                                {
                                    Direccion = reader.GetString("Direccion")
                                },
                                UsuarioCreador = new Usuario
                                {
                                    Id = reader.GetInt32("id_usuario_creador"),
                                    Nombre = reader.GetString("nombre_usuario_creador"),
                                    Apellido = reader.GetString("apellido_usuario_creador")
                                },
                                UsuarioFinalizador = reader.IsDBNull(reader.GetOrdinal("UsuarioFinalizadorId")) ? null : new Usuario
                                {
                                    Id = reader.GetInt32("UsuarioFinalizadorId"),
                                    Nombre = reader.GetString("UsuarioFinalizadorNombre"),
                                    Apellido = reader.GetString("UsuarioFinalizadorApellido")
                                }
                            };
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERROR en ObtenerPorId: {ex}");
            throw;
        }

        return contrato;
    }

    public int Alta(Contrato p, int usuarioId)
    {
        int idGenerado = 0;
        try
        {
            using (var connection = new MySqlConnection(connectionString))
            {

                var sql = @"
                INSERT INTO contratos 
                    (id_inmueble, id_inquilino, monto, fecha_inicio, fecha_fin, fecha_rescision, multa, creado_en, actualizado_en, usuario_creador_id)
                VALUES 
                    (@id_inmueble, @id_inquilino, @monto, @fecha_inicio, @fecha_fin, @fecha_rescision, @multa, NOW(), NOW(), @usuarioId);
            ";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id_inmueble", p.Id_Inmueble);
                    command.Parameters.AddWithValue("@id_inquilino", p.Id_Inquilino);
                    command.Parameters.AddWithValue("@monto", p.Monto);
                    command.Parameters.AddWithValue("@fecha_inicio", p.Fecha_Inicio);
                    command.Parameters.AddWithValue("@fecha_fin", p.Fecha_Fin);
                    command.Parameters.AddWithValue("@fecha_rescision", p.Fecha_Rescision ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@multa", p.Multa ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@usuarioId", usuarioId);

                    connection.Open();
                    command.ExecuteNonQuery();
                    idGenerado = (int)command.LastInsertedId;
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
        int filasAfectadas = 0;

        try
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @"
                DELETE FROM contratos
                WHERE id = @id;
            ";

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
            Console.WriteLine($"ERROR en Baja: {ex}");
            throw;
        }

        return filasAfectadas;
    }

    public int Modificacion(Contrato entidad, int usuarioId)
    {
        int filasAfectadas = 0;

        try
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @"
                UPDATE contratos
                SET id_inmueble = @id_inmueble,
                    id_inquilino = @id_inquilino,
                    monto = @monto,
                    fecha_inicio = @fecha_inicio,
                    fecha_fin = @fecha_fin,
                    fecha_rescision = @fecha_rescision,
                    multa = @multa,
                    actualizado_en = NOW(),
                    usuario_finalizador_id = @usuarioId
                WHERE id = @id;
            ";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", entidad.Id);
                    command.Parameters.AddWithValue("@id_inmueble", entidad.Id_Inmueble);
                    command.Parameters.AddWithValue("@id_inquilino", entidad.Id_Inquilino);
                    command.Parameters.AddWithValue("@monto", entidad.Monto);
                    command.Parameters.AddWithValue("@fecha_inicio", entidad.Fecha_Inicio);
                    command.Parameters.AddWithValue("@fecha_fin", entidad.Fecha_Fin);
                    command.Parameters.AddWithValue("@fecha_rescision", entidad.Fecha_Rescision ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@multa", entidad.Multa ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@usuarioId", usuarioId);

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



    public IList<Contrato> ObtenerContratosPorInmueble(int q)
    {
        Contrato contrato = null;
        List<Contrato> res = new List<Contrato>();

        try
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @"
                SELECT c.id, c.id_inmueble, c.id_inquilino, c.fecha_inicio, c.fecha_fin, c.fecha_rescision, c.monto
                FROM contratos c
                WHERE 
                     c.id_inmueble = @id_inmueble
                     ;
            ";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id_inmueble", q);
                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            contrato = new Contrato
                            {
                                Id = reader.GetInt32("id"),
                                Id_Inmueble = reader.GetInt32("id_inmueble"),
                                Id_Inquilino = reader.GetInt32("id_inquilino"),
                                Fecha_Inicio = reader.GetDateTime("fecha_inicio"),
                                Fecha_Fin = reader.GetDateTime("fecha_fin"),
                                Fecha_Rescision = reader.IsDBNull(reader.GetOrdinal("fecha_rescision"))
                                    ? null
                                    : reader.GetDateTime("fecha_rescision"),
                                Monto = reader.GetDecimal("monto")
                            };
                            res.Add(contrato);
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERROR en ObtenerFechasOcupadasPorInmueble: {ex.Message}");
            throw;
        }
        // foreach (var c in res)
        // {
        //     Console.WriteLine($"Contrato ID: {c.Id}, Inmueble: {c.Id_Inmueble}, Inquilino: {c.Id_Inquilino}, Desde: {c.Fecha_Inicio.ToShortDateString()} hasta {c.Fecha_Fin.ToShortDateString()}");
        // }

        return res;
    }

    public IList<Contrato> ObtenerContratosPorInquilino(int q)
    {
        Contrato contrato = null;
        List<Contrato> res = new List<Contrato>();

        try
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @"
                SELECT c.id, c.id_inmueble, c.id_inquilino, c.fecha_inicio, c.fecha_fin, c.fecha_rescision, c.monto,
                i.Direccion
                FROM contratos c
                JOIN inmuebles i ON c.id_inmueble = i.id
                WHERE 
                     c.id_inquilino = @id_inquilino
                     and c.fecha_rescision IS NULL;
            ";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id_inquilino", q);
                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            contrato = new Contrato
                            {
                                Id = reader.GetInt32("id"),
                                Id_Inmueble = reader.GetInt32("id_inmueble"),
                                Id_Inquilino = reader.GetInt32("id_inquilino"),
                                Fecha_Inicio = reader.GetDateTime("fecha_inicio"),
                                Fecha_Fin = reader.GetDateTime("fecha_fin"),
                                Fecha_Rescision = reader.IsDBNull(reader.GetOrdinal("fecha_rescision"))
                                    ? null
                                    : reader.GetDateTime("fecha_rescision"),
                                Monto = reader.GetDecimal("monto"),
                                Inmueble = new Inmueble
                                {
                                    Direccion = reader.GetString("Direccion")
                                }
                            };
                            res.Add(contrato);
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERROR en ObtenerFechasOcupadasPorInquilino: {ex.Message}");
            throw;
        }

        return res;
    }

    public int Alta(Contrato p)
    {
        throw new NotImplementedException();
    }

    public int Modificacion(Contrato entidad)
    {
        throw new NotImplementedException();
    }

    public int ContarContratosFilter(DateTime? fechaDesde, DateTime? fechaHasta, int? diasVencimiento, int? idInmuebleSeleccionado)
    {
        int count = 0;
        using var conn = new MySqlConnection(connectionString);
        var sql = @"SELECT COUNT(*) 
            FROM contratos c
            JOIN inquilinos i ON c.id_inquilino = i.id
            WHERE 1 = 1";

        if (fechaDesde.HasValue && fechaHasta.HasValue)
        {
            sql += @"
              AND (
                  c.fecha_inicio <= @fechaHasta
                  AND COALESCE(c.fecha_rescision, c.fecha_fin) >= @fechaDesde
              )";
        }

        if (diasVencimiento.HasValue)
        {
            sql += @"
            AND (
                COALESCE(c.fecha_rescision, c.fecha_fin)
                BETWEEN CURDATE() AND DATE_ADD(CURDATE(), INTERVAL @diasVencimiento DAY)
            )";
        }
        if (idInmuebleSeleccionado.HasValue && idInmuebleSeleccionado.Value > 0)
        {
            sql += @"
              AND c.id_inmueble = @idInmuebleSeleccionado
            ";
        }
        using var cmd = new MySqlCommand(sql, conn);



        // 5) Agrego solo los parámetros que correspondan
        if (fechaDesde.HasValue && fechaHasta.HasValue)
        {
            cmd.Parameters.AddWithValue("@fechaDesde", fechaDesde.Value.Date);
            cmd.Parameters.AddWithValue("@fechaHasta", fechaHasta.Value.Date);
        }
        if (diasVencimiento.HasValue)
        {
            cmd.Parameters.AddWithValue("@diasVencimiento", diasVencimiento.Value);
        }
        if (idInmuebleSeleccionado.HasValue && idInmuebleSeleccionado.Value > 0)
        {
            cmd.Parameters.AddWithValue("@idInmuebleSeleccionado", idInmuebleSeleccionado.Value);
        }

        conn.Open();
        count = Convert.ToInt32(cmd.ExecuteScalar());
        return count;

    }
}