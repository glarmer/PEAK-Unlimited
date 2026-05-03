using System;
using System.Collections.Generic;
using BepInEx.Logging;
using PEAKUnlimited.Util.Debugging;
using Photon.Pun;
using UnityEngine;
using Zorro.Core;
using Random = UnityEngine.Random;

namespace PEAKUnlimited;

public enum CampfireFoods
{
    Marshmallow = 46,
    Glizzy = 154
}

public static class Utility
{
    public static List<Vector3> GetEvenlySpacedPointsAroundCampfire(int numPoints, float innerRadius, float outerRadius, Vector3 campfirePosition, Vector3 campfireAngles, Segment advanceToSegment)
    {
        List<Vector3> points = new List<Vector3>();
        Quaternion campfireRotation = Quaternion.Euler(campfireAngles);
        
        for (int i = 0; i < numPoints; i++)
        {
            float radius = outerRadius;
            if (i % 2 == 0)
            {
                radius = innerRadius;
            }
            
            float angle = i * Mathf.PI * 2f / numPoints;
            float x = radius * Mathf.Cos(angle);
            float z = radius * Mathf.Sin(angle);
            
            Vector3 localPos = new Vector3(x, 0f, z);
            Vector3 rotatedOffset = campfireRotation * localPos;
            Vector3 worldPos = campfirePosition + rotatedOffset;
            worldPos.y += -0.05f;
            
            //Fixes edge cases where SetToGround places marshmallows below the floor
            Vector3 worldPosGrounded = SetToGround(worldPos);
            if (Vector3.Distance(worldPos, worldPosGrounded) <= 1f)
            {
                worldPos = worldPosGrounded;
            }
            
            points.Add(worldPos);
        }
        
        return points;
    }

    public static void SpawnMarshmallows(int number, Campfire campfire)
    {
        if (campfire == null)
            return;

        if (Plugin.ConfigurationHandler?.ConfigHotDogChance == null)
            return;

        if (Plugin.Marshmallows == null)
            return;

        ItemDatabase itemDatabase = SingletonAsset<ItemDatabase>.Instance;

        if (itemDatabase?.itemLookup == null)
            return;

        Vector3 campfirePosition = campfire.transform.position;
        Vector3 campfireAngles = campfire.transform.eulerAngles;
        Segment advanceToSegment = campfire.advanceToSegment;

        IEnumerable<Vector3> positions = GetEvenlySpacedPointsAroundCampfire(number, 2f, 2.5f,
            campfirePosition, campfireAngles, advanceToSegment);

        if (positions == null)
            return;

        HashSet<GameObject> marshmallows = new HashSet<GameObject>();
        foreach (Vector3 position in positions)
        {
            float chance = Random.Range(0.0f, 1.0f);
            float glizzyChance = Plugin.ConfigurationHandler.ConfigHotDogChance.Value;

            CampfireFoods randomEnum = chance < glizzyChance
                ? CampfireFoods.Glizzy
                : CampfireFoods.Marshmallow;

            ushort randomValue = (ushort)randomEnum;
            if (!itemDatabase.itemLookup.TryGetValue(randomValue, out Item obj) || obj == null)
            {
                UnlimitedLogger.GetInstance()?.DebugMessage(
                    LogLevel.Warning,
                    DebugLogType.MarshmallowLogic,
                    $"No item found in itemLookup for key {randomValue} / {randomEnum}."
                );

                continue;
            }

            obj.GetName();
            Vector3 directionToCampfire = campfirePosition - position;

            if (directionToCampfire == Vector3.zero)
                directionToCampfire = campfire.transform.forward;

            Quaternion rotation = Quaternion.LookRotation(directionToCampfire.normalized, Vector3.up);
            rotation *= Quaternion.Euler(0f, Random.Range(-30f, -150f), 0f);

            GameObject spawnedItem = InstantiateItem(obj, position, rotation);
            if (spawnedItem != null)
            {
                marshmallows.Add(spawnedItem);
            }
        }

        if (!Plugin.Marshmallows.ContainsKey(campfire))
        {
            Plugin.Marshmallows.Add(campfire, marshmallows);
        }
        else
        {
            foreach (GameObject marshmallow in marshmallows)
            {
                if (marshmallow != null)
                {
                    Plugin.Marshmallows[campfire].Add(marshmallow);
                }
            }
        }
    }
    
    private static Vector3 SetToGround(Vector3 vector)
    {
        return HelperFunctions.GetGroundPos(vector, HelperFunctions.LayerType.TerrainMap);
    }

    public static GameObject InstantiateItem(Item item, Vector3 position, Quaternion rotation)
    {
        if (!PhotonNetwork.IsConnected)
            return null;
        UnlimitedLogger.GetInstance().DebugMessage(LogLevel.Info,DebugLogType.CampfireLogic,$"Spawn item: {item.name} at {position}");
        return PhotonNetwork.Instantiate("0_Items/" + item.name, position, rotation);
    }
}