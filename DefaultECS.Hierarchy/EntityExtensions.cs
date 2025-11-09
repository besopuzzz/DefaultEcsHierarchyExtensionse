using DefaultEcs.Hierarchy.Components;

namespace DefaultEcs.Hierarchy
{
    /// <summary>
    /// Представляет класс-расширение для создания иерархии.
    /// </summary>
    public static class EntityExtensions
    {
        /// <summary>
        /// Возращает родителя для указанной сущности.
        /// </summary>
        /// <param name="entity">Дочерняя сущность.</param>
        /// <returns>Возвращает результат получения родителя для дочерней сущности. Если результат <c>null</c>, значит дочерняя сущность находится в корне.</returns>
        public static Entity? GetParent(this in Entity entity)
        {
            if (!entity.Has<Children>())
                return null;

            ref var result = ref entity.Get<Children>();

            return result.Parent;
        }

        /// <summary>
        /// Устанавливает для дочеренй сущности родителя.
        /// </summary>
        /// <param name="child">Сущность, для которой устанавливается родитель.</param>
        /// <param name="parent">Сущность-родитель или <c>null</c>, если дочернюю сущность необходимо установить в корень.</param>
        public static void SetParent(this in Entity child, in Entity? parent)
        {
            if (child.Has<Children>() && child.Get<Children>().Parent == parent)
                return;

            if (parent.HasValue && parent.Value.IsParent(child))
                return;

            child.Set(new Children(parent));
        }

        /// <summary>
        /// Проверяет родство между двумя сущностями.
        /// </summary>
        /// <param name="entity">Сущность, для которой выполняется проверка.</param>
        /// <param name="parent">Сущность, предполагаемо являющиеся предком.</param>
        /// <returns>Возвращает <c>true</c>, если указанная сущность является предком. Иначе - <c>false</c>.</returns>
        public static bool IsParent(this in Entity entity, in Entity parent)
        {
            if (!entity.Has<Children>())
                return false;
            
            ref var result = ref entity.Get<Children>();

            if (!result.Parent.HasValue)
                return false;

            if (result.Parent == parent)
                return true;

            return result.Parent.Value.IsParent(in parent);
        }

        /// <summary>
        /// Выполняет поиск первого предка с компонентом типа <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">Тип компонента.</typeparam>
        /// <param name="entity">Сущность, у которой начинается поиск предка.</param>
        /// <returns>Возвращает найденного предка или <c>null</c>, если предок не найден.</returns>
        public static Entity? FindParentWith<T>(this in Entity entity)
        {
            if (!entity.Has<Children>())
                return null;

            var parent = entity.Get<Children>().Parent;

            if (!parent.HasValue)
                return null;

            if(parent.Value.Has<T>())
                return parent;

            return parent.Value.FindParentWith<T>();
        }
    }
}

