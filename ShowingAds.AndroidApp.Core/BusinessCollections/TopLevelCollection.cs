using Newtonsoft.Json;
using ShowingAds.AndroidApp.Core.BusinessCollections.CycleEnumerators;
using ShowingAds.AndroidApp.Core.BusinessCollections.Visitors;
using ShowingAds.AndroidApp.Core.DataAccess.Interfaces;
using ShowingAds.AndroidApp.Core.Network.WebClientCommands.Filters;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ShowingAds.AndroidApp.Core.BusinessCollections
{
    public class TopLevelCollection<T> : Component, IDisposable
        where T : Component
    {
        private readonly IDataStore<List<T>> _store;
        public List<T> Components { get; private set; }
        private readonly RandomEnumerator<T> _enumerator;

        public TopLevelCollection(IDataStore<List<T>> store)
        {
            Components = new List<T>();
            _enumerator = new RandomEnumerator<T>(Components);
            _store = store ?? throw new ArgumentNullException(nameof(store));
            LoadComponents();
        }

        public override void Accept(BaseVisitor visitor)
        {
            foreach (var component in Components)
                component.Accept(visitor);
            visitor.VisitTopCollection(this);
        }

        public override bool IsValid(BaseFilter filter)
        {
            for (int i = 0; i < Components.Count; i++)
                if (Components[i].IsValid(filter) == false)
                    Remove(Components[i--]);
            return true;
        }

        public override void Add(Component component)
        {
            Components.Add((T)component);
            _enumerator.AddNode((T)component);
        }

        public override void Remove(Component component)
        {
            Components.Remove((T)component);
            _enumerator.RemoveNode((T)component);
        }

        public void SaveComponents() => _store.Save(Components);

        public void LoadComponents()
        {
            try
            {
                var loadedComponents = _store.Load() ?? new List<T>();

                foreach (var component in loadedComponents)
                {
                    Components.Add(component);
                    _enumerator.AddNode(component);
                }
            }
            catch (Exception ex)
            {
                ServerLog.Error("TopCollection", ex.Message);
            }
        }

        public void Dispose() => _store.Dispose();

        public override Guid GetId() => throw new ArgumentNullException("TopLevelCollection doesn't have id");

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
