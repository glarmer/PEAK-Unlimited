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
            
            float angle = i * Mathf.PI * 2f / numPoints; // Even spacing: 2π / n
            float x = radius * Mathf.Cos(angle);
            float z = radius * Mathf.Sin(angle);
            
            Vector3 localPos = new Vector3(x, 0f, z);
            Vector3 rotatedOffset = campfireRotation * localPos;
            Vector3 worldPos = campfirePosition + rotatedOffset;
            worldPos.y += -0.05f;

            points.Add(worldPos);
        }
        
        return points;
    }

    public static List<GameObject> SpawnMarshmallows(int number, Vector3 campfirePosition, Vector3 campfireAngles, Segment advanceToSegment)
    {
        List<GameObject> marshmallows = new List<GameObject>();
        Item obj = SingletonAsset<ItemDatabase>.Instance.itemLookup[46];
        Plugin.Logger.LogInfo((object) ("Plugin PeakUnlimited " + obj.GetName()));
        obj.GetName();
        foreach (Vector3 position in GetEvenlySpacedPointsAroundCampfire(number, 2f, 2.5f, campfirePosition, campfireAngles,
                     advanceToSegment))
        {
            Vector3 directionToCampfire = (campfirePosition - position).normalized;
            Quaternion rotation = Quaternion.LookRotation(directionToCampfire, Vector3.up);
            rotation *= Quaternion.Euler(0f, Random.Range(-30f, -150f), 0f);
            marshmallows.Add(Add(obj, position, rotation).gameObject);
        }
        Plugin.Logger.LogInfo((object) ("Plugin PeakUnlimited added with position: " + obj.GetName()));
        return marshmallows;
    }

    public static Item Add(Item item, Vector3 position, Quaternion rotation)
    {
        if (!PhotonNetwork.IsConnected)
            return null;
        Plugin.Logger.LogInfo((object) string.Format("Spawn item: {0} at {1}", (object) item, (object) position));
        return PhotonNetwork.Instantiate("0_Items/" + item.name, position, rotation).GetComponent<Item>();
    }
}