using System.Collections.Generic;
using Accord.Statistics.Models.Markov.Learning; //baum welch learning
using Accord.Statistics.Models.Markov; // hidden markov model
using Accord.Statistics.Distributions.Univariate; // general discrete distribution
using Accord.IO; //For saving
using Accord.Statistics.Models.Markov.Topology;


public static class TrainingModule
{
    public static HiddenMarkovModel<GeneralDiscreteDistribution, int> CreateModel(int N, int M, GestureType g)
    {
        HiddenMarkovModel<GeneralDiscreteDistribution, int> model;
        model = HiddenMarkovModel.CreateDiscrete(N, M);
        model.Tag = (g).ToString();
        return model;
    }

    static int[][] SplitArray(int[] arr, float newSize)
    {
        //How many sequences of length newSize can be created from observationSymbols array
        int lngth = UnityEngine.Mathf.FloorToInt(arr.Length / newSize);
        int[][] seq = new int[lngth][];

        List<int> temp = new List<int>();
        int seqArrIndex = 0;
        for (int i = 0; i < arr.Length; i++)
        {
            temp.Add(arr[i]);
            if (temp.Count >= newSize)
            {
                seq[seqArrIndex] = temp.ToArray();
                temp.Clear();
                seqArrIndex++;
            }
        }
        return seq;
    }

    public static void RunSimpleTraining(int[] gestureSequences, int sequenceLength, HiddenMarkovModel<GeneralDiscreteDistribution, int> hmm)
    {
        int[][] sequences = SplitArray(gestureSequences, sequenceLength);

        // Try to fit the model to the data until the difference in
        //  the average log-likelihood changes only by as little as 0.0001
        var teacher = new BaumWelchLearning<GeneralDiscreteDistribution, int>(hmm)
        {
            Tolerance = 0.0001,
            MaxIterations = 0
        };
       

        //UnityEngine.Debug.LogFormat("Sequence length {0} for {1}", sequences.Length, hmm.Tag);
        teacher.Learn(sequences);
    }

    public static void RunComplexTraining(int[] gestureSequences, int sequenceLength,
        HiddenMarkovModel<GeneralDiscreteDistribution, int>[] simpleHMMs,
        HiddenMarkovModel<GeneralDiscreteDistribution, int> hmm)
    {
        int[][] sequences = SplitArray(gestureSequences, sequenceLength);

        List<int> labels = new List<int>();

        //Loop through all sequences of given gesture
        for (int a = 0; a < sequences.Length; a++)
        {
            //Save results of logLikeliHood in the array
            float[] results = new float[simpleHMMs.Length];

            //Get loglikelihood of the sequence from each simpleHMM
            for (int s = 0; s < simpleHMMs.Length; s++)
            {
                results[s] = (float)simpleHMMs[s].LogLikelihood(sequences[a]);
            }

            //Label is found by finding the biggest value in an array and then finding index of that element in an array
            labels.Add(System.Array.IndexOf(results, UnityEngine.Mathf.Max(results)));
        }
        //Split labels into sets of 10
        int[][] labelSeq = SplitArray(labels.ToArray(), sequenceLength);

        //Use Baum-Welch algorithm to train the complex HMMs

        // Try to fit the model to the data until the difference in
        //  the average log-likelihood changes only by as little as 0.0001
        var teacher = new BaumWelchLearning<GeneralDiscreteDistribution, int>(hmm)
        {
            Tolerance = 0.0001,
            MaxIterations = 0
        };

        teacher.Learn(labelSeq);
    }
}

public static class GestureModelSerializer
{
    public static void SaveModel(HiddenMarkovModel<GeneralDiscreteDistribution, int> hmm)
    {
        string fileName = "TrainedHMMs/" + (hmm.Tag).ToString() + "_HMM.accord";
        Serializer.Save(obj: hmm, path: fileName);
    }

    public static HiddenMarkovModel<GeneralDiscreteDistribution, int> LoadHMM(GestureType g)
    {
        HiddenMarkovModel<GeneralDiscreteDistribution, int> GestureHMM;
        string fileName = "TrainedHMMs/" + g.ToString() + "_HMM.accord";
        Serializer.Load(fileName, out GestureHMM);

        return GestureHMM;
    }
}
