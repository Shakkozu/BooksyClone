-- enable btree_gist module
CREATE EXTENSION IF NOT EXISTS btree_gist;


CREATE TABLE resource (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(), -- Klucz główny
	"guid" UUID not null UNIQUE,
    correlation_id UUID NOT NULL UNIQUE,                  -- Unikalny identyfikator korelacji
    owner_id UUID NOT NULL,                        -- Id właściciela
    created_at TIMESTAMP NOT NULL DEFAULT now()    -- Data utworzenia
);

-- Tabela ResourceLock
CREATE TABLE resource_lock (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(), -- Klucz główny
	"guid" UUID not null UNIQUE,
    resource_id UUID NOT NULL REFERENCES resource(id) ON DELETE CASCADE, -- Powiązanie z tabelą Resource
    created_by UUID NOT NULL,                       -- Id użytkownika tworzącego blokadę
    "timestamp" TIMESTAMP NOT NULL DEFAULT now(),     -- Znacznik czasu utworzenia blokady
    "from" TIMESTAMP NOT NULL,                      -- Początek zakresu czasowego blokady
    "to" TIMESTAMP NOT NULL                         -- Koniec zakresu czasowego blokady
);