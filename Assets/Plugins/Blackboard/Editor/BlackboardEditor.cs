using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System;
//using BRSFramework.Tools;

[CustomEditor(typeof(Blackboard))]
public class BlackboardEditor : Editor
{
    private Blackboard m_Target;
    // Sorted copy of m_Target.m_Data
    private List<BlackboardData> m_Data;

    private int m_Filter = 0;
    private Dictionary<string, BlackboardGroup> m_GroupTable;
    private string[] m_GroupOptions;
    private string[] m_GroupFilterOptions;

    // GUI.skin.box with black background
    private GUIStyle m_BlackBoxStyle;
    // Pressed button skin, used to create a false toggle
    private GUIStyle m_InvertedButtonStyle;

    // Whether m_Data needs to be refreshed
    private bool m_ReprocessData;

    // Whether the add variable view is open
    private bool m_AddingVariable;

    private string m_NewVariableKey = "";
    private ScriptableObject m_NewVariableValue;
    private int m_NewVariableGroupIndex = 0;

    // Group being rendered in OnInspectorGUI
    private string m_CurrentOpenGroup;

    // ScrollView scroll
    private Vector2 m_Scroll;

    // Reference widths calculated during OnInspectorGUI according to window size
    private float m_OuterItemReferenceWidth;
    protected float m_InnerItemReferenceWidth;
    private float m_AdderItemReferenceWidth;

    // Size constants, carefully chosen (a.k.a. decided by trial and error). 
    // Guide: m_XYZSize = absolute size
    //        m_XYZRatio = proportion of m_InnerItemReferenceWidth to be used as size
    private const float kFilterPopupSize = 10f;
    private const float kInspectButtonSize = 25f;
    private const float kDeleteButtonSize = 18f;
    protected const float kSpacingSize = 10f;

    private const float kOuterPaddingEstimatedSize = 30f; // estimated space used by nesting/padding
    private const float kInnerPaddingEstimatedSize = kOuterPaddingEstimatedSize + 30f + 4f * kSpacingSize; // estimated space used by nesting/padding/spacing
    private const float kAdderPaddingEstimatedSize = kOuterPaddingEstimatedSize + 25f + 3f * kSpacingSize; // estimated space used by nesting/padding/spacing

    protected float kKeyRatio = 0.32f;
    protected float kValueRatio = 0.36f;
    protected float kInvokeRatio = 0.14f;
    protected float kListenerRatio = 0.18f;

    private const float kAddButtonSize = 18f;

    private const float kAdderKeyRatio = 0.35f;
    private const float kAdderValueRatio = 0.40f;
    private const float kAdderGroupRatio = 0.25f;

    private const string kWarningOwnerListenerNull = "[Blackboard Editor] The owner listener has been destroyed but you are still trying to access it.";

    // List of properties that should not be shown in the value inspection view
    private List<string> m_IgnoredSOProperties;

    protected virtual void OnEnable()
    {
        m_Target = (Blackboard)target;

        ClearListenersIfNotPlaying();
        BuildIgnoreList();
        AssembleGroupData();
        ProcessVariables();
        ClearExpandedValues();
    }
    private void ClearListenersIfNotPlaying()
    {
        if (!Application.isPlaying)
        {
            for (int i = 0; i < m_Target.m_Data.Count; ++i)
            {
                m_Target.m_Data[i].ClearListeners();
            }
        }
    }

    private void BuildIgnoreList()
    {
        m_IgnoredSOProperties = new List<string>()
        {
            "m_Script" // m_Script shows the C# script the SO is based on in its editor. 
        };
    }

    // Fetches BlackboardGroups in the project, setting up m_GroupTable, m_GroupOptions and m_GroupFilterOptions
    private void AssembleGroupData()
    {
        string[] guids = AssetDatabase.FindAssets("t:" + typeof(BlackboardGroup).ToString());

        m_GroupTable = new Dictionary<string, BlackboardGroup>() { { BlackboardData.kNoGroupOption, null } }; // Groupless option set by default

        for (int i = 0; i < guids.Length; ++i)
        {
            BlackboardGroup group = AssetDatabase.LoadAssetAtPath<BlackboardGroup>(AssetDatabase.GUIDToAssetPath(guids[i]));

            m_GroupTable.Add(group.Name, group);
        }

        m_GroupOptions = m_GroupTable.Keys.ToArray();
        m_GroupFilterOptions = new string[] { "All" }.Concat(m_GroupOptions).ToArray();
    }

