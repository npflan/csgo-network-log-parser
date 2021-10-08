using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CSGO_DataLogger
{
    public static class LogManager
    {
        private static MySqlConnectionStringBuilder mySqlConnectionStringBuilder = new MySqlConnectionStringBuilder()
        {
            Server = "10.100.106.5",
            //Server = "127.0.0.1",
            Database = "CS_Game_Events",
            UserID = "root",
            Password = "Jul3mand!",
            SslMode = MySqlSslMode.None,
        };

        //private const string RegexKilled = "\\<(?'Murderer'STEAM_[0-9]*:[0-9]*:[0-9]*)>.*killed.*\\><(?'Victim'STEAM_[0-9]*:[0-9]*:[0-9]*)\\>.*with\\s\"(?'Weapon'[a - zA - Z0 - 9] *)\"(.*(?'Headshot'headshot))?";
        //private const string RegexPurchased = "\\<(?'Customer'STEAM_[0-9]*:[0-9]*:[0-9]*)>.*purchased\\s\"(?'Product'[a - zA - Z0 - 9] *)\"";
        //private const string RegexThrew = "\\<(?'User'STEAM_[0-9]*:[0-9]*:[0-9]*)>.*threw\\s(?'Item'[a-zA-Z0-9]*)";
        //private const string RegexAttacked = "\\<(?'Perpetrator'STEAM_[0-9]*:[0-9]*:[0-9]*|BOT)>.*attacked.*<(?'Victim'STEAM_[0-9]*:[0-9]*:[0-9]*|BOT)>.*with\\s\"(?'Weapon'[a - zA - Z0 - 9] *)\".*damage\\s\"(?'DamageDone'[0-9]*)\".*damage_armor\\s\"(?'ArmorDamageDone'[0-9]*)\".*health\\s\"(?'RemainingHealth'[0-9]*)\".*armor\\s\"(?'RemainingArmor'[0-9]*)\".*hitgroup\\s\"(?'Hitgroup'[a-zA-Z0-9]*)\"";

        private static Dictionary<string, string> regexHash = new Dictionary<string, string>() {
            { "Log", "(?<log>.*)" },
            //"(?<KillerName>.*)<[0-9]*><(?<Killer>STEAM_[0-9]*:[0-9]*:[0-9]*|BOT)><(?<KillerTeam>CT|TERRORIST)>"\sassisted\skilling\s"(?<VictimName>.*)<(?<Victim>STEAM_[0-9]*:[0-9]*:[0-9]*|BOT)><(?<VictimTeam>CT|TERRORIST)>"
            { "PlayerAssisted", "\"(?<KillerName>.*)<[0-9]*><(?<Killer>STEAM_[0-9]*:[0-9]*:[0-9]*|BOT)><(?<KillerTeam>CT|TERRORIST)>\"\\sassisted\\skilling\\s\"(?<VictimName>.*)<(?<Victim>STEAM_[0-9]*:[0-9]*:[0-9]*|BOT)><(?<VictimTeam>CT|TERRORIST)>\"" },
            
            //<(?'Perpetrator'STEAM_[0-9]*:[0-9]*:[0-9]*)>.*attacked.*<(?'Victim'STEAM_[0-9]*:[0-9]*:[0-9]*)>.*with\s"(?'Weapon'[a - zA - Z0 - 9] *)".*damage\s"(?'DamageDone'[0-9]*)".*damage_armor\s"(?'ArmorDamageDone'[0-9]*)".*health\s"(?'RemainingHealth'[0-9]*)".*armor\s"(?'RemainingArmor'[0-9]*)".*hitgroup\s"(?'Hitgroup'[a-zA-Z0-9]*)"
            { "PlayerAttacked", "\\<(?'Perpetrator'STEAM_[0-9]*:[0-9]*:[0-9]*)>.*attacked.*<(?'Victim'STEAM_[0-9]*:[0-9]*:[0-9]*)>.*with\\s\"(?'Weapon'[a - zA - Z0 - 9] *)\".*damage\\s\"(?'DamageDone'[0-9]*)\".*damage_armor\\s\"(?'ArmorDamageDone'[0-9]*)\".*health\\s\"(?'RemainingHealth'[0-9]*)\".*armor\\s\"(?'RemainingArmor'[0-9]*)\".*hitgroup\\s\"(?'Hitgroup'[a-zA-Z0-9]*)\"" },

            { "PlayerSays", "\"(?<PlayerName>.*)<[0-9]*><(?<PlayerID>STEAM_[0-9]*:[0-9]*:[0-9]*|BOT)>.*say.*\"(?<Say>.*)\"" },
            //????RL 10/13/2017 - 18:55:05: "Lionel<34><STEAM_1:0:777440><CT>" say "test"

            //"(?<Player1Name>.*)<[0-9]*><(?<Player1ID>STEAM_[0-9]*:[0-9]*:[0-9]*|BOT)><(?<Player1Team>CT|TERRORIST)>"\sblinded\sfor\s(?<Time>[0-9.]*)\sby\s\"(?<Player2Name>.*)<(?<Player2ID>STEAM_[0-9]*:[0-9]*:[0-9]*|BOT)><(?<Player2Team>CT|TERRORIST)>\"\sfrom\s[a-zA-Z]*\sentindex\s(?<EntityID>[0-9]*)
            { "PlayerBlinded", "\"(?<Player1Name>.*)<[0-9]*><(?<Player1ID>STEAM_[0-9]*:[0-9]*:[0-9]*|BOT)><(?<Player1Team>CT|TERRORIST)>\"\\sblinded\\sfor\\s(?<Time>[0-9.]*)\\sby\\s\\\"(?<Player2Name>.*)<(?<Player2ID>STEAM_[0-9]*:[0-9]*:[0-9]*|BOT)><(?<Player2Team>CT|TERRORIST)>\\\"\\sfrom\\s[a-zA-Z]*\\sentindex\\s(?<EntityID_INT>[0-9]*)" },

            //"(?<KillerName>.*)<[0-9]*><(?<Killer>STEAM_[0-9]*:[0-9]*:[0-9]*|BOT)><(?<KillerTeam>CT|TERRORIST)>"\s\[(?<killerx>[-0-9]*)\s(?<killery>[-0-9]*)\s(?<killerz>[-0-9]*)\]\scommitted\ssuicide\swith\s"(?<Item>.*)"
            { "PlayerCommitedSuicide", "\"(?<KillerName>.*)<[0-9]*><(?<Killer>STEAM_[0-9]*:[0-9]*:[0-9]*|BOT)><(?<KillerTeam>CT|TERRORIST)>\"\\s\\[(?<killerx>[-0-9]*)\\s(?<killery>[-0-9]*)\\s(?<killerz>[-0-9]*)\\]\\scommitted\\ssuicide\\swith\\s\"(?<Item>.*)\"" },

            //"(?<PlayerName>.*)<[0-9]*><(?<PlayerID>STEAM_[0-9]*:[0-9]*:[0-9]*|BOT)><>"\sconnected,
            { "PlayerConnected", "\"(?<PlayerName>.*)<[0-9]*><(?<PlayerID>STEAM_[0-9]*:[0-9]*:[0-9]*|BOT)><>\"\\sconnected," },

            //"(?<PlayerName>.*)<[0-9]*><(?<PlayerID>STEAM_[0-9]*:[0-9]*:[0-9]*|BOT)><>"\sentered\sthe\sgame
            { "PlayerEnteredTheGame", "\"(?<PlayerName>.*)<[0-9]*><(?<PlayerID>STEAM_[0-9]*:[0-9]*:[0-9]*|BOT)><>\"\\sentered\\sthe\\sgame" },
            
            //"(?<KillerName>.*)<[0-9]*><(?<Killer>STEAM_[0-9]*:[0-9]*:[0-9]*|BOT)><(?<KillerTeam>CT|TERRORIST)>"\s\[(?<killerx>[-0-9]*)\s(?<killery>[-0-9]*)\s(?<killerz>[-0-9]*)\]\skilled\s"(?<VictimName>.*)<[0-9]*><(?<Victim>STEAM_[0-9]*:[0-9]*:[0-9]*|BOT)><(?<VictimTeam>CT|TERRORIST)>"\s\[(?<VictimX>[-0-9]*)\s(?<VictimY>[-0-9]*)\s(?<VictimZ>[-0-9]*)\]\swith\s"(?<Weapon>.*)"(\s\((?<Headshot>headshot)\))?
            { "PlayerKilled", "\"(?<KillerName>.*)<[0-9]*><(?<Killer>STEAM_[0-9]*:[0-9]*:[0-9]*|BOT)><(?<KillerTeam>CT|TERRORIST)>\"\\s\\[(?<killerx>[-0-9]*)\\s(?<killery>[-0-9]*)\\s(?<killerz>[-0-9]*)\\]\\skilled\\s\"(?<VictimName>.*)<[0-9]*><(?<Victim>STEAM_[0-9]*:[0-9]*:[0-9]*|BOT)><(?<VictimTeam>CT|TERRORIST)>\"\\s\\[(?<VictimX>[-0-9]*)\\s(?<VictimY>[-0-9]*)\\s(?<VictimZ>[-0-9]*)\\]\\swith\\s\"(?<Weapon>.*)\"(\\s\\((?<Headshot>headshot)\\))?" },

            //"(?<Name>.*)<[0-9]*><(?<UserID>STEAM_[0-9]*:[0-9]*:[0-9]*|BOT)><(?<Team>TERRORIST|CT)>"\spurchased\s"(?<Item>.*)"
            { "PlayerPurchased", "\"(?<Name>.*)<[0-9]*><(?<UserID>STEAM_[0-9]*:[0-9]*:[0-9]*|BOT)><(?<Team>TERRORIST|CT)>\"\\spurchased\\s\"(?<Item>.*)\"" },

            //"(?<PlayerName>.*)<[0-9]*><(?<PlayerID>STEAM_[0-9]*:[0-9]*:[0-9]*|BOT)>"\sswitched\sfrom\steam\s<(?<TeamFrom>Unassigned|TERRORIST|CT)>\sto\s<(?<TeamTo>Unassigned|TERRORIST|CT)>
            { "PlayerSwitchedTeam", "\\\"(?<PlayerName>.*)<[0-9]*><(?<PlayerID>STEAM_[0-9]*:[0-9]*:[0-9]*|BOT)>\"\\sswitched\\sfrom\\steam\\s<(?<TeamFrom>Unassigned|TERRORIST|CT)>\\sto\\s<(?<TeamTo>Unassigned|TERRORIST|CT)>" },

            //"(?<KillerName>.*)<[0-9]*><(?<Killer>STEAM_[0-9]*:[0-9]*:[0-9]*|BOT)><(?<KillerTeam>CT|TERRORIST)>"\sthrew\s(?<Item>.*)\s\[(?<ThrowX>[-0-9]*)\s(?<ThrowY>[-0-9]*)\s(?<ThrowZ>[-0-9]*)\](\s[a-zA-Z]*\sentindex\s(?<EntityID>[0-9]*)\))?
            { "PlayerThrew", "\"(?<KillerName>.*)<[0-9]*><(?<Killer>STEAM_[0-9]*:[0-9]*:[0-9]*|BOT)><(?<KillerTeam>CT|TERRORIST)>\"\\sthrew\\s(?<Item>.*)\\s\\[(?<ThrowX>[-0-9]*)\\s(?<ThrowY>[-0-9]*)\\s(?<ThrowZ>[-0-9]*)\\](\\s[a-zA-Z]*\\sentindex\\s(?<EntityID_INT>[0-9]*)\\))?" },

            //Molotov\sprojectile\sspawned\sat\s(?<ThrownX>[0-9.-]*)\s(?<ThrownY>[0-9.-]*)\s(?<ThrownZ>[0-9.-]*),\svelocity\s(?<VelocityX>[0-9.-]*)\s(?<VelocityY>[0-9.-]*)\s(?<VelocityZ>[0-9.-]*)
            { "PlayerThrewMolotov", "Molotov\\sprojectile\\sspawned\\sat\\s(?<ThrownX>[0-9.-]*)\\s(?<ThrownY>[0-9.-]*)\\s(?<ThrownZ>[0-9.-]*),\\svelocity\\s(?<VelocityX>[0-9.-]*)\\s(?<VelocityY>[0-9.-]*)\\s(?<VelocityZ>[0-9.-]*)" },

            //"(?<PlayerName>.*)<[0-9]*><(?<PlayerID>STEAM_[0-9]*:[0-9]*:[0-9]*|BOT)><(?<Team>Unassigned|TERRORIST|CT)>"\striggered\s"(?<EventID>[a-zA-Z0-9_-]*)"(\s\(value\s"(?<Value>.*)"\))?
            { "PlayerTriggered", "\"(?<PlayerName>.*)<[0-9]*><(?<PlayerID>STEAM_[0-9]*:[0-9]*:[0-9]*|BOT)><(?<Team>Unassigned|TERRORIST|CT)>\"\\striggered\\s\"(?<EventID>[a-zA-Z0-9_-]*)\"(\\s\\(value\\s\"(?<Value>.*)\"\\))?" },

            //World\striggered\s"(?<Type>[a-zA-Z0-9_\(\)]*)"(\son\s"(?<Map>[a-zA-Z0-9_]*)")?
            { "WorldTriggered", "World\\striggered\\s\"(?<Type>[a-zA-Z0-9_\\(\\)]*)\"(\\son\\s\"(?<Map>[a-zA-Z0-9_]*)\")?" },

            //"(?<PlayerName>.*)<[0-9]*><(?<PlayerID>STEAM_[0-9]*:[0-9]*:[0-9]*|BOT)><(?<PlayerTeam>CT|TERRORIST)>"\sdisconnected\s\(reason\s"(?<PlayerReason>.*)"\)
            { "PlayerDisconnected", "\"(?<PlayerName>.*)<[0-9]*><(?<PlayerID>STEAM_[0-9]*:[0-9]*:[0-9]*|BOT)><(?<PlayerTeam>Unassigned|CT|TERRORIST)?>\"\\sdisconnected\\s\\(reason\\s\"(?<PlayerReason>.*)\"\\)" },

            //Team\s"(?<Team>CT|TERRORIST)"\sscored\s"(?<Score>[0-9]*)"\swith\s"(?<Players>[0-9])"\splayers
            { "TeamScored", "Team\\s\"(?<Team>CT|TERRORIST)\"\\sscored\\s\"(?<Score_INT>[0-9]*)\"\\swith\\s\"(?<Players_INT>[0-9])\"\\splayers" },

            //Team\s"(?<Team>CT|TERRORIST)"\striggered\s"(?<Event>[a-zA-Z0-9-_]*)"\s\(CT\s"(?<TeamCT>[0-9]*)"\)\s\(T\s"(?<TeamT>[0-9]*)"\)
            { "TeamTriggered", "Team\\s\"(?<Team>CT|TERRORIST)\"\\striggered\\s\"(?<Event>[a-zA-Z0-9-_]*)\"\\s\\(CT\\s\"(?<TeamCT_INT>[0-9]*)\"\\)\\s\\(T\\s\"(?<TeamT_INT>[0-9]*)\"\\)" },

            //\"(?<KillerName>.*)<[0-9]*><(?<Killer>STEAM_[0-9]*:[0-9]*:[0-9]*|BOT)><(?<KillerTeam>CT|TERRORIST)>\"\\s\\[(?<killerx>[-0-9]*)\\s(?<killery>[-0-9]*)\\s(?<killerz>[-0-9]*)\\]\\swas\\skilled\\sby\\sthe\\sbomb.
            { "PlayerKilledByTheBomb", "\"(?<KillerName>.*)<[0-9]*><(?<Killer>STEAM_[0-9]*:[0-9]*:[0-9]*|BOT)><(?<KillerTeam>CT|TERRORIST)>\"\\s\\[(?<killerx>[-0-9]*)\\s(?<killery>[-0-9]*)\\s(?<killerz>[-0-9]*)\\]\\swas\\skilled\\sby\\sthe\\sbomb." },

            //Game\sOver:\s(?<Type>.*)\s(?<Map>[a-zA-Z0-9-_]*)\sscore\s(?<Team1>[0-9]*):(?<Team2>[0-9]*)\safter\s(?<LengthInMin>[0-9]*)\smin
            { "GameOver", "Game\\sOver:\\s(?<Type>.*)\\s(?<Map>[a-zA-Z0-9-_]*)\\sscore\\s(?<Team1_INT>[0-9]*):(?<Team2_INT>[0-9]*)\\safter\\s(?<LengthInMin_INT>[0-9]*)\\smin" },

            // "(?<Player1Name>.*)<[0-9]*><(?<Player1ID>STEAM_[0-9]*:[0-9]*:[0-9]*|BOT)><(?<Player1Team>CT|TERRORIST)>"\s\[(?<Player1X>[-0-9]*)\s(?<Player1Y>[-0-9]*)\s(?<Player1Z>[-0-9]*)\]\sattacked\s"(?<Player2Name>.*)<[0-9]*><(?<Player2ID>STEAM_[0-9]*:[0-9]*:[0-9]*|BOT)><(?<Player2Team>CT|TERRORIST)>"\s\[(?<Player2X>[-0-9]*)\s(?<Player2Y>[-0-9]*)\s(?<Player2Z>[-0-9]*)\]\swith\s"(?<Item>.*)"\s\(damage\s"(?<DamageHealth>[0-9]*)"\)\s\(damage_armor\s"(?<DamageArmor>[0-9]*)"\)\s\(health\s"(?<Health>[0-9]*)"\)\s\(armor\s"(?<Armor>[0-9]*)"\)\s\(hitgroup\s"(?<Hitgroup>[0-9a-zA-Z_\s]*)"\)
            { "PlayerDamaged", "\"(?<Player1Name>.*)<[0-9]*><(?<Player1ID>STEAM_[0-9]*:[0-9]*:[0-9]*|BOT)><(?<Player1Team>CT|TERRORIST)>\"\\s\\[(?<Player1X>[-0-9]*)\\s(?<Player1Y>[-0-9]*)\\s(?<Player1Z>[-0-9]*)\\]\\sattacked\\s\"(?<Player2Name>.*)<[0-9]*><(?<Player2ID>STEAM_[0-9]*:[0-9]*:[0-9]*|BOT)><(?<Player2Team>CT|TERRORIST)>\"\\s\\[(?<Player2X>[-0-9]*)\\s(?<Player2Y>[-0-9]*)\\s(?<Player2Z>[-0-9]*)\\]\\swith\\s\"(?<Item>.*)\"\\s\\(damage\\s\"(?<DamageHealth_INT>[0-9]*)\"\\)\\s\\(damage_armor\\s\"(?<DamageArmor_INT>[0-9]*)\"\\)\\s\\(health\\s\"(?<Health_INT>[0-9]*)\"\\)\\s\\(armor\\s\"(?<Armor_INT>[0-9]*)\"\\)\\s\\(hitgroup\\s\"(?<Hitgroup>[0-9a-zA-Z_\\s]*)\"\\)" },
        };

        public static void ParseLog(string server, string log)
        {
            foreach (var regex in regexHash)
            {
                var match = Regex.Match(log, regex.Value);
                if (!match.Success)
                {
                    continue;
                }

                try
                {
                    SaveObject(regex.Key, server, match);
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("Table '") && ex.Message.Contains("doesn't exist"))
                    {
                        try
                        {
                            CreateTable(regex.Key, match);
                            SaveObject(regex.Key, server, match);
                        }
                        catch (Exception ex2)
                        {
                            if (ex2.Message.Contains("Table '") && ex2.Message.Contains("' already exists'"))
                            {
                                try
                                {
                                    SaveObject(regex.Key, server, match);
                                }
                                catch (Exception ex3)
                                {
                                    throw ex3;
                                }
                            }
                            else
                            {
                                throw ex2;
                            }
                        }
                    }
                    else
                    {
                        throw ex;
                    }

                    if (regex.Key != "Log")
                        break;

                }
            }
        }

        private static void CreateTable(string objectType, Match match)
        {
            string CreateTable = $"CREATE TABLE `{objectType}` (`id` INT NOT NULL AUTO_INCREMENT,`Timestamp` DATETIME NOT NULL,`Server` VARCHAR(255) NOT NULL,";
            foreach (Group group in match.Groups)
            {
                if (int.TryParse(group.Name, out int i))
                {
                    continue;
                }

                string groupName = group.Name;
                if (groupName.EndsWith("_INT"))
                {
                    groupName = groupName.Replace("_INT", "");
                    CreateTable += $"`{groupName}` INT NULL,";
                }
                else
                {
                    CreateTable += $"`{groupName}` VARCHAR(255) NULL,";
                }
            }

            CreateTable += " PRIMARY KEY(`id`));";

            using (MySqlConnection conn = new MySqlConnection(mySqlConnectionStringBuilder.ConnectionString))
            {
                using (MySqlCommand comm = new MySqlCommand())
                {
                    try
                    {
                        comm.Connection = conn;
                        comm.CommandText = CreateTable;

                        conn.Open();
                        comm.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
        }

        private static void SaveObject(string objectType, string server, Match match)
        {
            using (MySqlConnection conn = new MySqlConnection(mySqlConnectionStringBuilder.ConnectionString))
            {
                using (MySqlCommand comm = new MySqlCommand())
                {
                    comm.Connection = conn;
                    comm.CommandText = $"INSERT INTO {objectType} (Timestamp,Server";
                    string CommandTextAppend = $"VALUES(@Timestamp,@Server";

                    comm.Parameters.AddWithValue("@Server", server);
                    comm.Parameters.AddWithValue("@Timestamp", DateTime.Now);

                    foreach (Group group in match.Groups)
                    {
                        if (!group.Success)
                        {
                            continue;
                        }
                        if (int.TryParse(group.Name, out int i))
                        {
                            continue;
                        }

                        string groupName = group.Name;
                        string groupValue = group.Value;
                        if (groupName.EndsWith("_INT"))
                        {
                            groupName = groupName.Replace("_INT", "");
                            if (int.TryParse(groupValue, out int groupValueNew))
                            {
                                comm.Parameters.AddWithValue($"@{groupName}", groupValueNew);
                            }
                            else
                            {
                                comm.Parameters.AddWithValue($"@{groupName}", DBNull.Value);
                            }

                        }
                        else
                        {
                            comm.Parameters.AddWithValue($"@{groupName}", groupValue);
                        }

                        comm.CommandText += $",{groupName}";
                        CommandTextAppend += $",@{groupName}";

                    }
                    comm.CommandText += $") {CommandTextAppend})";

                    try
                    {
                        conn.Open();
                        comm.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
        }
    }
}