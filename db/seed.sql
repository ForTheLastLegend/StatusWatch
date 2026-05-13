-- Données initiales : services + quelques incidents pour avoir de quoi tester
-- les pages /Incidents et /Services/{id}

INSERT INTO services (nom, description, url, categorie, statut) VALUES
    ('API publique', 'API REST exposée aux clients externes', 'https://api.statuswatch.local/health', 'API', 'operational'),
    ('Site vitrine', 'Site marketing public', 'https://www.statuswatch.local', 'Web', 'operational'),
    ('Base de données prod', 'Cluster PostgreSQL principal', 'https://db.statuswatch.local/ping', 'Infrastructure', 'operational'),
    ('Service d''auth', 'Service d''authentification interne', 'https://auth.statuswatch.local/health', 'API', 'operational');

-- Incidents de test : 2 actifs, 1 résolu. IDs services = ordre d'insertion au dessus
INSERT INTO incidents (titre, description, statut, severite, date_debut, date_fin, service_id) VALUES
    ('Latence élevée sur /v1/orders', 'Temps de réponse > 3s constaté sur la route /v1/orders depuis 14h.', 'investigating', 'major', NOW() - INTERVAL '2 hours', NULL, 1),
    ('Connexion BDD intermittente', 'Quelques timeouts ponctuels rapportés par le service de commandes.', 'monitoring', 'minor', NOW() - INTERVAL '6 hours', NULL, 3),
    ('Erreur 500 sur le site vitrine', 'Crash du process suite à une mauvaise config de cache. Corrigé après redémarrage.', 'resolved', 'major', NOW() - INTERVAL '3 days', NOW() - INTERVAL '3 days' + INTERVAL '45 minutes', 2);
