Merge "Cilindr.step";
SetFactory("OpenCASCADE");

Characteristic Length { 1,2,3,4 } = 1.4179;

SetString["transfinite 1", "10 Bump 0.6024"];
Transfinite Curve { 1 } = 10 Using Bump 0.6024;
SetString["transfinite 2", "19 Bump 0.6351"];
Transfinite Curve { 2 } = 19 Using Bump 0.6351;
SetString["transfinite 3", "14 Progression 1.1719"];
Transfinite Curve { 3 } = 14 Using Progression 1.1719;
SetString["transfinite 4", "18 Progression 0.8144"];
Transfinite Curve { 4 } = 18 Using Progression 0.8144;
SetString["transfinite 5", "18 Beta 0.7946"];
Transfinite Curve { 5 } = 18 Using Beta 0.7946;
SetString["transfinite 6", "17 Bump 0.8172"];
Transfinite Curve { 6 } = 17 Using Bump 0.8172;

Characteristic Length { Point{:} } = 1.4179;
Field[1] = Extend;
Field[1].SurfacesList = { Surface{:} };
Field[1].CurvesList = { Curve{:} };
Field[1].DistMax = 1.6351;
Field[1].SizeMax = 2;
Field[1].Power = 0;
Mesh.MeshSizeExtendFromBoundary = -2;
Background Field = 1;

Mesh.Algorithm = 6;
Mesh.MeshSizeMin = 0.7166;
Mesh.MeshSizeMax = 3.1842;
Mesh.MeshSizeFactor = 0.8024;

