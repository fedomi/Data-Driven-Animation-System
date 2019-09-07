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
import math as math

def readFile():
    N = 0
    file = open("F:\ProyectosUnity\VanillaMotionMatching\VanillaMotionMatching\Assets\Scripts\Python\MotionData_011.txt", "r")
    X = []
    j = 0
    for line in file:
        line_array = line.split(",")[1:]
        if(len(line_array) == 118):
            #print(len(line_array))
            line_floats = []
            for i in line_array:
                line_floats.append(round(float(i),12))
            N = len(line_floats)
            X.append(line_floats)
    return X, N

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
            w_pose.append(0.001*data[0][i])
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

X, N = readFile();

with open("F:\ProyectosUnity\VanillaMotionMatching\VanillaMotionMatching\Assets\Scripts\Python\MotionData011NN256_WS001e10A.pkl", "rb") as input_file:
    nna = pickle.load(input_file)
with open("F:\ProyectosUnity\VanillaMotionMatching\VanillaMotionMatching\Assets\Scripts\Python\MotionData011NN256_WS001e10B.pkl", "rb") as input_file:
    nnb = pickle.load(input_file)
with open("F:\ProyectosUnity\VanillaMotionMatching\VanillaMotionMatching\Assets\Scripts\Python\MotionData011NN256_WS001e10C.pkl", "rb") as input_file:
    nnc = pickle.load(input_file)
with open("F:\ProyectosUnity\VanillaMotionMatching\VanillaMotionMatching\Assets\Scripts\Python\MotionData011NN256_WS001e10D.pkl", "rb") as input_file:
    nnd = pickle.load(input_file)

def phaseFunction(phase):
    if(phase >= 0 and phase < 2*math.pi *(1/4)):
        return nna
    elif (phase >= 2*math.pi *(1/4) and phase < 2*math.pi *(2/4)):
        return nnb
    elif (phase >= 2*math.pi *(2/4) and phase < 2*math.pi *(3/4)):
        return nnc
    elif (phase >= 2*math.pi *(3/4) and phase < 2*math.pi *(4/4)):
        return nnd
    
    
    
#print("Model loaded")    
#print("Neural Network was loaded succesfully!")
#print("Python is now waiting for input...")
x_pose = [X[i] for i in range(0, len(X)-1)]

#scaler = preprocessing.StandardScaler()
#StandardScaler(copy=True, with_mean=True, with_std=True)

scaler = preprocessing.RobustScaler()
RobustScaler(copy=True, quantile_range=(25.0, 75.0))

scaler.fit(x_pose)


#print(scaler.mean_)




while(True):
    line = input('').split(",")
    #line_floats = [float(x) for x in line[1:]]
    line_floats = [round(float(x),12) for x in line[1:]]
    line_encoded = [line_floats]
    scaled = scaler.transform(line_encoded)
    #print("Scaled")
    #print(scaled)
    #scaled = preprocessing.normalize(line_encoded)
    weighted = weight_pose(scaled)
    nn = phaseFunction(line_floats[117])
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


#L = [1.32, 2.54, 3.56]
#print(" ".join(str(x) for x in L))    
    
    
    
'''
    line = input()
    line_array = line.split(",")
    line_floats = []
    for i in line_array:
        line_floats.append(float(i))
    print(line_floats[0])    
'''

