//Файл положить где находится Tavr.step, чтобы можно было посмотреть в gmsh генерацию сетки
Merge "Tavr.STEP";

//Первый сварной шов

Recombine Surface {13,14,15,16,17};

Transfinite Curve {25,29,32} = 3;
Transfinite Curve {27,31,33} = 3;

Transfinite Surface {13,14,15};

//Треугольные поверхности, важна первая точка куда gmsh будет вырождать элементы (переход с квада на треугольник\ или с гексы на призму)
Transfinite Surface {17} = {17,18,21};
Transfinite Surface {16} = {19,20,22};

Transfinite Volume {3};

//Второй сварной шов

Recombine Surface{18, 19, 20, 21, 22};

Transfinite Curve {34, 38, 41} = 3;
Transfinite Curve {36, 40, 42} = 3;

Transfinite Surface {18, 19, 20};

//Треугольные поверхности, важна первая точка куда gmsh будет вырождать элементы (переход с квада на треугольник\ или с гексы на призму)
Transfinite Surface {22} = {24,27,23};
Transfinite Surface {21} = {26,28,25};

Transfinite Volume {4};