    void ProcessVariables()
    {
        m_Data = m_Target.m_Data.OrderBy(x => Array.IndexOf(m_GroupOptions, x.GroupName)).ToList();
        m_ReprocessData = false;
    }

    void ClearExpandedValues()
    {
        // Un-expand value inspection view for everyone
        for (int i = 0; i < m_Data.Count; ++i)
        {
            m_Data[i].m_ExpandedValue = false;
        }
    }

    public override void OnInspectorGUI()
    {
        if (m_BlackBoxStyle == null || m_InvertedButtonStyle == null)
        {
            CreateStyles(); // Must be called from OnInspectorGUI
        }

        if (m_ReprocessData)
        {
            ProcessVariables();
        }

        Rect scrollView = EditorGUILayout.BeginVertical();

        // Scrollbar misbehaves like crazy, so we set a higher width to the scrollview so it will be rendered off window
        m_Scroll = EditorGUILayout.BeginScrollView(m_Scroll, GUILayout.Width(EditorGUIUtility.currentViewWidth), GUILayout.Height(scrollView.height));

        // Account for scrollbar and fixed size items
        m_OuterItemReferenceWidth = EditorGUIUtility.currentViewWidth - kOuterPaddingEstimatedSize;
        m_InnerItemReferenceWidth = EditorGUIUtility.currentViewWidth - kFilterPopupSize - kDeleteButtonSize - kInnerPaddingEstimatedSize;
        m_AdderItemReferenceWidth = EditorGUIUtility.currentViewWidth - kAdderPaddingEstimatedSize - kDeleteButtonSize;

        DrawGuide();

        m_CurrentOpenGroup = "";

        if (m_Data.Count == 0)
        {
            EditorGUILayout.BeginHorizontal(GUI.skin.box);
            EditorGUILayout.LabelField("There are no variables on this blackboard.");
            EditorGUILayout.EndHorizontal();
        }

        for (int i = 0; i < m_Data.Count; ++i)
        {
            // Filter
            if (m_Filter > 0 && m_Data[i].GroupName != m_GroupFilterOptions[m_Filter])
            {
                continue;
            }

            DrawGroup(m_Data[i]);

            DrawData(m_Data[i]);
        }

        if (!string.IsNullOrEmpty(m_CurrentOpenGroup))
        {
            CloseGroup(); // Final group has to be closed manually if it exists
        }

        EditorGUILayout.EndScrollView();

        GUILayout.Space(5);

        DrawVariableAdder();

        DrawCodeGenerator();

        EditorGUILayout.EndVertical();

        EditorUtility.SetDirty(m_Target);
    }

    private void DrawGuide()
    {
        // Double boxes, to force better alignment with variables
        EditorGUILayout.BeginHorizontal(m_BlackBoxStyle);

        EditorGUILayout.BeginHorizontal(m_BlackBoxStyle);

        m_Filter = EditorGUILayout.Popup(m_Filter, m_GroupFilterOptions, EditorStyles.miniButtonMid, GUILayout.Width(kFilterPopupSize));
        DrawLabels();
        GUILayout.Space(kSpacingSize);
        EditorGUILayout.LabelField("", EditorStyles.boldLabel, GUILayout.Width(kDeleteButtonSize)); // Empty label still forces black box to expand

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndHorizontal();
    }

    protected virtual void DrawLabels()
    {
        EditorGUILayout.LabelField("Key", EditorStyles.boldLabel, GUILayout.Width(kKeyRatio * m_InnerItemReferenceWidth));
        GUILayout.Space(kSpacingSize);
        EditorGUILayout.LabelField("Value", EditorStyles.boldLabel, GUILayout.Width(kValueRatio * m_InnerItemReferenceWidth));
        GUILayout.Space(kSpacingSize);
        EditorGUILayout.LabelField("Invoke", EditorStyles.boldLabel, GUILayout.Width(kInvokeRatio * m_InnerItemReferenceWidth));
        GUILayout.Space(kSpacingSize);
        EditorGUILayout.LabelField("Listeners", EditorStyles.boldLabel, GUILayout.Width(kListenerRatio * m_InnerItemReferenceWidth));
    }

