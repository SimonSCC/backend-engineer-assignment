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
            connection = new($"Server=192.168.0.35;Port=5432;Database=PokeDex;User Id=postgres;password=discotek;");
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

        public void InsertAllPokedexEntries()
        {
            string jsonText = File.ReadAllText("./pokedex.json");
            List<PokedexEntry> list = JsonConvert.DeserializeObject<List<PokedexEntry>>(jsonText);

            foreach (PokedexEntry entry in list)
            {
                int pokemonBaseId = (int)InsertBasePokemon(entry.Base);
                int pokemonNameId = (int)InsertPokemonName(entry.Name);
                InsertPokedexEntry(entry, pokemonBaseId, pokemonNameId);

            }
        }

        private bool InsertPokedexEntry(PokedexEntry entry, int pokemonBaseId, int pokemonNameId)
        {
            try
            {
                connection.Open();
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

                    if (cmd.ExecuteNonQuery() > 0)
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
                connection.Close();
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

        private object InsertPokemonName(Name entry)
        {
            try
            {
                connection.Open();
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


                    return cmd.ExecuteScalar();
                }
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

        private object InsertBasePokemon(PokemonBase entry)
        {
            try
            {
                connection.Open();
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

                    return cmd.ExecuteScalar();
                }
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

        public void Dispose()
        {
            Debug.WriteLine($"\nDisposing object {this}\n");
            ((IDisposable)connection).Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
