using UnityEngine;
using System.IO;
using System.Collections.Generic;


public static class GestureSerializer
{
    /// <summary>
    /// Save gesture data to a file
    /// </summary>
    /// <param name="g">Gesture type to be saved</param>
    /// <param name="data">Gesture data to be saved</param>
    public static void SaveGestureDataRaw(GestureType g, Vector3[] data)
    {
        string gestureName = g.ToString();
        string jsonData;
        string mainDirectoryName = "HeadGestureData/";
        string gestureDirectoryName = mainDirectoryName + gestureName + "/";
        int initialGestureIndex = 0;

        //Create object to be saved
        GestureDataContainer gObj = new GestureDataContainer
        {
            name = gestureName,
            angularVelocity = data
        };

        //Make a json string from the object above
        jsonData = JsonUtility.ToJson(gObj);

        //Check if main folder exist
        if (!Directory.Exists(mainDirectoryName))
        {
            //if it doesn't, create it
            Directory.CreateDirectory(mainDirectoryName);
        }

        //Check if folder for this type of head gesture exists
        if (!Directory.Exists(gestureDirectoryName))
        {
            //if it doesn't, create it
            Directory.CreateDirectory(gestureDirectoryName);
        }

        //Create file for this gesture
        CreateFile(gestureDirectoryName, initialGestureIndex, jsonData);
    }

    /// <summary>
    /// Creates a file in correct folder with correct name with given gesture data.
    /// Recursive function keeps the data in separate files for each gesture.
    /// </summary>
    /// <param name="directoryName">Directory where gesture is going to be saved</param>
    /// <param name="gestureNumber">It's used as a means of separating data into separate folders</param>
    /// <param name="data">Data in json(string) format</param>
    static void CreateFile(string directoryName, int gestureNumber, string data)
    {
        string filePath = directoryName + gestureNumber + ".json";

        //Check if data file exists
        if (File.Exists(filePath))
        {
            gestureNumber += 1;
            CreateFile(directoryName, gestureNumber, data);
        }
        else
        {
            File.WriteAllText(filePath, data);
        }
    }

    /// <summary>
    /// Get all gesture data in a single array and size of each list per gesture type.
    /// </summary>
    /// <param name="gestureCount">How many gestures types should this function return</param>
    /// <param name="sizeOfListPerGesture">Array to which size of list containing each gesture data will be returned</param>
    /// <param name="data">Array to which all collective data will be returned</param>
    public static void LoadAllGesturesDataRaw(int gestureCount, out int[] sizeOfListPerGesture, out double[][] data)
    {
        //This holds the gestures data read from the files
        List<double[]> allGestures = new List<double[]>();

        sizeOfListPerGesture = new int[gestureCount];

        //Loop through all simple gestures
        for (int i = 0; i < gestureCount; i++)
        {
            //Get simple gesture data from file
            double[][] temp = LoadGestureDataRaw((GestureType)i);

            //Remember the size of each array
            sizeOfListPerGesture[i] = temp.Length;

            //Add the gesture data to the list
            allGestures.AddRange(temp);
        }

        //Return all gesture data
        data = allGestures.ToArray();
    }

    /// <summary>
    /// Get data of one gesture type.
    /// </summary>
    /// <param name="g">Gesture Type data to be returned</param>
    /// <returns></returns>
    public static double[][] LoadGestureDataRaw(GestureType g)
    {
        //Temporary container of data from file
        Vector3[] data;

        //Read Data from file into the array of vectors
        ReadRawData(g, out data);

        //Create new array of arrays to hold the data
        //This is to do with the format required by the Accord.NET algorithms
        double[][] arrData = new double[data.Length][];

        //Take the data from the angularVelocity vector and put them in the array of arrays
        for (int i = 0; i < data.Length; i++)
        {
            arrData[i] = new[]
            {
                (double)
                data[i].x,
                data[i].y,
                data[i].z
            };
        }

        return arrData;
    }

    /// <summary>
    /// Saves quantized gesture data to the .json file
    /// </summary>
    /// <param name="g">Gesture type to be saved</param>
    /// <param name="gestureData"></param>
    public static void SaveGestureDataQuantized(GestureType g, int[] gestureData)
    {
        string gestureName = g.ToString();
        string mainDirName = "QuantizedData/";
        string gestureDirectoryName = mainDirName + gestureName;

        string jsonData;

        GestureQuantizedDataContainer obj = new GestureQuantizedDataContainer
        {
            data = gestureData
        };

        //Make a json string from the object above
        jsonData = JsonUtility.ToJson(obj);

        //Check if main folder exist
        if (!Directory.Exists(mainDirName))
        {
            //if it doesn't, create it
            Directory.CreateDirectory(mainDirName);
        }

        string filePath = gestureDirectoryName + ".json";

        File.WriteAllText(filePath, jsonData);
    }

    /// <summary>
    /// Reads quantized gesture data from the .json file
    /// </summary>
    /// <param name="g">Gesture Type data to be returned</param>
    /// <returns></returns>
    public static int[] LoadGestureDataQuantized(GestureType g)
    {
        string gestureName = g.ToString();
        string mainDirName = "QuantizedData/";
        string gestureDirectoryName = mainDirName + g.ToString();

        string readString = File.ReadAllText(gestureDirectoryName+".json");
        GestureQuantizedDataContainer obj = new GestureQuantizedDataContainer();
        obj = JsonUtility.FromJson<GestureQuantizedDataContainer>(readString);

        return obj.data;
    }

    /// <summary>
    /// Gets all data from given gesture type into a Vector3[]
    /// </summary>
    /// <param name="g">Gesture Type data to be retrieved</param>
    /// <param name="container">Data container</param>
    static void ReadRawData(GestureType g, out Vector3[] container)
    {
        string mainDirectoryName = "HeadGestureData/";
        string gestureDirectoryName = mainDirectoryName + g.ToString()+"/";

        //Get directory info
        DirectoryInfo dir = new DirectoryInfo(gestureDirectoryName);
        //Get fileInfo from ^ directoryInfo of type Json
        FileInfo[] info = dir.GetFiles("*.json");

        //Temporarily holds data from the file
        List<Vector3> l = new List<Vector3>();

        //Loop through all files and get their data into the list ^
        foreach (FileInfo f in info)
        {
            string readString = File.ReadAllText(gestureDirectoryName + f.Name);
            GestureDataContainer temp = new GestureDataContainer();
            temp = JsonUtility.FromJson<GestureDataContainer>(readString);

            l.AddRange(temp.angularVelocity);
        }

        //return angular velocity array
        container = l.ToArray();
    }

}
