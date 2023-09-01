using System.Collections.Generic;

namespace RPG.Stats
{
    public interface IModifierProvider
    {
        IEnumerable<float> GetAdditiveProvider(Stats stat);
        IEnumerable<float> GetMultiplicativeProvider(Stats stat);
    }
}
