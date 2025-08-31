using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Zorro.Core;

namespace PEAKUnlimited;

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

    public static List<GameObject> SpawnMarshmallows(int number, Vector3 campfirePosition, Vector3 campfireAngles, Segment advanceToSegment)
    {
        List<GameObject> marshmallows = new List<GameObject>();
        Item obj = SingletonAsset<ItemDatabase>.Instance.itemLookup[46];
        obj.GetName();
        foreach (Vector3 position in GetEvenlySpacedPointsAroundCampfire(number, 2f, 2.5f, campfirePosition, campfireAngles,
                     advanceToSegment))
        {
            Vector3 directionToCampfire = (campfirePosition - position).normalized;
            Quaternion rotation = Quaternion.LookRotation(directionToCampfire, Vector3.up);
            rotation *= Quaternion.Euler(0f, Random.Range(-30f, -150f), 0f);
            marshmallows.Add(Add(obj, position, rotation).gameObject);
        }
        return marshmallows;
    }
    
    private static Vector3 SetToGround(Vector3 vector)
    {
        return HelperFunctions.GetGroundPos(vector, HelperFunctions.LayerType.TerrainMap);
    }

    public static Item Add(Item item, Vector3 position, Quaternion rotation)
    {
        if (!PhotonNetwork.IsConnected)
            return null;
        Plugin.Logger.LogInfo($"Spawn item: {item.name} at {position}");
        return PhotonNetwork.Instantiate("0_Items/" + item.name, position, rotation).GetComponent<Item>();
    }
}