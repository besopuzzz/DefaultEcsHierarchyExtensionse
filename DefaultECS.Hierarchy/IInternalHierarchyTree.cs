namespace DefaultEcs.Hierarchy
{
    /// <summary>
    /// Представляет защищенный интерфейс иерархического дерева, поддерживающий методы добавления и удаления компонент-ключей.
    /// </summary>
    internal interface IInternalHierarchyTree : IHierarchyTree
    {
        /// <summary>
        /// Добавляет или обновляет компонент-ключ у сущности.
        /// </summary>
        /// <param name="entity">Экземпляр сущности.</param>
        void AddOrChangeMarker(in Entity entity);

        /// <summary>
        /// Удаляет компонент-ключ у сущности.
        /// </summary>
        /// <param name="entity">Экземпляр сущности.</param>
        void RemoveMarker(in Entity entity);
    }
}
