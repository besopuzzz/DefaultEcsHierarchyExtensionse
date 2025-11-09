using System;

namespace DefaultEcs.Hierarchy
{
    /// <summary>
    /// Предоставляет класс управляемого иерархического дерева. Данный экземпляр является единственным в пределах одного <see cref="World"/>.
    /// </summary>
    internal sealed class TreeSingleton : Tree
    {
        #region Tree

        /// <inheritdoc cref="IHierarchyTree.ParentAdded"/>
        public override event ParentAddedHandler? ParentAdded
        {
            add 
            {
                _tree.ParentAdded += value; 
            }
            remove { _tree.ParentAdded -= value; }
        }

        /// <inheritdoc cref="IHierarchyTree.ParentRemoved"/>
        public override event ParentRemovedHandler? ParentRemoved
        {
            add
            {
                _tree.ParentRemoved += value; 
            }
            remove { _tree.ParentRemoved -= value; }
        }

        /// <inheritdoc cref="IHierarchyTree.ParentChanged"/>
        public override event ParentChangedHandler? ParentChanged
        {
            add
            {
                _tree.ParentChanged += value; 
            }
            remove { _tree.ParentChanged -= value; }
        }

        /// <summary>
        /// Освобождает объект. Если это последняя ссылка, то объект будет окончательно освобожден.
        /// </summary>
        public override void Dispose()
        {
            _tree.Trees.Release(this);
        }

        ///<inheritdoc cref = "Tree.AddOrChangeMarker(in Entity)" />
        public override void AddOrChangeMarker(in Entity entity)
        {
            _tree.AddOrChangeMarker(in entity);
        }

        ///<inheritdoc cref = "Tree.RemoveMarker(in Entity)" />
        public override void RemoveMarker(in Entity entity)
        {
            _tree.RemoveMarker(in entity);
        }

        ///<inheritdoc cref = "Tree.GetParent(in Entity)" />
        public override Entity? GetParent(in Entity entity)
        {
            return _tree.GetParent(in entity);
        }

        ///<inheritdoc cref = "Tree.TryGetChildren(in Entity)" />
        public override bool TryGetChildren(in Entity? entity, out ReadOnlySpan<Entity> childs)
        {
            return _tree.TryGetChildren(entity, out childs);
        }

        #endregion

        #region TreeSingleton

        /// <summary>
        /// Ссылка на мир, в котором используется иерархия.
        /// </summary>
        public World World => _tree.Trees.World;

        private readonly Tree _tree;
        private readonly object _lockObject = new object();
        private int _referenceCount;
        private bool _disposed = false;

        /// <summary>
        /// Инициализирует новый экземпляр дерева.
        /// </summary>
        /// <param name="tree">Ссылка на глобальный экземпляр.</param>
        internal TreeSingleton(Tree tree) : base(tree.Trees)
        {
            _tree = tree;
        }

        /// <summary>
        /// Увеличивает счетчик ссылок.
        /// </summary>
        internal void AddRef()
        {
            lock (_lockObject)
            {
                if (_disposed)
                    throw new ObjectDisposedException(GetType().FullName);

                _referenceCount++;
            }
        }

        /// <summary>
        /// Уменьшает счетчик ссылок и освобождает основной <see cref="Tree"/>, если ссылок больше нет.
        /// </summary>
        /// <returns></returns>
        internal int Release()
        {
            if (_disposed) return 0;

            lock (_lockObject)
            {
                _referenceCount--;

                if (_referenceCount <= 0)
                {
                    _disposed = true;

                    _tree.Dispose();
                }

                return _referenceCount;
            }
        }

        #endregion
    }
}
