from sklearn.neural_network import MLPClassifier
from sklearn.neural_network import MLPRegressor
from sklearn import preprocessing
from sklearn.preprocessing import Normalizer
import numpy as np
import pickle
import time
import json
import fileinput
import sys

def readFile():
    N = 0
    file = open("F:\ProyectosUnity\VanillaMotionMatching\VanillaMotionMatching\Assets\Scripts\Python\ReducedTrajectoryData.txt", "r")
    X = []
    j = 0
    for line in file:
        if(j > 100):
            line_array = line.split(",")
            line_floats = []
            for i in line_array:
                line_floats.append(float(i))
            N = len(line_floats)
            X.append(line_floats)
        j += 1
        if (j > 28500):
            return X, N

def round_float(x):
    return np.round(x,4)

def first(the_iterable, condition = lambda x: True):
    for i in the_iterable:
        if (condition(i)):
            return i
    
print("Loading the ML model.")
#X, N = readFile();
#x_pose = [X[i] for i in range(0, len(X)-1)]
#print (np.shape(x_pose[1]))

with open("F:\ProyectosUnity\VanillaMotionMatching\VanillaMotionMatching\Assets\Scripts\Python\MotionData008NN.pkl", "rb") as input_file:
    nn = pickle.load(input_file)
    
#print("Neural Network was loaded succesfully!")
#print("Python is now waiting for input...")


while(True):
    line = input('').split(",")
    #line_floats = [float(x) for x in line[1:]]
    line_floats = [float(x) for x in line[1:]]
    line_encoded = [line_floats]
    scaled = preprocessing.normalize(line_encoded)
    #scaled = preprocessing.scale(line_floats)
    #print(len(line_floats))
    line_array = np.array(scaled[0]).reshape(1,-1)
    result = nn.predict(line_array)
    print(" ".join(str(round(float(x),4)) for x in result[0][0:]))


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

