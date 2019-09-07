from sklearn.neural_network import MLPClassifier
from sklearn.neural_network import MLPRegressor
from sklearn import preprocessing
from sklearn.preprocessing import Normalizer
from sklearn.preprocessing import StandardScaler
from sklearn.preprocessing import RobustScaler
import numpy as np
import pickle
import time
import json
import fileinput
import sys
import math


def interval(p, i, L):
    interval_length = (2*math.pi)/L
    tol = 0.2
    if((i != 0) and (i != L-1)):
        if((p >= (i * interval_length) - tol) and (p <= ((i+1)*interval_length) + tol)):
            return True
        else: return False
    elif (i == 0):
        if((p >= 0 and (p <= (interval_length) + tol)) or (p >= 2*math.pi - tol)):
            return True
        else: return False
    elif (i == L-1):
        if((p >= (i * interval_length) - tol and (p <= 2*math.pi)) or (p <= tol)):
            return True
        else: return False
    else: return False

def readFile():
    N = 0
    file = open("F:\ProyectosUnity\VanillaMotionMatching\VanillaMotionMatching\Assets\Scripts\Python\MotionData_Experiment0708.txt", "r")
    X0 = []
    X1 = []
    X2 = []
    X3 = []
    X4 = []
    X5 = []
    
    j = 0
    for line in file:
        line_array = line.split(",")[1:]
        n = len(line_array)
        #print(n)
        if(n == 198):
            #print(len(line_array))
            line_floats = []
            phase = (float(line_array[n-1]))
            for i in line_array:
                line_floats.append(round(float(i),12))
            N = len(line_floats)
            if(interval(phase,0,6)):
                X0.append(line_floats)
            if(interval(phase,1,6)):
                X1.append(line_floats)
            if(interval(phase,2,6)):
                X2.append(line_floats)
            if(interval(phase,3,6)):
                X3.append(line_floats)
            if(interval(phase,4,6)):
                X4.append(line_floats)
            if(interval(phase,5,6)):
                X5.append(line_floats)
    return X0,X1,X2,X3,X4,X5, N

def round_float(x):
    return np.round(x,4)

def first(the_iterable, condition = lambda x: True):
    for i in the_iterable:
        if (condition(i)):
            return i
    
    
def weight_pose(data):
    w_pose = []
    #print("Weighting pose of lenght")
    #print (len(data[0]))
    for i in range(0,len(data[0])):
        if i < 18:
            w_pose.append(0.005*data[0][i])  # leg joints
        elif i < 57:
            w_pose.append(0.005*data[0][i]) # other joints
        elif i >= 57 and i < 67:
            w_pose.append(data[0][i])
        elif i >= 87 and i < 97:
            w_pose.append(data[0][i])
        elif i >= 97 and i < 102:
            w_pose.append(data[0][i])
        elif i >= 112 and i < 117:
            w_pose.append(data[0][i])
        else:
            w_pose.append(data[0][i])
    return [w_pose]
    
def weight_dataset(set):
    result = []
    for x in set:
        result.append(weight_pose(x))
    return result
    

    
    
print("Loading the ML model.")
#X, N = readFile();
#x_pose = [X[i] for i in range(0, len(X)-1)]
#print (np.shape(x_pose[1]))

# Loading data
X0,X1,X2,X3,X4,X5, N = readFile();

# Loading models
with open("F:\ProyectosUnity\VanillaMotionMatching\VanillaMotionMatching\Assets\Scripts\Python\MotionDataExperiment1908NN01.pkl", "rb") as input_file:
    nn0 = pickle.load(input_file)
with open("F:\ProyectosUnity\VanillaMotionMatching\VanillaMotionMatching\Assets\Scripts\Python\MotionDataExperiment1908NN02.pkl", "rb") as input_file:
    nn1 = pickle.load(input_file)
with open("F:\ProyectosUnity\VanillaMotionMatching\VanillaMotionMatching\Assets\Scripts\Python\MotionDataExperiment1908NN03.pkl", "rb") as input_file:
    nn2 = pickle.load(input_file)
with open("F:\ProyectosUnity\VanillaMotionMatching\VanillaMotionMatching\Assets\Scripts\Python\MotionDataExperiment1908NN04.pkl", "rb") as input_file:
    nn3 = pickle.load(input_file)
