using System;

namespace DefaultEcs.Hierarchy.Components
{
    /// <summary>
    /// Представляет структуру-компонент для выстраивания иерархии.
    /// </summary>
    internal readonly struct Children : IEquatable<Children>
    {
        /// <summary>
        /// Ссылка на родителя. Если <c>null</c>, то владелец компонента <see cref="Entity"/> находится в корне.
        /// </summary>
        public readonly Entity? Parent;

        /// <summary>
        /// Инициализирует новый экзмепляр структуры с указанным родителем.
        /// </summary>
        /// <param name="parent">Ссылка на родительскую сущность.</param>
        public Children(Entity? value)
        {
            Parent = value;
        }

        /// <summary>
        /// Сравнивает этот компонент с другим и возвращает результат.
        /// </summary>
        /// <param name="other">Другой компонент.</param>
        /// <returns>Возвращает <c>true</c>, если другой компонент имеет того же родителя. Иначе - <c>false</c>.</returns>
        public bool Equals(Children other) => Parent == other.Parent;

        /// <summary>
        /// Сравнивает этот компонент с другим объектом.
        /// </summary>
        /// <param name="obj">Другой объект.</param>
        /// <returns>Возвращает <c>true</c>, если другой объект является <see cref="Children"/> и имеет того же родителя. Иначе - <c>false</c>.</returns>
        public override bool Equals(object obj) => obj is Children other && Equals(other);

        /// <summary>
        /// Возвращает значение хэш-кода.
        /// </summary>
        /// <returns>Значение хэш-кода.</returns>
        public override int GetHashCode() => Parent.GetHashCode();

        /// <summary>
        /// Сравнивает два компонента на равенство.
        /// </summary>
        /// <param name="left">Левый компонент.</param>
        /// <param name="right">Правый компонент.</param>
        /// <returns>Результат сравнения. Возвращает <c>true</c>, если оба компонента имеют одного родителя. Иначе - <c>false</c>.</returns>
        public static bool operator ==(Children left, Children right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Сравнивает два компонента на неравенство.
        /// </summary>
        /// <param name="left">Левый компонент.</param>
        /// <param name="right">Правый компонент.</param>
        /// <returns>Результат сравнения. Возвращает <c>true</c>, если оба компонента не имеют одного родителя. Иначе - <c>false</c>.</returns>
        public static bool operator !=(Children left, Children right)
        {
            return !(left == right);
        }
    }

}
