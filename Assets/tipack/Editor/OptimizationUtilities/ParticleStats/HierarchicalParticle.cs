using System.Collections.Generic;

namespace tiplay.Toolkit.OptimizationToolkit
{
    public class HierarchicalParticle
    {
        public ParticleStatsData source;
        public List<ParticleStatsData> instances = new List<ParticleStatsData>();

        public HierarchicalParticle(ParticleStatsData source)
        {
            this.source = source;
        }
    }
}

