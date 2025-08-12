using HarmonyLib;
using Photon.Pun;
using Zorro.Core;

namespace PEAKUnlimited;

using BepInEx.Logging;
using HarmonyLib;
using Photon.Pun;
using UnityEngine;
using Zorro.Core;
public class CampfireAwakePatch
{
        [HarmonyPatch(typeof(Campfire), "Awake")]
        [HarmonyPostfix]
        static void Postfix(Campfire __instance)
        {
            Plugin.Logger.LogInfo("Campfire Awake Patch!");
            if (!PhotonNetwork.IsMasterClient)
                return;
            
            
            //Backpack addition
            if (Plugin.config.IsExtraBackpacksEnabled)
            {
                Plugin.Logger.LogInfo("Backpackification enabled and starting!");
                Item obj = SingletonAsset<ItemDatabase>.Instance.itemLookup[6];
                int numberOfExtraPlayers = Plugin._numberOfPlayers - Plugin.VANILLA_MAX_PLAYERS;
                int number = 0;
                if (numberOfExtraPlayers > 0) {
                    double backpackNumber = numberOfExtraPlayers * 0.25;
                    
                    if (backpackNumber % 4 == 0)
                    {
                        number = (int)backpackNumber;
                    }
                    else
                    {
                        number = (int)backpackNumber;
                        if (Random.Range(0f, 1f) <= backpackNumber - number)
                        {
                            number++;
                        }
                    }
                }
                if (Plugin.config.CheatBackpacks  != 0)
                {
                    Plugin.Logger.LogInfo("Cheat Backpacks enabled = " + Plugin.config.CheatBackpacks);
                    number = Plugin.config.CheatBackpacks  - 1; //Minus one as there is already a backpack present
                }
                Plugin.Logger.LogInfo("Backpacks enabled = " + number);
                if (number > 0)
                {
                    foreach (Vector3 position in Utility.GetEvenlySpacedPointsAroundCampfire(number, 3.3f, 3.7f,
                                 __instance.gameObject.transform.position,
                                 __instance.advanceToSegment))
                    {
                        Vector3 finalPosition = position;
                        if (__instance.gameObject.transform.parent.gameObject.name.ToLower().Contains("wings"))
                        {
                            finalPosition =
                                position + new Vector3(0, 10f, 0f); // stops backpacks on the beach spawning underground...
                        }
                        Utility.Add(obj, finalPosition).transform.parent = __instance.gameObject.transform;
                    }
                }
                else
                {
                    Plugin.Logger.LogInfo("Not enough players to add additional backpacks, use the Cheat Backpack configuration setting if you want to override this!");
                }
            }
            //End of backpack addition
            
            //Marshmallow addition
            if (__instance.gameObject.transform.parent.gameObject.name.ToLower().Contains("wings"))
            {
                return;
            }
            Plugin.campfireList.Add(__instance);
            Plugin.Logger.LogInfo("Marshmellowifying campfire...!");
            int amountOfMarshmallowsToSpawn = Plugin._numberOfPlayers - Plugin.VANILLA_MAX_PLAYERS;
            if (Plugin.config.CheatMarshmallows != 0)
            {
                Plugin.Logger.LogInfo("Adding cheatmellows!");
                amountOfMarshmallowsToSpawn = Plugin.config.CheatMarshmallows - Plugin.VANILLA_MAX_PLAYERS;
                if (Plugin._numberOfPlayers < Plugin.VANILLA_MAX_PLAYERS)
                {
                    amountOfMarshmallowsToSpawn = Plugin.config.CheatMarshmallows - Plugin._numberOfPlayers;
                }
            }
            
            Plugin.Logger.LogInfo("Start of campfire patch!");
            if (PhotonNetwork.IsMasterClient && (Plugin._numberOfPlayers > Plugin.VANILLA_MAX_PLAYERS || Plugin.config.CheatMarshmallows != 0))
            {
                Plugin.Logger.LogInfo("More than 4 players, preparing to marshmallowify! Number: " + Plugin._numberOfPlayers);
                Vector3 position = __instance.gameObject.transform.position;
                Plugin.marshmallows.Add(__instance, Utility.SpawnMarshmallows(amountOfMarshmallowsToSpawn, position, __instance.advanceToSegment));
            }
            else
            {
                Plugin.Logger.LogInfo("Not enough players for extra marshmallows, use the extra marshmallows cheat configuration option to override this!");
            }
            Plugin.isAfterAwake = true;
        }
    }