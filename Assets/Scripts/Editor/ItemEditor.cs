using UnityEditor;

// This tells Unity: "Use this script to draw the Inspector for the 'Item' class"
namespace Editor
{
    [CustomEditor(typeof(Item))]
    public class ItemEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            // Always call this at the start to update the values
            serializedObject.Update();

            // Find the category property in the Item script
            SerializedProperty categoryProp = serializedObject.FindProperty("category");

            // Check if the currently selected category is Throwable
            if (categoryProp.enumValueIndex == (int)Item.ItemCategory.Throwable)
            {
                // Draw the Inspector exactly as it normally would (showing speed)
                DrawDefaultInspector();
            }
            else
            {
                // Draw the Inspector, but explicitly HIDE the 'speed' variable
                // (We also hide "m_Script" so Unity doesn't draw the script reference twice)
                DrawPropertiesExcluding(serializedObject, "m_Script", "speed");
            }

            // Apply any changes made in the Inspector back to the object
            serializedObject.ApplyModifiedProperties();
        }
    }
}