using DefaultEcs.Hierarchy.Components;
using System;

namespace DefaultEcs.Hierarchy
{
    /// <summary>
    /// Представляет класс иерархического дерево по указанному ключу.
    /// Вся иерархия этого дерева выстраивается через компонент-ключ <typeparamref name="TKey"/>.
    /// </summary>
    /// <typeparam name="TKey">Ключ, по которому выстраивается иерархия.</typeparam>
    internal sealed class Tree<TKey> : Tree
    {
        #region Tree

        /// <inheritdoc cref="Tree.ParentAdded"/>
        public override event ParentAddedHandler? ParentAdded;

        /// <inheritdoc cref="Tree.ParentRemoved"/>
        public override event ParentRemovedHandler? ParentRemoved;

        /// <inheritdoc cref="Tree.ParentChanged"/>
        public override event ParentChangedHandler? ParentChanged;


        /// <inheritdoc cref="Tree.GetParent"/>
        public override Entity? GetParent(in Entity entity)
        {
            if (!entity.Has<HierarchyKey<TKey>>())
                return null;

            return entity.Get<HierarchyKey<TKey>>().Parent;
        }

        /// <inheritdoc cref="Tree.TryGetChildren"/>
        public override bool TryGetChildren(in Entity? entity, out ReadOnlySpan<Entity> children)
        {
            return _map.TryGetEntities(new HierarchyKey<TKey>(entity), out children);
        }

        /// <inheritdoc cref="Tree.Dispose"/>
        public override void Dispose()
        {
            _root.ParentChanged -= OnParentChanged;

            _map.Dispose();

            _keyAdded.Dispose();
            _keyRemoved.Dispose();
            _keyEnabled.Dispose();
            _keyDisabled.Dispose();

            _tKeyAdded.Dispose();
            _tKeyChanged.Dispose();
            _tKeyRemoved.Dispose();
        }

        #endregion

        #region Tree<TKey>

        private readonly TreeReference _root; 
        private readonly EntityMultiMap<HierarchyKey<TKey>> _map;
        private readonly IDisposable _keyAdded;
        private readonly IDisposable _keyRemoved;
        private readonly IDisposable _keyEnabled;
        private readonly IDisposable _keyDisabled;

        private readonly IDisposable _tKeyAdded;
        private readonly IDisposable _tKeyChanged;
        private readonly IDisposable _tKeyRemoved;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="Tree{TKey}"/>  с указанным ключом <typeparamref name="TKey"/>.
        /// </summary>
        /// <param name="trees">Экземпляр менеджера используемых деревьев.</param>
        public Tree(Trees trees) : base(trees)
        {
            var world = trees.World;

            _root = trees.GetRoot();
            _root.ParentChanged += OnParentChanged;

            _map = world.GetEntities().AsMultiMap<HierarchyKey<TKey>>();

            _keyAdded = world.SubscribeComponentAdded<TKey>((in entity, in key) => AddOrChangeMarker(in entity));
            _keyRemoved = world.SubscribeComponentRemoved<TKey>((in entity, in key) => RemoveMarker(in entity));
            _keyEnabled = world.SubscribeComponentEnabled<TKey>((in entity, in key) => TurnOnChildren(in entity));
            _keyDisabled = world.SubscribeComponentDisabled<TKey>((in entity, in key) => TurnOffChildren(in entity));

            _tKeyAdded = world.SubscribeComponentAdded<HierarchyKey<TKey>>(OnAdded);
            _tKeyChanged = world.SubscribeComponentChanged<HierarchyKey<TKey>>(OnChanged);
            _tKeyRemoved = world.SubscribeComponentRemoved<HierarchyKey<TKey>>(OnRemoved);

            using var set = world.GetEntities().With<TKey>().AsSet();

            foreach (ref readonly var entity in set.GetEntities())
            {
                AddOrChangeMarker(in entity);
            }
        }

        #endregion

        #region Root
        private void OnParentChanged(in Entity entity, in Entity? old, in Entity? value)
        {
            AddOrChangeMarker(in entity);
        }

        #endregion

        #region HierarchyKey<TKey>

