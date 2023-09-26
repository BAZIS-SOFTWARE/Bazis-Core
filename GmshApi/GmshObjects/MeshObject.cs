using GmshApi.Api;
using GmshApi.GmshController;
using Model;
using System;
using System.Collections.Generic;

namespace GmshApi
{ 
    /// <summary>
    /// Класс для построения сеточных элементов
    /// </summary>
    public class MeshObject : IMeshApi
    {
        private ulong id;
        private IMeshQuality meshQuality;

        /// <summary>
        /// Задать\получить размер элементов перед генерацией
        /// </summary>
        public double Density
        {
            get
            {
                return ControllerGeneral.OptionGetNumber("Mesh.MeshSizeFactor");
            }
            set
            {
                ControllerGeneral.OptionSetNumber("Mesh.MeshSizeFactor", value);
            }
        }
        /// <summary>
        /// Коснтруктор по умолчанию
        /// </summary>
        internal MeshObject()
        {

        }
        /// <summary>
        /// Конструктор с загрузкой файла-геометрии
        /// </summary>
        /// <param name="Id">[In]Путь до файла</param>
        internal MeshObject(ulong Id)  

        {
            id = Id;
            meshQuality = new MeshQuality();
        }

        /// <summary>
        /// сделать больше квадратов на сетке
        /// </summary>
        public void QuadrangulateMesh()
        {
            ControllerMesh.Recombine();
        }
        /// <summary>
        /// Уплотнить сетку
        /// </summary>
        public void RefineMesh()
        {
            ControllerMesh.Refine();
        }
        /// <summary>
        /// Получить узлы
        /// </summary>
        /// <returns>узлы</returns>
        public IEnumerable<Node> GetNodes()
        {
            return ControllerMesh.GetNodes();
        }
        /// <summary>
        /// Получить линии 
        /// </summary>
        /// <returns>линии</returns>
        public IEnumerable<ModelObject> GetElement1D()
        {
            return ControllerMesh.GetElements(1, -1);
        }

        public void Generate(int dim)
        {
            ControllerMesh.Generate(dim);
        }

        /// <summary>
        /// получить плоские элементы
        /// </summary>
        /// <returns>плоские элементы</returns>
        public IEnumerable<ModelObject> GetElement2D()
        {
            return ControllerMesh.GetElements(2, -1);
        }
        /// <summary>
        /// получить поверхности
        /// </summary>
        /// <returns>поверхности</returns>
        public IEnumerable<ModelObject> GetSurfaces()
        {
            return ControllerMesh.GetElements(2, -1);
        }
        /// <summary>
        /// получить поверхности
        /// </summary>
        /// <returns>поверхности</returns>
        public IEnumerable<ModelObject> GetSurfaceById(int surfaceId)
        {
            return ControllerMesh.GetElements(2, surfaceId);
        }
        /// <summary>
        /// Удалить поверхность по идентификатору
        /// </summary>
        /// <param name="surfaceId">[In]Идентификатор поверхности</param>
        public void RemoveSurfaceById(int surfaceId)
        {
            ControllerMesh.DeleteElements(new int[] { 2 , surfaceId });
        }
        /// <summary>
        /// Удаляет указанные элементы сетки
        /// </summary>
        /// <param name="elementsTags">[In]Идентификаторы элементов сетки</param>
        public void RemoveElements2D(int[] elementsTags) => ControllerMesh.DeleteMeshElements(elementsTags);

        /// <summary>
        /// получить объёмные элементы
        /// </summary>
        /// <returns>объёмные элементы</returns>
        public IEnumerable<ModelObject> GetElement3D()
        {
            return ControllerMesh.GetElements(3, -1);
        }
        /// <summary>
        /// Задать алгоритм триангуляции перед генерацией сетки
        /// </summary>
        /// <param name="algorithm">[In]Тип алгоритма</param>
        public void ChangeTriangulationAlgorithm2D(Triangulation2DAlgorithm algorithm = Triangulation2DAlgorithm.FrontalDelaunay)
        {
            ControllerGeneral.OptionSetNumber("Mesh.Algorithm", Convert.ToDouble(algorithm));
        }
        /// <summary>
        /// Возвращает информацию о элементах по паре dim:tag
        /// </summary>
        /// <param name="dim">[In]Размерность</param>
        /// <param name="tag">[In]Идентификатор</param>
        /// <param name="elemInfo">[Out]ElementInfo для заполнения</param>
        /// <param name="elemsTags">[Out]Теги элементов</param>
        /// <param name="nodesTags">[Out]Теги узлов внутри dim:tag</param>
        public void GetElementsInfo(int dim, int tag, out ElementInfo[] elemInfo,
                                            out long[][] elemsTags, out long[][] nodesTags)
        {
            ControllerMesh.GetElementTags(dim, tag, out elemInfo, out elemsTags, out nodesTags);
        }


        /// <summary>
        /// Задать число точек рапределения по кривой
        /// </summary>
        /// <param name="number">[In]Число точек</param>
        /// <param name="ta">[In]Алгоритм распределения</param>
        /// <param name="parameter">[In]Коэффициент</param>
        /// <param name="curve">[In]Идентификатор кривой</param>
        public void SetMeshPointsOnCurve(int number, TransfiniteAlgorithm ta, double parameter, int curve)
        {
            switch (ta)
            {
                case TransfiniteAlgorithm.Progressive:
                    ControllerMesh.SetTransfiniteCurve(curve, number, "Progression", parameter);
                    break;
                case TransfiniteAlgorithm.Bump:
                    ControllerMesh.SetTransfiniteCurve(curve, number, "Bump", parameter);
                    break;
                case TransfiniteAlgorithm.Beta:
                    ControllerMesh.SetTransfiniteCurve(curve, number, "Beta", parameter);
                    break;
            }
        }

        public IMeshQuality QualityAnalyse()
        {
            return meshQuality;
        }
    }
}