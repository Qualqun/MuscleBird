using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "BuildableDrawerData", menuName = "Scriptable Objects/BuildableDrawerData")]
public class BuildDrawerData : ScriptableObject
{
    public int nbColumns = 4;
    public int nbRows = 4;
    public int nbValues = 2;

    public Wrapper<int>[] matrix;

    private void Awake()
    {
        if (matrix == null)
            ResetGrid();
    }

    public void ResetGrid()
    {
        matrix = new Wrapper<int>[nbColumns * nbRows];

        for (int i = 0; i < nbColumns * nbRows; i++)
        {
            matrix[i] = new Wrapper<int>();
            matrix[i].values = new int[nbColumns * nbRows];
        }
    }
}


#if UNITY_EDITOR
[CustomEditor(typeof(BuildDrawerData))]
public class BuildableGridEditor : Editor
{

    SerializedProperty nbColumns;
    SerializedProperty nbRows;
    SerializedProperty nbValues;

    SerializedProperty matrix;

    SerializedProperty array;

    private void OnEnable()
    {
        nbColumns = serializedObject.FindProperty("nbColumns");
        nbRows = serializedObject.FindProperty("nbRows");
        nbValues = serializedObject.FindProperty("nbValues");

        matrix = serializedObject.FindProperty("matrix");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        BuildDrawerData script = (BuildDrawerData)target;

        EditorGUILayout.PropertyField(nbColumns);
        EditorGUILayout.PropertyField(nbRows);
        EditorGUILayout.PropertyField(nbValues);

        DrawGrid();

        if (GUILayout.Button("Reset"))
            script.ResetGrid();

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawGrid()
    {
        try
        {
            GUILayout.BeginVertical();

            for (int i = 0; i < nbRows.intValue; i++)
            {
                GUILayout.BeginHorizontal();
                array = matrix.GetArrayElementAtIndex(i).FindPropertyRelative("values");

                for (int j = 0; j < nbColumns.intValue; j++)
                {
                    var value = array.GetArrayElementAtIndex(j);
                    int element = value.intValue;
                    if (GUILayout.Button(element.ToString(), GUILayout.MaxWidth(30)))
                    {
                        value.intValue = NextIndex(value.intValue);
                    }
                    //GUILayout.Label(array.GetArrayElementAtIndex(j).intValue.ToString());
                }

                GUILayout.EndHorizontal();
            }

            GUILayout.EndVertical();

        }
        catch (System.Exception e)
        {
            Debug.LogWarning(e);
        }
    }

    private int NextIndex(int index)
    {
        int result = ++index % nbValues.intValue;
        return result;
    }
}
#endif
