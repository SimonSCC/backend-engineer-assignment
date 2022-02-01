CREATE TABLE PokemonName(
	NameId INT GENERATED ALWAYS AS IDENTITY,
	English VARCHAR(255),
	Japanese VARCHAR(255),
	Chinese VARCHAR(255),
	French VARCHAR(255),
	PRIMARY KEY(NameId)
);

CREATE TABLE PokemonBase(
	PokemonId INT GENERATED ALWAYS AS IDENTITY,
	HP INT,
	Attack INT,
	Defense INT,
	SpAttack INT,
	SpDefense INT,
	Speed INT,
	PRIMARY KEY(PokemonId)
);

CREATE TABLE PokedexEntry(
EntryId INT generated always as identity,
NameId INT,
SerializedTypes VARCHAR(255),
PokemonBaseId INT,
	PRIMARY KEY(EntryId),
	FOREIGN KEY(NameId) REFERENCES PokemonName,
	FOREIGN KEY(PokemonBaseId) REFERENCES PokemonBase
);