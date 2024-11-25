
Create database SistemaProducto
GO
Use SistemaProducto
GO


CREATE TABLE ROLES (
    id_rol INT IDENTITY(1,1) PRIMARY KEY,
    nombre_rol NVARCHAR(50) NOT NULL UNIQUE
);
GO

CREATE TABLE USUARIO (
    id_usuario INT IDENTITY(1,1) PRIMARY KEY,
    nombre NVARCHAR(100) NOT NULL,
    email NVARCHAR(100) NOT NULL UNIQUE,
    contrasena_hash VARBINARY(MAX) NOT NULL,
    direccion NVARCHAR(255),
    telefono NVARCHAR(15), 
    ciudad NVARCHAR(100), 
    codigo_postal NVARCHAR(10), 
    fecha_registro DATETIME2 DEFAULT GETDATE(),
    id_rol INT,
    url_imagen NVARCHAR(MAX),
    FOREIGN KEY (id_rol) REFERENCES ROLES(id_rol)
);
GO

CREATE TABLE CATEGORIA(
 id_categoria INT PRIMARY KEY IDENTITY(1,1),
 nombre VARCHAR(100),
 url_imagen NVARCHAR(MAX)
)
GO


 -- producto

CREATE TABLE PRODUCTO (
    id_producto INT IDENTITY(1,1) PRIMARY KEY,
	id_usuario INT,
    nombre NVARCHAR(100) NOT NULL,
    descripcion NVARCHAR(MAX),
    precio DECIMAL(10, 2) NOT NULL,
    stock INT NOT NULL,
    id_categoria INT,
	url_imagen NVARCHAR(MAX),
    FOREIGN KEY (id_usuario) REFERENCES USUARIO(id_usuario),
    FOREIGN KEY (id_categoria) REFERENCES CATEGORIA(id_categoria) 
);
GO


CREATE TABLE DETALLE_PRODUCTO (
    id_detalle_producto INT IDENTITY(1,1) PRIMARY KEY,
    id_producto INT NOT NULL,
    descripcion_detallada NVARCHAR(MAX), 
    FOREIGN KEY (id_producto) REFERENCES PRODUCTO(id_producto)
);
GO

CREATE TABLE FAVORITOS (
    id_usuario INT NOT NULL,
    id_producto INT NOT NULL,
    fecha_agregado DATETIME2 DEFAULT GETDATE(),
    PRIMARY KEY (id_usuario, id_producto),
    FOREIGN KEY (id_usuario) REFERENCES USUARIO(id_usuario) ON DELETE CASCADE,
    FOREIGN KEY (id_producto) REFERENCES PRODUCTO(id_producto) ON DELETE CASCADE
);
GO


 CREATE TABLE TRANSACCION (
    id_transaccion INT IDENTITY(1,1) PRIMARY KEY,
    id_usuario INT NOT NULL,
    id_producto INT NOT NULL,
    cantidad INT NOT NULL,
    precio_total DECIMAL(10, 2) NOT NULL,
    direccion NVARCHAR(255),
    numero_telefonico NVARCHAR(15),
    nombre_titular NVARCHAR(100),
    numero_tarjeta NVARCHAR(20),
    fecha_expiracion DATE, 
    cvv NVARCHAR(4),
    fecha_transaccion DATETIME2 DEFAULT GETDATE(),
    FOREIGN KEY (id_usuario) REFERENCES USUARIO(id_usuario),
    FOREIGN KEY (id_producto) REFERENCES PRODUCTO(id_producto)
);
GO

---ROLES


CREATE TABLE RESEÑA_PRODUCTO (
    reseña_id INT IDENTITY(1,1) PRIMARY KEY,
    id_producto INT NOT NULL,
    id_usuario INT NOT NULL,
    valoracion_estrellas INT CHECK (valoracion_estrellas BETWEEN 1 AND 5),
    titulo_reseña NVARCHAR(50) NOT NULL,
    reseña NVARCHAR(MAX),  
    FOREIGN KEY (id_usuario) REFERENCES USUARIO(id_usuario),
    FOREIGN KEY (id_producto) REFERENCES PRODUCTO(id_producto),
	INDEX idx_producto (id_producto),
    INDEX idx_usuario (id_usuario)
);
GO



CREATE PROCEDURE sp_InsertarRol
    @NombreRol NVARCHAR(50)
