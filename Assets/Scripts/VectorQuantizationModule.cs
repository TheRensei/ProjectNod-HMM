using System.IO;
using UnityEngine;
using System.Linq;
using Accord.MachineLearning; // KMeans
using System.Collections.Generic;

[System.Serializable]
public class Cluster
{
    public Vector3[] clusterCenters;
}

public static class VectorQuantizationModule
{
    public static List<int[]> RunTrainingVectorQuantization(int clusterCount, int gestureCount, double[][] gestureData, int[] sizeOfListPerGesture)
    {
        //Run K-Means
        int[] labels = RunKMeans(clusterCount, gestureData);

        //Remove Redundant Symbols
        List<List<int>> gestureObservationSymbols = new List<List<int>>();

        SeparateGestureData(gestureCount, labels, sizeOfListPerGesture, ref gestureObservationSymbols);

        RemoveReduntantSymbols(ref gestureObservationSymbols);

        //change list<int> per gesture to int[]
        List<int[]> finalGestureData = new List<int[]>();

        for (int i = 0; i < gestureCount; i++)
        {
            finalGestureData.Add(gestureObservationSymbols[i].ToArray());
        }


        return finalGestureData;
    }

    public static int AssignObservationSymbol(Vector3 angVel, Vector3[] cc)
    {
        float shortestDistance = Mathf.Infinity;

        int observationSymbol = 0;

        foreach (Vector3 c in cc)
        {
            float dist = Vector3.Distance(angVel, c);
            if (dist < shortestDistance)
            {
                shortestDistance = dist;
                observationSymbol = System.Array.IndexOf(cc, c);
            }
        }
        return observationSymbol;
    }

    public static Vector3[] GetClusterCenters()
    {
        string mainDirectoryName = "VectorQuantization/";

        if (!Directory.Exists(mainDirectoryName))
        {
            return null;
        }

        if(!File.Exists(mainDirectoryName + "ClusterCenters.json"))
        {
            return null;
        }

        string readString = File.ReadAllText(mainDirectoryName + "ClusterCenters.json");

       Cluster c = JsonUtility.FromJson<Cluster>(readString);

        return c.clusterCenters;
    }

    static void SaveClusterCenters(double[][] cc)
    {
        Vector3[] clusters = new Vector3[cc.Length];

        for (int i = 0; i < cc.Length; i++)
        {
            clusters[i] = new Vector3(
                (float)cc[i][0],
                (float)cc[i][1],
                (float)cc[i][2]);
        }

        Cluster c = new Cluster
        {
            clusterCenters = clusters
        };

        string jsonData = JsonUtility.ToJson(c);
        string mainDirectoryName = "VectorQuantization/";

        if (!Directory.Exists(mainDirectoryName))
        {
            //if it doesn't, create it
            Directory.CreateDirectory(mainDirectoryName);
        }

        string filePath = mainDirectoryName + "ClusterCenters.json";

        File.WriteAllText(filePath, jsonData);
    }

    static int[] RunKMeans(int k, double[][] observations)
    {
        //Accord.Math.Random.Generator.Seed = 0;

        //Create new K-Means algorithm
        KMeans kmeans = new KMeans(k: k)
        {
            Tolerance = 0.05
        };

        //Compute and retrieve the data centroids
        KMeansClusterCollection clusters = kmeans.Learn(observations);

        //Save for testing vector quantization
        SaveClusterCenters(clusters.Centroids);

        // Use the centroids to parition all the data
        //Labels are the observation symbols
        int [] labels = clusters.Decide(observations);

        return labels;
    }

    static void RemoveReduntantSymbols(ref List<List<int>> observationSymbols)
    {
        //Get all symbols that mean that the head is remaining still to be removed from all gestures besides BeingIdle
        var redundantSymbols = observationSymbols[(int)GestureType.BeingIdle].Distinct();

        //remove redundant symbols for all gestures besides being idle
        for (int i = 1; i < observationSymbols.Count; i++)
        {
            foreach (int s in redundantSymbols)
            {
                //remove all objects in a list that are equal to redundant symbol
                observationSymbols[i].RemoveAll(obj => obj == s);
            }

            if(observationSymbols[i].Count == 0)
            {
                Debug.LogErrorFormat("Array is too small to proceed! \nSequence length {0} for {1}", observationSymbols[i].Count, (GestureType)i);
            }
        }
    }

    static void SeparateGestureData(int gestureCount, int[] observationSymbols, int[] sizeOfListPerGesture, ref List<List<int>> gestureObservationSymbol)
    {
        //Used to loop through all data
        int currentObsIndex = 0;

        //split observation symbols into separate lists per each gesture
        for (int i = 0; i < gestureCount; i++)
        {
            List<int> tempObs = new List<int>();
            for (int a = 0; a < sizeOfListPerGesture[i]; a++, currentObsIndex++)
            {
                tempObs.Add(observationSymbols[currentObsIndex]);
            }
            gestureObservationSymbol.Add(tempObs);
        }
    }
}
