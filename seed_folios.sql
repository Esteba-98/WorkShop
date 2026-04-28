-- Asigna folio OT-XXXX a las órdenes que quedaron con Folio vacío.
-- Ejecutar una sola vez desde pgAdmin o psql.

DO $$
DECLARE
    r RECORD;
    contador INT := 1;
    max_folio INT;
BEGIN
    -- Detectar el número más alto ya asignado
    SELECT COALESCE(MAX(CAST(SUBSTRING("Folio" FROM 4) AS INT)), 0)
    INTO max_folio
    FROM "Mantenimientos"
    WHERE "Folio" LIKE 'OT-%'
      AND "Folio" ~ '^OT-[0-9]{4}$';

    contador := max_folio + 1;

    -- Recorrer los que tienen folio vacío, en orden cronológico
    FOR r IN
        SELECT "Id"
        FROM "Mantenimientos"
        WHERE "Folio" = ''
        ORDER BY "Fecha"
    LOOP
        UPDATE "Mantenimientos"
        SET "Folio" = 'OT-' || LPAD(contador::TEXT, 4, '0')
        WHERE "Id" = r."Id";

        contador := contador + 1;
    END LOOP;

    RAISE NOTICE 'Folios asignados. Último número: OT-%', LPAD((contador - 1)::TEXT, 4, '0');
END $$;
