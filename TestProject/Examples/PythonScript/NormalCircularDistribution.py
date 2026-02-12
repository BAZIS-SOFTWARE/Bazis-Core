import math
def CalcPower(x, y, z, power = 1000):    
    
    if z != 0 :
        return 0.0

    diameter = 6.0 # диаметр лазерного пятна
    deviation = 2.0  
    
    # Округляем до 4 знаков после запятой
    precision = 4  
    distance = math.sqrt(pow(x,2) + pow(y,2))

    if abs(round(float(distance), precision)) > diameter/2 :
       return 0.0
   
    q = math.exp(-pow(distance,2)/(2*pow(deviation, 2))) * power # Вт/мм**3     
   
    #debug_info = f"x={x}({type(x)}), y={y}, z={z}"    

    return round(q, precision)  

    # Это заставит VS показать сообщение
    #raise Exception(f"Point outside bounds: {debug_info}")