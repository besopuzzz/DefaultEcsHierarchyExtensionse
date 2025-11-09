using System;

namespace DefaultEcs.Hierarchy
{
    /// <summary>
    /// Представляет класс иерархического дерево по указанному компоненту и ключу.
    /// Вся иерархия этого дерева выстраивается через компонент-ключ <typeparamref name="TKey"/> и компонент <typeparamref name="TComponent"/>.
    /// </summary>
    /// <typeparam name="TComponent">Компонент, который работает в паре с <typeparamref name="TKey"/>.</typeparam>
    /// <typeparam name="TKey">Ключ, по которому выстраивается иерархия.</typeparam>
    internal sealed class Tree<TComponent, TKey> : Tree
    {
        #region Tree
        /// <inheritdoc cref="Tree.ParentAdded"/>
        public override event ParentAddedHandler? ParentAdded
        {
            add
            {
                _tree.ParentAdded += value;
            }

            remove
            {
                _tree.ParentAdded -= value;
            }
        }

        /// <inheritdoc cref="Tree.ParentRemoved"/>
        public override event ParentRemovedHandler? ParentRemoved
        {
            add
            {
                _tree.ParentRemoved += value;
            }

            remove
            {
                _tree.ParentRemoved -= value;
            }
        }

        /// <inheritdoc cref="Tree.ParentChanged"/>
        public override event ParentChangedHandler? ParentChanged
        {
            add
            {
                _tree.ParentChanged += value;
            }

            remove
            {
                _tree.ParentChanged -= value;
            }
        }

        /// <inheritdoc cref="Tree.GetParent"/>
        public override Entity? GetParent(in Entity entity)
        {
            return _tree.GetParent(in entity);
        }

        /// <inheritdoc cref="Tree.TryGetChildren"/>
        public override bool TryGetChildren(in Entity? entity, out ReadOnlySpan<Entity> children)
        {
            return _tree.TryGetChildren(entity, out children);
        }

        /// <inheritdoc cref="Tree.Dispose"/>
        public override void Dispose()
        {
            _entitySet.EntityAdded -= AddOrChangeMarker;
            _entitySet.EntityRemoved -= RemoveMarker;
            _entitySet.Dispose();
            _tree.Dispose();
        }

        /// <inheritdoc cref="Tree.AddOrChangeMarker"/>
        public override void AddOrChangeMarker(in Entity entity)
        {
            ((IInternalHierarchyTree)_tree).AddOrChangeMarker(in entity);
        }

        /// <inheritdoc cref="Tree.RemoveMarker"/>
        public override void RemoveMarker(in Entity entity)
        {
            if (entity.Has<TKey>())
                return;

            ((IInternalHierarchyTree)_tree).RemoveMarker(in entity);
        }

        #endregion

        #region Tree<TComponent, TKey>

        private readonly EntitySet _entitySet;
        private readonly TreeReference _tree;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="Tree{TComponent, TKey}"/>  с указанным ключом <typeparamref name="TKey"/>.
        /// </summary>
        /// <param name="trees">Экземпляр менеджера используемых деревьев.</param>
        /// <param name="reference">Экземпляр ссылочного дерева с типом компонент-ключа <typeparamref name="TKey"/>.</param>
        public Tree(Trees trees, TreeReference reference) : base(trees)
        {
            _tree = reference;

            _entitySet = trees.World.GetEntities().With<TComponent>().AsSet();
            _entitySet.EntityAdded += AddOrChangeMarker;
            _entitySet.EntityRemoved += RemoveMarker;

            foreach (ref readonly var entity in _entitySet.GetEntities())
            {
                AddOrChangeMarker(in entity);
            }
        }

        #endregion
    }
}
