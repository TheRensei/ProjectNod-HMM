using System.Collections.Generic;
using UnityEngine;
using Accord.Statistics.Models.Markov; // hidden markov model
using Accord.Statistics.Distributions.Univariate; // general discrete distribution

public class TrainingPerformer : MonoBehaviour
{
    [SerializeField] int totalGestureCount = 9;
    [Space]
    [SerializeField] int simpleN = 3;
    [SerializeField] int simpleM = 17;
    [Space]
    [SerializeField] int complexN = 3;
    [SerializeField] int complexM = 3;
    [Space]
    [SerializeField] int sequenceLength = 10;

    public void RunVectorQuantization()
    {
        int[] sizeOfListPerGesture;
        double[][] gestureData;

        //Read data of all gestures from the disk
        GestureSerializer.LoadAllGesturesDataRaw(
            totalGestureCount,
            out sizeOfListPerGesture,
            out gestureData);


        //Quantize vectors
        List<int[]> gestureSequences = VectorQuantizationModule.RunTrainingVectorQuantization(
            simpleM,
            totalGestureCount,
            gestureData,
            sizeOfListPerGesture);

        //Save the quantized gesture data to disk
        for (int i = 0; i < totalGestureCount; i++)
        {
            GestureSerializer.SaveGestureDataQuantized((GestureType)i, gestureSequences[i]);
        }

        Debug.Log("Vector Quantization Done!");
    }

    public void RunSimpleTraining()
    {
        List<HiddenMarkovModel<GeneralDiscreteDistribution, int>> SimpleHMMs = new List<HiddenMarkovModel<GeneralDiscreteDistribution, int>>();

        //Create and train simple HMMs
        int simpleGestureCount = 7;
        for (int i = 0; i < simpleGestureCount; i++)
        {
            //Load quantized gesture data
            int[] gestureSequences = GestureSerializer.LoadGestureDataQuantized((GestureType)i);
            //Create
            SimpleHMMs.Add(TrainingModule.CreateModel(simpleN, simpleM, (GestureType)i));
            //Train
            TrainingModule.RunSimpleTraining(gestureSequences, sequenceLength, SimpleHMMs[i]);
            //Save
            GestureModelSerializer.SaveModel(SimpleHMMs[i]);
        }

        Debug.Log("Simple Training Done!");
    }

    public void RunComplexTraining()
    {
        //Load saved simple Models from disk
        List<HiddenMarkovModel<GeneralDiscreteDistribution, int>> SimpleHMMs = new List<HiddenMarkovModel<GeneralDiscreteDistribution, int>>();
        for (int i = 0; i < 7; i++)
        {
            SimpleHMMs.Add(GestureModelSerializer.LoadHMM((GestureType)i));
        }

        //Load quantized gesture data
        int[] shakingGestureSequences = GestureSerializer.LoadGestureDataQuantized((GestureType)7);
        int[] noddingGestureSequences = GestureSerializer.LoadGestureDataQuantized((GestureType)8);

        //Create Complex HMMs
        HiddenMarkovModel<GeneralDiscreteDistribution, int> shakingHMM = TrainingModule.CreateModel(complexN, complexM, GestureType.Shaking);
        HiddenMarkovModel<GeneralDiscreteDistribution, int> noddingHMM = TrainingModule.CreateModel(complexN, complexM, GestureType.Nodding);

        //Run Complex Training
        TrainingModule.RunComplexTraining(shakingGestureSequences, sequenceLength, SimpleHMMs.ToArray(), shakingHMM);
        TrainingModule.RunComplexTraining(noddingGestureSequences, sequenceLength, SimpleHMMs.ToArray(), noddingHMM);

        //Save Complex Models
        GestureModelSerializer.SaveModel(shakingHMM);
        GestureModelSerializer.SaveModel(noddingHMM);

        Debug.Log("Complex Training Done!");
    }
}
