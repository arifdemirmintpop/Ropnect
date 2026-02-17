using System.Linq;
using UnityEngine;

namespace tiplay.EditorUtilities
{
    public static class ParticleSystemEditorUtility
    {
        public static ParticleSystem[] FindUsedParticlesFromActiveScene()
        {
            return PrefabEditorUtility.FindAllSourcePrefabsFromActiveScene<ParticleSystem>();
        }

        public static ParticleSystem[] FindAllFromActiveScene(bool includeReferences)
        {
            return ComponentEditorUtility.FindAllFromActiveScene<ParticleSystem>(includeReferences);
        }
    }
}