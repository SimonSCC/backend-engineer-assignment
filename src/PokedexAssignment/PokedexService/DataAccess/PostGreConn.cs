using Models.Pokemon;
using Newtonsoft.Json;
using Npgsql;
using SharedLibary.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokedexService.DataAccess
{
    public class PostGreConn : IDisposable
    {
        private readonly NpgsqlConnection connection;

        public PostGreConn()
        {
            connection = new($"Server={ConnectionManager.PostgreSQLLocalhostDatabase};Port=5432;Database=PokeDex;User Id=postgres;password=discotek;");
        }

        public void TestConnection()
        {
            connection.Open();
            using (NpgsqlCommand cmd = new NpgsqlCommand("select * from test", connection))
            {
                using (NpgsqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Console.WriteLine(reader.GetString(0));
                    }
                }
            }
            connection.Close();
        }

        public PokedexEntry GetPokedexEntryById(int id)
        {
            PokedexEntry newEntry = new PokedexEntry();
            int nameId = 0;
            string typeAsString = string.Empty;
            int baseId = 0;

            try
            {
                connection.Open();
                using (NpgsqlCommand cmd = new NpgsqlCommand($"select * from pokedexentry WHERE EntryID = {id}", connection))
                {
                    using (NpgsqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            newEntry.Id = reader.GetInt32(0);
                            nameId = reader.GetInt32(1);
                            typeAsString = reader.GetString(2);
                            baseId = reader.GetInt32(3);
                        }
                    }
                }

                newEntry.Name = GetPokemonNameById(nameId);
                newEntry.Type = DeserializeTypeList(typeAsString);
                newEntry.Base = GetPokemonBaseById(baseId);
                return newEntry;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
            finally
            {
                connection.Close();
            }
        }

        public async Task DeletePokedexEntryById(int id)
        {
            try
            {
                PokedexEntry toDelete = GetPokedexEntryById(id);
                await connection.OpenAsync();
                using (NpgsqlCommand cmd = new NpgsqlCommand($"delete from pokedexentry WHERE entryid = {toDelete.Id}", connection))
                {
                    int affectedRows = await cmd.ExecuteNonQueryAsync();
                }
                using (NpgsqlCommand cmd = new NpgsqlCommand($"delete from pokemonbase WHERE PokemonId = {toDelete.Base.Id}", connection))
                {
                    int affectedRows = await cmd.ExecuteNonQueryAsync();
                }
                using (NpgsqlCommand cmd = new NpgsqlCommand($"delete from pokemonname WHERE NameId = {toDelete.Name.Id}", connection))
                {
                    int affectedRows = await cmd.ExecuteNonQueryAsync();
                }
            }
            catch (Exception e)
            {

                Console.WriteLine(e.Message);
            }
            finally
            {
                await connection.CloseAsync();
            }
        }

        public async Task UpdatePokedexEntry(PokedexEntry result)
        {
            try
            {
                connection.Open();
                using (NpgsqlCommand cmd = new NpgsqlCommand($"update pokedexentry SET SerializedTypes = @serializedtypes WHERE EntryId = @entryid", connection))
                {
                    var parameter = cmd.CreateParameter();
                    parameter.ParameterName = "serializedtypes";
                    parameter.Value = SerializeType(result.Type);
                    cmd.Parameters.Add(parameter);

                    var parameter1 = cmd.CreateParameter();
                    parameter1.ParameterName = "entryid";
                    parameter1.Value = result.Id;
                    cmd.Parameters.Add(parameter1);

                    await cmd.ExecuteNonQueryAsync();
                }

                using (NpgsqlCommand cmd = new NpgsqlCommand($"update pokemonbase " +
                    $"SET HP = @hp, Attack = @attack, Defense = @defense, SpAttack = @spattack, SpDefense = @spdefence, Speed = @speed WHERE PokemonId = @pokemonid", connection))
                {
                    var parameter = cmd.CreateParameter();
                    parameter.ParameterName = "hp";
                    parameter.Value = result.Base.HP;
                    cmd.Parameters.Add(parameter);

                    var parameter1 = cmd.CreateParameter();
                    parameter1.ParameterName = "attack";
                    parameter1.Value = result.Base.Attack;
                    cmd.Parameters.Add(parameter1);

                    var parameter2 = cmd.CreateParameter();
                    parameter2.ParameterName = "defense";
                    parameter2.Value = result.Base.Defense;
                    cmd.Parameters.Add(parameter2);

                    var parameter3 = cmd.CreateParameter();
                    parameter3.ParameterName = "spattack";
                    parameter3.Value = result.Base.SpAttack;
                    cmd.Parameters.Add(parameter3);

                    var parameter4 = cmd.CreateParameter();
                    parameter4.ParameterName = "spdefence";
                    parameter4.Value = result.Base.SpDefense;
                    cmd.Parameters.Add(parameter4);

                    var parameter5 = cmd.CreateParameter();
                    parameter5.ParameterName = "speed";
                    parameter5.Value = result.Base.Speed;
                    cmd.Parameters.Add(parameter5);

                    await cmd.ExecuteNonQueryAsync();
                }

                using (NpgsqlCommand cmd = new NpgsqlCommand($"update pokemonname " +
                   $"SET English = @english, Japanese = @japanese, Chinese = @chinese, French = @french WHERE NameId = @nameid", connection))
                {
                    var parameter = cmd.CreateParameter();
                    parameter.ParameterName = "english";
                    parameter.Value = result.Name.English;
                    cmd.Parameters.Add(parameter);

                    var parameter1 = cmd.CreateParameter();
                    parameter1.ParameterName = "japanese";
                    parameter1.Value = result.Name.Japanese;
                    cmd.Parameters.Add(parameter1);

                    var parameter2 = cmd.CreateParameter();
                    parameter2.ParameterName = "chinese";
                    parameter2.Value = result.Name.Chinese;
                    cmd.Parameters.Add(parameter2);

                    var parameter3 = cmd.CreateParameter();
                    parameter3.ParameterName = "french";
                    parameter3.Value = result.Name.French;
                    cmd.Parameters.Add(parameter3);

                    await cmd.ExecuteNonQueryAsync();
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                await connection.CloseAsync();
            }
        }

        public async Task PostNewPokedex(PokedexEntry result)
        {
            int pokemonBaseId = (int)await InsertBasePokemon(result.Base);
            int pokemonNameId = (int)await InsertPokemonName(result.Name);
            await InsertPokedexEntry(result, pokemonBaseId, pokemonNameId);
        }

        private PokemonBase GetPokemonBaseById(int id)
        {
            using (NpgsqlCommand cmd = new NpgsqlCommand($"select * from pokemonbase WHERE PokemonId = {id}", connection))
            {
                using (NpgsqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        return new PokemonBase()
                        {
                            Id = reader.GetInt32(0),
                            HP = reader.GetInt32(1),
                            Attack = reader.GetInt32(2),
                            Defense = reader.GetInt32(3),
                            SpAttack = reader.GetInt32(4),
                            SpDefense = reader.GetInt32(5),
                            Speed = reader.GetInt32(6)
                        };
                    }
                }
            }
            Console.WriteLine("Couldn't get pokemonbase");
            return null;
        }

        private List<string> DeserializeTypeList(string v)
        {
            List<string> result = new();
            string[] splitString = v.Split(',');
            foreach (string item in splitString)
            {
                result.Add(item);
            }
            return result;
        }

        private Name GetPokemonNameById(int id)
        {
            using (NpgsqlCommand cmd = new NpgsqlCommand($"select * from pokemonname WHERE NameId = {id}", connection))
            {
                using (NpgsqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        return new Name()
                        {
                            Id = reader.GetInt32(0),
                            English = reader.GetString(1),
                            Japanese = reader.GetString(2),
                            Chinese = reader.GetString(3),
                            French = reader.GetString(4)
                        };
                    }
                }
            }
            Console.WriteLine("Couldn't get pokemonname");
            return null;
        }

        public async Task InsertAllPokedexEntries()
        {
            string jsonText = File.ReadAllText("./pokedex.json");
            List<PokedexEntry> list = JsonConvert.DeserializeObject<List<PokedexEntry>>(jsonText);

            foreach (PokedexEntry entry in list)
            {
                await PostNewPokedex(entry);
            }
        }

        private async Task<bool> InsertPokedexEntry(PokedexEntry entry, int pokemonBaseId, int pokemonNameId)
        {
            try
            {
                await connection.OpenAsync();
                using (NpgsqlCommand cmd = new NpgsqlCommand("INSERT INTO pokedexentry (NameId, SerializedTypes, PokemonBaseId)" +
                    "VALUES (@nameid, @serializedtypes, @pokemonbaseid)", connection))
                {
                    var parameter = cmd.CreateParameter();
                    parameter.ParameterName = "nameid";
                    parameter.Value = pokemonNameId;
                    cmd.Parameters.Add(parameter);

                    var parameter2 = cmd.CreateParameter();
                    parameter2.ParameterName = "serializedtypes";
                    parameter2.Value = SerializeType(entry.Type);
                    cmd.Parameters.Add(parameter2);

                    var parameter3 = cmd.CreateParameter();
                    parameter3.ParameterName = "pokemonbaseid";
                    parameter3.Value = pokemonBaseId;
                    cmd.Parameters.Add(parameter3);

                    if (await cmd.ExecuteNonQueryAsync() > 0)
                        return true;
                    else return false;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
            finally
            {
                await connection.CloseAsync();
            }
        }

        private string SerializeType(List<string> type)
        {
            string result = string.Empty;
            foreach (string item in type)
            {
                result += item + ", ";
            }
            return result.Substring(0, result.Length - 2);
        }

        private async Task<object> InsertPokemonName(Name entry)
        {
            try
            {
                await connection.OpenAsync();
                using (NpgsqlCommand cmd = new NpgsqlCommand("INSERT INTO pokemonname (English, Japanese, Chinese, French)" +
                    "VALUES (@english, @japanese, @chinese, @french) RETURNING NameId", connection))
                {
                    var parameter = cmd.CreateParameter();
                    parameter.ParameterName = "english";
                    parameter.Value = entry.English;
                    cmd.Parameters.Add(parameter);

                    var parameter2 = cmd.CreateParameter();
                    parameter2.ParameterName = "japanese";
                    parameter2.Value = entry.Japanese;
                    cmd.Parameters.Add(parameter2);

                    var parameter3 = cmd.CreateParameter();
                    parameter3.ParameterName = "chinese";
                    parameter3.Value = entry.Chinese;
                    cmd.Parameters.Add(parameter3);

                    var parameter4 = cmd.CreateParameter();
                    parameter4.ParameterName = "french";
                    parameter4.Value = entry.French;
                    cmd.Parameters.Add(parameter4);


                    return await cmd.ExecuteScalarAsync();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
            finally
            {
                await connection.CloseAsync();
            }
        }

        private async Task<object> InsertBasePokemon(PokemonBase entry)
        {
            try
            {
                await connection.OpenAsync();
                using (NpgsqlCommand cmd = new NpgsqlCommand("INSERT INTO pokemonbase (HP,Attack,Defense,SpAttack,SpDefense,Speed)" +
                    "VALUES (@hp, @attack, @defense, @spattack, @spdefense, @speed) RETURNING PokemonId", connection))
                {
                    var parameter = cmd.CreateParameter();
                    parameter.ParameterName = "hp";
                    parameter.Value = entry.HP;
                    cmd.Parameters.Add(parameter);

                    var parameter2 = cmd.CreateParameter();
                    parameter2.ParameterName = "attack";
                    parameter2.Value = entry.Attack;
                    cmd.Parameters.Add(parameter2);

                    var parameter3 = cmd.CreateParameter();
                    parameter3.ParameterName = "defense";
                    parameter3.Value = entry.Defense;
                    cmd.Parameters.Add(parameter3);

                    var parameter4 = cmd.CreateParameter();
                    parameter4.ParameterName = "spattack";
                    parameter4.Value = entry.SpAttack;
                    cmd.Parameters.Add(parameter4);

                    var parameter5 = cmd.CreateParameter();
                    parameter5.ParameterName = "spdefense";
                    parameter5.Value = entry.SpDefense;
                    cmd.Parameters.Add(parameter5);

                    var parameter6 = cmd.CreateParameter();
                    parameter6.ParameterName = "speed";
                    parameter6.Value = entry.Speed;
                    cmd.Parameters.Add(parameter6);

                    return await cmd.ExecuteScalarAsync();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
            finally
            {
                await connection.CloseAsync();
            }
        }

        public void Dispose()
        {
            Debug.WriteLine($"\nDisposing object {this}\n");
            ((IDisposable)connection).Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
