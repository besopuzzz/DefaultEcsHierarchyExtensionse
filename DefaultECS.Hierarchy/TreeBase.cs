using DefaultEcs.Hierarchy.Components;
using System;

namespace DefaultEcs.Hierarchy
{
    /// <summary>
    /// Представляет класс иерархического дерева по ключу <see cref="Children"/>.
    /// </summary>
    internal sealed class TreeBase : Tree
    {
        #region Tree

        /// <inheritdoc cref="Tree.ParentAdded"/>
        public override event ParentAddedHandler? ParentAdded;

        /// <inheritdoc cref="Tree.ParentRemoved"/>
        public override event ParentRemovedHandler? ParentRemoved;

        /// <inheritdoc cref="Tree.ParentChanged"/>
        public override event ParentChangedHandler? ParentChanged;

        /// <inheritdoc cref="Tree.AddOrChangeMarker(in Entity)"/>
        public override void AddOrChangeMarker(in Entity entity)
        {
            ParentAdded?.Invoke(in entity);
        }

        /// <inheritdoc cref="Tree.RemoveMarker(in Entity)"/>
        public override void RemoveMarker(in Entity entity)
        {
            ParentRemoved?.Invoke(in entity);
        }

        /// <inheritdoc cref="Tree.Dispose(in Entity)"/>
        public override void Dispose()
        {
            _entityDisposed.Dispose();
            _map.Dispose();
            _keyAdded.Dispose();
            _keyChanged.Dispose();
            _keyRemoved.Dispose();
        }

        /// <inheritdoc cref="Tree.TryGetChildren(in Entity)"/>
        public override bool TryGetChildren(in Entity? entity, out ReadOnlySpan<Entity> childs)
        {
            return _map.TryGetEntities(new Children(entity), out childs);
        }

        /// <inheritdoc cref="Tree.GetParent(in Entity)"/>
        public override Entity? GetParent(in Entity entity)
        {
            return entity.FindParentWith<Children>();
        }

        #endregion

        #region TreeBase

        private readonly EntityMultiMap<Children> _map;
        private readonly IDisposable _keyAdded;
        private readonly IDisposable _keyChanged;
        private readonly IDisposable _keyRemoved;
        private readonly IDisposable _entityDisposed;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="TreeBase"/>.
        /// </summary>
        /// <param name="trees">Экземпляр менеджера используемых деревьев.</param>
        internal TreeBase(Trees trees) : base(trees)
        {
            _entityDisposed = Trees.World.SubscribeEntityDisposed(OnEntityDisposed);
            _map = Trees.World.GetEntities().AsMultiMap<Children>();
            _keyAdded = Trees.World.SubscribeComponentAdded<Children>((in entity, in value) => AddOrChangeMarker(in entity));
            _keyChanged = Trees.World.SubscribeComponentChanged<Children>((in entity, in old, in value) => ParentChanged?.Invoke(in entity, in old.Parent, in value.Parent));
            _keyRemoved = Trees.World.SubscribeComponentRemoved<Children>((in entity, in value) => RemoveMarker(in entity));
        }

        /// <summary>
        /// Вызывает освобождение всех потомков.
        /// </summary>
        /// <param name="entity">Сущность, чьих потомков надо освободить.</param>
        private void OnEntityDisposed(in Entity entity)
        {
            if (!TryGetChildren(entity, out var entities))
                return;

            foreach (ref readonly var child in entities)
            {
                child.Dispose();
            }
        }

        #endregion
    }
}
