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
        Color selectionColor;
        byte transparency;
        bool hideInsideSurfaces;
        int updateDepth;
        bool hasChanges;

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
                Raise(true);
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
                Raise(true);
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
                Raise(true);
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
        public event EventHandler Changed;

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
            Raise(view.SetSetColor(setInfo, color));
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
            Raise(view.SetViewMode(setInfo, viewMode));
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

            Raise(view.SetHidden(numbers, !visible));
            RemoveEmptyView(objType, view);
        }

        /// <inheritdoc/>
        public void HideSelected()
        {
            using (BeginUpdate())
            {
                foreach (var view in views.Values)
                {
                    Raise(view.SetHidden(view.GetSelected(), true));
                    Raise(view.ClearSelection());
                }

                RemoveEmptyViews();
            }
        }

        /// <inheritdoc/>
        public void ShowAll()
        {
            using (BeginUpdate())
            {
                foreach (var view in views.Values)
                    Raise(view.ShowAll());

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
            var view = GetOrCreateView(objType);
            Raise(view.Select(numbers));
            RemoveEmptyView(objType, view);
        }

        /// <inheritdoc/>
        public void Deselect(ObjType objType, IEnumerable<int> numbers)
        {
            if (!TryGetView(objType, out var view))
                return;

            Raise(view.Deselect(numbers));
            RemoveEmptyView(objType, view);
        }

        /// <inheritdoc/>
        public void ToggleSelection(ObjType objType, IEnumerable<int> numbers)
        {
            var view = GetOrCreateView(objType);
            Raise(view.ToggleSelection(numbers));
            RemoveEmptyView(objType, view);
        }

        /// <inheritdoc/>
        public void SetSelection(ObjType objType, IEnumerable<int> numbers)
        {
            using (BeginUpdate())
            {
                foreach (var pair in views)
                    if (pair.Key != objType)
                        Raise(pair.Value.ClearSelection());

                var view = GetOrCreateView(objType);
                Raise(view.SetSelection(numbers));
                RemoveEmptyViews();
            }
        }

        /// <inheritdoc/>
        public void ClearSelection(ObjType objType)
        {
            if (!TryGetView(objType, out var view))
                return;

            Raise(view.ClearSelection());
            RemoveEmptyView(objType, view);
        }

        /// <inheritdoc/>
        public void ClearSelection()
        {
            using (BeginUpdate())
            {
                foreach (var view in views.Values)
                    Raise(view.ClearSelection());

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
                    Raise(pair.Value.Prune(number => model.ObjectData.Find(objType, number) != null));
                    Raise(pair.Value.PruneSetStyles(model.ObjectData.GetSetsInfo(objType)));
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
                foreach (var view in views.Values)
                    Raise(view.ClearSelectionAndHidden());

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
        /// Уведомляет подписчиков об изменении состояния.
        /// </summary>
        private void Raise(bool changed)
        {
            if (!changed)
                return;

            if (updateDepth > 0)
            {
                hasChanges = true;
                return;
            }

            Changed?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Завершает пакет изменений представления.
        /// </summary>
        private void EndUpdate()
        {
            updateDepth--;
            if (updateDepth != 0 || !hasChanges)
                return;

            hasChanges = false;
            Changed?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Ограничивает время жизни пакета изменений.
        /// </summary>
        private sealed class UpdateScope : IDisposable
        {
            readonly ModelView view;
            bool isDisposed;

            /// <summary>
            /// Создаёт ограничитель пакета изменений.
            /// </summary>
            public UpdateScope(ModelView view)
            {
                this.view = view;
            }

            /// <inheritdoc/>
            public void Dispose()
            {
                if (isDisposed)
                    return;

                isDisposed = true;
                view.EndUpdate();
            }
        }
    }
}
