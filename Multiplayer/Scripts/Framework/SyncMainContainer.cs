using System.Collections.Generic;

namespace Assets.Multiplayer.Framework
{
    public sealed class SyncMainContainer
    {
        public List<BehaviourContainer> BehaviorContainers { get; }

        public SyncMainContainer() 
        {
            BehaviorContainers = new List<BehaviourContainer>();
        }

        public void Add(params BehaviourContainer[] behaviorContainer)
        {
            BehaviorContainers.AddRange(behaviorContainer);
        }

        public void Add(IEnumerable<BehaviourContainer> behaviorContainers)
        {
            BehaviorContainers.AddRange(behaviorContainers);
        }
    }
}
