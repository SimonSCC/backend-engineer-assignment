Have a rabbit container running on the standard port, 
and specify its IP in the connection manager class is the sharedlibary project.

In connection manager also specify the connection string to your local postgresql database.
The database should be constructed using the CreationOfPokedexDB.sql script in the sql scripts folder provided.

To add data to your database, uncomment line 19, 20 in the program.cs class in the PokedexService project
Then run the project alone.
Close program, and uncomment again.

Now with everything ready: run the program with docker-compose in visual studio.