AS
BEGIN
    INSERT INTO ROLES (nombre_rol)
    VALUES (@NombreRol);
END;
GO
 
--USUARIO
CREATE PROCEDURE MostrarUsuarios
AS
BEGIN
    SELECT * FROM USUARIO;
END; 
GO

 

CREATE PROCEDURE sp_BuscarUsuarioPorID
    @id_usuario INT
AS
BEGIN  
    SELECT 
        id_usuario, 
        nombre,  
        direccion, 
        telefono, 
        ciudad, 
        codigo_postal, 
        fecha_registro,
        id_rol,
        url_imagen,
		email
    FROM 
        Usuario
    WHERE 
        id_usuario = @id_usuario;
END
GO
 

CREATE PROCEDURE sp_RegistrarUsuario
    @nombre NVARCHAR(100),
    @email NVARCHAR(100),
    @contrasena NVARCHAR(255),
    @direccion NVARCHAR(255) = NULL,
    @telefono NVARCHAR(15) = NULL,
    @ciudad NVARCHAR(100) = NULL,
    @codigo_postal NVARCHAR(10) = NULL, 
    @url_imagen NVARCHAR(MAX) = NULL,
    @nombre_rol VARCHAR(50) = NULL,
    @patron VARCHAR(60) 
AS
BEGIN
     
    IF EXISTS (SELECT 1 FROM USUARIO WHERE email = @email)
    BEGIN
        RETURN;
    END
	 
    DECLARE @ID_ROL INT;

    IF @nombre_rol IS NULL
    BEGIN
        SET @nombre_rol = 'USER';
    END

	-- si la url es nula se asigna una imagen por defecto
	IF @url_imagen IS NULL
	BEGIN
	  SET @url_imagen = 'https://firebasestorage.googleapis.com/v0/b/carkexfirebase.appspot.com/o/Fotos_perfil%2F142d9cd4-ad8c-400b-b556-bedede55aa0b.png?alt=media&token=1e02cc62-4db6-4771-9d5a-4f31e50093b5'
	END

    SET @ID_ROL = (SELECT id_rol FROM ROLES WHERE nombre_rol = @nombre_rol);

    IF @ID_ROL IS NULL
    BEGIN
        RETURN;
    END

    INSERT INTO USUARIO (nombre, email, contrasena_hash, direccion, telefono, ciudad, codigo_postal, fecha_registro, id_rol, url_imagen)
    VALUES (@nombre, @email, ENCRYPTBYPASSPHRASE(@patron, @contrasena), @direccion, @telefono, @ciudad, @codigo_postal, GETDATE(), @ID_ROL, @url_imagen);

    DECLARE @nuevoUsuarioId INT = SCOPE_IDENTITY();

    DECLARE @nombreRol NVARCHAR(50);

    SELECT @nombreRol = nombre_rol FROM ROLES WHERE id_rol = @ID_ROL; 

    SELECT @nuevoUsuarioId AS UsuarioId, @nombreRol AS NombreRol;
END
GO


CREATE PROCEDURE ActualizarUsuario 
    @id_usuario INT,
    @nombre NVARCHAR(100), 
    @contrasena NVARCHAR(255),   
    @direccion NVARCHAR(255) = NULL,
    @telefono NVARCHAR(15) = NULL,
    @ciudad NVARCHAR(100) = NULL,
    @codigo_postal NVARCHAR(10) = NULL,
    @url_imagen NVARCHAR(MAX) = NULL,
    @patron VARCHAR(60) 
AS
BEGIN
    UPDATE USUARIO
        SET
            nombre = @nombre, 
            contrasena_hash = ENCRYPTBYPASSPHRASE(@patron, @contrasena),   
            direccion = @direccion,
            telefono = @telefono,
            ciudad = @ciudad,
            codigo_postal = @codigo_postal,
            url_imagen = @url_imagen
        WHERE id_usuario = @id_usuario;    
END;
GO



CREATE PROCEDURE sp_VerificarCuenta
    @Email NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON; 
   
    SELECT COUNT(*)
    FROM USUARIO
    WHERE email = @Email; 
END;
GO

CREATE PROCEDURE sp_ValidarCredenciales
    @Email NVARCHAR(100),
    @contrasena NVARCHAR(255),
    @patron NVARCHAR(255)