    private void DrawGroup(BlackboardData data)
    {
        // Only open a new group if we're past the one we were processing
        if (m_CurrentOpenGroup == data.GroupName)
        {
            return;
        }

        // Was there a different, valid group before us? Close it.
        if (!string.IsNullOrEmpty(m_CurrentOpenGroup))
        {
            CloseGroup();
        }

        m_CurrentOpenGroup = data.GroupName;

        EditorGUILayout.BeginVertical(GUI.skin.box);
        EditorGUILayout.LabelField(data.GroupName, GUILayout.MaxWidth(200f)); // Not providing a max width causes a misalignment

    }

    private void CloseGroup()
    {
        EditorGUILayout.EndVertical();
    }

    private void DrawData(BlackboardData data)
    {
        EditorGUILayout.BeginHorizontal(GUI.skin.box);

        // [Group]
        GUI.enabled = !Application.isPlaying;
        int myIndex = Array.IndexOf(m_GroupOptions, data.GroupName);
        int newIndex = EditorGUILayout.Popup(myIndex, m_GroupOptions, EditorStyles.miniButtonMid, GUILayout.Width(kFilterPopupSize));

        BlackboardGroup group = m_GroupTable[m_GroupOptions[newIndex]];

        if (data.m_Group != group)
        {
            data.m_Group = group;
            m_ReprocessData = true; // Force list to resort
        }
        GUI.enabled = true;
        // [/Group]

        // [Key]
        GUI.enabled = !Application.isPlaying;
        string key = EditorGUILayout.TextField(data.Key, GUILayout.Width(kKeyRatio * m_InnerItemReferenceWidth)).Trim();

        if (key != data.Key && KeyExists(key))
        {
            Debug.LogError("[Blackboard Editor] The given key already exists.");
        }
        else
        {
            data.Key = key;
        }
        GUI.enabled = true;
        // [/Key]

        GUILayout.Space(kSpacingSize);

        // [Value]
        GUI.enabled = !Application.isPlaying;
        float valueSize = kValueRatio * m_InnerItemReferenceWidth - kInspectButtonSize - 5;
        data.Value = (ScriptableObject)EditorGUILayout.ObjectField(data.Value, typeof(ScriptableObject), true, GUILayout.Width(valueSize));

        // [Value Inspect Button] (this is not a toggle, just a button that switches its skin)
        GUI.enabled = (data.Value != null);
        if (data.m_ExpandedValue)
        {
            // Unicode sequence because I don't trust git nor VS to not mangle the actual character
            if (GUILayout.Button("\u25B2", m_InvertedButtonStyle, GUILayout.Width(kInspectButtonSize)))
            {
                data.m_ExpandedValue = false;
            }
        }
        else
        {
            if (GUILayout.Button("\u25BC", GUILayout.Width(kInspectButtonSize)))
            {
                data.m_ExpandedValue = true;
            }
        }
        GUI.enabled = true;
        // [/Value Inspect Button]
        // [/Value]

        GUILayout.Space(kSpacingSize);

        // [Invoke Button]
        GUI.enabled = Application.isPlaying;
        if (GUILayout.Button("Invoke", GUILayout.Width(kInvokeRatio * m_InnerItemReferenceWidth)))
        {
            data.Invoke();
        }
        GUI.enabled = true;
        // [/Invoke Button]

        GUILayout.Space(kSpacingSize);

        // [Listeners]
        DrawListeners(data.GetOwnerListeners());
        // [/Listeners]

        GUILayout.Space(kSpacingSize);

        DrawCustomItens(data.Key);

        // [Delete Button]
        GUI.enabled = !Application.isPlaying;
        GUI.backgroundColor = Color.red;
        if (GUILayout.Button("x", EditorStyles.miniButtonMid, GUILayout.Width(kDeleteButtonSize)) && !Application.isPlaying)
        {
            m_Target.m_Data.Remove(data);
            m_ReprocessData = true;
        }
        GUI.backgroundColor = Color.white;
        GUI.enabled = true;
        // [/Delete Button]

        EditorGUILayout.EndHorizontal();

        // [Value Inspection View]
        if (data.m_ExpandedValue)
        {
            // Double box skin may be aesthetically unpleasant, but prevents a misalignment. Don't remove it. 
            EditorGUILayout.BeginHorizontal(GUI.skin.box, GUILayout.Width(m_OuterItemReferenceWidth - 5));

            GUILayout.FlexibleSpace(); // Center

            EditorGUILayout.BeginHorizontal(GUI.skin.box);

            GUILayout.Space(kSpacingSize);

            EditorGUILayout.BeginVertical();

            SerializedObject serializedObject = new SerializedObject(data.Value);

            SerializedProperty iterator = serializedObject.GetIterator();

            iterator.NextVisible(true);

            bool atLeastOneField = false;

            // Setting width for a property field group is fruitless, since they always follow the variables below.
            float labelTemp = EditorGUIUtility.labelWidth;
            float fieldTemp = EditorGUIUtility.fieldWidth;
            EditorGUIUtility.labelWidth = 0.25f * m_OuterItemReferenceWidth;
            EditorGUIUtility.fieldWidth = 0.25f * m_OuterItemReferenceWidth;

            // Create a property field for each visible property, skipping the ones in the m_IgnoreList
            do
            {
                if (!m_IgnoredSOProperties.Contains(iterator.name))
                {
                    EditorGUILayout.PropertyField(iterator, true);
                    atLeastOneField = true;
                }
            } while (iterator.NextVisible(false));

            EditorGUIUtility.labelWidth = labelTemp;
            EditorGUIUtility.fieldWidth = fieldTemp;

            if (!atLeastOneField)
            {
                EditorGUILayout.LabelField("This object has no visible properties.");
            }
            else
            {
                serializedObject.ApplyModifiedProperties();
            }

            serializedObject.Dispose();

            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();

            GUILayout.FlexibleSpace(); // Center

            EditorGUILayout.EndHorizontal();
        }
        // [/Value Inspection View]

    }

