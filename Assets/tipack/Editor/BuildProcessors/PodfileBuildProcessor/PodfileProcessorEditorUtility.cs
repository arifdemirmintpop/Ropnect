using UnityEditor;

namespace tiplay
{
    public class PodfileProcessorEditorUtility
    {
        public static void DrawProcessorSettings()
        {
            using (var scope = new EditorGUI.ChangeCheckScope())
            {
                PodfileProcessorData.ProcessorEnabled = EditorGUILayout.Toggle("Processor Enabled", PodfileProcessorData.ProcessorEnabled);

                if (PodfileProcessorData.ProcessorEnabled)
                {
                    PodfileProcessorData.AddStaticLinkage = EditorGUILayout.Toggle("Add Static Linkage", PodfileProcessorData.AddStaticLinkage);
                    PodfileProcessorData.AutoRunPodInstall = EditorGUILayout.Toggle("Auto Run Pod Install", PodfileProcessorData.AutoRunPodInstall);
                }

                if (scope.changed)
                    PodfileProcessorData.SaveData();
            }
        }
    }
}