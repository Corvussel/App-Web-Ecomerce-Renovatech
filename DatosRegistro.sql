Use SistemaProducto
GO

-- Insertar el primer rol
EXEC sp_InsertarRol @NombreRol = 'ADMIN'; 
EXEC sp_InsertarRol @NombreRol = 'USER';
 
-- Primer registro
EXEC sp_RegistrarCategoria @nombre = 'Electrónicos'; 
EXEC sp_RegistrarCategoria @nombre = 'Ropa';


-- First insertion


EXEC sp_InsertarProducto
    @nombre = 'Laptop HP Pavilion',
    @descripcion = 'Laptop HP Pavilion con procesador Intel Core i5, 8GB RAM, 256GB SSD',
    @precio = 799.99,
    @stock = 50,
    @id_categoria = 1,
    @url_imagen = 'wwww.google.com';

-- Second insertion
EXEC sp_InsertarProducto
    @nombre = 'Smartphone Samsung Galaxy S21',
    @descripcion = 'Smartphone Samsung Galaxy S21 con pantalla AMOLED de 6.2", 128GB almacenamiento',
    @precio = 799.99,
    @stock = 100,
    @id_categoria = 2,
    @url_imagen = 'wwww.google.com';


	--BUSCAR POR CATEGORIA 
EXEC sp_BuscarProductosPorCategoria @id_categoria = 1;

    --Descripcion de producto
EXEC InsertarDetalleProducto @id_producto = 1, @descripcion_detallada = 'Descripción detallada', @descripcion_preparacion = 'Preparación del producto', @tipo_mensaje = 'Solo Disfruta el momente';


-- Declaración de variables para los parámetros
DECLARE @nombre NVARCHAR(100) = 'Producto de Prueba';
DECLARE @descripcion NVARCHAR(MAX) = 'Descripción corta del producto de prueba';
DECLARE @precio DECIMAL(10, 2) = 99.99;
DECLARE @stock INT = 100;
DECLARE @id_categoria INT = 1; -- Asegúrate de que esta categoría exista en tu tabla de categorías
DECLARE @url_imagen NVARCHAR(MAX) = 'https://ejemplo.com/imagen.jpg';
DECLARE @descripcion_detallada NVARCHAR(MAX) = 'Esta es una descripción detallada del producto de prueba. Incluye todas las características importantes.';
DECLARE @descripcion_preparacion NVARCHAR(MAX) = 'Instrucciones paso a paso para preparar o usar el producto de prueba.';
DECLARE @tipo_mensaje NVARCHAR(50) = 'Inserción de prueba';

-- Ejecutar el procedimiento almacenado
EXEC sp_InsertarProductoConDetalle
    @nombre,
    @descripcion,
    @precio,
    @stock,
    @id_categoria,
    @url_imagen,
    @descripcion_detallada,
    @descripcion_preparacion,
    @tipo_mensaje;
	 
 


EXEC ObtenerProductoDetalle 1

EXEC sp_BuscarProductosPorID  1


SELECT *FROM TRANSACCION
SELECT *FROM ROLES
SELECT *FROM USUARIO
SELECT *FROM PRODUCTO
SELECT *FROM DETALLE_PRODUCTO
SELECT *FROM CATEGORIA
SELECT *FROM FAVORITOS
SELECT *FROM RESEÑA_PRODUCTO
 


DELETE FROM CATEGORIA WHERE id_categoria>1
DELETE FROM USUARIO WHERE id_usuario>0
DELETE FROM ROLES WHERE id_rol>0 
DELETE FROM TRANSACCION WHERE id_producto>1 
DELETE FROM DETALLE_PRODUCTO WHERE id_producto>1

EXEC sp_ValidarCredenciales 'admin@gmail.com','123', 'SISE2024'