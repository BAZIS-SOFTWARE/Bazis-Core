# Набор библиотек для хранения геометрии и сетки, а также пре/пост обработки результатов
**принятые обозначения:**  
(->) наследование от класса<br/>

В состав входят следующие библиотеки:
- Project (ProjectData: библиотека, хранящая и управляющая граничными условиями,сеточными и геометрическими сущностями. Осуществляет функции CRUD над перечисленными объектами)
  + TaskData
    > Functions
      <details>
      - SphereFunction -> Arc<br/>
      - CillindricalFunction -> Lazer<br/>
      - CillindricalFunction -> FSWPin<br/>
      - CillindricalFunction -> FSWShoulder<br/>
      </details>
    > Data -> ObjectData -> ValueableData (абстрактный класс для хранения физических данных об условиях задачи)<br/>
    > ValueableData -> HeatData<br/>
    > ValueableData -> LoadData<br/>
    > ValueableData -> MatData<br/>
    > ValueableData -> MediaData<br/>
    > ValueableData -> ClampData<br/>
  + ModelData
    > ObjectData<br/>
    > GroupData
- Geometry (геометрические примитивые: **точка**, **линия**, **плоскость** и методы работы с ними. Также векторные операции)
- GmshApi (интерфейс с добавленными функциями к GMSH сеточному генератору. Спецификация ниже)
- Model (библиотека, хранящая сеточные и геометрические сущности: узел сетки или геометрии, элементы, поверхности и т.д. Осуществляет функции CRUD над перечисленными объектами)
  + ModelData
    > ObjectData (класс для хранения коллекций ObjectsCollection)
      - ObjectsCollection (класс для хранения наборов ObjectsSet)
        + ObjectsSet (класс для хранения геометрических **GeometryObjects** и сеточных **MeshObjects** сущностей ModelObject)
          <details>
          > ModelObject -> beam<br/>
          > ModelObject -> Node<br/>
          > ModelObject -> Hexa<br/>
          > ModelObject -> Penta<br/>
          > ModelObject -> Tetra<br/>
          > ModelObject -> Tetra_c<br/>
          > ModelObject -> Triangle<br/>
          > ModelObject -> Triangle_c<br/>
          > ModelObject -> Quad<br/>
          > ModelObject -> Curve<br/>
          > ModelObject -> GeometryPoint<br/>
          > ModelObject -> SurfaceFigure<br/>
          > ModelObject -> VolumeFigure<br/>
          </details>
    > GroupData (класс для хранения ссылок на геометрические и сеточные сущности ModelObject)
- ModelController (реализует агрегатные функции:
  + Построение сечения внутри элементов сетки
  + Поиск контуров сетки
  + Поиск открытых поверхностей
  + Поиск свободных узлов (не зависящих от сетки)
  + Поиск связанных узлов (узел - набор связанных узлов через ребра элементов)
  + Продвинутый выбор
  + Генерация массивов данных для вывода на экран, используя спецификацию OpenGL
  + Генерация 2d элементов на открытых поверхностях 3d
  + Генерация 1d элементов на открытых границах 2d
- PostProc (генерация поля результатов на открытых поверхностях элементов, участвующих в расчетах)
- PreProc (автоматизированое создание инструкций согласно типу и виду задачи, а также набору граничных условий)
  
**MeshObjects** (набор классов реализующих конкретные сеточные сущности: Node,Hexa,Penta и т.д.)

**GeometryObjects** (набор классов реализующих конкретные геометрические сущности: GeometryPoint, Curve и т.д.)

# Спецификация Model
<details>
![ModelData](https://github.com/user-attachments/assets/8e83b944-4b13-44b6-8b0e-b7ea48f7b836)<br/>
![ObjectsData](https://github.com/user-attachments/assets/cdfcf914-2c1c-4d55-88f6-396057acc585)<br/>
![ObjectsCollection](https://github.com/user-attachments/assets/97cd62d2-77ce-4f3c-ab2c-d6c74464813c)<br/>
![ObjectsSet](https://github.com/user-attachments/assets/bdfb771b-a905-4162-993b-8176b827b11a)
</details>

# Спецификация GmshApi
 <details>
![YouML_image](https://github.com/user-attachments/assets/3c507a90-e613-4f01-8973-8a583237ff37)
</details>

# Схемы интегрирования конечных элементов для решения задач для всех типов элементов 2D и 3D (Проект Model).

Естественные функции формы и их производные для различных элементов.

Beam (одномерный элемент):
 
N_i=(1/2)*(1-ξ),
N_j=(1/2)*(1+ξ),
(dN_i)/dξ=-1/2,
(dN_j)/dξ=1/2,

![image](https://github.com/user-attachments/assets/591598b4-bb94-4639-b53a-1bcaf2ec752f)

Пентаэдр (Пента) 

![пента](https://github.com/user-attachments/assets/c5ffbc19-4f08-4fbf-9cfc-0acd4dc4e272)

Xi, Xj, …Xn - функции формы для пенты (Ni,Nj,…Nn – другое обозначение тех же самых функций форм):

![Ф ф для пенты](https://github.com/user-attachments/assets/ae04fed3-4d50-417d-81c1-b436021b7bae)

Производные функции форм:
<details>
(dN_i)/dξ=-(1-ζ)/2<br/>
(dN_j)/dξ=(1-ζ)/2<br/>
(dN_k)/dξ=0<br/>
(dN_l)/dξ=-(1+ζ)/2<br/>
(dN_m)/dξ=(1+ζ)/2<br/>
(dN_n)/dξ=0<br/>
<br/>
(dN_i)/dη=-(1-ζ)/2<br/>
(dN_j)/dη=0<br/>
(dN_k)/dη=(1-ζ)/2<br/>
(dN_l)/dη=-(1+ζ)/2<br/>
(dN_m)/dη=0<br/>
(dN_n)/dη=(1+ζ)/2<br/>
<br/>
(dN_i)/dζ=-(1-ξ-η)/2<br/>
(dN_j)/dζ=-ξ/2<br/>
(dN_k)/dζ=-η/2<br/>
(dN_l)/dζ=(1-ξ-η)/2<br/>
(dN_m)/dζ=ξ/2<br/>
(dN_n)/dζ=η/2<br/>
</details>

# Разработан алгоритм для определения положения точки относительно источника в виде двойного эллипсоида Голдака. 
![image](https://github.com/user-attachments/assets/1a09a437-25b1-4018-8cd1-c4f8797b6d25)



