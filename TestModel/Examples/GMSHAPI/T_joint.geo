// Gmsh project created on Mon Oct 17 00:57:04 2022
//+
Point(1) = {3, 0, 0, 1.0};
//+
Point(2) = {-3, 0, 0, 1.0};
//+
Point(3) = {-6, 0, 0, 1.0};
//+
Point(4) = {6, 0, 0, 1.0};
//+
Point(5) = {6, 6, 0, 1.0};
//+
Point(6) = {3, 6, 0, 1.0};
//+
Point(7) = {-3, 6, 0, 1.0};
//+
Point(8) = {-6, 6, 0, 1.0};
//+
Point(9) = {-3, 56, 0, 1.0};
//+
Point(10) = {3, 56, 0, 1.0};
//+
Point(11) = {-3, 9, 0, 1.0};
//+
Point(12) = {3, 9, 0, 1.0};
//+
Point(13) = {-3, 15, 0, 1.0};
//+
Point(14) = {3, 15, 0, 1.0};
//+
Point(15) = {-12, 6, 0, 1.0};
//+
Point(16) = {-12, 0, 0, 1.0};
//+
Point(17) = {12, 6, 0, 1.0};
//+
Point(18) = {12, 0, 0, 1.0};
//+
Point(19) = {50, 6, 0, 1.0};
//+
Point(20) = {50, 0, 0, 1.0};
//+
Point(21) = {-50, 6, 0, 1.0};
//+
Point(22) = {-50, 0, 0, 1.0};////+
Line(1) = {2, 1};
//+
Line(2) = {7, 6};
//+
Line(3) = {1, 6};
//+
Line(4) = {2, 7};
//+
Line(5) = {1, 4};
//+
Line(6) = {6, 5};
//+
Line(7) = {4, 5};
//+
Line(8) = {4, 18};
//+
Line(9) = {5, 17};
//+
Line(10) = {18, 17};
//+
Line(11) = {2, 3};
//+
Line(12) = {7, 8};
//+
Line(13) = {3, 8};
//+
Line(14) = {3, 16};
//+
Line(15) = {8, 15};
//+
Line(16) = {16, 15};
//+
Line(17) = {18, 20};
//+
Line(18) = {17, 19};
//+
Line(19) = {20, 19};
//+
Line(20) = {16, 22};
//+
Line(21) = {15, 21};
//+
Line(22) = {22, 21};
//+
Line(23) = {7, 11};
//+
Line(24) = {6, 12};
//+
Line(25) = {11, 12};
//+
Line(26) = {8, 11};
//+
Line(27) = {12, 5};
//+
Line(28) = {11, 13};
//+
Line(29) = {12, 14};
//+
Line(30) = {13, 14};
//+
Line(31) = {14, 10};
//+
Line(32) = {13, 9};
//+
Line(33) = {10, 9};
//+
Curve Loop(1) = {1, 3, -2, -4};
//+
Plane Surface(1) = {1};
//+
Curve Loop(2) = {3, 6, -7, -5};
//+
Plane Surface(2) = {2};
//+
Curve Loop(3) = {12, -13, -11, 4};
//+
Plane Surface(3) = {3};
//+
Curve Loop(4) = {9, -10, -8, 7};
//+
Plane Surface(4) = {4};
//+
Curve Loop(5) = {15, -16, -14, 13};
//+
Plane Surface(5) = {5};
//+
Curve Loop(6) = {10, 18, -19, -17};
//+
Plane Surface(6) = {6};
//+
Curve Loop(7) = {16, 21, -22, -20};
//+
Plane Surface(7) = {7};
//+
Curve Loop(8) = {2, 24, -25, -23};
//+
Plane Surface(8) = {8};
//+
Curve Loop(9) = {25, 29, -30, -28};
//+
Plane Surface(9) = {9};
//+
Curve Loop(10) = {32, -33, -31, -30};
//+
Plane Surface(10) = {10};
//+
Curve Loop(11) = {12, 26, -23};
//+
Plane Surface(11) = {11};
//+
Curve Loop(12) = {6, -27, -24};
//+
Plane Surface(12) = {12};
//+
Transfinite Curve {3, 7, 10, 9, 8, 4, 13, 16, 15, 14, 22, 19, 2, 1, 25, 30, 33} = 4 Using Progression 1;
//+
Transfinite Curve {26, 23, 12, 11, 5, 6, 27, 24} = 3 Using Progression 1;
//+
Transfinite Curve {32, 31, 18, 17, 21, 20} = 9 Using Progression 1.2;
//+
Transfinite Surface {1} Right;
//+
Transfinite Surface {2} Right;
//+
Transfinite Surface {3} Right;
//+
Transfinite Surface {4} Right;
//+
Transfinite Surface {5} Right;
//+
Transfinite Surface {6} Right;
//+
Transfinite Surface {7} Right;
//+
Transfinite Surface {8} Right;
//+
Transfinite Surface {9} Right;
//+
Transfinite Surface {10} Right;
//+
Transfinite Surface {11} Right;
//+
Transfinite Surface {12} Right;
//+
Transfinite Curve {28, 29, 30, 25} = 4 Using Progression 1;
//+
Transfinite Surface {9} Alternated;
//+
Transfinite Curve {28} = 4 Using Progression 1;
//+
Transfinite Curve {29} = 4 Using Progression 1;
//+
Transfinite Curve {30} = 4 Using Progression 1;
//+
Transfinite Curve {25} = 4 Using Progression 1;
//+
Transfinite Curve {33} = 4 Using Progression 1;
//+
Transfinite Surface {9} Right;
//+
Transfinite Surface {10} Right;
//+
Transfinite Surface {8} Right;
//+
Transfinite Surface {12} Right;
//+
Transfinite Surface {11} Right;
//+