    protected virtual void DrawCustomItens(string key)
    {

    }

    private void DrawListeners(List<UnityEngine.Object> listeners)
    {
        int count = listeners != null ? listeners.Count : 0;

        if (count > 0)
        {
            List<string> elements = listeners.Select((l, i) => i + " - " + l.name).ToList<string>();
            elements.Insert(0, "");
            elements.Insert(0, count + " listener" + (count > 1 ? "s" : ""));

            int selectedIndex = EditorGUILayout.Popup(0, elements.ToArray(), GUILayout.Width(kListenerRatio * m_InnerItemReferenceWidth));

            if (selectedIndex != 0)
            {
                EditorGUIUtility.PingObject(listeners[selectedIndex - 2]);
            }
        }
        else
        {
            EditorGUILayout.LabelField("No listener", GUILayout.Width(kListenerRatio * m_InnerItemReferenceWidth));
        }
    }


    private void DrawVariableAdder()
    {
        if (Application.isPlaying)
        {
            return;
        }

        EditorGUILayout.BeginHorizontal();

        if (m_AddingVariable)
        {
            if (GUILayout.Button("Add", m_InvertedButtonStyle))
            {
                m_AddingVariable = false;
            }
        }
        else
        {
            if (GUILayout.Button("Add"))
            {
                m_AddingVariable = true;
                m_NewVariableKey = "";
                m_NewVariableValue = null;
                m_NewVariableGroupIndex = 0;
            }
        }

        EditorGUILayout.EndHorizontal();

        if (m_AddingVariable)
        {
            EditorGUILayout.BeginVertical(m_BlackBoxStyle);

            EditorGUILayout.BeginHorizontal(m_BlackBoxStyle);

            EditorGUILayout.LabelField("New Key", EditorStyles.boldLabel, GUILayout.Width(kAdderKeyRatio * m_AdderItemReferenceWidth));
            GUILayout.Space(kSpacingSize);
            EditorGUILayout.LabelField("Value", EditorStyles.boldLabel, GUILayout.Width(kAdderValueRatio * m_AdderItemReferenceWidth));
            GUILayout.Space(kSpacingSize);
            EditorGUILayout.LabelField("Group", EditorStyles.boldLabel, GUILayout.Width(kAdderGroupRatio * m_AdderItemReferenceWidth));
            GUILayout.Space(kSpacingSize);
            EditorGUILayout.LabelField("", EditorStyles.boldLabel, GUILayout.Width(kAddButtonSize));

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal(m_BlackBoxStyle);

            // [Key]
            m_NewVariableKey = EditorGUILayout.TextField(m_NewVariableKey, GUILayout.Width(kAdderKeyRatio * m_AdderItemReferenceWidth)).Trim();
            // [/Key]

            GUILayout.Space(kSpacingSize);

            // [Value]
            ScriptableObject value = (ScriptableObject)EditorGUILayout.ObjectField(m_NewVariableValue, typeof(ScriptableObject), true, GUILayout.Width(kAdderValueRatio * m_AdderItemReferenceWidth));
            if (value != m_NewVariableValue)
            {
                m_NewVariableValue = value;
                if (string.IsNullOrEmpty(m_NewVariableKey))
                {
                    m_NewVariableKey = m_NewVariableValue.name;
                }
            }
            // [/Value]

            GUILayout.Space(kSpacingSize);

            // [Group]
            m_NewVariableGroupIndex = EditorGUILayout.Popup(m_NewVariableGroupIndex, m_GroupOptions, EditorStyles.miniButtonMid, GUILayout.Width(kAdderGroupRatio * m_AdderItemReferenceWidth));
            // [/Group]

            GUILayout.Space(kSpacingSize);

            // [Add Button]
            GUI.backgroundColor = Color.green;
            if (GUILayout.Button("+", EditorStyles.miniButtonMid, GUILayout.Width(kAddButtonSize)) && ValidateInputs())
            {
                BlackboardData data = new BlackboardData();
                data.Key = m_NewVariableKey;
                data.Value = m_NewVariableValue;
                data.m_Group = m_GroupTable[m_GroupOptions[m_NewVariableGroupIndex]];

                m_NewVariableKey = "";
                m_NewVariableValue = null;
                m_NewVariableGroupIndex = 0;

                m_Target.m_Data.Add(data);
                m_ReprocessData = true;
            }
            GUI.backgroundColor = Color.white;
            // [/Add Button]

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndHorizontal();
        }

    }


