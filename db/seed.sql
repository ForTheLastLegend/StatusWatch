-- Données initiales : quelques services réalistes, pas d'incidents pour l'instant.

INSERT INTO services (nom, description, url, categorie, statut) VALUES
    ('API publique', 'API REST exposée aux clients externes', 'https://api.statuswatch.local/health', 'API', 'operational'),
    ('Site vitrine', 'Site marketing public', 'https://www.statuswatch.local', 'Web', 'operational'),
    ('Base de données prod', 'Cluster PostgreSQL principal', 'https://db.statuswatch.local/ping', 'Infrastructure', 'operational'),
    ('Service d''auth', 'Service d''authentification interne', 'https://auth.statuswatch.local/health', 'API', 'operational');
