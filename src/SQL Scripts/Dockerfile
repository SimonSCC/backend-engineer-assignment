FROM postgres
ENV POSTGRES_PASSWORD pokemon123
ENV POSTGRES_DB pokedex-postgres
COPY CreationOfPokedexDB.sql /docker-entrypoint-initdb.d/
