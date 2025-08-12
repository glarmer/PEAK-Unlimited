using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Zorro.Core;

namespace PEAKUnlimited;

public class Utility
{
    public static List<Vector3> GetEvenlySpacedPointsAroundCampfire(int numPoints, float innerRadius, float outerRadius, Vector3 campfirePosition, Segment advanceToSegment)
    {
        List<Vector3> points = new List<Vector3>();
    
        for (int i = 0; i < numPoints; i++)
        {
            float radius = outerRadius;
            if (i % 2 == 0)
            {
                radius = innerRadius;
            }
            
            float angle = i * Mathf.PI * 2f / numPoints; // Even spacing: 2π / n
            float x = radius * Mathf.Cos(angle);
            float z = radius * Mathf.Sin(angle);
            
            points.Add(SetToGround(new Vector3(x, 0f, z) + campfirePosition));
        }
        
        return points;
    }

    public static List<GameObject> SpawnMarshmallows(int number, Vector3 campfirePosition, Segment advanceToSegment)
    {
        List<GameObject> marshmallows = new List<GameObject>();
        Item obj = SingletonAsset<ItemDatabase>.Instance.itemLookup[46];
        Plugin.Logger.LogInfo((object) ("Plugin PeakUnlimited " + obj.GetName()));
        obj.GetName();
        foreach (Vector3 position in GetEvenlySpacedPointsAroundCampfire(number, 2.5f, 3f, campfirePosition,
                     advanceToSegment))
        {
            marshmallows.Add(Add(obj, position).gameObject);
        }
        Plugin.Logger.LogInfo((object) ("Plugin PeakUnlimited added with position: " + obj.GetName()));
        return marshmallows;
    }

    private static Vector3 SetToGround(Vector3 vector)
    {
        return HelperFunctions.GetGroundPos(vector, HelperFunctions.LayerType.TerrainMap);
    }

    public static Item Add(Item item, Vector3 position)
    {
        if (!PhotonNetwork.IsConnected)
            return null;
        Plugin.Logger.LogInfo((object) string.Format("Spawn item: {0} at {1}", (object) item, (object) position));
        return PhotonNetwork.Instantiate("0_Items/" + item.name, position, Quaternion.Euler(0f, Random.Range(0f, 360f), 0f)).GetComponent<Item>();
    }
}