AS
BEGIN  
 
    SELECT U.id_usuario, R.nombre_rol
    FROM USUARIO U
    INNER JOIN ROLES R
        ON U.id_rol = R.id_rol
    WHERE U.email = @Email 
      AND CONVERT(VARCHAR(512), DECRYPTBYPASSPHRASE(@patron, U.contrasena_hash)) = @contrasena; 
END;
GO

 
--categoria

CREATE PROCEDURE sp_ListarCategorias
AS
BEGIN
    SELECT id_categoria, nombre , url_imagen
    FROM CATEGORIA
    ORDER BY nombre;
END;
GO

CREATE PROCEDURE sp_BuscarCategoriaID
 @id_categoria INT
AS
BEGIN
    SELECT id_categoria, nombre , url_imagen
    FROM CATEGORIA
	WHERE id_categoria = @id_categoria
    ORDER BY nombre;
END;
GO
 

CREATE PROCEDURE sp_RegistrarCategoria
    @nombre VARCHAR(100),
	@url_imagen NVARCHAR(MAX) = NULL
AS
BEGIN
    INSERT INTO CATEGORIA (nombre, url_imagen)
    VALUES (@nombre,@url_imagen); 
    SELECT SCOPE_IDENTITY() AS NuevaCategoriaID;
END;
GO

CREATE PROCEDURE sp_ActualizarCategoria
    @id_categoria INT,
    @nombre VARCHAR(100),
    @url_imagen NVARCHAR(MAX)
AS
BEGIN
    
    IF EXISTS (SELECT 1 FROM CATEGORIA WHERE id_categoria = @id_categoria)
    BEGIN
         
        UPDATE CATEGORIA
        SET nombre = @nombre,
            url_imagen = @url_imagen
        WHERE id_categoria = @id_categoria;
 
    END 
END
GO


--Producto

CREATE PROCEDURE sp_MostrarProductos
AS
BEGIN
    SELECT * FROM PRODUCTO;
END;
GO 


CREATE PROCEDURE sp_ObtenerUltimoProducto
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP 1 
        id_producto,
		id_usuario,
        nombre,
        descripcion,
        precio,
        stock,
        id_categoria,
        url_imagen
    FROM PRODUCTO
    ORDER BY id_producto DESC;
END;
GO


CREATE PROCEDURE sp_BuscarProductosPorCategoria
    @id_categoria INT = NULL,
    @nombre_categoria VARCHAR(100) = NULL
AS
BEGIN
   

    SELECT 
        p.id_producto,
		p.id_usuario,
        p.nombre AS nombre_producto,
        p.descripcion,
        p.precio,
        p.stock,
        p.url_imagen,
        c.id_categoria,
        c.nombre AS nombre_categoria
    FROM 
        PRODUCTO p
    INNER JOIN 
        CATEGORIA c ON p.id_categoria = c.id_categoria
    WHERE 
        (@id_categoria IS NULL OR c.id_categoria = @id_categoria)
        AND
        (@nombre_categoria IS NULL OR c.nombre LIKE '%' + @nombre_categoria + '%')
    ORDER BY 
        p.nombre;
END;
GO
 

CREATE PROCEDURE sp_BuscarProductosPorID
    @id_producto INT
AS
BEGIN
   

    SELECT 
        p.id_producto,
		p.id_usuario,
        p.nombre AS nombre_producto,
        p.descripcion,
        p.precio,
        p.stock,
	    p.url_imagen,
		p.id_categoria
    FROM 
        PRODUCTO p 
    WHERE p.id_producto = @id_producto
       
END;
GO
 

CREATE PROCEDURE sp_InsertarProductoConDetalle
    @idUsuario NVARCHAR(100),
    @nombre NVARCHAR(100),
    @descripcion NVARCHAR(MAX),
    @precio DECIMAL(10, 2),
    @stock INT,
    @id_categoria INT,
    @url_imagen NVARCHAR(MAX) = NULL,
    @descripcion_detallada NVARCHAR(MAX)
AS
BEGIN 
    -- Iniciar una transacción para asegurar la atomicidad
    BEGIN TRANSACTION;

    BEGIN TRY 

        INSERT INTO PRODUCTO (nombre, descripcion, precio, stock, id_categoria, url_imagen,id_usuario)
        VALUES (@nombre, @descripcion, @precio, @stock, @id_categoria, @url_imagen,@idUsuario);
         
        DECLARE @NuevoProductoID INT = SCOPE_IDENTITY();
         
        INSERT INTO DETALLE_PRODUCTO (id_producto, descripcion_detallada)
        VALUES (@NuevoProductoID, @descripcion_detallada);  

        -- confirmar la transacción
        COMMIT TRANSACTION; 
         
        SELECT @NuevoProductoID AS NuevoProductoID;
    END TRY
    BEGIN CATCH
       --cancelar en caso de Error
        ROLLBACK TRANSACTION;
         
    END CATCH;
