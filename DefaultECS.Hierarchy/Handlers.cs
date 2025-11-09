namespace DefaultEcs.Hierarchy
{
    /// <summary>
    /// Представляет делегат, описывающий метод-событие добавления родителя в сущность.
    /// <para>
    /// Данное событие возникает, когда у сущности добавили маркер <see cref="Hierarchy.Components.HierarchyKey{TKey}"/>, в зависимости от текущего <see cref="TreeReference"/>.
    /// Событие можно расценивать как сигнал, что сущность теперь является частью иерархии.
    /// </para>
    /// </summary>
    /// <param name="entity">Сущность, для которой вызвали событие добавления маркера <see cref="Hierarchy.Components.HierarchyKey{TKey}"/>.</param>
    public delegate void ParentAddedHandler(in Entity entity);

    /// <summary>
    /// Представляет делегат, описывающий метод-событие удаления родителя в сущности.
    /// <para>
    /// Данное событие возникает, когда у сущности удалили маркер <see cref="Hierarchy.Components.HierarchyKey{TKey}"/>, в зависимости от текущего <see cref="TreeReference"/>.
    /// Событие можно расценивать как сигнал, что сущность больше не является частью иерархии.
    /// </para>
    /// </summary>
    /// <param name="entity">Сущность, для которой вызвали событие удаления маркера <see cref="Hierarchy.Components.HierarchyKey{TKey}"/>.</param>
    public delegate void ParentRemovedHandler(in Entity entity);

    /// <summary>
    /// Представляет делегат, описывающий метод-событие обновления родителя в сущности.
    /// <para>
    /// Данное событие возникает, когда у сущности обновили маркер <see cref="Hierarchy.Components.HierarchyKey{TKey}"/>, в зависимости от текущего <see cref="TreeReference"/>.
    /// Событие можно расценивать как сигнал, что у сущности, возможно, изменилось расположение в иерархии или изменился родитель.
    /// </para>
    /// </summary>
    /// <param name="entity">Сущность, для которой вызвали событие обновления маркера <see cref="Hierarchy.Components.HierarchyKey{TKey}"/>.</param>
    public delegate void ParentChangedHandler(in Entity entity, in Entity? oldParent, in Entity? newParent);
}
