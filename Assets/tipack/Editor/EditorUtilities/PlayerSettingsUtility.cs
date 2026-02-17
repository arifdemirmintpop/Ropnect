using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;
using UnityEditor;

namespace tiplay.EditorUtilities
{
    public static class PlayerSettingsUtility
    {
        public static void SetApplicationIdentifier(BuildTargetGroup targetGroup, string companyName, string productName)
        {
            var regex = new Regex(@"[^\d\w+]");
            companyName = regex.Replace(companyName, string.Empty);
            productName = regex.Replace(productName, string.Empty);
            string identifier = $"com.{companyName}.{productName}".ToLower();

            PlayerSettings.SetApplicationIdentifier(targetGroup, identifier);
        }
    }
}

