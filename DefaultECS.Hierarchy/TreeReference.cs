using System;

namespace DefaultEcs.Hierarchy
{
    /// <summary>
    /// Представляет класс-обертку над глобальным singleton классом <see cref="IHierarchyTree"/> (в рамках текущего <see cref="World"/>).
    /// </summary>
    public sealed class TreeReference : IInternalHierarchyTree, IDisposable
    {
        /// <summary>
        /// Ссылка на мир, в котором используется иерархия.
        /// </summary>
        public World World => _tree.World;

        /// <inheritdoc cref="IHierarchyTree.ParentAdded"/>
        public event ParentAddedHandler? ParentAdded;

        /// <inheritdoc cref="IHierarchyTree.ParentChanged"/>
        public event ParentChangedHandler? ParentChanged;

        /// <inheritdoc cref="IHierarchyTree.ParentRemoved"/>
        public event ParentRemovedHandler? ParentRemoved;

        private readonly TreeSingleton _tree;

        /// <summary>
        /// Инициализирует новый экземпляр ссылочного дерева.
        /// </summary>
        /// <param name="tree">Ссылка на глобальный экземпляр.</param>
        internal TreeReference(TreeSingleton tree)
        {
            _tree = tree;

            _tree.ParentAdded += _tree_ParentAdded;
            _tree.ParentChanged += _tree_ParentChanged;
            _tree.ParentRemoved += _tree_ParentRemoved;
        }

        private void _tree_ParentAdded(in Entity entity)
        {
            ParentAdded?.Invoke(in entity);
        }

        private void _tree_ParentChanged(in Entity entity, in Entity? oldParent, in Entity? newParent)
        {
            ParentChanged?.Invoke(in entity, in oldParent, in newParent);
        }

        private void _tree_ParentRemoved(in Entity entity)
        {
            ParentRemoved?.Invoke(in entity);
        }

        /// <summary>
        /// Освобождает объект и отписывается от событий мира <see cref="World"/>.
        /// </summary>
        public void Dispose()
        {
            _tree.ParentAdded -= _tree_ParentAdded;
            _tree.ParentChanged -= _tree_ParentChanged;
            _tree.ParentRemoved -= _tree_ParentRemoved;

            _tree.Dispose();
        }

        /// <inheritdoc cref="IInternalHierarchyTree.AddOrChangeMarker"/>
        void IInternalHierarchyTree.AddOrChangeMarker(in Entity entity)
        {
            _tree.AddOrChangeMarker(in entity);
        }

        /// <inheritdoc cref="IInternalHierarchyTree.RemoveMarker"/>
        void IInternalHierarchyTree.RemoveMarker(in Entity entity)
        {
            _tree.RemoveMarker(in entity);
        }

        /// <inheritdoc cref="IHierarchyTree.GetParent"/>
        public Entity? GetParent(in Entity entity)
        {
            return _tree.GetParent(in entity);
        }

        /// <inheritdoc cref="IHierarchyTree.TryGetChildren"/>
        public bool TryGetChildren(in Entity? entity, out ReadOnlySpan<Entity> childs)
        {
            return _tree.TryGetChildren(entity, out childs);
        }
    }
}