using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using Facepunch;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Oxide.Core;
using Oxide.Core.Plugins;
using Oxide.Game.Rust.Cui;
using UnityEngine;

namespace Oxide.Plugins
{
    [Info("Custom Kits", "Marco Fernandes", 1.0)]
    [Description("Customised Kits for the boys")]
    public class Kits : RustPlugin
    {
        //Kits:
        //starter - 2k Wood, 2k Stone, 500 Frags, 50 Low grade fuel, 100 Cloth (Single Use)
        //tools - Pickaxe, Hatchet (1hr CD)
        //food - 5 Corn (2hr CD)
        //torch - 1 Torch (Infinite CD)

        Dictionary<ulong, bool> starterCooldowns = new Dictionary<ulong, bool>();
        Dictionary<ulong, int> toolsCooldowns = new Dictionary<ulong, int>();
        Dictionary<ulong, int> foodCooldowns = new Dictionary<ulong, int>();

        [ChatCommand("kit")]
        void Kit(BasePlayer player, string command, string[] args)
        {
            ulong TARGET = player.userID;

            if (args.Length == 0)
            {
                SendReply(player, "Please enter a kit name.");
            }
            else if (args[0].ToLower() == "starter")
            {
                if (player.inventory.AllItems().Length > 31)
                {
                    SendReply(player, "Inventory Full!");
                }
                else
                    if (!starterCooldowns.ContainsKey(TARGET) == true)
                    {
                        //Based on usage (1 use only)
                        Item wood = ItemManager.CreateByName("wood", 2000);
                        player.inventory.GiveItem(wood);

                        Item stone = ItemManager.CreateByName("stones", 2000);
                        player.inventory.GiveItem(stone);

                        Item metal_frags = ItemManager.CreateByName("metal.fragments", 500);
                        player.inventory.GiveItem(metal_frags);

                        Item low_grade = ItemManager.CreateByName("lowgradefuel", 50);
                        player.inventory.GiveItem(low_grade);

                        Item cloth = ItemManager.CreateByName("cloth", 100);
                        player.inventory.GiveItem(cloth);


                        var starterUseState = false;
                        starterCooldowns.Add(TARGET, starterUseState);
                        if (starterCooldowns.ContainsKey(TARGET) == true)
                        {
                            string starterDefaultString = string.Format("Kit starter already claimed");
                            SendReply(player, starterDefaultString);
                            if (starterCooldowns[TARGET] == true)
                            {
                                starterCooldowns[TARGET] = true;
                                starterCooldowns.Remove(TARGET);
                            }
                            else if (starterCooldowns.ContainsKey(TARGET) == false)
                            {
                                starterCooldowns.Add(TARGET, starterUseState);
                            }
                        }

                        SendReply(player, "You have claimed kit starter");
                }
                else
                {
                    string starterDefaultString = string.Format("Kit starter already claimed");
                    SendReply(player, starterDefaultString);
                }

            }
            else if (args[0].ToLower() == "tools")
            {
                if (player.inventory.AllItems().Length > 34)
                {
                    SendReply(player, "Inventory Full!");
                }
                else
                    if (!toolsCooldowns.ContainsKey(TARGET) == true)
                    {
                        //Based on time (2hr cd)
                        Item metal_pickaxe = ItemManager.CreateByName("pickaxe", 1);
                        player.inventory.GiveItem(metal_pickaxe);

                        Item metal_hatchet = ItemManager.CreateByName("hatchet", 1);
                        player.inventory.GiveItem(metal_hatchet);

                        var toolsWaitTime = 7200;
                        toolsCooldowns.Add(TARGET, toolsWaitTime);
                        Timer toolsTimer = null;
                        toolsTimer = timer.Every(1f, () =>
                        {
                            if (toolsCooldowns.ContainsKey(TARGET) == true)
                            {
                                var toolsTimeRemaining = toolsCooldowns[TARGET];
                                string toolsDefaultString = string.Format("Kit tools already claimed - {0} seconds remaining.", toolsTimeRemaining);
                                if (toolsCooldowns[TARGET] == 0)
                                {
                                    toolsTimer.Destroy();
                                    toolsCooldowns.Remove(TARGET);
                                }
                                else if (toolsCooldowns[TARGET] != 0)
                                {
                                    toolsCooldowns[TARGET] = toolsCooldowns[TARGET] - 1;
                                }

                                else if (toolsCooldowns.ContainsKey(TARGET) == false)
                                {
                                    toolsCooldowns.Add(TARGET, toolsWaitTime);
                                }
                            }
                        });

                        SendReply(player, "You have claimed kit tools");
                    }
                else
                {
                    var toolsTimeRemaining = toolsCooldowns[TARGET];
                    string toolsDefaultString = string.Format("Kit tools already claimed - {0} seconds remaining.", toolsTimeRemaining);
                    SendReply(player, toolsDefaultString);
                }
            }
            else if (args[0].ToLower() == "torch")
            {
                if (player.inventory.AllItems().Length > 36)
                {
                    SendReply(player, "Inventory Full!");
                }
                else
                {
                    //Based on time (infinite)
                    Item torch = ItemManager.CreateByName("torch", 1);
                    player.inventory.GiveItem(torch);
                    SendReply(player, "You have claimed kit torch");
                }
            }
            else if (args[0].ToLower() == "food")
            {
                if (player.inventory.AllItems().Length > 36)
                {
                    SendReply(player, "Inventory Full!");
                }
                else
                    if (!foodCooldowns.ContainsKey(TARGET) == true)
                {
                    //Based on time (1hr cd)
                    Item food = ItemManager.CreateByName("corn", 5);
                    player.inventory.GiveItem(food);

                    var foodWaitTime = 3600;
                    foodCooldowns.Add(TARGET, foodWaitTime);
                    Timer foodTimer = null;
                    foodTimer = timer.Every(1f, () =>
                    {
                        if (foodCooldowns.ContainsKey(TARGET) == true)
                        {
                            var foodTimeRemaining = foodCooldowns[TARGET];
                            string foodDefaultString = string.Format("Kit food already claimed - {0} seconds remaining.", foodTimeRemaining);
                            if (foodCooldowns[TARGET] == 0)
                            {
                                foodTimer.Destroy();
                                foodCooldowns.Remove(TARGET);
                            }
                            else if (foodCooldowns[TARGET] != 0)
                            {
                                foodCooldowns[TARGET] = foodCooldowns[TARGET] - 1;
                            }

                            else if (foodCooldowns.ContainsKey(TARGET) == false)
                            {
                                foodCooldowns.Add(TARGET, foodWaitTime);
                            }
                        }
                    });

                    SendReply(player, "You have claimed kit food");
                }
                else
                {
                    var foodTimeRemaining = foodCooldowns[TARGET];
                    string foodDefaultString = string.Format("Kit food already claimed - {0} seconds remaining.", foodTimeRemaining);
                    SendReply(player, foodDefaultString);
                }
            }
            else
            {
                SendReply(player, "Invalid Kit");
            }
        }

        [ChatCommand("kits")]
        void Kits_List(BasePlayer player)
        {
            //List of Kits
            SendReply(player, "starter - 1 Use Only\ntools - 2 Hour Cooldown\nfood - 1 Hour Cooldown\ntorch - Infinite Uses");
        }
        }
    }
