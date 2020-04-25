using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TrainingPerformer))]
class TrainingPerformerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        EditorGUILayout.Space();

        TrainingPerformer myScript = (TrainingPerformer)target;

        if (GUILayout.Button("Run Vector Quantization"))
        {
            myScript.RunVectorQuantization();
        }

        if (GUILayout.Button("Run Simple Training"))
        {
            myScript.RunSimpleTraining();
        }

        if (GUILayout.Button("Run Complex Training"))
        {
            myScript.RunComplexTraining();
        }
    }
}

[CustomEditor(typeof(TestingPerformer))]
class TestingPerformerEditor : Editor
{
    public override void OnInspectorGUI()
    {

        TestingPerformer myScript = (TestingPerformer)target;

        if (GUILayout.Button("Run Simple Testing"))
        {
            myScript.RunSimpleTest();
        }

        if (GUILayout.Button("Run Complex Testing"))
        {
            myScript.RunComplexTest();
        }
    }
}

[CustomEditor(typeof(RealTimePerformer))]
class RealTimePerformerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        EditorGUILayout.Space();

        RealTimePerformer myScript = (RealTimePerformer)target;

        if (GUILayout.Button("Run Real-Time Testing"))
        {
            myScript.StartTest();
        }
    }
}



