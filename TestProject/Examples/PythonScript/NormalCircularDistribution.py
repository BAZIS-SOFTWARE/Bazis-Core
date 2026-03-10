import math
def CalcPower(x, y, z, power = 1000):    
    
    if z != 0 :
        return 0.0

    diameter = 6.0 # laser spot diameter
    deviation = 2.0  
    
    # Round to 4 decimal places
    precision = 4  
    distance = math.sqrt(pow(x,2) + pow(y,2))

    if abs(round(float(distance), precision)) > diameter/2 :
       return 0.0
   
    q = math.exp(-pow(distance,2)/(2*pow(deviation, 2))) * power # W/mm**3     
   
    #debug_info = f"x={x}({type(x)}), y={y}, z={z}"    

    return round(q, precision)  

    # This will make VS show a message
    #raise Exception(f"Point outside bounds: {debug_info}")