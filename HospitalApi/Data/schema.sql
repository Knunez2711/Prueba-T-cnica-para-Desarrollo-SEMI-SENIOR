Hospital=# CREATE OR REPLACE FUNCTION crear_paciente (
    p_tipo_documento VARCHAR,
    p_numero_documento VARCHAR,
    p_nombre VARCHAR,
    p_fecha_nacimiento DATE,
    p_correo_electronico VARCHAR,
    p_genero VARCHAR,
    p_direccion VARCHAR,
    p_numero_telefono VARCHAR,
    p_activo BOOLEAN
)
RETURNS SETOF pacientes AS $$
BEGIN
    IF EXISTS (
        SELECT 1 FROM pacientes WHERE tipo_documento = p_numero_documento
    )THEN
        RAISE EXCEPTION 'DUPLICADO: ya existe un paciente con este tipo y numero de documento.';
    END IF;

    RETURN QUERY
    INSERT INTO pacientes(
        tipo_documento, numero_documento, nombre, fecha_nacimiento, correo, genero, direccion, telefono, email,
        activo
        )
        VALUES (
            p_tipo_documento, p_numero_documento, p_nombre, p_fecha_nacimiento, p_correo_electronico, p_genero,
            p_direccion, p_numero_telefono, p_activo
        )
        RETURNING *;
    END
    $$ LANGUAGE plpgsql;


CREATE OR REPLACE FUNCTION obtener_pacientes()
RETURNS SETOF pacientes AS $$
BEGIN
    RETURN QUERY
    SELECT *FROM pacientes
    ORDER BY id DESC;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION obtener_pacientes_por_id(p_paciente_id INT)
RETURNS SETOF pacientes AS $$
BEGIN
    RETURN QUERY
    SELECT * FROM pacientes
    WHERE id = p_paciente_id;
END;
$$ LANGUAGE plpgsql;

/*
actualizar paciente
*/

CREATE OR REPLACE FUNCTION actualizar_paciente(
    p_paciente_id INT,
    p_tipo_documento VARCHAR,
    p_numero_documento VARCHAR,
    p_nombre VARCHAR,
    p_fecha_nacimiento DATE,
    p_correo_electronico VARCHAR,
    p_genero VARCHAR,
    p_direccion VARCHAR,
    p_numero_telefono VARCHAR,
    p_activo BOOLEAN
)
RETURNS SETOF pacientes AS $$
BEGIN
    IF EXISTS (
        SELECT 1 FROM pacientes WHERE id = p_paciente_id
    )THEN
        RAISE EXCEPTION 'NO ENCONTRADO: Paciente no encontrado.';
    END IF;

    IF EXISTS(
        SELECT 1 FROM pacientes WHERE tipo_documento = p_tipo_documento  AND numero_documento = p_numero_documento
        AND id <> p_paciente_id
    )THEN
        RAISE EXCEPTION 'DUPLICADO: Ya existe otro paciente con este tipo y numero de documento';
    END IF;

    INSERT INTO historial_contacto_paciente (paciente_id, correo_anterior, telefono_anterior)
    SELECT id, correo_electronico, numero_telefono
    FROM pacientes
    WHERE id = p_pciente_id;

    RETURN QUERY
    UPDATE pacientes
    SET tipo_documento = p_tipo_documento,
        numero_documento = p_numero_documento,
        nombre = p_nombre,
        fecha_nacimiento = p_fecha_nacimiento,
        correo = p_correo_electronico,
        genero = p_genero,
        direccion = p_direccion,
        telefono = p_numero_telefono,
        activo = p_activo
    WHERE id = p_paciente_id
    RETURNING *;
END;
$$ LANGUAGE plpgsql;