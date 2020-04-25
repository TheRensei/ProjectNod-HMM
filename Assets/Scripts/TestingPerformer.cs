using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Accord.Statistics.Models.Markov; // hidden markov model
using Accord.Statistics.Distributions.Univariate; // general discrete distribution

public class TestingPerformer : MonoBehaviour
{
    readonly int simpleGestureCount = 7;
    readonly int complexGestureCount = 2;
    readonly int sequenceLength = 10;

    public void RunSimpleTest()
    {
        List<int[][]> splitGestureData = new List<int[][]>();

        HiddenMarkovModel<GeneralDiscreteDistribution, int>[] SimpleHMMs = new HiddenMarkovModel<GeneralDiscreteDistribution, int>[simpleGestureCount];

        for (int i = 0; i < simpleGestureCount; i++)
        {
            //Load quantized simple gestures
            int[] gestureData = GestureSerializer.LoadGestureDataQuantized((GestureType)i);

            //split into sequences of 10
            splitGestureData.Add(ArrayUtilities.SplitArray(gestureData, sequenceLength));

            //Load simple HMMs
            SimpleHMMs[i] = (GestureModelSerializer.LoadHMM((GestureType)i));
        }

        float simpleGesturesAccuracy = 0;

        //Loop through all simple gestures
        for (int i = 0; i < simpleGestureCount; i++)
        {
            float correct = 0;

            //Loop through all sequences in each gesture data
            for (int j = 0; j < splitGestureData[i].Length; j++)
            {
                float[] results = new float[SimpleHMMs.Length];

                //Loop through all simpleHMMs
                for (int k = 0; k < SimpleHMMs.Length; k++)
                {
                    //calculate posterior probability
                    double l1 = SimpleHMMs[k].LogLikelihood(splitGestureData[i][j]);
                    results[k] = (float)l1;
                }

                //find label of the hmm that had the biggest probability
                int gestureLabel = System.Array.IndexOf(results, Mathf.Max(results));

                //The expected label for the sequence is the index of the gesture data loaded from disk
                if (gestureLabel == i)
                    correct++;
            }

            //Measure average accuracy
            float averageAcurracy = correct / (float)splitGestureData[i].Length * 100f;
            simpleGesturesAccuracy += averageAcurracy;
            Debug.LogFormat("Average accuracy: {0}% reported for {1} HMM", averageAcurracy, (GestureType)i);
        }

        Debug.LogFormat("Average accuracy of all simple gestures reported: {0}", simpleGesturesAccuracy/simpleGestureCount);
        Debug.Log("Simple Testing Done");
    }

    public void RunComplexTest()
    {
        //Assign labels to the sequences using simpleHMMs
        HiddenMarkovModel<GeneralDiscreteDistribution, int>[] SimpleHMMs = new HiddenMarkovModel<GeneralDiscreteDistribution, int>[simpleGestureCount];
        for (int i = 0; i < simpleGestureCount; i++)
        {
            SimpleHMMs[i] = (GestureModelSerializer.LoadHMM((GestureType)i));
        }

        HiddenMarkovModel<GeneralDiscreteDistribution, int>[] complexHMMs = new HiddenMarkovModel<GeneralDiscreteDistribution, int>[complexGestureCount];
        complexHMMs[0] = GestureModelSerializer.LoadHMM(GestureType.Shaking);
        complexHMMs[1] = GestureModelSerializer.LoadHMM(GestureType.Nodding);


        float averageComplexAccuracy = 0;

        for (int i = 0; i < complexGestureCount; i++)
        {
            //Load quantized complex gesture data and split it into sequences of 10
            int[][] splitData = ArrayUtilities.SplitArray(GestureSerializer.LoadGestureDataQuantized((GestureType)i+simpleGestureCount), sequenceLength);

            int[] labelledData = new int[splitData.Length];

            for (int j = 0; j < splitData.Length; j++)
            {
                float[] results = new float[SimpleHMMs.Length];

                //Loop through all simpleHMMs
                for (int k = 0; k < SimpleHMMs.Length; k++)
                {
                    //calculate posterior probability
                    double l1 = SimpleHMMs[k].LogLikelihood(splitData[j]);
                    results[k] = (float)l1;
                }

                //find biggest probability
                float a = Mathf.Max(results);

                //find label of the hmm that had the biggest probability
                int gestureLabel = System.Array.IndexOf(results, a);

                labelledData[j] = gestureLabel;
            }

            //Split labelled data array
            int[][] labelledSequence = ArrayUtilities.SplitArray(labelledData, sequenceLength);

            float correct = 0;
            float smallest = Mathf.Infinity;
            //Loop through all shaking sequences
            for (int j = 0; j < labelledSequence.Length; j++)
            {
                float[] results = new float[2];
                results[0] = (float)complexHMMs[0].LogLikelihood(labelledSequence[j]);
                results[1] = (float)complexHMMs[1].LogLikelihood(labelledSequence[j]);

                int gestureLabel = System.Array.IndexOf(results, Mathf.Max(results)) + simpleGestureCount;

                if (gestureLabel == i + simpleGestureCount)
                    correct++;

                //-------------------------
                if (results[i] < smallest)
                    smallest = results[i];
            }

            //Debug.LogFormat("Threshold {0} found for {1}", smallest, (GestureType)i+simpleGestureCount);

            //Measure average accuracy
            float averageAcurracy = correct / (float)labelledSequence.Length * 100f;
            averageComplexAccuracy += averageAcurracy;
            Debug.LogFormat("Average accuracy: {0}% reported for {1} HMM", averageAcurracy, (GestureType)i+simpleGestureCount);
        }

        Debug.LogFormat("Average accuracy of all complex gestures reported: {0}", averageComplexAccuracy/complexGestureCount);
        Debug.Log("Complex Testing Done");
    }
}