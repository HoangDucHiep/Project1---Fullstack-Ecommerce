-- Create keycloak user and database
CREATE USER keycloak WITH PASSWORD 'keycloak123';
CREATE DATABASE keycloakdb OWNER keycloak;
GRANT ALL PRIVILEGES ON DATABASE keycloakdb TO keycloak;
