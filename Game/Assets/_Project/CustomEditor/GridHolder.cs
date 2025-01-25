using System;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "GridHolder", menuName = "Scriptable Objects/GridHolder")]
public class GridHolder : ScriptableObject
{
    public int nbColumns = 4;
    public int nbRows = 4;
    public int nbValues = 2;

    public Wrapper<int>[] grid;

    private void Awake()
    {
        if (grid == null)
            ResetGrid();
    }

    public void ResetGrid()
    {
        grid = new Wrapper<int>[nbColumns * nbRows];

        for (int i = 0; i < nbColumns * nbRows; i++)
        {
            grid[i] = new Wrapper<int>();
            grid[i].values = new int[nbColumns * nbRows];
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(GridHolder))]
public class GridHolderEditor : Editor
{
    SerializedProperty grid;
    SerializedProperty array;

    SerializedProperty nbValues;

    SerializedProperty nbRows;
    SerializedProperty nbColumns;


    private void OnEnable()
    {
        //lenght = Enum.GetValues(typeof(bool)).Length;
        nbColumns = serializedObject.FindProperty("nbColumns");
        nbRows = serializedObject.FindProperty("nbRows");
        nbValues = serializedObject.FindProperty("nbValues");
        grid = serializedObject.FindProperty("grid");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        GridHolder script = (GridHolder)target;

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
                array = grid.GetArrayElementAtIndex(i).FindPropertyRelative("values");

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