//+
Extrude {0, 0, 100} {
  Surface{1}; 
}
//+
Transfinite Curve {41, 40, 49, 45} = 30 Using Progression 1;
//+
Transfinite Curve {37, 36, 35, 38} = 4 Using Progression 1;
//+
Transfinite Surface {50} Right;
//+
Transfinite Surface {42} Right;
//+
Transfinite Surface {54} Right;
//+
Transfinite Surface {46} Right;
//+
Transfinite Surface {55} Right;
//+
Transfinite Surface {1} Right;
//+
Transfinite Volume{1} = {7, 6, 1, 2, 32, 28, 24, 23};
//+
//+
Extrude {0, 0, 100} {
  Surface{2}; 
}
//+
Transfinite Curve {67, 71} = 30 Using Progression 1;
//+
Transfinite Curve {58, 60} = 3 Using Progression 1;
//+
Transfinite Curve {59} = 4 Using Progression 1;
//+
Transfinite Surface {77} Right;
//+
Transfinite Surface {68} Right;
//+
Transfinite Surface {72} Right;
//+
Transfinite Surface {76} Right;
//+
//+
Transfinite Volume{2} = {38, 28, 24, 42, 5, 6, 1, 4};
//+
Extrude {0, 0, 100} {
  Surface{3}; 
}
//+
Transfinite Curve {85, 89} = 30 Using Progression 1;
//+
Transfinite Curve {79, 81} = 3 Using Progression 1;
//+
Transfinite Curve {80} = 4 Using Progression 1;
//+
Transfinite Surface {86} Right;
//+
Transfinite Surface {94} Right;
//+
Transfinite Surface {90} Right;
//+
Transfinite Surface {99} Right;
//+
Transfinite Volume{3} = {32, 44, 48, 23, 7, 8, 3, 2};
//+
Extrude {0, 0, 100} {
  Surface{4}; 
}
//+
Transfinite Curve {107, 111} = 30 Using Progression 1;
//+
Transfinite Curve {101, 103, 102} = 4 Using Progression 1;
//+
Transfinite Surface {108} Right;
//+
Transfinite Surface {116} Right;
//+
Transfinite Surface {112} Right;
//+
Transfinite Surface {121} Right;
//+
Transfinite Volume{4} = {50, 38, 42, 54, 17, 5, 4, 18};
//+
Extrude {0, 0, 100} {
  Surface{5}; 
}
//+
Transfinite Curve {129, 133} = 30 Using Progression 1;
//+
Transfinite Curve {123, 125, 124} = 4 Using Progression 1;
//+
Transfinite Surface {130} Right;
//+
Transfinite Surface {138} Right;
//+
Transfinite Surface {134} Right;
//+
Transfinite Surface {143} Right;
//+
Transfinite Volume{5} = {44, 56, 60, 48, 8, 15, 16, 3};
//+//+

