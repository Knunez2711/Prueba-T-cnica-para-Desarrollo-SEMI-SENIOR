CREATE TABLE IF NOT EXISTS pacientes
(
    id SERIAL PRIMARY KEY,
    tipo_documento VARCHAR(50),
    numero_documento VARCHAR(50),
    nombre VARCHAR(150),
    fecha_nacimiento DATE,
    correo VARCHAR(100),
    genero VARCHAR(10),
    direccion VARCHAR(200),
    telefono VARCHAR(20),
    email VARCHAR(100),
    activo BOOLEAN NOT NULL DEFAULT TRUE,
    fecha_creacion TIMESTAMP NOT NULL DEFAULT NOW()
);

CREATE UNIQUE INDEX IF NOT EXISTS ux_pacientes_tipo_numero_documento
    ON pacientes (tipo_documento, numero_documento);

CREATE TABLE IF NOT EXISTS historial_contacto_paciente
(
    historial_id SERIAL PRIMARY KEY,
    paciente_id INTEGER NOT NULL REFERENCES pacientes(id),
    correo_anterior VARCHAR(255) NOT NULL,
    telefono_anterior VARCHAR(20) NOT NULL,
    fecha_cambio TIMESTAMP NOT NULL DEFAULT NOW()
);