with open("F:\ProyectosUnity\VanillaMotionMatching\VanillaMotionMatching\Assets\Scripts\Python\MotionDataExperiment1908NN05.pkl", "rb") as input_file:
    nn4 = pickle.load(input_file)
with open("F:\ProyectosUnity\VanillaMotionMatching\VanillaMotionMatching\Assets\Scripts\Python\MotionDataExperiment1908NN06.pkl", "rb") as input_file:
    nn5 = pickle.load(input_file)
    
    
    


# Building scalers
x0_pose = [X0[i][0:57] + X0[i][133:194] + [X0[i][(len(X0[i]) - 1)]] for i in range(0, len(X0)-1)]
#x0_pose = [X0[i][0:57] + X0[i][133:170] + [X0[i][(len(X0[i]) - 1)]] for i in range(0, len(X0)-1)]
y0_pose = [X0[i][0:133] + [X0[i][(len(X0[i]) - 5)] , X0[i][(len(X0[i]) - 4)], X0[i][(len(X0[i]) - 3)]] for i in range(1, len(X0))]
#scaler0 = preprocessing.RobustScaler()
#RobustScaler(copy=True, quantile_range=(25.0, 75.0))
scaler0 = preprocessing.StandardScaler()
scaler0_out = preprocessing.StandardScaler()
scaler0.fit(x0_pose)
scaler0_out.fit(y0_pose)

x1_pose = [X1[i][0:57] + X1[i][133:194] + [X1[i][(len(X1[i]) - 1)]] for i in range(0, len(X1)-1)]
#x1_pose = [X1[i][0:57] + X1[i][133:170] + [X1[i][(len(X1[i]) - 1)]] for i in range(0, len(X1)-1)]
y1_pose = [X1[i][0:133] + [X1[i][(len(X1[i]) - 5)] , X1[i][(len(X1[i]) - 4)], X1[i][(len(X1[i]) - 3)]] for i in range(1, len(X1))]
#scaler1 = preprocessing.RobustScaler()
#RobustScaler(copy=True, quantile_range=(25.0, 75.0))
scaler1 = preprocessing.StandardScaler()
scaler1.fit(x1_pose)
scaler1_out = preprocessing.StandardScaler()
scaler1_out.fit(y1_pose)



x2_pose = [X2[i][0:57] + X2[i][133:194] + [X2[i][(len(X2[i]) - 1)]] for i in range(0, len(X2)-1)]
#x2_pose = [X2[i][0:57] + X2[i][133:170] + [X2[i][(len(X2[i]) - 1)]] for i in range(0, len(X2)-1)]
y2_pose = [X2[i][0:133] + [X2[i][(len(X2[i]) - 5)] , X2[i][(len(X2[i]) - 4)], X2[i][(len(X2[i]) - 3)]] for i in range(1, len(X2))]
#scaler2 = preprocessing.RobustScaler()
#RobustScaler(copy=True, quantile_range=(25.0, 75.0))
scaler2 = preprocessing.StandardScaler()
scaler2.fit(x2_pose)
scaler2_out = preprocessing.StandardScaler()
scaler2_out.fit(y2_pose)


x3_pose = [X3[i][0:57] + X3[i][133:194] + [X3[i][(len(X3[i]) - 1)]]for i in range(0, len(X3)-1)]
#x3_pose = [X3[i][0:57] + X3[i][133:170] + [X3[i][(len(X3[i]) - 1)]]for i in range(0, len(X3)-1)]
y3_pose = [X3[i][0:133] + [X3[i][(len(X3[i]) - 5)] , X3[i][(len(X3[i]) - 4)], X3[i][(len(X3[i]) - 3)]] for i in range(1, len(X3))]
#scaler3 = preprocessing.RobustScaler()
#RobustScaler(copy=True, quantile_range=(25.0, 75.0))
scaler3 = preprocessing.StandardScaler()
scaler3.fit(x3_pose)
scaler3_out = preprocessing.StandardScaler()
scaler3_out.fit(y3_pose)