END;
GO

CREATE PROCEDURE sp_ActualizarProductoConDetalle
    @id_producto INT,
    @nombre NVARCHAR(100),
    @descripcion NVARCHAR(MAX),
    @precio DECIMAL(10, 2),
    @stock INT,
    @id_categoria INT,
    @url_imagen NVARCHAR(MAX) = NULL,
    @descripcion_detallada NVARCHAR(MAX)
AS
BEGIN 
  
    BEGIN TRANSACTION;

    BEGIN TRY 
      
        UPDATE PRODUCTO
        SET nombre = @nombre,
            descripcion = @descripcion,
            precio = @precio,
            stock = @stock,
            id_categoria = @id_categoria,
            url_imagen = @url_imagen
        WHERE id_producto = @id_producto;
		 
        IF EXISTS (SELECT 1 FROM DETALLE_PRODUCTO WHERE id_producto = @id_producto)
        BEGIN
            
            UPDATE DETALLE_PRODUCTO
            SET descripcion_detallada = @descripcion_detallada
            WHERE id_producto = @id_producto;
        END
        ELSE
        BEGIN
           
            INSERT INTO DETALLE_PRODUCTO (id_producto, descripcion_detallada)
            VALUES (@id_producto, @descripcion_detallada);
        END
 
        COMMIT TRANSACTION;
          
        SELECT @id_producto AS ProductoActualizadoID;
    END TRY
    BEGIN CATCH
    
        ROLLBACK TRANSACTION;  

        THROW;
    END CATCH;
END;
GO


--
CREATE PROCEDURE ObtenerProductoDetalle
    @id_producto INT
AS
BEGIN   
        SELECT 
            P.id_producto,
			p.id_usuario,
            P.nombre, 
            P.precio,  
			p.descripcion,
		    p.url_imagen,
			p.id_categoria,
            DP.id_detalle_producto,
            DP.descripcion_detallada
        FROM 
            PRODUCTO P
        LEFT JOIN 
            DETALLE_PRODUCTO DP
        ON 
            P.id_producto = DP.id_producto
        WHERE 
            P.id_producto = @id_producto; 
END;
GO


-- favoritos sp

CREATE PROCEDURE AgregarProductoAFavoritos
    @id_usuario INT,
    @id_producto INT
AS
BEGIN
  
    IF NOT EXISTS (
        SELECT 1 FROM FAVORITOS
        WHERE id_usuario = @id_usuario AND id_producto = @id_producto
    )
    BEGIN
        INSERT INTO FAVORITOS (id_usuario, id_producto, fecha_agregado)
        VALUES (@id_usuario, @id_producto, GETDATE());
    END
    ELSE
    BEGIN
         
        PRINT 'El producto ya está en favoritos';
    END
END;
GO

CREATE PROCEDURE EliminarProductoDeFavoritos
    @id_usuario INT,
    @id_producto INT
AS
BEGIN
    DELETE FROM FAVORITOS
    WHERE id_usuario = @id_usuario AND id_producto = @id_producto;
   
    PRINT 'Producto eliminado de favoritos';
END;
GO

CREATE PROCEDURE ObtenerFavoritosPorUsuario
    @id_usuario INT
AS
BEGIN
    SELECT p.id_producto, p.nombre, p.descripcion, p.precio, p.url_imagen, f.fecha_agregado
    FROM PRODUCTO p
    INNER JOIN FAVORITOS f 
	ON p.id_producto = f.id_producto
    WHERE f.id_usuario = @id_usuario
    ORDER BY f.fecha_agregado DESC;
END;
GO

--TRANSACCION DE PRODUCTO

CREATE PROCEDURE RegistrarTransaccion
    @IdUsuario INT,
    @IdProducto INT,
    @Cantidad INT,
    @PrecioTotal DECIMAL(10, 2),
    @Direccion NVARCHAR(255),
    @NumeroTelefonico NVARCHAR(15),
    @NombreTitular NVARCHAR(100),
    @NumeroTarjeta NVARCHAR(20),
    @FechaExpiracion DATE,
    @CVV NVARCHAR(4)
