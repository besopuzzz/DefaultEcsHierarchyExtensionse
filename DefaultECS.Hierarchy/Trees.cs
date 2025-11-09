using DefaultEcs.Hierarchy.Components;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace DefaultEcs.Hierarchy
{
    /// <summary>
    /// Представляет класс для выстраивания иерархических деревьев, а так же управляет их жизненным циклом.
    /// Используйте статические методы для создания иерархии по конкретным типам.
    /// </summary>
    public sealed class Trees :  IDisposable, IEnumerable<IHierarchyTree>
    { 
        /// <summary>
        /// Получает ссылку на текущий мир.
        /// </summary>
        internal World World { get; }

        private readonly static Dictionary<World, Trees> _trees = [];

        private readonly ConcurrentDictionary<Type, TreeSingleton> _cache = [];
        private readonly IDisposable _worldDisposedEvent;

        /// <summary>
        /// Создает или получает <see cref="Trees"/> для указанного мира.
        /// </summary>
        /// <param name="world">Ссылка на мир.</param>
        /// <returns>Экземпляр кэшированного <see cref="Trees"/></returns>
        private static Trees GetOrCreate(World world)
        {
            if (!_trees.TryGetValue(world, out Trees trees))
            {
                trees = new Trees(world);

                _trees.Add(world, trees);
            }

            return trees;
        }

        internal static void UseHierarchy(World world)
        {
            if (_trees.ContainsKey(world))
                return;

            GetOrCreate(world);
        }

        internal TreeReference GetRoot()
        {
            return GetRoot(World);
        }

        /// <summary>
        /// Возвращает ссылочный тип иерархического дерева, используемый по умолчанию.
        /// </summary>
        /// <param name="world">Ссылка на мир.</param>
        /// <returns>Ссылочный тип дерева.</returns>
        public static TreeReference GetRoot(World world)
        {
            var trees = GetOrCreate(world);

            if (!trees._cache.TryGetValue(typeof(Children), out var tree))
            {
                tree = new TreeSingleton(new TreeBase(trees));

                trees._cache.TryAdd(typeof(Children), tree);
            }

            tree.AddRef();

            return new TreeReference(tree);
        }

        /// <summary>
        /// Возвращает ссылочный тип иерархического дерева, выстроенный по компонент-ключу <typeparamref name="TKey"/>.
        /// </summary>
        /// <typeparam name="TKey">Тип компонент-ключа.</typeparam>
        /// <param name="world">Ссылка на мир.</param>
        /// <returns>Ссылочный тип дерева.</returns>
        public static TreeReference GetOrCreate<TKey>(World world)
        {
            var trees = GetOrCreate(world);

            if (!trees._cache.TryGetValue(typeof(Tree<TKey>), out var tree))
            {
                tree = new TreeSingleton(new Tree<TKey>(trees));

                trees._cache.TryAdd(typeof(Tree<TKey>), tree);
            }

            tree.AddRef();

            return new TreeReference(tree);
        }

        /// <summary>
        /// Возвращает ссылочный тип иерархического дерева, выстроенный для компонентов <typeparamref name="T"/> по компонент-ключу <typeparamref name="TKey"/>.
        /// </summary>
        /// <typeparam name="T">Тип компонента.</typeparam>
        /// <typeparam name="TKey">Тип компонент-ключа.</typeparam>
        /// <param name="world">Ссылка на мир.</param>
        /// <returns>Ссылочный тип дерева.</returns>
        public static TreeReference GetOrCreate<T, TKey>(World world)
        {
            var trees = GetOrCreate(world);

            if (!trees._cache.TryGetValue(typeof(Tree<T, TKey>), out var tree))
            {
                tree = new TreeSingleton(new Tree<T, TKey>(trees, GetOrCreate<TKey>(world)));

                trees._cache.TryAdd(typeof(Tree<T, TKey>), tree);
            }

            tree.AddRef();

            return new TreeReference(tree);
        }

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="Trees"/>.
        /// </summary>
        /// <param name="world">Ссылка на мир.</param>
        private Trees(World world)
        {
            World = world;

            _worldDisposedEvent = world.SubscribeWorldDisposed(_ => Dispose());
        }

        /// <summary>
        /// Удаляет с кэша указанное дерево, если на него больше никто не имеет ссылок.
        /// </summary>
        /// <param name="tree">Ссылка на дерево.</param>
        internal void Release(TreeSingleton tree)
        {
            var key = _cache.FirstOrDefault(x => x.Value == tree).Key;

            if (key == null)
                return;

            if (tree.Release() <= 0)
            {
                _cache.TryRemove(key, out _);
            }
        }

        /// <summary>
        /// Очищает все кэшированные деревья и отписывается от событий мира <see cref="World"/>.
        /// </summary>
        public void Dispose()
        {
            _worldDisposedEvent.Dispose();
            _trees.Remove(World);

            foreach (var tree in _cache.Values)
            {
                tree.Dispose();
            }

            _cache.Clear();
        }

        /// <inheritdoc cref="IEnumerable.GetEnumerator"/>
        public IEnumerator<IHierarchyTree> GetEnumerator()
        {
            foreach (var trees in _trees.Values)
            {
                foreach (var hierarchy in trees._cache.Values)
                {
                    yield return hierarchy;
                }
            }
        }

        /// <inheritdoc cref="IEnumerable.GetEnumerator"/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
