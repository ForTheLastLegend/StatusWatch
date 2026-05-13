-- Schéma BDD StatusWatch
-- 5 tables : services, incidents, incident_updates, ping_logs, users

DROP TABLE IF EXISTS ping_logs CASCADE;
DROP TABLE IF EXISTS incident_updates CASCADE;
DROP TABLE IF EXISTS incidents CASCADE;
DROP TABLE IF EXISTS services CASCADE;
DROP TABLE IF EXISTS users CASCADE;

CREATE TABLE services (
    id          SERIAL PRIMARY KEY,
    nom         VARCHAR(100) NOT NULL,
    description TEXT,
    url         VARCHAR(300),
    categorie   VARCHAR(50),
    statut      VARCHAR(20) NOT NULL DEFAULT 'operational'
);

CREATE TABLE incidents (
    id          SERIAL PRIMARY KEY,
    titre       VARCHAR(200) NOT NULL,
    description TEXT,
    statut      VARCHAR(20) NOT NULL DEFAULT 'investigating',
    severite    VARCHAR(20) NOT NULL DEFAULT 'minor',
    date_debut  TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    date_fin    TIMESTAMP,
    service_id  INT NOT NULL REFERENCES services(id)
);

CREATE TABLE incident_updates (
    id            SERIAL PRIMARY KEY,
    incident_id   INT NOT NULL REFERENCES incidents(id) ON DELETE CASCADE,
    message       TEXT NOT NULL,
    date_creation TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE ping_logs (
    id         SERIAL PRIMARY KEY,
    service_id INT NOT NULL REFERENCES services(id) ON DELETE CASCADE,
    statut     VARCHAR(10) NOT NULL,
    latence_ms INT,
    checked_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE users (
    id            SERIAL PRIMARY KEY,
    nom           VARCHAR(100),
    email         VARCHAR(200) NOT NULL UNIQUE,
    password_hash VARCHAR(500) NOT NULL,
    role          VARCHAR(20) NOT NULL DEFAULT 'Viewer'
);

-- index utiles pour les requêtes fréquentes
CREATE INDEX idx_incidents_service ON incidents(service_id);
CREATE INDEX idx_incidents_statut ON incidents(statut);
CREATE INDEX idx_incident_updates_incident ON incident_updates(incident_id);
CREATE INDEX idx_ping_logs_service ON ping_logs(service_id, checked_at DESC);