AS
BEGIN
    INSERT INTO TRANSACCION (
        id_usuario,
        id_producto,
        cantidad,
        precio_total,
        direccion,
        numero_telefonico,
        nombre_titular,
        numero_tarjeta,
        fecha_expiracion,
        cvv,
        fecha_transaccion
    )
    VALUES (
        @IdUsuario,
        @IdProducto,
        @Cantidad,
        @PrecioTotal,
        @Direccion,
        @NumeroTelefonico,
        @NombreTitular,
        @NumeroTarjeta,
        @FechaExpiracion,
        @CVV,
        GETDATE() -- Captura la fecha y hora actuales
    );

	 SELECT SCOPE_IDENTITY() AS NuevoTransaccionID;
END;
GO

CREATE PROCEDURE Sp_BuscarTransaccionPorID
@idTransaccion INT
AS
BEGIN
    SELECT *FROM TRANSACCION WHERE id_transaccion =  @idTransaccion
END
GO

CREATE PROCEDURE RegistrarResenaProducto
    @IdProducto INT,
    @IdUsuario INT,
    @ValoracionEstrellas INT,
    @TituloReseña NVARCHAR(50),
    @Reseña NVARCHAR(MAX)
AS
BEGIN
    
      INSERT INTO RESEÑA_PRODUCTO (id_producto, id_usuario, valoracion_estrellas, titulo_reseña, Reseña)
      VALUES (@IdProducto, @IdUsuario, @ValoracionEstrellas, @TituloReseña, @Reseña);
         
      SELECT SCOPE_IDENTITY() AS NewCommentId;
 
END;
GO

CREATE PROCEDURE Sp_ObtenerReseñas 
AS
BEGIN
    
    SELECT r.reseña_id,r.id_usuario, r.id_producto , r.valoracion_estrellas, r.titulo_reseña,
	r.reseña, u.nombre as nombre_usuario,u.url_imagen as url_imagen_usuario, p.nombre as nombre_producto,
	p.url_imagen as url_imagen_producto

	FROM RESEÑA_PRODUCTO r
	INNER JOIN USUARIO u
	ON r.id_usuario = u.id_usuario
	INNER JOIN PRODUCTO p
	ON r.id_producto = p.id_producto 
END;
GO

CREATE PROCEDURE Sp_ObtenerReseñasPorUsuario
@Id_Usuario INT
AS
BEGIN
    
    SELECT r.reseña_id,r.id_usuario, r.id_producto , r.valoracion_estrellas, r.titulo_reseña,
	r.reseña, u.nombre as nombre_usuario,u.url_imagen as url_imagen_usuario, p.nombre as nombre_producto,
	p.url_imagen as url_imagen_producto

	FROM RESEÑA_PRODUCTO r
	INNER JOIN USUARIO u
	ON r.id_usuario = u.id_usuario
	INNER JOIN PRODUCTO p
	ON r.id_producto = p.id_producto
	WHERE r.id_usuario = @Id_Usuario 
END;
GO

CREATE PROCEDURE Sp_ObtenerReseñasPorProducto
@id_Producto INT
AS
BEGIN
    
    SELECT r.reseña_id,r.id_usuario, r.id_producto , r.valoracion_estrellas, r.titulo_reseña,
	r.reseña, u.nombre as nombre_usuario,u.url_imagen as url_imagen_usuario, p.nombre as nombre_producto,
	p.url_imagen as url_imagen_producto

	FROM RESEÑA_PRODUCTO r
	INNER JOIN USUARIO u
	ON r.id_usuario = u.id_usuario
	INNER JOIN PRODUCTO p
	ON r.id_producto = p.id_producto
	WHERE r.id_producto = @id_Producto 
END;
GO


EXEC sp_InsertarRol @NombreRol = 'USER';
GO
EXEC sp_InsertarRol @NombreRol = 'ADMIN'; 
GO

 
EXEC sp_RegistrarUsuario
    @nombre  = 'Flores',
    @email = 'admin@gmail.com',
    @contrasena = '123',
    @direccion  = 'Av lima peru',
    @telefono  = '992587084',
    @ciudad  = 'Lima',
    @codigo_postal  = '15777', 
    @nombre_rol  = 'ADMIN',
	@patron = 'SISE2024'
 