        /// <summary>
        /// Метод вызывает событие <see cref="ParentAdded"/> и перебирает детей своего 
        /// родителя, чтобы сделать их своими потомками, если они таковы являются по факту.
        /// </summary>
        /// <param name="entity">Сущность, для которого вызвано это событие.</param>
        /// <param name="key">Экземпляр компонент-ключа.</param>
        private void OnAdded(in Entity entity, in HierarchyKey<TKey> key)
        {
            ParentAdded?.Invoke(in entity);

            if (!_map.TryGetEntities(key, out var entities))
                return;

            foreach (ref readonly var child in entities)
            {
                if (child == entity)
                    continue;

                var finedParent = child.FindParentWith<HierarchyKey<TKey>>();

                if (finedParent == entity)
                {
                    child.Set(new HierarchyKey<TKey>(entity, key.Order + 1));
                }
            }
        }

        /// <summary>
        /// Метод вызывает событие <see cref="ParentChanged"/> и обновляет компонент-ключ у своих детей.
        /// </summary>
        /// <param name="entity">Сущность, для которого вызвано это событие.</param>
        /// <param name="old">Экземпляр компонент-ключа, который был ранее.</param>
        /// <param name="key">Экземпляр нового компонент-ключа.</param>
        private void OnChanged(in Entity entity, in HierarchyKey<TKey> old, in HierarchyKey<TKey> key)
        {
            ParentChanged?.Invoke(in entity, in old.Parent, in key.Parent);

            if (!_map.TryGetEntities(new HierarchyKey<TKey>(entity), out var entities))
                return;

            foreach (ref readonly var child in entities)
            {
                child.Set(new HierarchyKey<TKey>(entity, key.Order + 1));
            }
        }

        /// <summary>
        /// Метод вызывает событие <see cref="ParentRemoved"/> и обновляет компонент-ключ у своих детей.
        /// </summary>
        /// <param name="entity">Сущность, для которого вызвано это событие.</param>
        /// <param name="key">Экземпляр компонент-ключа.</param>
        private void OnRemoved(in Entity entity, in HierarchyKey<TKey> key)
        {
            ParentRemoved?.Invoke(in entity);

            if (!_map.TryGetEntities(new HierarchyKey<TKey>(entity), out var entities))
                return;

            foreach (ref readonly var child in entities)
            {
                child.Set(new HierarchyKey<TKey>(key.Parent, key.Order + 1));
            }
        }

        #endregion

        #region TKey

        /// <inheritdoc cref="Tree.AddOrChangeMarker"/>
        public override void AddOrChangeMarker(in Entity entity)
        {
            var parent = entity.FindParentWith<TKey>();
            var order = 0;

            if (parent.HasValue)
                order = parent.Value.Get<HierarchyKey<TKey>>().Order + 1;

            var key = new HierarchyKey<TKey>(parent, order);

            entity.Set(key);
        }

        /// <inheritdoc cref="Tree.RemoveMarker"/>
        public override void RemoveMarker(in Entity entity)
        {
            if (!entity.Has<HierarchyKey<TKey>>())
                return;

            entity.Remove<HierarchyKey<TKey>>();
        }

        /// <summary>
        /// Включает компонент-ключ у всех потомков.
        /// </summary>
        /// <param name="entity">Сущность, чьих потомков включаем.</param>
        private void TurnOnChildren(in Entity entity)
        {
            if (!_root.TryGetChildren(entity, out var entities))
                return;

            foreach (ref readonly var children in entities)
            {
                children.Enable<HierarchyKey<TKey>>();

                TurnOnChildren(in children);
            }
        }

        /// <summary>
        /// Выключает компонент-ключ у всех потомков.
        /// </summary>
        /// <param name="entity">Сущность, чьих потомков выключаем.</param>
        private void TurnOffChildren(in Entity entity)
        {
            if (!_map.TryGetEntities(new HierarchyKey<TKey>(entity), out var entities))
                return;

            foreach (ref readonly var child in entities)
            {
                child.Disable<HierarchyKey<TKey>>();

                TurnOffChildren(in child);
            }
        }

        #endregion
    }
}
