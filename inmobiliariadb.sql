-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Servidor: 127.0.0.1
-- Tiempo de generación: 08-05-2025 a las 02:41:00
-- Versión del servidor: 10.4.32-MariaDB
-- Versión de PHP: 8.2.12

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Base de datos: `inmobiliariadb`
--

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `contratos`
--

CREATE TABLE `contratos` (
  `id` int(11) NOT NULL,
  `id_inmueble` int(11) NOT NULL,
  `id_inquilino` int(11) NOT NULL,
  `monto` decimal(10,2) NOT NULL,
  `fecha_inicio` date NOT NULL,
  `fecha_fin` date NOT NULL,
  `fecha_rescision` date DEFAULT NULL,
  `multa` decimal(10,2) DEFAULT NULL,
  `usuario_creador_id` int(11) DEFAULT NULL,
  `usuario_finalizador_id` int(11) DEFAULT NULL,
  `creado_en` timestamp NOT NULL DEFAULT current_timestamp(),
  `actualizado_en` timestamp NOT NULL DEFAULT current_timestamp() ON UPDATE current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `contratos`
--

INSERT INTO `contratos` (`id`, `id_inmueble`, `id_inquilino`, `monto`, `fecha_inicio`, `fecha_fin`, `fecha_rescision`, `multa`, `usuario_creador_id`, `usuario_finalizador_id`, `creado_en`, `actualizado_en`) VALUES
(68, 70, 8, 97000.00, '2025-05-07', '2025-07-06', NULL, NULL, 1, NULL, '2025-05-08 00:08:55', '2025-05-08 00:08:55'),
(69, 62, 16, 60000.00, '2025-08-07', '2026-01-06', NULL, NULL, 1, NULL, '2025-05-08 00:09:57', '2025-05-08 00:09:57');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `imagenes`
--

CREATE TABLE `imagenes` (
  `Id` int(11) NOT NULL,
  `InmuebleId` int(11) NOT NULL,
  `Url` varchar(255) NOT NULL,
  `EsPortada` tinyint(1) NOT NULL DEFAULT 0
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `imagenes`
--

INSERT INTO `imagenes` (`Id`, `InmuebleId`, `Url`, `EsPortada`) VALUES
(44, 14, '/Uploads/Inmuebles/14/556b7309-20f3-46af-a21a-471be6128096.jpeg', 1),
(46, 14, '/Uploads/Inmuebles/14/6026c027-1a09-4837-8d8b-c5d577d54644.jpeg', 1),
(47, 14, '/Uploads/Inmuebles/14/ee9aaaad-9aff-4186-bdfc-bebc63e7a8fe.jpeg', 0),
(48, 14, '/Uploads/Inmuebles/14/52d4cf1c-3db5-4a1f-8c61-b326cb806c1c.jpeg', 0),
(49, 18, '/Uploads/Inmuebles/18/fe1605bf-3700-40da-8f3f-0b931bf9b319.jpeg', 1);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `inmuebles`
--

CREATE TABLE `inmuebles` (
  `Id` int(11) NOT NULL,
  `Direccion` varchar(255) NOT NULL,
  `Ambientes` int(11) NOT NULL,
  `Precio` int(11) NOT NULL,
  `Disponible` tinyint(1) DEFAULT 1,
  `id_propietario` int(11) DEFAULT NULL,
  `id_tipo_inmueble` int(11) DEFAULT NULL,
  `id_uso_inmueble` int(11) DEFAULT NULL,
  `Coordenadas` int(11) NOT NULL,
  `Superficie` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `inmuebles`
--

INSERT INTO `inmuebles` (`Id`, `Direccion`, `Ambientes`, `Precio`, `Disponible`, `id_propietario`, `id_tipo_inmueble`, `id_uso_inmueble`, `Coordenadas`, `Superficie`) VALUES
(5, 'Calle falsa', 2, 55000, 1, 5, NULL, NULL, 0, 0),
(14, 'AV siempre vivas', 3, 40000, 1, 5, 1, 1, 34555555, 78),
(18, 'calle 123', 3, 30000, 1, 5, 2, 1, 2321, 122),
(59, 'Av. Siempre Viva 742', 3, 95000, 1, 5, 1, 1, 123456, 80),
(60, 'Calle Falsa 123', 2, 85000, 0, 6, 2, 2, 654321, 60),
(61, 'San Martín 456', 4, 120000, 1, 5, 1, 1, 111222, 100),
(62, 'Belgrano 789', 1, 60000, 1, 6, 3, 2, 333444, 45),
(63, 'Mitre 1010', 3, 100000, 0, 7, 2, 1, 987654, 85),
(64, 'Lavalle 2020', 2, 95000, 1, 8, 1, 2, 555666, 70),
(65, 'Urquiza 3030', 5, 150000, 0, 9, 2, 2, 777888, 110),
(66, 'Rivadavia 4040', 3, 105000, 1, 5, 1, 1, 999000, 90),
(67, 'Castelli 5050', 2, 89000, 1, 6, 3, 1, 123123, 65),
(68, 'Cordoba 6060', 4, 110000, 1, 7, 2, 2, 321321, 95),
(69, 'Sarmiento 7070', 1, 50000, 0, 8, 1, 1, 456456, 40),
(70, 'Alsina 8080', 3, 97000, 1, 9, 3, 1, 789789, 75),
(71, 'Maipu 9090', 2, 87000, 0, 10, 2, 2, 147147, 55),
(72, 'España 111', 4, 119000, 1, 10, 1, 1, 258258, 100),
(73, 'Italia 222', 2, 86000, 1, 8, 2, 1, 369369, 60),
(74, 'Brasil 333', 3, 102000, 0, 10, 3, 2, 147258, 85),
(75, 'Uruguay 444', 5, 160000, 1, 7, 1, 2, 963852, 120),
(76, 'Perú 555', 3, 99000, 0, 7, 2, 2, 321654, 90),
(77, 'Colombia 666', 2, 93000, 1, 9, 1, 1, 852741, 75),
(78, 'Chile 777', 1, 67000, 1, 9, 3, 1, 741852, 50);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `inquilinos`
--

CREATE TABLE `inquilinos` (
  `Id` int(11) NOT NULL,
  `Nombre` varchar(50) NOT NULL,
  `Apellido` varchar(50) NOT NULL,
  `Dni` varchar(10) NOT NULL,
  `Email` varchar(100) NOT NULL,
  `Telefono` varchar(20) NOT NULL,
  `Trabajo` varchar(100) DEFAULT NULL,
  `Ingresos` decimal(10,2) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `inquilinos`
--

INSERT INTO `inquilinos` (`Id`, `Nombre`, `Apellido`, `Dni`, `Email`, `Telefono`, `Trabajo`, `Ingresos`) VALUES
(7, 'Laura', 'Pérez', '30500100', 'laura.perez@mail.com', '3512345678', 'Diseñadora UX', 210000.00),
(8, 'Carlos', 'Gómez', '29488555', 'carlosg@mail.com', '3519876543', 'Programador', 280000.00),
(9, 'Mariana', 'Suárez', '28700122', 'mariana.suarez@mail.com', '3516543210', 'Contadora', 190000.00),
(10, 'José', 'Ramírez', '30123999', 'jose.ramirez@mail.com', '3511122334', 'Chofer', 160000.00),
(11, 'Natalia', 'Ortiz', '31230988', 'natalia.ortiz@mail.com', '3519988776', 'Administrativa', 175000.00),
(13, 'Verónica', 'Castro', '29877321', 'vero.castro@mail.com', '3517766554', 'Psicóloga', 230000.00),
(14, 'Andrés', 'Martínez', '27544011', 'andres.martinez@mail.com', '3518899001', 'Técnico en redes', 200000.00),
(15, 'Gabriela', 'Silva', '30011234', 'gabriela.silva@mail.com', '3516677889', 'Abogada', 250000.00),
(16, 'Lucas', 'Rojas', '29009876', 'lucas.rojas@mail.com', '3513344556', 'Electricista', 170000.00),
(17, 'Silvina', 'Alonso', '30330110', 'silvina.alonso@mail.com', '3511112233', 'Enfermera', 185000.00),
(18, 'Esteban', 'Moreno', '29123456', 'esteban.moreno@mail.com', '3512223344', 'Mecánico', 165000.00),
(19, 'Lucía', 'Herrera', '28456987', 'lucia.herrera@mail.com', '3513334455', 'Docente', 195000.00),
(20, 'Matías', 'Vega', '30098765', 'matias.vega@mail.com', '3514445566', 'Chef', 180000.00),
(21, 'Florencia', 'Díaz', '29911223', 'florencia.diaz@mail.com', '3515556677', 'Marketing', 220000.00);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `pagos`
--

CREATE TABLE `pagos` (
  `id` int(11) NOT NULL,
  `contrato_id` int(11) NOT NULL,
  `usuario_alta_id` int(11) DEFAULT NULL,
  `usuario_anulador_id` int(11) DEFAULT NULL,
  `numero_pago` int(11) NOT NULL,
  `fecha_pago` date DEFAULT NULL,
  `concepto` varchar(200) DEFAULT NULL,
  `importe` decimal(10,2) NOT NULL,
  `anulado` tinyint(1) NOT NULL DEFAULT 0,
  `creado_en` timestamp NOT NULL DEFAULT current_timestamp(),
  `anulado_en` timestamp NULL DEFAULT NULL,
  `fecha_vencimiento` date NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `pagos`
--

INSERT INTO `pagos` (`id`, `contrato_id`, `usuario_alta_id`, `usuario_anulador_id`, `numero_pago`, `fecha_pago`, `concepto`, `importe`, `anulado`, `creado_en`, `anulado_en`, `fecha_vencimiento`) VALUES
(108, 68, NULL, NULL, 1, NULL, 'Alquiler Mensual', 97000.00, 0, '2025-05-08 00:08:55', NULL, '2025-05-08'),
(109, 68, NULL, NULL, 2, NULL, 'Alquiler Mensual', 97000.00, 0, '2025-05-08 00:08:55', NULL, '2025-06-08'),
(110, 69, NULL, NULL, 1, NULL, 'Alquiler Mensual', 60000.00, 0, '2025-05-08 00:09:57', NULL, '2025-08-08'),
(111, 69, NULL, NULL, 2, NULL, 'Alquiler Mensual', 60000.00, 0, '2025-05-08 00:09:57', NULL, '2025-09-08'),
(112, 69, NULL, NULL, 3, NULL, 'Alquiler Mensual', 60000.00, 0, '2025-05-08 00:09:57', NULL, '2025-10-08'),
(113, 69, NULL, NULL, 4, NULL, 'Alquiler Mensual', 60000.00, 0, '2025-05-08 00:09:57', NULL, '2025-11-08'),
(114, 69, NULL, NULL, 5, NULL, 'Alquiler Mensual', 60000.00, 0, '2025-05-08 00:09:57', NULL, '2025-12-08');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `propietarios`
--

CREATE TABLE `propietarios` (
  `Id` int(11) NOT NULL,
  `Nombre` varchar(50) NOT NULL,
  `Apellido` varchar(50) NOT NULL,
  `Dni` varchar(10) NOT NULL,
  `Email` varchar(100) NOT NULL,
  `Telefono` varchar(20) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `propietarios`
--

INSERT INTO `propietarios` (`Id`, `Nombre`, `Apellido`, `Dni`, `Email`, `Telefono`) VALUES
(5, 'Francisco', 'Avgustin', '81875645', 'franavtn@gmail.com', '64505369'),
(6, 'Juan', 'Pérez', '30123456', 'juan.perez@example.com', '1123456789'),
(7, 'Ana', 'González', '30234567', 'ana.gonzalez@example.com', '1134567890'),
(8, 'Carlos', 'Ramírez', '30345678', 'carlos.ramirez@example.com', '1145678901'),
(9, 'Laura', 'Martínez', '30456789', 'laura.martinez@example.com', '1156789012'),
(10, 'Marcos', 'Fernández', '30567890', 'marcos.fernandez@example.com', '1167890123'),
(11, 'Carlos', 'Ramire', '11044227', 'CarlosRamire123@gmail.com', '26649868'),
(12, 'Laura', 'Fernández', '30303030', 'laura.fernandez@mail.com', '1122334455'),
(13, 'Carlos', 'Pérez', '31313131', 'carlos.perez@mail.com', '1199887766'),
(14, 'María', 'Gómez', '32323232', 'maria.gomez@mail.com', '1177665544'),
(15, 'Federico', 'López', '33333333', 'federico.lopez@mail.com', '1166554433'),
(16, 'Luciana', 'Martínez', '34343434', 'luciana.martinez@mail.com', '1155443322');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `tipoinmueble`
--

CREATE TABLE `tipoinmueble` (
  `id` int(11) NOT NULL,
  `nombre` varchar(100) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `tipoinmueble`
--

INSERT INTO `tipoinmueble` (`id`, `nombre`) VALUES
(1, 'Casa'),
(2, 'Departamento'),
(3, 'Local'),
(4, 'Depósito'),
(5, 'PH'),
(6, 'Cabaña'),
(7, 'Oficina'),
(8, 'Galpón'),
(9, 'Terreno');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `usoinmueble`
--

CREATE TABLE `usoinmueble` (
  `id` int(11) NOT NULL,
  `nombre` varchar(50) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `usoinmueble`
--

INSERT INTO `usoinmueble` (`id`, `nombre`) VALUES
(1, 'Residencial'),
(2, 'Comercial');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `usuarios`
--

CREATE TABLE `usuarios` (
  `id` int(11) NOT NULL,
  `email` varchar(255) NOT NULL,
  `password` varchar(255) NOT NULL,
  `nombre` varchar(100) NOT NULL,
  `apellido` varchar(100) NOT NULL,
  `rol` enum('Administrador','Empleado') NOT NULL DEFAULT 'Empleado',
  `avatar_url` varchar(500) DEFAULT NULL,
  `creado_en` timestamp NOT NULL DEFAULT current_timestamp(),
  `actualizado_en` timestamp NOT NULL DEFAULT current_timestamp() ON UPDATE current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `usuarios`
--

INSERT INTO `usuarios` (`id`, `email`, `password`, `nombre`, `apellido`, `rol`, `avatar_url`, `creado_en`, `actualizado_en`) VALUES
(1, 'avgustinfrancisco1@inmobiliaria.com', '$2a$11$eAX3MjkykJIcP/YqwhVMduCvbsDnHHpy5lc3Oxmwl7SZm45lAEh9i', 'Francisco', 'Avgustini', 'Administrador', '/Avatars/1/fb194938-493a-4164-a540-5c64db927ffa.jpeg', '2025-05-01 14:18:09', '2025-05-07 01:26:09'),
(3, 'GermanGarmendia@gmail.com', '$2a$11$25Bwz4HeusdsUnM60kykQ.wIsCiEDhGbIDQ7MVS15V97TW3AYtVWq', 'German', 'Garmendia', 'Empleado', NULL, '2025-05-01 22:22:10', '2025-05-07 00:10:19');

--
-- Índices para tablas volcadas
--

--
-- Indices de la tabla `contratos`
--
ALTER TABLE `contratos`
  ADD PRIMARY KEY (`id`),
  ADD KEY `inmueble_id` (`id_inmueble`),
  ADD KEY `inquilino_id` (`id_inquilino`),
  ADD KEY `fk_contratos_usuario_creador` (`usuario_creador_id`),
  ADD KEY `fk_contratos_usuario_finalizador` (`usuario_finalizador_id`);

--
-- Indices de la tabla `imagenes`
--
ALTER TABLE `imagenes`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `IX_Imagenes_InmuebleId` (`InmuebleId`);

--
-- Indices de la tabla `inmuebles`
--
ALTER TABLE `inmuebles`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `fk_tipo_inmueble` (`id_tipo_inmueble`),
  ADD KEY `fk_uso_inmueble` (`id_uso_inmueble`),
  ADD KEY `fk_propietario` (`id_propietario`);

--
-- Indices de la tabla `inquilinos`
--
ALTER TABLE `inquilinos`
  ADD PRIMARY KEY (`Id`),
  ADD UNIQUE KEY `Dni` (`Dni`),
  ADD UNIQUE KEY `Email` (`Email`);

--
-- Indices de la tabla `pagos`
--
ALTER TABLE `pagos`
  ADD PRIMARY KEY (`id`),
  ADD KEY `fk_pagos_contrato` (`contrato_id`),
  ADD KEY `idx_pagos_usuario_anulador` (`usuario_anulador_id`),
  ADD KEY `idx_pagos_usuario_alta` (`usuario_alta_id`);

--
-- Indices de la tabla `propietarios`
--
ALTER TABLE `propietarios`
  ADD PRIMARY KEY (`Id`),
  ADD UNIQUE KEY `Dni` (`Dni`),
  ADD UNIQUE KEY `Email` (`Email`);

--
-- Indices de la tabla `tipoinmueble`
--
ALTER TABLE `tipoinmueble`
  ADD PRIMARY KEY (`id`);

--
-- Indices de la tabla `usoinmueble`
--
ALTER TABLE `usoinmueble`
  ADD PRIMARY KEY (`id`);

--
-- Indices de la tabla `usuarios`
--
ALTER TABLE `usuarios`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `email` (`email`);

--
-- AUTO_INCREMENT de las tablas volcadas
--

--
-- AUTO_INCREMENT de la tabla `contratos`
--
ALTER TABLE `contratos`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=70;

--
-- AUTO_INCREMENT de la tabla `imagenes`
--
ALTER TABLE `imagenes`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=57;

--
-- AUTO_INCREMENT de la tabla `inmuebles`
--
ALTER TABLE `inmuebles`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=79;

--
-- AUTO_INCREMENT de la tabla `inquilinos`
--
ALTER TABLE `inquilinos`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=22;

--
-- AUTO_INCREMENT de la tabla `pagos`
--
ALTER TABLE `pagos`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=115;

--
-- AUTO_INCREMENT de la tabla `propietarios`
--
ALTER TABLE `propietarios`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=17;

--
-- AUTO_INCREMENT de la tabla `tipoinmueble`
--
ALTER TABLE `tipoinmueble`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=10;

--
-- AUTO_INCREMENT de la tabla `usoinmueble`
--
ALTER TABLE `usoinmueble`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=3;

--
-- AUTO_INCREMENT de la tabla `usuarios`
--
ALTER TABLE `usuarios`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=6;

--
-- Restricciones para tablas volcadas
--

--
-- Filtros para la tabla `contratos`
--
ALTER TABLE `contratos`
  ADD CONSTRAINT `contratos_ibfk_1` FOREIGN KEY (`id_inmueble`) REFERENCES `inmuebles` (`Id`),
  ADD CONSTRAINT `contratos_ibfk_2` FOREIGN KEY (`id_inquilino`) REFERENCES `inquilinos` (`Id`),
  ADD CONSTRAINT `fk_contratos_usuario_creador` FOREIGN KEY (`usuario_creador_id`) REFERENCES `usuarios` (`id`) ON DELETE SET NULL ON UPDATE CASCADE,
  ADD CONSTRAINT `fk_contratos_usuario_finalizador` FOREIGN KEY (`usuario_finalizador_id`) REFERENCES `usuarios` (`id`) ON DELETE SET NULL ON UPDATE CASCADE;

--
-- Filtros para la tabla `imagenes`
--
ALTER TABLE `imagenes`
  ADD CONSTRAINT `FK_Imagenes_Inmuebles` FOREIGN KEY (`InmuebleId`) REFERENCES `inmuebles` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- Filtros para la tabla `inmuebles`
--
ALTER TABLE `inmuebles`
  ADD CONSTRAINT `fk_propietario` FOREIGN KEY (`id_propietario`) REFERENCES `propietarios` (`Id`),
  ADD CONSTRAINT `fk_tipo_inmueble` FOREIGN KEY (`id_tipo_inmueble`) REFERENCES `tipoinmueble` (`id`),
  ADD CONSTRAINT `fk_uso_inmueble` FOREIGN KEY (`id_uso_inmueble`) REFERENCES `usoinmueble` (`id`);

--
-- Filtros para la tabla `pagos`
--
ALTER TABLE `pagos`
  ADD CONSTRAINT `fk_pagos_contrato` FOREIGN KEY (`contrato_id`) REFERENCES `contratos` (`id`) ON DELETE CASCADE,
  ADD CONSTRAINT `fk_pagos_usuario_alta` FOREIGN KEY (`usuario_alta_id`) REFERENCES `usuarios` (`id`) ON DELETE SET NULL ON UPDATE CASCADE,
  ADD CONSTRAINT `fk_pagos_usuario_anulador` FOREIGN KEY (`usuario_anulador_id`) REFERENCES `usuarios` (`id`) ON DELETE SET NULL ON UPDATE CASCADE;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
