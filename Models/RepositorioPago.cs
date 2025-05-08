using MySql.Data.MySqlClient;

namespace Inmobiliaria_Avgustin.Models
{
    public class RepositorioPago : RepositorioBase, IRepositorioPago
    {

        public RepositorioPago(IConfiguration context) : base(context) { }

        IList<Pago> IRepositorio<Pago>.ObtenerTodos()
        {
            throw new NotImplementedException();
        }

        public Pago ObtenerPorId(int id)
        {
            Pago pago = null;
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = @"
            SELECT p.*, 
                   ua.id AS ua_id, ua.nombre AS ua_nombre, ua.apellido AS ua_apellido,
                   uf.id AS uf_id, uf.nombre AS uf_nombre, uf.apellido AS uf_apellido
            FROM pagos p
            LEFT JOIN usuarios ua ON p.usuario_alta_id = ua.id
            LEFT JOIN usuarios uf ON p.usuario_anulador_id = uf.id
            WHERE p.id = @Id";

                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            pago = new Pago
                            {
                                Id = reader.GetInt32("id"),
                                ContratoId = reader.IsDBNull(reader.GetOrdinal("contrato_id")) ? 0 : reader.GetInt32("contrato_id"),
                                NumeroPago = reader.IsDBNull(reader.GetOrdinal("numero_pago")) ? 0 : reader.GetInt32("numero_pago"),
                                FechaPago = reader.IsDBNull(reader.GetOrdinal("fecha_pago")) ? (DateTime?)null : reader.GetDateTime("fecha_pago"),
                                Concepto = reader.IsDBNull(reader.GetOrdinal("concepto")) ? string.Empty : reader.GetString("concepto"),
                                Importe = reader.IsDBNull(reader.GetOrdinal("importe")) ? 0m : reader.GetDecimal("importe"),
                                Anulado = reader.IsDBNull(reader.GetOrdinal("anulado")) ? false : reader.GetBoolean("anulado"),
                                CreadoEn = reader.IsDBNull(reader.GetOrdinal("creado_en")) ? DateTime.MinValue : reader.GetDateTime("creado_en"),
                                AnuladoEn = reader.IsDBNull(reader.GetOrdinal("anulado_en")) ? (DateTime?)null : reader.GetDateTime("anulado_en"),
                                Fecha_Vencimiento = reader.IsDBNull(reader.GetOrdinal("fecha_vencimiento")) ? DateTime.MinValue : reader.GetDateTime("fecha_vencimiento"),

                                UsuarioCreador = reader.IsDBNull(reader.GetOrdinal("ua_id")) ? null : new Usuario
                                {
                                    Id = reader.GetInt32("ua_id"),
                                    Nombre = reader.GetString("ua_nombre"),
                                    Apellido = reader.GetString("ua_apellido")
                                },
                                UsuarioAnulador = reader.IsDBNull(reader.GetOrdinal("uf_id")) ? null : new Usuario
                                {
                                    Id = reader.GetInt32("uf_id"),
                                    Nombre = reader.GetString("uf_nombre"),
                                    Apellido = reader.GetString("uf_apellido")
                                }
                            };
                        }
                    }
                }
            }
            return pago;
        }


        public int Alta(Pago p)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = "INSERT INTO Pagos (Contrato_Id, Numero_Pago, Concepto, Importe, Anulado, Fecha_Vencimiento) " +
                               "VALUES (@Contrato_Id, @NumeroPago, @Concepto, @Importe, @Anulado, @Fecha_Vencimiento)";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Contrato_Id", p.ContratoId);
                    command.Parameters.AddWithValue("@NumeroPago", p.NumeroPago);
                    command.Parameters.AddWithValue("@Concepto", p.Concepto);
                    command.Parameters.AddWithValue("@Importe", p.Importe);
                    command.Parameters.AddWithValue("@Anulado", p.Anulado);
                    command.Parameters.AddWithValue("@Fecha_Vencimiento", p.Fecha_Vencimiento);

                    return command.ExecuteNonQuery();
                }
            }
        }

        public int Modificacion(Pago entidad)
        {

            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = "UPDATE Pagos SET Concepto = @Concepto WHERE Id = @Id";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", entidad.Id);
                    command.Parameters.AddWithValue("@Concepto", entidad.Concepto);
                    return command.ExecuteNonQuery();
                }
            }
        }

        public int Baja(int id)
        {
            throw new NotImplementedException();
        }

        public IList<Pago> ObtenerPagosPorContrato(int contratoId)
        {

            IList<Pago> pagos = new List<Pago>();
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT * FROM Pagos WHERE contrato_Id = @contrato_Id";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@contrato_Id", contratoId);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Pago pago = new Pago
                            {
                                Id = reader.GetInt32("Id"),
                                ContratoId = reader.IsDBNull(reader.GetOrdinal("Contrato_Id")) ? 0 : reader.GetInt32("Contrato_Id"),
                                NumeroPago = reader.IsDBNull(reader.GetOrdinal("Numero_Pago")) ? 0 : reader.GetInt32("Numero_Pago"),
                                FechaPago = reader.IsDBNull(reader.GetOrdinal("Fecha_Pago")) ? (DateTime?)null : reader.GetDateTime("Fecha_Pago"),
                                Concepto = reader.IsDBNull(reader.GetOrdinal("Concepto")) ? string.Empty : reader.GetString("Concepto"),
                                Importe = reader.IsDBNull(reader.GetOrdinal("Importe")) ? 0m : reader.GetDecimal("Importe"),
                                Anulado = reader.IsDBNull(reader.GetOrdinal("Anulado")) ? false : reader.GetBoolean("Anulado"),
                                CreadoEn = reader.IsDBNull(reader.GetOrdinal("Creado_En")) ? DateTime.MinValue : reader.GetDateTime("Creado_En"),
                                AnuladoEn = reader.IsDBNull(reader.GetOrdinal("Anulado_En")) ? (DateTime?)null : reader.GetDateTime("Anulado_En"),
                                Fecha_Vencimiento = reader.IsDBNull(reader.GetOrdinal("Fecha_Vencimiento")) ? DateTime.MinValue : reader.GetDateTime("Fecha_Vencimiento")
                            };
                            pagos.Add(pago);
                        }
                    }
                }
            }
            return pagos;
        }

        public IList<Pago> ObtenerPagosPorInquilino(int inquilinoId)
        {
            throw new NotImplementedException();
        }

        public IList<Pago> ObtenerPagosPorPropietario(int propietarioId)
        {
            throw new NotImplementedException();
        }

        public int AnularPagosFueraDeRango(int contratoId, DateTime Rescision)
        {
            int filasAfectadas = 0;
            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    var sql = @"UPDATE Pagos
                        SET
                          anulado = 1,
                          anulado_en = NOW()
                        WHERE contrato_id = @contratoId
                          AND anulado = 0
                          AND fecha_vencimiento > @Rescision;
                        ";

                    using (var command = new MySqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@contratoId", contratoId);
                        command.Parameters.AddWithValue("@Rescision", Rescision);

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

        public int ObtenerUltimoNumeroPago(int contratoId)
        {
            int ultimoNumeroPago = 0;
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT MAX(Numero_Pago) FROM Pagos WHERE contrato_Id = @contrato_Id";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@contrato_Id", contratoId);
                    object result = command.ExecuteScalar();
                    if (result != DBNull.Value)
                    {
                        ultimoNumeroPago = Convert.ToInt32(result);
                    }
                }
            }
            return ultimoNumeroPago;
        }

        public int Anular(int id, int usuarioId)
        {
            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "UPDATE Pagos SET Anulado = 1, Anulado_En = NOW(), usuario_anulador_id = @usuarioId WHERE Id = @Id";
                    using (var command = new MySqlCommand(query, connection))
                    {

                        command.Parameters.AddWithValue("@usuarioId", usuarioId);
                        command.Parameters.AddWithValue("@Id", id);
                        return command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR en Anular: {ex}");
                throw;
            }
        }

        public int Pagar(Pago pago, DateTime fechaPago, int usuarioId)
        {
            // ver datos
            pago.FechaPago = fechaPago;
            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "UPDATE Pagos SET Fecha_Pago = @FechaPago, usuario_alta_id = @usuarioId WHERE Id = @Id";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Id", pago.Id);
                        command.Parameters.AddWithValue("@FechaPago", pago.FechaPago);
                        command.Parameters.AddWithValue("@usuarioId", usuarioId);
                        return command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR en Pagar: {ex}");
                throw;
            }
        }
    }



}