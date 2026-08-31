using Model.Interfaces.MeshObjects;
using Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Linq;
using MathNet.Numerics;
using System.Xml.Linq;
using Model.Interfaces.ObjectsCollections;
using Model.MeshObjects;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Security.Cryptography;

namespace Model.ObjectsCollections
{
    /// <summary>
    /// ObjectsCollection. Класс содержащий наборы объектов.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ObjectsCollection<T> : IObjectsCollection<T> where T : IModelObject
    {

        public ObjectsSet<T> this[string key]
        {
            get { return objects[key]; }
        }

        public ObjectsSet<T> this[int key]
        {
            get { return objects.ElementAt(key).Value; }
        }

        /// <inheritdoc/>

        public ICollection<string> Names => objects.Keys;
/// <inheritdoc/>

        public ICollection<ObjectsSet<T>> Objects => objects.Values;
/// <inheritdoc/>

        public int Count => objects.Values.Sum(x => x.Count);

        /// <inheritdoc/> 

        Dictionary<string, ObjectsSet<T>> objects = new Dictionary<string, ObjectsSet<T>>();
        /// <inheritdoc/>

        public void Add(string setName)
        {
            objects.Add(setName, new ObjectsSet<T>(setName));
        }
/// <inheritdoc/>

        public void Add(ObjectsSet<T> set)
        {
            if (objects.Count != 0)
            {

                foreach (var _set in objects)
                {
                    if (_set.Value.Any(x => set.ContainsKey(x.Key)))
                        throw new Exception($"В наборе {set.Name} есть элементы существующие в коллекции {typeof(T)}");
                    //foreach (var item in _set.Value)
                    //if (set.ContainsKey(item.Key))
                    //throw new Exception("Номер объекта уже добавлен в коллекцию");
                }
            }

            objects.Add(set.Name, set);
        }

        /// <inheritdoc/>

        public void Add(string setName, T addObj)
        {
            if (!objects.ContainsKey(setName))
                objects.Add(setName, new ObjectsSet<T>(setName) { });
            
            
            if (!ContainsNumber(addObj.Number))
                objects[setName].Add(addObj.Number, addObj);
            else 
                throw new Exception("Объект с таким номером уже добавлен в коллекцию!");
        }
        /// <inheritdoc/>
        public void AddRange(string setName, IEnumerable<T> addObjs)
        {
            if (!objects.ContainsKey(setName))
                objects.Add(setName, new ObjectsSet<T>(setName));

            foreach (var addObj in addObjs)
            {
                if (!ContainsNumber(addObj.Number))
                    objects[setName].Add(addObj.Number, addObj);
                else throw new Exception("Объект с таким номером уже добавлен в коллекцию!");
            }    
        }
/// <inheritdoc/>


        public override string ToString()
        {
            return objects.Count.ToString();
        }

        /// <inheritdoc/>

        public void Clear()
        {
            foreach (var item in objects)
                item.Value.ClearWithExistState();
            objects.Clear();
        }
/// <inheritdoc/>

        public void ClearEmptySets()
        {
            objects = objects.Where(x => x.Value.Count != 0).
    ToDictionary(x => x.Key, y => y.Value);
        }
/// <inheritdoc/>

        public void RemoveNotExistedObjects()
        {
            foreach (var item in objects)
                item.Value.ClearNotExisted();
        }


/// <inheritdoc/>

public bool ContainsName(string collName)
        {
            return objects.ContainsKey(collName);
        }
/// <inheritdoc/>

        public bool ContainsNumber(int objNumbr)
        {
            return objects.Any(x => x.Value.ContainsKey(objNumbr));
        }
/// <inheritdoc/>

        public T Find(int objNumbr)
        {
            foreach (var objsSet in objects)
            {
                if (objsSet.Value.ContainsKey(objNumbr))
                    return objsSet.Value[objNumbr];
            }
            return default(T);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator<KeyValuePair<string, ObjectsSet<T>>> IEnumerable<KeyValuePair<string, ObjectsSet<T>>>.GetEnumerator()
        {
            foreach (var item in objects)
            {
                yield return item;
            }
        }
        /// <summary>
        /// GetMaxNumber
        /// </summary>
        /// <returns></returns>
        public int GetMaxNumber()
        {
            return objects.Max(x => x.Value.DefaultIfEmpty().Max(y => y.Key));
        }
        /// <summary>
        /// GetMinNumber
        /// </summary>
        /// <returns></returns>
        public int GetMinNumber()
        {
            return objects.Min(x => x.Value.DefaultIfEmpty().Min(y => y.Key));
        }
/// <inheritdoc/>

        public IEnumerable<T> GetObjects()
        {
            foreach (var item in objects)
            {
                foreach (var itemValue in item.Value)
                {
                    yield return itemValue.Value;
                }
            }
        }
/// <inheritdoc/>

        public void Remove(int objNumber)
        {
            foreach (var item in objects)
            {
                item.Value[objNumber].ExistState = false;
                item.Value.Remove(objNumber);
            }
        }
/// <inheritdoc/>

        public bool Remove(string name)
        {
            foreach (var item in objects[name])
                item.Value.ExistState = false;

            return objects.Remove(name);
        }

        /// <summary>
        /// Добавляет (склеивает) наборы из другой коллекции в текущую коллекцию.
        /// </summary>
        /// <param name="collection">Коллекция наборов, которые будут добавлены. Не должна быть <c>null</c>.</param>
        /// <param name="preserveOriginalSetNames">Указывает, нужно ли сохранить оригинальные имена наборов. 
        /// Если <c>true</c>, то имена наборов будут сохранены. В противном случае, будут сгенерированы новые имена.</param>
        public void Append(IObjectsCollection<T> collection, bool preserveOriginalSetNames = false)
        {
            var shifting = 0;

            if (objects.Count != 0)
            {
                var max = Count != 0 ? GetMaxNumber() : 0;
                var min = collection.Count != 0 ? collection.GetMinNumber() : 0;
                if (min <= max)
                    shifting = max + 1;
            }
            var counter = objects.Count;

            foreach (var set in collection)
            {
                var newSetName = preserveOriginalSetNames ? GetUniqueSetName(set.Key) : GetUniqueSetName($"append_{counter++}");
                Add(newSetName);
                objects[newSetName].SetColor(set.Value.Color);

                foreach (var item in set.Value)
                {
                    var number = item.Value.Number + shifting;
                    item.Value.SetNumber(number);
                    objects[newSetName].Add(number, item.Value);
                }
            }
        }

        /// <summary>
        /// Переименовывает набор объектов в коллекции.
        /// </summary>
        /// <param name="oldName">Текущее имя набора. Если набор с таким именем отсутствует — метод ничего не делает.</param>
        /// <param name="newName">Новое имя набора. Имя присваивается и самому объекту `ObjectsSet&lt;T&gt;`.</param>
        public void ChangeName(string oldName, string newName)
        {
            ObjectsSet<T> temp = new ObjectsSet<T>(newName);
            if (ContainsName(oldName))
            {
                var item1= objects[oldName];
                temp = item1;
                temp.Name = newName;
                objects.Remove(oldName);
                objects.Add(newName, temp);
            }
        }

        private string GetUniqueSetName(string baseName)
        {
            if (!objects.ContainsKey(baseName))
                return baseName;
            var index = 1;
            while (objects.ContainsKey($"{baseName}_{index}"))
                index++;
            return $"{baseName}_{index}";
        }
    }
}
