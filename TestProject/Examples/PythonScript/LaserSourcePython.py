def CalcPower(x, y, z, power = 6):
    n = 0.3 # КПД
    a = 3 # длина источника
    b = 1 # ширина источника
    c = 0.8 # глубина источника
    q = power * n / (a * b * c) # Вт/мм**3
    if float (x) <= a and float(y) <= b and float (z) <= c:
        return float(q)
    else: return 0