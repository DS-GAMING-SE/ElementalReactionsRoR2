using System;
using System.Collections.Generic;
using System.Text;

namespace ElementalReactionsMod.Environment
{
    public class SS2StormElementalRain : ElementalRain
    {
        // Add this to the stormcontroller prefab. Set default _raining to false
        // get MSU.GameplayEvent component, sub to start and end events to enable and disable raining
        // disable this whole component if there is already an elementalrain.instance. This should spawn after the original one because of difference in spawning events
    }
}
