using Newtonsoft.Json;
using ShowingAds.AndroidApp.Core.BusinessCollections.CycleEnumerators;
using ShowingAds.AndroidApp.Core.BusinessCollections.Visitors;
using ShowingAds.AndroidApp.Core.Network.WebClientCommands.Filters;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace ShowingAds.AndroidApp.Core.BusinessCollections
{
    public class LowLevelCollection<T> : Component
        where T : Component
    {
        [JsonProperty("id")]
        public Guid Id { get; private set; }

        [JsonProperty("components")]
        public List<T> Components { get; private set; }

        [JsonIgnore]
        private readonly RandomEnumerator<T> _enumerator;

        [JsonConstructor]
        public LowLevelCollection(Guid id, List<T> components)
        {
            Id = id;
            Components = components ?? throw new ArgumentNullException(nameof(components));
            _enumerator = new RandomEnumerator<T>(Components);
        }

        public override void Accept(BaseVisitor visitor)
        {
            foreach (var component in Components)
                component.Accept(visitor);
            visitor.VisitLowCollection(this);
        }

        public override void Add(Component component)
        {
            Components.Add((T)component);
            _enumerator.AddNode((T)component);
        }

        public override bool IsValid(BaseFilter filter)
        {
            for (int i = 0; i < Components.Count; i++)
                if (Components[i].IsValid(filter) == false)
                    Remove(Components[i--]);
            return Convert.ToBoolean(Components.Count);
        }

        public override void Remove(Component component)
        {
            Components.Remove((T)component);
            _enumerator.RemoveNode((T)component);
        }

        public override Guid GetId() => Id;

        public bool TryGetNext(out T component)
        {
            var enumerator = _enumerator as LinearEnumerator<T>;
            var isSuccess = enumerator.MoveNext();
            component = enumerator.Current as T;
            return isSuccess;
        }

        public bool TryGetRandom(out T component)
        {
            var isSuccess = _enumerator.MoveNext();
            component = _enumerator.Current as T;
            return isSuccess;
        }
    }
}
