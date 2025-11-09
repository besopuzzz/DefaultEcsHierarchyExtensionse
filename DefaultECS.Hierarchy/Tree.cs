using System;

namespace DefaultEcs.Hierarchy
{
    /// <summary>
    /// Представляет абстрактный класс иерархического дерева. Только для наследования.
    /// </summary>
    internal abstract class Tree : IInternalHierarchyTree, IDisposable
    {
        /// <summary>
        /// Получает ссылку на менеджер используемых деревьев.
        /// </summary>
        public Trees Trees { get; }

        /// <inheritdoc cref="IHierarchyTree.ParentAdded"/>
        public abstract event ParentAddedHandler? ParentAdded;

        /// <inheritdoc cref="IHierarchyTree.ParentRemoved"/>
        public abstract event ParentRemovedHandler? ParentRemoved;

        /// <inheritdoc cref="IHierarchyTree.ParentChanged"/>
        public abstract event ParentChangedHandler? ParentChanged;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="Tree"/>.
        /// </summary>
        /// <param name="trees">Экземпляр менеджера используемых деревьев.</param>
        public Tree(Trees trees)
        {
            Trees = trees;
        }

        /// <summary>
        /// Освобождает дерево от прослушивания всех мировых <see cref="World"/> событий.
        /// </summary>
        public virtual void Dispose()
        {

        }

        /// <inheritdoc cref="IInternalHierarchyTree.AddOrChangeMarker"/>
        public abstract void AddOrChangeMarker(in Entity entity);

        /// <inheritdoc cref="IInternalHierarchyTree.RemoveMarker"/>
        public abstract void RemoveMarker(in Entity entity);
        
        /// <inheritdoc cref="IHierarchyTree.GetParent"/>
        public abstract Entity? GetParent(in Entity entity);

        /// <inheritdoc cref="IHierarchyTree.TryGetChildren"/>
        public abstract bool TryGetChildren(in Entity? entity, out ReadOnlySpan<Entity> childs);
    }

}
