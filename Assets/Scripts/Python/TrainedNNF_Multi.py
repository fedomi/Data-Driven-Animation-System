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
    tol = 0.5
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
    file = open("F:\ProyectosUnity\VanillaMotionMatching\VanillaMotionMatching\Assets\Scripts\Python\MotionData_Experiment01.txt", "r")
    X0 = []
    X1 = []
    X2 = []
    X3 = []
    
    j = 0
    for line in file:
        line_array = line.split(",")[1:]
        n = len(line_array)
        if(n == 194):
            #print(len(line_array))
            line_floats = []
            phase = (float(line_array[n-1]))
            for i in line_array:
                line_floats.append(round(float(i),12))
            N = len(line_floats)
            if(interval(phase,0,4)):
                X0.append(line_floats)
            if(interval(phase,1,4)):
                X1.append(line_floats)
            if(interval(phase,2,4)):
                X2.append(line_floats)
            if(interval(phase,3,4)):
                X3.append(line_floats)
    return X0,X1,X2,X3, N

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
        if i < 57:
            w_pose.append(0.005*data[0][i])
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
X0,X1,X2,X3, N = readFile();

# Loading models
with open("F:\ProyectosUnity\VanillaMotionMatching\VanillaMotionMatching\Assets\Scripts\Python\MotionDataExperiment01NN01.pkl", "rb") as input_file:
    nn0 = pickle.load(input_file)
with open("F:\ProyectosUnity\VanillaMotionMatching\VanillaMotionMatching\Assets\Scripts\Python\MotionDataExperiment01NN02.pkl", "rb") as input_file:
    nn1 = pickle.load(input_file)
with open("F:\ProyectosUnity\VanillaMotionMatching\VanillaMotionMatching\Assets\Scripts\Python\MotionDataExperiment01NN03.pkl", "rb") as input_file:
    nn2 = pickle.load(input_file)
with open("F:\ProyectosUnity\VanillaMotionMatching\VanillaMotionMatching\Assets\Scripts\Python\MotionDataExperiment01NN04.pkl", "rb") as input_file:
    nn3 = pickle.load(input_file)
    
    
    
#print("Neural Network was loaded succesfully!")
#print("Python is now waiting for input...")

# Building scalers
x0_pose = [X0[i][0:57] + X0[i][133:] for i in range(0, len(X0)-1)]
scaler0 = preprocessing.RobustScaler()
RobustScaler(copy=True, quantile_range=(25.0, 75.0))
scaler0.fit(x0_pose)

x1_pose = [X1[i][0:57] + X1[i][133:] for i in range(0, len(X1)-1)]
scaler1 = preprocessing.RobustScaler()
RobustScaler(copy=True, quantile_range=(25.0, 75.0))
scaler1.fit(x1_pose)

x2_pose = [X2[i][0:57] + X2[i][133:] for i in range(0, len(X2)-1)]
scaler2 = preprocessing.RobustScaler()
RobustScaler(copy=True, quantile_range=(25.0, 75.0))
scaler2.fit(x2_pose)

x3_pose = [X3[i][0:57] + X3[i][133:] for i in range(0, len(X3)-1)]
scaler3 = preprocessing.RobustScaler()
RobustScaler(copy=True, quantile_range=(25.0, 75.0))
scaler3.fit(x3_pose)


#print(scaler.mean_)



# Main loop
while(True):
    line = input('').split(",")
    #line_floats = [float(x) for x in line[1:]]
    line_floats = [round(float(x),12) for x in line[1:58]] + [round(float(x),12) for x in line[134:]]
    #line_cut = line_floats[0:57] + line_floats[133:]
    l = len(line_floats)
    phase = line_floats[l-1]
    i = (math.pi)/2
    
    if(phase >= 0 and phase < i):
        nn = nn0
        scaler = scaler0
    elif (phase >= i and phase < 2*i):
        nn = nn1
        scaler = scaler1
    elif (phase >= 2*i and phase < 3*i):
        nn = nn2
        scaler = scaler2
    else:
        nn = nn3
        scaler = scaler3
    
    
    
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
    result = nn.predict(line_array)
    #print ("Output")
    print(" ".join(str(round(float(x),12)) for x in result[0][0:]))