    private bool ValidateInputs()
    {
        if (string.IsNullOrEmpty(m_NewVariableKey))
        {
            Debug.LogError("[Blackboard Editor] Creation of a variable requires at least a key.");
            return false;
        }

        if (KeyExists(m_NewVariableKey))
        {
            Debug.LogError("[Blackboard Editor] There is already a variable with the given key on this Blackboard.");
            return false;
        }

        return true;
    }

    private bool KeyExists(string key)
    {
        for (int i = 0; i < m_Data.Count; ++i)
        {
            if (m_Data[i].Key == key)
            {
                return true;
            }
        }

        return false;
    }

    private void DrawCodeGenerator()
    {
        if (Application.isPlaying)
        {
            return;
        }

        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Generate Variable Index"))
        {
            Debug.LogWarning("You need to implement this");

            //GenerateVariableIndex();
        }

        EditorGUILayout.EndHorizontal();
    }

    //private void GenerateVariableIndex()
    //{
    //    List<string> ids = new List<string>();

    //    for (int i = 0; i < m_Data.Count; i++)
    //    {
    //        ids.Add(m_Data[i].Key);
    //    }

    //    CodeGenerator.Generate(m_Target.name + "VariableIndex", ids.ToArray());
    //}

    private void CreateStyles()
    {
        // Create black 2x2 texture, apply to style
        m_BlackBoxStyle = new GUIStyle(GUI.skin.box);

        Texture2D blackBackground = new Texture2D(2, 2);

        Color[] pixels = new Color[4] { Color.black, Color.black, Color.black, Color.black };

        blackBackground.SetPixels(pixels);
        blackBackground.Apply();

        m_BlackBoxStyle.normal.background = blackBackground;

        // Swap button looks when clicked and when not clicked
        m_InvertedButtonStyle = new GUIStyle(GUI.skin.button);

        var temp = m_InvertedButtonStyle.normal;
        m_InvertedButtonStyle.normal = m_InvertedButtonStyle.active;
        m_InvertedButtonStyle.active = temp;
    }
} 
