-- Données initiales pour StatusWatch
-- 6 services avec URLs réelles (httpbin / public APIs) pour que le PingBackgroundService remonte de vraies latences
-- Mix de statuts (operational/degraded/outage), incidents historiques étalés + incidents actifs au moment de la démo
-- ~ 20 mises à jour pour des timelines crédibles
-- ~ 200 ping_logs sur les 24-48 dernières heures pour avoir un historique d'uptime visible

-- nettoyage des donnees metier (la table users est preservee, l'admin reste accessible)
TRUNCATE TABLE ping_logs RESTART IDENTITY CASCADE;
TRUNCATE TABLE incident_updates RESTART IDENTITY CASCADE;
TRUNCATE TABLE incidents RESTART IDENTITY CASCADE;
TRUNCATE TABLE services RESTART IDENTITY CASCADE;

-- ============================================================
-- Services (6 entrees, URLs publiques pour ping reel)
-- ============================================================
INSERT INTO services (nom, description, url, categorie, statut) VALUES
    ('API publique',           'API REST principale exposée aux clients externes',                  'https://api.github.com',                 'API',            'operational'),
    ('Site vitrine',           'Site marketing public et documentation',                            'https://www.google.com',                 'Web',            'operational'),
    ('Base de données prod',   'Cluster PostgreSQL principal (port 5432)',                          'https://www.postgresql.org',             'Infrastructure', 'operational'),
    ('Service d''auth',        'Service d''authentification SSO (OAuth + cookies)',                 'https://httpbin.org/delay/2',            'API',            'degraded'),
    ('CDN assets statiques',   'CDN pour images, JS et CSS, distribué via Cloudflare',              'https://cdn.jsdelivr.net/npm/bootstrap', 'Infrastructure', 'operational'),
    ('Webhook ingestion',      'Endpoint de réception des webhooks partenaires',                    'https://httpbin.org/status/503',         'API',            'outage');

-- ============================================================
-- Incidents : 4 actifs (dont 1 critical) + 6 historiques resolus
-- Les statuts services sont recalcules ci-dessous via UPDATE explicite
-- (le RecalculateServiceStatus de l'app les ajustera automatiquement au prochain Create/Update/Delete d'incident)
-- ============================================================

-- ACTIFS au moment de la demo
INSERT INTO incidents (titre, description, statut, severite, date_debut, date_fin, service_id) VALUES
    ('Webhooks partenaires en panne complète',
     'Plus aucun webhook reçu depuis 09:15. Investigation en cours sur la file de messages RabbitMQ.',
     'identified', 'critical',
     NOW() - INTERVAL '4 hours', NULL, 6),

    ('Latence élevée sur le service d''auth',
     'Les requêtes /auth/token prennent > 2s depuis ce matin. Pic de trafic constaté.',
     'monitoring', 'major',
     NOW() - INTERVAL '2 hours', NULL, 4),

    ('Lenteurs intermittentes sur la BDD',
     'Quelques requêtes au-delà de 500ms sur la replica de lecture. Pas d''impact bloquant.',
     'investigating', 'minor',
     NOW() - INTERVAL '45 minutes', NULL, 3),

    ('Pic d''erreurs 502 sur l''API publique',
     'Taux d''erreur à 1.2% sur /v1/orders. En cours d''analyse.',
     'investigating', 'major',
     NOW() - INTERVAL '20 minutes', NULL, 1);

-- HISTORIQUES (resolus) sur les 3 derniers mois
INSERT INTO incidents (titre, description, statut, severite, date_debut, date_fin, service_id) VALUES
    ('Coupure DNS chez le provider',
     'Résolution DNS HS pendant 18 min, propagation lente. Bascule vers le DNS de secours.',
     'resolved', 'critical',
     NOW() - INTERVAL '5 days', NOW() - INTERVAL '5 days' + INTERVAL '18 minutes', 2),

    ('Maintenance planifiée du CDN',
     'Migration vers la nouvelle région Cloudflare. Aucune interruption visible côté client.',
     'resolved', 'minor',
     NOW() - INTERVAL '12 days', NOW() - INTERVAL '12 days' + INTERVAL '2 hours', 5),

    ('Erreur 500 sur le site vitrine',
     'Mauvaise config de cache après déploiement. Rollback en 45 min.',
     'resolved', 'major',
     NOW() - INTERVAL '21 days', NOW() - INTERVAL '21 days' + INTERVAL '45 minutes', 2),

    ('Timeout sur l''auth pendant le pic de connexion',
     'Saturation du pool de connexions. Capacité augmentée + cache de sessions ajouté.',
     'resolved', 'major',
     NOW() - INTERVAL '34 days', NOW() - INTERVAL '34 days' + INTERVAL '1 hour 12 minutes', 4),

    ('Latence inhabituelle sur les webhooks',
     'Trafic anormal d''un partenaire qui retried en boucle. Rate limit mis en place.',
     'resolved', 'minor',
     NOW() - INTERVAL '47 days', NOW() - INTERVAL '47 days' + INTERVAL '30 minutes', 6),

    ('Indisponibilité totale de l''API publique',
     'Migration de la BDD principale qui s''est mal passée. Rollback complet, post-mortem prévu.',
     'resolved', 'critical',
     NOW() - INTERVAL '62 days', NOW() - INTERVAL '62 days' + INTERVAL '2 hours 17 minutes', 1);

-- ============================================================
-- Mises à jour d'incidents (timelines des incidents actifs)
-- ============================================================

-- Incident 1 (webhooks panne complete, critical, identified)
INSERT INTO incident_updates (incident_id, message, date_creation) VALUES
    (1, 'Détection automatique : aucun webhook reçu depuis 4 minutes. Investigation lancée.',
        NOW() - INTERVAL '4 hours'),
    (1, 'Cause probable identifiée : RabbitMQ consumer planté après un message malformé. Restart en cours.',
        NOW() - INTERVAL '3 hours 30 minutes'),
    (1, 'Restart effectué, la queue se vide. On surveille l''ingestion. Mise à jour dans 30 min.',
        NOW() - INTERVAL '2 hours 45 minutes'),
    (1, 'Toujours instable, le consumer retombe en erreur sur certains payloads. Patch en cours de rédaction.',
        NOW() - INTERVAL '1 hour'),
    (1, 'Patch validé en staging. Déploiement prod prévu dans les 30 prochaines minutes.',
        NOW() - INTERVAL '15 minutes');

-- Incident 2 (latence auth, major, monitoring)
INSERT INTO incident_updates (incident_id, message, date_creation) VALUES
    (2, 'Pic de trafic détecté, latence p95 à 2.3s. Investigation en cours.',
        NOW() - INTERVAL '2 hours'),
    (2, 'Pool de connexions augmenté de 50 à 80. Latence redescend à 1.1s.',
        NOW() - INTERVAL '1 hour 30 minutes'),
    (2, 'Stabilisation à ~800ms. On reste en monitoring jusqu''à la fin du pic du matin.',
        NOW() - INTERVAL '45 minutes');

-- Incident 3 (lenteurs BDD, minor, investigating)
INSERT INTO incident_updates (incident_id, message, date_creation) VALUES
    (3, 'Quelques requêtes au-delà de 500ms remontées par l''APM. Analyse en cours.',
        NOW() - INTERVAL '40 minutes'),
    (3, 'Aucun verrou anormal visible. Hypothèse : plan d''exécution sur un nouvel index manquant. À confirmer.',
        NOW() - INTERVAL '15 minutes');

-- Incident 4 (502 API, major, investigating)
INSERT INTO incident_updates (incident_id, message, date_creation) VALUES
    (4, 'Taux d''erreur 502 à 1.2% détecté sur /v1/orders. Analyse en cours sur les logs reverse-proxy.',
        NOW() - INTERVAL '18 minutes');

-- Quelques updates historiques sur les incidents resolus pour montrer une timeline complete
INSERT INTO incident_updates (incident_id, message, date_creation) VALUES
    (5, 'Coupure DNS détectée. Bascule vers le secondaire en cours.', NOW() - INTERVAL '5 days'),
    (5, 'Bascule effectuée, services rétablis. On garde un œil pour la propagation complète.', NOW() - INTERVAL '5 days' + INTERVAL '10 minutes'),
    (5, 'Tout est revenu à la normale. Post-mortem partagé à l''équipe.', NOW() - INTERVAL '5 days' + INTERVAL '20 minutes'),
    (7, 'Crash détecté au déploiement de 14:32. Investigation rapide.', NOW() - INTERVAL '21 days'),
    (7, 'Rollback déclenché. Site de nouveau accessible.', NOW() - INTERVAL '21 days' + INTERVAL '20 minutes'),
    (10, 'Incident majeur : API down. Toute l''équipe sur le pont.', NOW() - INTERVAL '62 days'),
    (10, 'Rollback terminé, services rétablis. Post-mortem complet à venir.', NOW() - INTERVAL '62 days' + INTERVAL '2 hours 17 minutes');

-- ============================================================
-- Ping logs : ~ 200 entrees sur les 24 dernieres heures
-- Chaque service est pingue toutes les 5 min, soit 288 cycles / jour
-- On limite a 200 pour ne pas trop charger en seed (assez pour calculer uptime + remplir l'historique)
-- ============================================================

-- Service 1 (API publique) : ~ 99% uptime, 1 down par jour
INSERT INTO ping_logs (service_id, statut, latence_ms, checked_at)
SELECT
    1,
    CASE WHEN gs % 50 = 0 THEN 'down' ELSE 'up' END,
    CASE WHEN gs % 50 = 0 THEN NULL ELSE 80 + (random()*40)::int END,
    NOW() - (gs * interval '7 minutes')
FROM generate_series(0, 200) AS gs;

-- Service 2 (Site vitrine) : 100% uptime
INSERT INTO ping_logs (service_id, statut, latence_ms, checked_at)
SELECT
    2, 'up', 45 + (random()*30)::int, NOW() - (gs * interval '7 minutes')
FROM generate_series(0, 200) AS gs;

-- Service 3 (BDD) : 100% uptime, latence un peu variable
INSERT INTO ping_logs (service_id, statut, latence_ms, checked_at)
SELECT
    3, 'up', 15 + (random()*25)::int, NOW() - (gs * interval '7 minutes')
FROM generate_series(0, 200) AS gs;

-- Service 4 (Auth, degraded) : 100% up mais latence elevee (1000-1800ms)
INSERT INTO ping_logs (service_id, statut, latence_ms, checked_at)
SELECT
    4, 'up', 1000 + (random()*800)::int, NOW() - (gs * interval '7 minutes')
FROM generate_series(0, 200) AS gs;

-- Service 5 (CDN) : 100% uptime
INSERT INTO ping_logs (service_id, statut, latence_ms, checked_at)
SELECT
    5, 'up', 20 + (random()*15)::int, NOW() - (gs * interval '7 minutes')
FROM generate_series(0, 200) AS gs;

-- Service 6 (Webhooks, outage) : 100% down (httpbin /status/503 renvoie 503)
INSERT INTO ping_logs (service_id, statut, latence_ms, checked_at)
SELECT
    6, 'down', 100 + (random()*200)::int, NOW() - (gs * interval '7 minutes')
FROM generate_series(0, 200) AS gs;

-- ============================================================
-- Utilisateurs : non touches par ce seed
-- L'admin existant (admin@statuswatch.local / Admin1234!) reste en place.
-- Pour creer d'autres comptes, utiliser /Account/Register depuis l'app.
-- ============================================================
