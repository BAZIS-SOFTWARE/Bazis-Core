def CalcPower(x, y, z, power = 6):
    n = 0.3 # КПД
    a = 3.0 # длина источника
    b = 1.0 # ширина источника
    c = 0.8 # глубина источника
    q = power * n / (a * b * c) # Вт/мм**3
        
    # debug_info = f"x={x}({type(x)}), y={y}, z={z}, a={a}, b={b}, c={c}"

    # Округляем до 6 знаков после запятой
    precision = 6

    if round(float(x), precision) <= round(a, precision) and \
       round(float(y), precision) <= round(b, precision) and \
       round(float(z), precision) <= round(c, precision):
       return float(q)
    else: return 0.0


    # Это заставит VS показать сообщение
    # raise Exception(f"Point outside bounds: {debug_info}")