//+
Point(61) = {50, 6, 100, 1.0};
//+
Point(62) = {50, 0, 100, 1.0};
//+
Point(63) = {-50, 6, 100, 1.0};
//+
Point(64) = {-50, 0, 100, 1.0};
//+
Line(134) = {61, 62};
//+
Line(135) = {50, 61};
//+
Line(136) = {54, 62};
//+
Line(137) = {61, 19};
//+
Line(138) = {20, 62};
//+
Line(139) = {56, 63};
//+
Line(140) = {60, 64};
//+
Line(141) = {21, 63};
//+
Line(142) = {22, 64};
//+
Line(143) = {64, 63};
//+
Curve Loop(13) = {139, -143, -140, -124};
//+
Plane Surface(144) = {13};
//+
Curve Loop(14) = {141, -143, -142, 22};
//+
Plane Surface(145) = {14};
//+
Curve Loop(15) = {20, 142, -140, -133};
//+
Plane Surface(146) = {15};
//+
Curve Loop(16) = {129, 139, -141, -21};
//+
Plane Surface(147) = {16};
//+
Curve Loop(17) = {134, -136, -102, 135};
//+
Plane Surface(148) = {17};
//+
Curve Loop(18) = {138, -134, 137, -19};
//+
Plane Surface(149) = {18};
//+
Curve Loop(19) = {138, -136, -111, 17};
//+
Plane Surface(150) = {19};
//+
Curve Loop(20) = {137, -18, 107, 135};
//+
Plane Surface(151) = {20};
//+
Surface Loop(1) = {149, 150, 148, 151, 6, 112};
//+
Volume(6) = {1};
//+
Surface Loop(2) = {147, 144, 145, 146, 7, 134};
//+
Volume(7) = {2};
//+
Transfinite Curve {137, 138, 141, 142} = 30 Using Progression 1;
//+
Transfinite Curve {135, 136, 139, 140} = 9 Using Progression 1.2;
//+
Transfinite Curve {134, 143} = 4 Using Progression 1;
//+
Transfinite Surface {149} Right;
//+
Transfinite Surface {148} Right;
//+
Transfinite Surface {151} Right;
//+
Transfinite Surface {150} Right;
//+
Transfinite Surface {144} Right;
//+
Transfinite Surface {145} Right;
//+
Transfinite Surface {146} Right;
//+
Transfinite Surface {147} Right;
//+
Transfinite Surface {7} Right;
//+
Transfinite Volume{6} = {61, 50, 54, 62, 19, 17, 18, 20};
//+
Extrude {0, 0, 100} {
  Surface{11}; Surface{12}; 
}
//+

//+
Transfinite Curve {154, 155, 172, 171} = 3 Using Progression 1;
//+
Transfinite Curve {179, 162} = 30 Using Progression 1;
//+
Transfinite Surface {168} Right;
//+
Transfinite Surface {185} Right;
//+
Transfinite Surface {184} Right;
//+
Transfinite Surface {180} Right;
//+
Transfinite Surface {167} Right;
//+
Transfinite Surface {163} Right;
//+
Transfinite Surface {168} Right;
//+


//+
Extrude {0, 0, 100} {
  Surface{8}; 
}
//+
Transfinite Curve {189} = 4 Using Progression 1;
//+
Transfinite Surface {207} Right;
//+
Transfinite Surface {202} Right;
//+
Transfinite Volume{10} = {76, 70, 32, 28, 12, 11, 7, 6};
//+
Extrude {0, 0, 100} {
  Surface{9}; 
}
//+
Transfinite Curve {211, 210, 212} = 4 Using Progression 1;
//+
Transfinite Curve {219, 223} = 30 Using Progression 1;
//+
Transfinite Surface {229} Right;
//+
Transfinite Surface {228} Right;
//+
Transfinite Surface {224} Right;
//+
Transfinite Surface {220} Right;
//+
Transfinite Volume{11} = {82, 86, 70, 76, 14, 13, 11, 12};
//+

