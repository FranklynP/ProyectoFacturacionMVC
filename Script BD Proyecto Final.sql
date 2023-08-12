USE master
go
DROP DATABASE IF EXISTS ProyectoFacturacionDB;
go
CREATE DATABASE ProyectoFacturacionDB
go
USE ProyectoFacturacionDB

go
CREATE TABLE Empleados (
    id INT IDENTITY(1,1) PRIMARY KEY,
    nombre VARCHAR(50) NOT NULL,
    apellido VARCHAR(50) NOT NULL,
    comision INT CHECK (comision >= 0 AND comision <= 100) NOT NULL,
    activo BIT DEFAULT 1 NOT NULL,
    fecha DATE DEFAULT GETDATE() NOT NULL
);

go
CREATE TABLE Articulos (
    id INT IDENTITY(1,1) PRIMARY KEY,
    descripcion VARCHAR(100) NOT NULL,
    precio DECIMAL(11,2) NOT NULL,
	stock INT NOT NULL,
    activo BIT DEFAULT 1 NOT NULL,
    fecha DATE DEFAULT GETDATE() NOT NULL
);

go
CREATE TABLE Clientes (
    id INT IDENTITY(1,1) PRIMARY KEY,
    nombre VARCHAR(100) NOT NULL,
	apellido VARCHAR(100) NOT NULL,
    rnc INT NOT NULL,
	cedula varchar(15) NOT NULL,
	cuenta_contable_id INT NOT NULL,
    fecha DATE DEFAULT GETDATE() NOT NULL
);

go
CREATE TABLE Facturas (
	id INT IDENTITY(1,1) PRIMARY KEY,
	num_factura INT NOT NULL,
	id_cliente INT NOT NULL,
	id_vendedor INT NOT NULL,
	comprobante INT,
	subtotal DECIMAL(11,2) NOT NULL,
	itbis DECIMAL(11,2) DEFAULT 0.18 NOT NULL, 
	total DECIMAL(11,2) NOT NULL,
	comentario TEXT,
	tipo_factura VARCHAR(50) NOT NULL DEFAULT 'venta',
	fecha_creacion DATE DEFAULT GETDATE() NOT NULL
);

go
CREATE TABLE ArticulosFactura (
	id INT IDENTITY(1,1) PRIMARY KEY,
	id_factura INT NOT NULL,
	id_articulo INT NOT NULL,
	cantidad INT NOT NULL,
	precio_unitario DECIMAL(11,2) NOT NULL,
	fecha DATE DEFAULT GETDATE() NOT NULL
);

go

CREATE TABLE CuentaContable (
	id INT PRIMARY KEY,
	descripcion VARCHAR(100) NOT NULL
);

go

CREATE TABLE AsientoContable (
	id INT IDENTITY(1,1) PRIMARY KEY,
	descripcion VARCHAR(100) NOT NULL,
	cuenta_db INT NOT NULL,
	cuenta_cr INT NOT NULL,
	monto DECIMAL(11,2) NOT NULL,
	estado BIT DEFAULT 0 NOT NULL,
	fecha_creacion DATE DEFAULT GETDATE() NOT NULL,
	id_factura INT NOT NULL,
);

go

INSERT INTO CuentaContable (id, descripcion) 
VALUES 
(1, 'Activos'),
(2, 'Efectivo en caja y banco'),
(3, 'Caja Chica'),
(4, 'Cuenta Corriente Banco X'),
(5, 'Inventarios y Mercancias'),
(6, 'test'),
(7, 'Cuentas x Cobrar'),
(8, 'Cuentas x Cobrar Cliente X'),
(12, 'Ventas'),
(13, 'Ingresos x Ventas'),
(47, 'Gastos'),
(48, 'Gastos Administrativos'),
(50, 'Gastos Generales'),
(65, 'Gasto depreciacion Activos Fijos'),
(66, 'Depreciacion Acumulada Activos Fijos'),
(70, 'Salarios y Sueldos Empleados'),
(71, 'Gastos de Nomina Empresa'),
(80, 'Compra de Mercancias'),
(81, 'Cuentas x Pagar');

go
SELECT * FROM Empleados
SELECT * FROM Articulos
SELECT * FROM Clientes
SELECT * FROM Facturas
SELECT * FROM ArticulosFactura
SELECT * FROM CuentaContable
SELECT * FROM AsientoContable