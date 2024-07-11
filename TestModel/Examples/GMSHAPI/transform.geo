SetFactory("OpenCASCADE");

//Mesh.Algorithm = 6;
//Установка настройки Mesh.MeshSizeMin - указывает минимальное значение диапазона в мм, в которые должны попадать элементы сетки при генерации
Mesh.MeshSizeMin = 0.1;
//Установка настройки Mesh.MeshSizeMax - указывает максимальное значение диапазона в мм, в которые должны попадать элементы сетки при генерации
Mesh.MeshSizeMax = 0.1;

DefineConstant[
  x = {0, Min -5, Max 5, Step 0.1, Name "Bloc 1/0x"}
  y = {0, Min -5, Max 5, Step 0.1, Name "Bloc 1/0y"}
  z = {0, Min -5, Max 5, Step 0.1, Name "Bloc 1/0z"}
  dx = {2, Min 0.1, Max 5, Step 0.1, Name "Bloc 1/dx"}
  dy = {2, Min 0.1, Max 5, Step 0.1, Name "Bloc 1/dy"}
  dz = {2, Min 0.1, Max 5, Step 0.1, Name "Bloc 1/dz"}
  x2 = {x+dx, Min -5, Max 5, Step 0.1, Name "Bloc 2/0x"}
  y2 = {0, Min -5, Max 5, Step 0.1, Name "Bloc 2/0y"}
  z2 = {0, Min -5, Max 5, Step 0.1, Name "Bloc 2/0z"}
  dx2 = {1, Min 0.1, Max 5, Step 0.1, Name "Bloc 2/dx"}
  dy2 = {1, Min 0.1, Max 5, Step 0.1, Name "Bloc 2/dy"}
  dz2 = {3, Min 0.1, Max 5, Step 0.1, Name "Bloc 2/dz"}
];

//Команда Box - строит объемный параллелепипед, первые три параметра точка начала координат, последние три - размерности фигуры по трем осям
Box(1) = {x,y,z, dx,dy,dz};
Box(2) = {x2,y2,z2, dx2,dy2,dz2};

//Команда Translate - задает вектор перемещения в первых фигурных скобках для геометрической сущности во вторых фигурных скобках
Translate{0.2,0.2,0.2}{ Volume{1}; }
//Команда Rotate - задает вектор вращения в первых фигурных скобках, точка поворота во вторых фигурных скобках, 
//угол поворота и геометрическую сущность в последних фигурных скобках
Rotate { {1,0,0}, {0,0,0}, Pi/3 } { Volume{1}; }
Rotate { {0,1,0}, {0,0,0}, Pi/3 } { Volume{1}; }

f() = BooleanFragments { Volume{1,2}; Delete; }{};

Translate{4,0,0}{ Duplicata{ Volume{1/*,2,3*/}; } }