//+
Point(87) = {-3, 56, 100, 1.0};
//+
Point(88) = {3, 56, 100, 1.0};
//+
Line(224) = {9, 87};
//+
Line(225) = {10, 88};
//+
Line(226) = {86, 87};
//+
Line(227) = {82, 88};
//+
Line(228) = {87, 88};
//+
Curve Loop(21) = {32, 224, -226, -223};
//+
Plane Surface(230) = {21};
//+
Curve Loop(22) = {31, 225, -227, -219};
//+
Plane Surface(231) = {22};
//+
Curve Loop(23) = {211, 226, 228, -227};
//+
Plane Surface(232) = {23};
//+
Curve Loop(24) = {224, 228, -225, 33};
//+
Plane Surface(233) = {24};
//+
Surface Loop(3) = {233, 230, 10, 231, 232, 224};
//+
Volume(12) = {3};

//+
Transfinite Curve {226} = 9 Using Progression 1.2;
//+
Transfinite Curve {227} = 9 Using Progression 1.2;
//+
Transfinite Curve {32} = 9 Using Progression 1.2;
//+
Transfinite Curve {31} = 9 Using Progression 1.2;
//+
Transfinite Curve {33, 228, 211, 30} = 4 Using Progression 1;
//+
Transfinite Curve {224, 225, 219, 223} = 30 Using Progression 1;
//+
Transfinite Surface {233} Right;
//+
Transfinite Surface {224} Right;
//+
Transfinite Surface {232} Right;
//+
Transfinite Surface {10} Right;
//+
Transfinite Surface {230} Right;
//+
Transfinite Surface {231} Right;
//+
Transfinite Volume{12};
//+
Transfinite Curve {21, 20, 139, 140} = 10 Using Progression 1.2;
//+
Transfinite Curve {141, 142, 129, 133} = 30 Using Progression 1;
//+
Transfinite Curve {22, 16, 124, 143} = 4 Using Progression 1;
//+
Transfinite Surface {147} Right;
//+
Transfinite Surface {146} Right;
//+
Transfinite Surface {7} Right;
//+
Transfinite Surface {144} Right;
//+
Transfinite Surface {145} Right;
//+
Transfinite Surface {134} Right;
//+
Transfinite Volume{7};

//+
Transfinite Surface {150};
//+
Transfinite Surface {151};
//+
Transfinite Surface {148};
//+
Transfinite Surface {149};
//+
Transfinite Surface {6};
//+
Transfinite Surface {5} Right;
//+
Transfinite Surface {130} Right;
//+
Transfinite Surface {138} Right;
//+
Transfinite Surface {134} Right;
//+
Transfinite Surface {90} Right;
//+
Transfinite Surface {147};
//+
Transfinite Surface {144};
//+
Transfinite Surface {130};
//+
Transfinite Surface {130};
//+
Transfinite Surface {130} Right;
//+
Transfinite Surface {143} Right;
//+
Transfinite Surface {143};
//+
Transfinite Surface {99};
//+
Transfinite Surface {163};
//+
Transfinite Surface {77};
//+
Transfinite Surface {232};
//+
Transfinite Surface {233};
//+
Transfinite Surface {233};
//+
Transfinite Surface {220};
//+
Transfinite Surface {42};
//+
Transfinite Surface {146};
//+
Transfinite Surface {138};
//+
Transfinite Surface {116};
//+
Transfinite Surface {76};
//+
Transfinite Surface {150};
//+
Transfinite Surface {150};
//+
Transfinite Surface {150} Right;
//+
Transfinite Surface {147} Alternated;
//+
Transfinite Surface {147};
//+
Transfinite Surface {145};
//+
Transfinite Surface {228};
//+
Transfinite Surface {144};
//+
Transfinite Surface {143};
//+
Transfinite Surface {99};
//+
Transfinite Surface {55};
//+
Transfinite Surface {77};
//+
Transfinite Surface {121};
//+
Transfinite Surface {148};
//+
Transfinite Surface {55};
//+
Transfinite Surface {121};
//+
Transfinite Surface {55};
//+
Transfinite Surface {55} Right;
//+
Transfinite Surface {121} Right;