x4_pose = [X4[i][0:57] + X4[i][133:194] + [X4[i][(len(X4[i]) - 1)]] for i in range(0, len(X4)-1)]
#x4_pose = [X4[i][0:57] + X4[i][133:170] + [X4[i][(len(X4[i]) - 1)]] for i in range(0, len(X4)-1)]
y4_pose = [X4[i][0:133] + [X4[i][(len(X4[i]) - 5)] , X4[i][(len(X4[i]) - 4)], X4[i][(len(X4[i]) - 3)]] for i in range(1, len(X4))]
#scaler4 = preprocessing.RobustScaler()
#RobustScaler(copy=True, quantile_range=(25.0, 75.0))
scaler4 = preprocessing.StandardScaler()
scaler4.fit(x4_pose)
scaler4_out = preprocessing.StandardScaler()
scaler4_out.fit(y4_pose)


x5_pose = [X5[i][0:57] + X5[i][133:194] + [X5[i][(len(X5[i]) - 1)]] for i in range(0, len(X5)-1)]
#x5_pose = [X5[i][0:57] + X5[i][133:170] + [X5[i][(len(X5[i]) - 1)]] for i in range(0, len(X5)-1)]
y5_pose = [X5[i][0:133] + [X5[i][(len(X5[i]) - 5)] , X5[i][(len(X5[i]) - 4)], X5[i][(len(X5[i]) - 3)]] for i in range(1, len(X5))]
#scaler4 = preprocessing.RobustScaler()
#RobustScaler(copy=True, quantile_range=(25.0, 75.0))
scaler5 = preprocessing.StandardScaler()
scaler5.fit(x5_pose)
scaler5_out = preprocessing.StandardScaler()
scaler5_out.fit(y5_pose)

#print(scaler.mean_)

#print("Neural Network was loaded succesfully!")
#print("Python is now waiting for input...")

# Main loop
while(True):
    line = input('').split(",")
    #line_floats = [float(x) for x in line[1:]]
    #t0 = time.time()
    #print(len(line))
    line_floats = [round(float(x),12) for x in line[1:58]] + [round(float(x),12) for x in line[134:195] + [round(float(line[198]),12)]]
    #line_floats = [round(float(x),12) for x in line[1:58]] + [round(float(x),12) for x in line[134:171] + [round(float(line[174]),12)]]
    #line_cut = line_floats[0:57] + line_floats[133:]
    l = len(line_floats)
    phase = line_floats[l-1]
    i = 2*(math.pi)/6
    
    if(phase >= 0 and phase < i):
        nn = nn0
        scaler = scaler0
        scaler_out = scaler0_out
    elif (phase >= i and phase < 2*i):
        nn = nn1
        scaler = scaler1
        scaler_out = scaler1_out
    elif (phase >= 2*i and phase < 3*i):
        nn = nn2
        scaler = scaler2
        scaler_out = scaler2_out
    elif (phase >= 3*i and phase < 4*i):
        nn = nn3
        scaler = scaler3
        scaler_out = scaler3_out
    elif (phase >= 4*i and phase < 5*i):
        nn = nn4
        scaler = scaler4
        scaler_out = scaler4_out
    elif (phase >= 5*i and phase < 6*i):
        nn = nn5
        scaler = scaler5
        scaler_out = scaler5_out
    else: 
        nn = nn4
        scaler = scaler4
        scaler_out = scaler4_out
    
    
    
    line_encoded = [line_floats]
    scaled = scaler.transform(line_encoded)
    #print("Scaled")
    #print(scaled)
    #scaled = preprocessing.normalize(line_encoded)
    weighted = weight_pose(scaled)
    #print("Weighted")
    #print (weighted)
    #scaled = preprocessing.scale(line_floats)
    #print(len(line_floats))
    #line_array = np.array(line_encoded[0]).reshape(1,-1)
    #line_array = np.array(scaled[0]).reshape(1,-1)
    line_array = np.array(weighted[0]).reshape(1,-1)
    result_scaled = nn.predict(line_array)
    result = scaler_out.inverse_transform(result_scaled)
    #t1 = time.time()
    #print("Response time was:")
    #print(t1 - t0)
    #print ("Output")
    print(" ".join(str(round(float(x),12)) for x in result[0][0:]))




