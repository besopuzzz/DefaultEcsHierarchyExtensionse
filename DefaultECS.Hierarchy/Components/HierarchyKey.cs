using System;

namespace DefaultEcs.Hierarchy.Components
{
    /// <summary>
    /// Представляет структуру-маркер для выстраивания иерархии по компонент-ключам типа <typeparamref name="TKey"/>.
    /// </summary>
    /// <typeparam name="TKey">Тип компонент-ключа.</typeparam>
    public readonly struct HierarchyKey<TKey> : IEquatable<HierarchyKey<TKey>>, IComparable<HierarchyKey<TKey>>
    {
        /// <summary>
        /// Ссылка на родителя с компонентом типа <c>TKey</c>. Если <c>null</c>, то владелец компонента <see cref="Entity"/> находится в корне.
        /// </summary>
        public readonly Entity? Parent;

        /// <summary>
        /// Значение порядкового номера.
        /// </summary>
        public readonly int Order;

        /// <summary>
        /// Инициализирует новый экзмепляр маркера с указанным родителем.
        /// </summary>
        /// <param name="parent">Ссылка на родительскую сущность.</param>
        public HierarchyKey(in Entity? parent)
        {
            Parent = parent;
            Order = 0;
        }

        /// <summary>
        /// Инициализирует новый экзмепляр маркера с указанным родителем и порядковым номером.
        /// </summary>
        /// <param name="parent">Ссылка на родительскую сущность.</param>
        /// <param name="order">Порядковый номер сущности.</param>
        public HierarchyKey(in Entity? parent, int order) : this(in parent)
        {
            Order = order;
        }

        /// <summary>
        /// Сравнивает порядковые номеры маркеров и возвращает результат.
        /// </summary>
        /// <param name="other">Другой маркер.</param>
        /// <returns>Результат сравнения. Подробности <seealso cref="IComparable{T}.CompareTo(T)"/>.</returns>
        public int CompareTo(HierarchyKey<TKey> other) => Order.CompareTo(other.Order);

        /// <summary>
        /// Сравнивает этот маркер с другим и возвращает результат.
        /// </summary>
        /// <param name="other">Другой маркер.</param>
        /// <returns>Возвращает <c>true</c>, если другой маркер имеет того же родителя. Иначе - <c>false</c>.</returns>
        public bool Equals(HierarchyKey<TKey> other) => Parent == other.Parent;

        /// <summary>
        /// Сравнивает этот маркер с другим объектом.
        /// </summary>
        /// <param name="obj">Другой объект.</param>
        /// <returns>Возвращает <c>true</c>, если другой объект является маркером и имеет того же родителя. Иначе - <c>false</c>.</returns>
        public override bool Equals(object obj) => obj is HierarchyKey<TKey> other && Equals(other);

        /// <summary>
        /// Возвращает значение хэш-кода.
        /// </summary>
        /// <returns>Значение хэш-кода.</returns>
        public override int GetHashCode() => Parent.GetHashCode();

        /// <summary>
        /// Сравнивает два маркера на равенство.
        /// </summary>
        /// <param name="left">Левый маркер.</param>
        /// <param name="right">Правый маркер.</param>
        /// <returns>Результат сравнения. Возвращает <c>true</c>, если оба маркера имеют одного родителя. Иначе - <c>false</c>.</returns>
        public static bool operator ==(HierarchyKey<TKey> left, HierarchyKey<TKey> right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Сравнивает два маркера на неравенство.
        /// </summary>
        /// <param name="left">Левый маркер.</param>
        /// <param name="right">Правый маркер.</param>
        /// <returns>Результат сравнения. Возвращает <c>true</c>, если оба маркера не имеют одного родителя. Иначе - <c>false</c>.</returns>
        public static bool operator !=(HierarchyKey<TKey> left, HierarchyKey<TKey> right)
        {
            return !(left == right);
        }
    }
}
