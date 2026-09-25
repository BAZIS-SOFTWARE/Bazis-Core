using Model.Interfaces;
using Model.Interfaces.ObjectsCollections;
using OperationalController.ModelScenePresentator;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace OperationalController
{
    /// <summary>
    /// Хранит состояние представления отдельно от расчётной модели.
    /// </summary>
    public class ModelView : IModelView
    {
        readonly IModelData model;
        readonly Dictionary<ObjType, IObjectsView> views = new Dictionary<ObjType, IObjectsView>();
        readonly ChangedSets changedSets = new ChangedSets();
        Color selectionColor;
        byte transparency;
        bool hideInsideSurfaces;
        int updateDepth; // глубина вложенности событий. 0 - можно отправлять

        /// <summary>
        /// Создаёт представление заданной модели.
        /// </summary>
        public ModelView(IModelData model)
        {
            this.model = model ?? throw new ArgumentNullException(nameof(model));
            selectionColor = Color.Red;
            transparency = byte.MaxValue;
        }

        /// <inheritdoc/>
        public IModelData Model => model;

        /// <inheritdoc/>
        public Color SelectionColor
        {
            get => selectionColor;
            set
            {
                if (selectionColor == value)
                    return;

                selectionColor = value;
                using (BeginUpdate())
                {
                    foreach (var objType in SelectedTypes)
                    {
                        var selectedNumbers = GetSelected(objType);
                        var selected = new List<int>(selectedNumbers);
                        Raise(ModelViewChange.SelectionColor, objType, selected);
                    }
                }
            }
        }

        /// <inheritdoc/>
        public byte Transparency
        {
            get => transparency;
            set
            {
                if (transparency == value)
                    return;

                transparency = value;
                Raise(ModelViewChange.Transparency);
            }
        }

        /// <inheritdoc/>
        public bool HideInsideSurfaces
        {
            get => hideInsideSurfaces;
            set
            {
                if (hideInsideSurfaces == value)
                    return;

                hideInsideSurfaces = value;
                Raise(ModelViewChange.InsideSurfaces);
            }
        }

        /// <inheritdoc/>
        public int SelectedCount
        {
            get
            {
                var count = 0;
                foreach (var view in views.Values)
                    count += view.SelectedCount;
                return count;
            }
        }

        /// <inheritdoc/>
        public IEnumerable<ObjType> SelectedTypes => GetSelectedTypes();

        /// <inheritdoc/>
        public event EventHandler<ModelViewChangedEventArgs> Changed;

        /// <inheritdoc/>
        public Color GetColor(ObjType objType, int number)
        {
            var setInfo = model.ObjectData.GetSetInfo(objType, number);
            if (setInfo == null)
                throw new ArgumentException("The object does not belong to a model set.", nameof(number));

            return GetColor(setInfo, number);
        }

        /// <inheritdoc/>
        public Color GetColor(ISetInfo setInfo, int number)
        {
            if (setInfo == null)
                throw new ArgumentNullException(nameof(setInfo));

            var view = GetOrCreateView(setInfo.ObjType);
            var setColor = view.GetSetColor(setInfo);
            var color = view.IsSelected(number) ? SelectionColor : setColor;
            return GetDisplayColor(color);
        }

        /// <summary>
        /// Применяет прозрачность представления к исходному цвету.
        /// </summary>
        private Color GetDisplayColor(Color color)
        {
            var alpha = color.A * Transparency / byte.MaxValue;
            return Color.FromArgb(alpha, color);
        }

        /// <inheritdoc/>
        public Color GetSetColor(ISetInfo setInfo)
        {
            if (setInfo == null)
                throw new ArgumentNullException(nameof(setInfo));

            var view = GetOrCreateView(setInfo.ObjType);
            return view.GetSetColor(setInfo);
        }

        /// <inheritdoc/>
        public void SetSetColor(ISetInfo setInfo, Color color)
        {
            if (setInfo == null)
                throw new ArgumentNullException(nameof(setInfo));

            var view = GetOrCreateView(setInfo.ObjType);
            var changed = view.SetSetColor(setInfo, color);
            if (changed)
                Raise(ModelViewChange.SetColor, setInfo);
        }

        /// <inheritdoc/>
        public ViewMode GetViewMode(ISetInfo setInfo)
        {
            if (setInfo == null)
                throw new ArgumentNullException(nameof(setInfo));

            var view = GetOrCreateView(setInfo.ObjType);
            return view.GetViewMode(setInfo);
        }

        /// <inheritdoc/>
        public void SetViewMode(ISetInfo setInfo, ViewMode viewMode)
        {
            if (setInfo == null)
                throw new ArgumentNullException(nameof(setInfo));

            var view = GetOrCreateView(setInfo.ObjType);
            var changed = view.SetViewMode(setInfo, viewMode);
            if (changed)
                Raise(ModelViewChange.ViewMode, setInfo);
        }

        /// <inheritdoc/>
        public bool GetVisible(ObjType objType, int number)
        {
            return !TryGetView(objType, out var view) || !view.IsHidden(number);
        }

        /// <inheritdoc/>
        public void SetVisible(ObjType objType, IEnumerable<int> numbers, bool visible)
        {
            if (numbers == null)
                throw new ArgumentNullException(nameof(numbers));

            IObjectsView view;
            if (visible)
            {
                if (!TryGetView(objType, out view))
                    return;
            }
            else
            {
                view = GetOrCreateView(objType);
            }

            var changedNumbers = new List<int>(numbers);
            var changed = view.SetHidden(changedNumbers, !visible);
            if (changed)
                Raise(ModelViewChange.Visibility, objType, changedNumbers);

            RemoveEmptyView(objType, view);
        }

        /// <inheritdoc/>
        public void HideSelected()
        {
            using (BeginUpdate())
            {
                foreach (var pair in views)
                {
                    var objType = pair.Key;
                    var view = pair.Value;
                    var selected = new List<int>(view.GetSelected());
                    var hiddenChanged = view.SetHidden(selected, true);
                    var selectionChanged = view.ClearSelection();
                    if (hiddenChanged || selectionChanged)
                        Raise(ModelViewChange.Visibility | ModelViewChange.Selection, objType, selected);
                }

                RemoveEmptyViews();
            }
        }

        /// <inheritdoc/>
        public void ShowAll()
        {
            using (BeginUpdate())
            {
                foreach (var pair in views)
                {
                    var changed = pair.Value.ShowAll();
                    if (changed)
                        Raise(ModelViewChange.Visibility, pair.Key);
                }

                RemoveEmptyViews();
            }
        }

        /// <inheritdoc/>
        public bool IsSelected(ObjType objType, int number)
        {
            return TryGetView(objType, out var view) && view.IsSelected(number);
        }

        /// <inheritdoc/>
        public IEnumerable<int> GetSelected(ObjType objType)
        {
            if (!TryGetView(objType, out var view))
                yield break;

            foreach (var number in view.GetSelected())
                yield return number;
        }

        /// <inheritdoc/>
        public IEnumerable<IModelObject> GetSelection()
        {
            foreach (var pair in views)
            {
                foreach (var number in pair.Value.GetSelected())
                {
                    var modelObject = model.ObjectData.Find(pair.Key, number);
                    if (modelObject != null)
                        yield return modelObject;
                }
            }
        }

        /// <inheritdoc/>
        public bool TryGetSingleSelectedType(out ObjType objType)
        {
            var hasType = false;
            objType = default;
            foreach (var type in SelectedTypes)
            {
                if (hasType)
                    return false;

                objType = type;
                hasType = true;
            }

            return hasType;
        }

        /// <inheritdoc/>
        public void Select(ObjType objType, IEnumerable<int> numbers)
        {
            if (numbers == null)
                throw new ArgumentNullException(nameof(numbers));

            var changedNumbers = new List<int>(numbers);
            var view = GetOrCreateView(objType);
            var changed = view.Select(changedNumbers);
            if (changed)
                Raise(ModelViewChange.Selection, objType, changedNumbers);

            RemoveEmptyView(objType, view);
        }

        /// <inheritdoc/>
        public void Deselect(ObjType objType, IEnumerable<int> numbers)
        {
            if (numbers == null)
                throw new ArgumentNullException(nameof(numbers));

            if (!TryGetView(objType, out var view))
                return;

            var changedNumbers = new List<int>(numbers);
            var changed = view.Deselect(changedNumbers);
            if (changed)
                Raise(ModelViewChange.Selection, objType, changedNumbers);

            RemoveEmptyView(objType, view);
        }

        /// <inheritdoc/>
        public void ToggleSelection(ObjType objType, IEnumerable<int> numbers)
        {
            if (numbers == null)
                throw new ArgumentNullException(nameof(numbers));

            var changedNumbers = new List<int>(numbers);
            var view = GetOrCreateView(objType);
            var changed = view.ToggleSelection(changedNumbers);
            if (changed)
                Raise(ModelViewChange.Selection, objType, changedNumbers);

            RemoveEmptyView(objType, view);
        }

        /// <inheritdoc/>
        public void SetSelection(ObjType objType, IEnumerable<int> numbers)
        {
            if (numbers == null)
                throw new ArgumentNullException(nameof(numbers));

            using (BeginUpdate())
            {
                foreach (var pair in views)
                {
                    if (pair.Key == objType)
                        continue;

                    var cleared = pair.Value.ClearSelection();
                    if (cleared)
                        Raise(ModelViewChange.Selection, pair.Key);
                }

                var changedNumbers = new List<int>(numbers);
                var view = GetOrCreateView(objType);
                var changed = view.SetSelection(changedNumbers);
                if (changed)
                    Raise(ModelViewChange.Selection, objType, changedNumbers);

                RemoveEmptyViews();
            }
        }

        /// <inheritdoc/>
        public void ClearSelection(ObjType objType)
        {
            if (!TryGetView(objType, out var view))
                return;

            var changed = view.ClearSelection();
            if (changed)
                Raise(ModelViewChange.Selection, objType);

            RemoveEmptyView(objType, view);
        }

        /// <inheritdoc/>
        public void ClearSelection()
        {
            using (BeginUpdate())
            {
                foreach (var pair in views)
                {
                    var changed = pair.Value.ClearSelection();
                    if (changed)
                        Raise(ModelViewChange.Selection, pair.Key);
                }

                RemoveEmptyViews();
            }
        }

        /// <inheritdoc/>
        public void Prune()
        {
            using (BeginUpdate())
            {
                foreach (var pair in views)
                {
                    var objType = pair.Key;
                    var prunedObjects = pair.Value.Prune(number => model.ObjectData.Find(objType, number) != null);
                    if (prunedObjects)
                        Raise(ModelViewChange.Visibility | ModelViewChange.Selection, objType);

                    var setsOfType = model.ObjectData.GetSetsInfo(objType);
                    var prunedStyles = pair.Value.PruneSetStyles(setsOfType);
                    if (prunedStyles)
                        Raise(ModelViewChange.Visibility, objType);
                }

                RemoveEmptyViews();
            }
        }

        /// <inheritdoc/>
        public void Reset()
        {
            if (views.Count == 0)
                return;

            using (BeginUpdate())
            {
                foreach (var pair in views)
                {
                    var changed = pair.Value.ClearSelectionAndHidden();
                    if (changed)
                        Raise(ModelViewChange.Visibility | ModelViewChange.Selection, pair.Key);
                }

                RemoveEmptyViews();
            }
        }

        /// <inheritdoc/>
        public IDisposable BeginUpdate()
        {
            updateDepth++;
            return new UpdateScope(this);
        }

        /// <summary>
        /// Возвращает или создаёт состояние представления объектов указанного типа.
        /// </summary>
        private IObjectsView GetOrCreateView(ObjType objType)
        {
            if (TryGetView(objType, out var view))
                return view;

            view = CreateView();
            views.Add(objType, view);
            return view;
        }

        /// <summary>
        /// Создаёт состояние представления одного типа объектов.
        /// </summary>
        private IObjectsView CreateView()
        {
            return new ObjectsView();
        }

        /// <summary>
        /// Возвращает состояние представления указанного типа.
        /// </summary>
        private bool TryGetView(ObjType objType, out IObjectsView view)
        {
            return views.TryGetValue(objType, out view);
        }

        /// <summary>
        /// Удаляет пустое состояние представления.
        /// </summary>
        private void RemoveEmptyView(ObjType objType, IObjectsView view)
        {
            if (view.IsEmpty)
                views.Remove(objType);
        }

        /// <summary>
        /// Удаляет все пустые состояния представления.
        /// </summary>
        private void RemoveEmptyViews()
        {
            var emptyTypes = new List<ObjType>();
            foreach (var pair in views)
                if (pair.Value.IsEmpty)
                    emptyTypes.Add(pair.Key);

            foreach (var objType in emptyTypes)
                views.Remove(objType);
        }

        /// <summary>
        /// Возвращает типы с выделенными объектами.
        /// </summary>
        private IEnumerable<ObjType> GetSelectedTypes()
        {
            foreach (var pair in views)
                if (pair.Value.SelectedCount > 0)
                    yield return pair.Key;
        }

        /// <summary>
        /// Помечает изменённым заданный набор.
        /// </summary>
        private void Raise(ModelViewChange change, ISetInfo setInfo)
        {
            changedSets.Add(setInfo, change);
            Flush();
        }

        /// <summary>
        /// Помечает изменёнными все наборы заданного типа.
        /// Применяется к операциям над типом целиком: снятие выделения, показ всех,
        /// подрезка и сброс — там затронутые номера отдельно не выделяются.
        /// </summary>
        private void Raise(ModelViewChange change, ObjType objType)
        {
            foreach (var setInfo in model.ObjectData.GetSetsInfo(objType))
            {
                if (setInfo != null)
                    changedSets.Add(setInfo, change);
            }

            Flush();
        }

        /// <summary>
        /// Помечает изменёнными наборы, которым принадлежат заданные номера.
        /// Разрешение номера в набор идёт по словарю набора, поэтому стоит столько же,
        /// сколько наборов у типа. Обход прекращается, как только помечены все они.
        /// </summary>
        private void Raise(ModelViewChange change, ObjType objType, IReadOnlyList<int> numbers)
        {
            var setsOfType = new List<ISetInfo>(model.ObjectData.GetSetsInfo(objType));
            if (setsOfType.Count == 0)
                return;

            if (setsOfType.Count == 1)
            {
                changedSets.Add(setsOfType[0], change);
            }
            else
            {
                var marked = new HashSet<ISetInfo>(ReferenceEqualityComparer.Instance);
                foreach (var number in numbers)
                {
                    var setInfo = model.ObjectData.GetSetInfo(objType, number);
                    if (setInfo == null)
                        continue;

                    changedSets.Add(setInfo, change);
                    marked.Add(setInfo);

                    if (marked.Count == setsOfType.Count)
                        break;
                }
            }

            Flush();
        }

        /// <summary>
        /// Помечает изменёнными все наборы модели. Для изменений, общих для всего вида:
        /// прозрачность, цвет выделения, скрытие внутренних граней.
        /// </summary>
        private void Raise(ModelViewChange change)
        {
            using (BeginUpdate())
            {
                foreach (ObjType objType in Enum.GetValues(typeof(ObjType)))
                    Raise(change, objType);
            }
        }

        /// <summary>
        /// Поднимает <see cref="Changed"/> и очищает накопитель.
        /// Ничего не делает, пока открыт пакет изменений или накопитель пуст.
        /// </summary>
        private void Flush()
        {
            if (updateDepth != 0 || changedSets.IsEmpty)
                return;

            var args = changedSets.CreateEventArgs();
            changedSets.Clear();
            Changed?.Invoke(this, args);
        }

        /// <summary>
        /// Завершает пакет изменений представления. Зовётся из <see cref="UpdateScope"/>,
        /// поэтому internal, а не private.
        /// </summary>
        internal void EndUpdate()
        {
            updateDepth--;
            Flush();
        }
    }
}
