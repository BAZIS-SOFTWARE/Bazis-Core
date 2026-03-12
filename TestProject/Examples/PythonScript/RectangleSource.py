def CalcPower(x, y, z, power = 600):
    n = 0.3 # efficiency
    a = 3.0 # длина
    b = 1.0 # ширина
    c = 0.8 # глубина
    q = power * n / (a * b * c) # Вт‚/мм**3
      
    # используем при отладке в интерпретаторе
    # debug_info = f"x={x}({type(x)}), y={y}, z={z}, a={a}, b={b}, c={c}"

    # округление
    precision = 6

    if round(float(x), precision) <= round(a, precision) and \
	   round(float(x), precision) >= round(-a, precision) and \
       round(float(y), precision) <= round(b, precision) and \
	   round(float(y), precision) >= round(-b, precision) and \
       round(float(z), precision) <= round(c, precision):
       return float(q)
    else: return 0.0


    # используем при отладке в интерпретаторе
    # raise Exception(f"Point outside bounds: {debug_info}")