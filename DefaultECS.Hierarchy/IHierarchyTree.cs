using System;

namespace DefaultEcs.Hierarchy
{
    /// <summary>
    /// Представляет интерфейс иерархического дерева.
    /// </summary>
    public interface IHierarchyTree
    {
        /// <summary>
        /// Событие, когда в сущность добавлен маркер <see cref="Hierarchy.Components.HierarchyKey{TKey}"/>.
        /// </summary>
        event ParentAddedHandler ParentAdded;

        /// <summary>
        /// Событие, когда у сущности удален маркер <see cref="Hierarchy.Components.HierarchyKey{TKey}"/>.
        /// </summary>
        event ParentRemovedHandler ParentRemoved;

        /// <summary>
        /// Событие, когда в сущности обновлен маркер <see cref="Hierarchy.Components.HierarchyKey{TKey}"/>.
        /// </summary>
        event ParentChangedHandler ParentChanged;

        /// <summary>
        /// Возвращает родителя указанной сущности по компонент-ключу.
        /// </summary>
        /// <param name="entity">Сущность, родителя которого требуется вернуть.</param>
        /// <returns>Экземпляр родителя или <c>null</c>, если сущность находится в корне.</returns>
        Entity? GetParent(in Entity entity);

        /// <summary>
        /// Пытается получить сущности по указанному сущности-родителю.
        /// </summary>
        /// <param name="entity">Родитель, для которого ищем детей или <c>null</c>, если ищем корневые сущности.</param>
        /// <param name="childs">Коллекция найденных сущностей.</param>
        /// <returns><c>true</c>, если сущности есть. Иначе - <c>false</c>.</returns>
        bool TryGetChildren(in Entity? entity, out ReadOnlySpan<Entity> childs);
    }
}