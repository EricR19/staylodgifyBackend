-- =============================================================================
-- MIGRACIÓN: Agregar amenidades y reglas de propiedad
-- Base de datos: saas_staylodgify (Aiven MySQL)
-- Fecha: 2026-01-15
-- =============================================================================

-- Agregar nuevos campos a la tabla properties
ALTER TABLE properties 
ADD COLUMN IF NOT EXISTS amenities JSON DEFAULT NULL COMMENT 'Lista de amenidades en formato JSON',
ADD COLUMN IF NOT EXISTS check_in_time TIME DEFAULT '15:00:00' COMMENT 'Hora de entrada',
ADD COLUMN IF NOT EXISTS check_out_time TIME DEFAULT '10:00:00' COMMENT 'Hora de salida',
ADD COLUMN IF NOT EXISTS house_rules JSON DEFAULT NULL COMMENT 'Reglas de la propiedad en formato JSON';

-- Verificar que los campos fueron creados
DESCRIBE properties;

-- =============================================================================
-- EJEMPLO DE DATOS: Cómo se verían los nuevos campos
-- =============================================================================

-- Actualizar propiedad existente con amenidades por defecto
-- UPDATE properties 
-- SET 
--     amenities = '["wifi", "parking", "air_conditioning", "kitchen", "tv", "washer"]',
--     check_in_time = '15:00:00',
--     check_out_time = '10:00:00',
--     house_rules = '{
--         "importantInfo": [
--             "Respetar horario de silencio (10:00 PM - 7:00 AM)",
--             "No se permiten mascotas",
--             "No fumar en las instalaciones",
--             "No se permiten fiestas ni eventos",
--             "Respetar a los vecinos y áreas comunes"
--         ],
--         "customNotes": "Bienvenido a nuestra propiedad. Por favor cuida las instalaciones."
--     }'
-- WHERE id = 1;

-- =============================================================================
-- LISTA DE AMENIDADES DISPONIBLES (referencia para el frontend)
-- =============================================================================
-- wifi              - WiFi gratuito
-- parking           - Estacionamiento
-- pool              - Piscina
-- air_conditioning  - Aire acondicionado
-- heating           - Calefacción
-- kitchen           - Cocina
-- washer            - Lavadora
-- dryer             - Secadora
-- tv                - Televisión
-- gym               - Gimnasio
-- hot_tub           - Jacuzzi
-- balcony           - Balcón
-- garden            - Jardín
-- bbq               - Área de BBQ
-- pet_friendly      - Se permiten mascotas
-- elevator          - Elevador
-- security          - Seguridad 24/7
-- reception         - Recepción
-- room_service      - Servicio a la habitación
-- restaurant        - Restaurante
-- bar               - Bar
-- spa               - Spa
-- beach_access      - Acceso a playa
-- mountain_view     - Vista a montaña
-- sea_view          - Vista al mar
-- city_view         - Vista a la ciudad
