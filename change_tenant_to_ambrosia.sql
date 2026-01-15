-- ========================================
-- CAMBIO DE NOMBRE DEL TENANT
-- De: "Hotel Demo" -> A: "Ambrosia Tarbaca"
-- ========================================

-- 1. Ver estado ANTES del cambio
SELECT 
    id, 
    name as 'Nombre Anterior', 
    subdomain, 
    contact_email 
FROM tenants 
WHERE id = 1;

-- 2. EJECUTAR EL CAMBIO
UPDATE tenants 
SET name = 'Ambrosia Tarbaca'
WHERE id = 1;

-- 3. Verificar estado DESPUÉS del cambio
SELECT 
    id, 
    name as 'Nombre Nuevo', 
    subdomain, 
    contact_email,
    phone
FROM tenants 
WHERE id = 1;

-- 4. Verificar que el endpoint pueda encontrarlo
-- También puedes probarlo con:
-- GET https://staylodgifybackendapi.onrender.com/api/Tenants/by-name/Ambrosia%20Tarbaca
