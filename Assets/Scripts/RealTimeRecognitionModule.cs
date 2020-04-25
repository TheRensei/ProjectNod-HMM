using System.Collections;
using Accord.Statistics.Models.Markov; // hidden markov model
using Accord.Statistics.Distributions.Univariate; // general discrete distribution
using System.Collections.Generic;
using UnityEngine;
using System;

public class RealTimeRecognitionModule
{
    public RealTimeRecognitionModule(
        int gestureCount,
        int simpleGestureCount,
        int complexGestureCount,
        int sequenceLength)
    {
        this.gestureCount = gestureCount;
        this.simpleGestureCount = simpleGestureCount;
        this.complexGestureCount = complexGestureCount;
        this.sequenceLength = sequenceLength;

        this.simpleBuffer = new int[sequenceLength];
        this.HMMs = new HiddenMarkovModel<GeneralDiscreteDistribution, int>[gestureCount];

        for (int i = 0; i < gestureCount; i++)
        {
            HMMs[i] = GestureModelSerializer.LoadHMM((GestureType)i);
        }

        clusterCenters = VectorQuantizationModule.GetClusterCenters();

        this.initialized = true;
    }

    HiddenMarkovModel<GeneralDiscreteDistribution, int>[] HMMs;
    Vector3[] clusterCenters;

    int gestureCount = 9;
    int simpleGestureCount = 7;
    int complexGestureCount = 2;
    int sequenceLength = 10;

    int[] simpleBuffer = new int[10];
    int currentBufferIndex = 0;
    Queue<int> complexQueue = new Queue<int>();

    private bool initialized = false;


    public GestureType Decide()
    {
        try
        {
            if (!initialized)
                throw new Exception("Module not initialized");
        }
        catch (Exception a)
        {
            Debug.LogError(a);
            return 0;
        }

        int gestureLabel = 0;

        if (currentBufferIndex < sequenceLength)
        {
            //Sample angular velocity, quantize it and buffer the observation symbol
            simpleBuffer[currentBufferIndex] = VectorQuantizationModule.AssignObservationSymbol(OVRManager.display.angularVelocity, clusterCenters);
            currentBufferIndex++;
        }
        else if(currentBufferIndex == sequenceLength)
        {
            //Technically clear's an array, start buffering from the beginning
            currentBufferIndex = 0;

            float[] results = new float[simpleGestureCount];

            //calculate posterior probabilities of all simple HMMs
            for (int i = 0; i < simpleGestureCount; i++)
            {
                double l1 = HMMs[i].LogLikelihood(simpleBuffer);
                results[i] = (float)l1;
            }

            //The Gesture label is retrieved by finding the biggest value in an array and finding an index of this element
            //The index is a gesture type
            gestureLabel = System.Array.IndexOf(results, Mathf.Max(results));

            //If the gesture type was one of the simple gestures creating the complex gestures
            if (gestureLabel >= 0 &&  gestureLabel <= 4)
            {
                //Buffer them
                if (complexQueue.Count < sequenceLength)
                {
                    complexQueue.Enqueue(gestureLabel);
                }
                else //when buffer is full
                {
                    complexQueue.Dequeue();
                    complexQueue.Enqueue(gestureLabel);

                    Array.Clear(results, 0, results.Length);

                    results = new float[complexGestureCount];

                    for (int i = 0; i < complexGestureCount; i++)
                    {
                        double l1 = HMMs[i + simpleGestureCount].LogLikelihood(complexQueue.ToArray());
                        results[i] = (float)l1;
                        Debug.Log(((GestureType)i + simpleGestureCount).ToString() + " " + results[i]);
                    }

                    if (results[0] > -10f || results[1] > -10f)
                    {
                        float m = Mathf.Max(results);
                        gestureLabel = Array.IndexOf(results, m) + simpleGestureCount;
                    }
                }
            }
        }

        return (GestureType)gestureLabel;
    }
}
