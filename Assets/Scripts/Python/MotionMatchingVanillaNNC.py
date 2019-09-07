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
import sklearn
import math as math

print(sklearn.__version__)



def readFile():
    N = 0
    file = open("F:\ProyectosUnity\VanillaMotionMatching\VanillaMotionMatching\Assets\Scripts\Python\MotionData_011.txt", "r")
    X = []
    j = 0
    for line in file:
        line_array = line.split(",")[1:]
        if(len(line_array) == 118 and ((float(line_array[117]) >= 2 * math.pi * (0/4) and float(line_array[117]) <= 2 * math.pi * (1/4)) or (float(line_array[117]) >= 2*math.pi - 0.5 and float(line_array[117]) <=  2*math.pi)) ):
        #if(len(line_array) == 118 and ((float(line_array[117]) >= 2 * math.pi * (1/4) - 0.5 and float(line_array[117]) < 2 * math.pi * (2/4) + 0.5)) ):
            #print(line_array[117])
            line_floats = []
            for i in line_array:
                line_floats.append(round(float(i),12))
            N = len(line_floats)
            X.append(line_floats)
    return X, N


def weight_pose(data):
    w_pose = []
    #print("Weighting pose of lenght")
    #print (len(data))
    for i in range(0,len(data)):
        if i < 57:
            w_pose.append(0.001*data[i])
        else:
            w_pose.append(data[i])
    return w_pose
    
def weight_dataset(set):
    result = []
    for x in set:
        result.append(weight_pose(x))
    return result
    
#X = [[0., 0., 0.], [1., 1., 1.], [2., 2., 2.]]
#y = [0, 1, 2]

print("Loading data")
X, N = readFile();
print(N)
#y = list(map(lambda x: x+1, np.arange(len(X))))

y = [x + 1 for x in range(0, len(X))]
x_pose = [X[i] for i in range(0, len(X)-1)]
y_pose = [X[i][0:57] for i in range(1, len(X))]

print("Scaling data...")
scaler = preprocessing.RobustScaler()
RobustScaler(copy=True, quantile_range=(25.0, 75.0))
scaler.fit(x_pose)


#scaler = preprocessing.StandardScaler()
#StandardScaler(copy=True, with_mean = True, with_std = True)
#scaler.fit(x_pose)


#x_scaled = preprocessing.scale(x_pose)
#x_scaled = preprocessing.normalize(x_pose)
x_scaled = scaler.transform(x_pose)
x_weighted = weight_dataset(x_scaled)

print("Data scaled")
#print y
#X = np.reshape(X,(len(X), N))
#y = np.reshape(y, (-1, 1))
print (np.shape(x_pose))
print (np.shape(y_pose))
#print (x_scaled)
#print (pose_weighted)
#print (x_weighted[0])
#print (y_pose)

#print(X)
#print (y)
'''
clf = MLPClassifier(solver='adam', alpha=1e-5, max_iter = 1000, verbose = 10,
                     hidden_layer_sizes=(512, 512), random_state=1)
'''
mlreg = MLPRegressor(solver='adam', alpha=1e-5, max_iter = 1000, verbose = 10, tol = 1e-10,
                     hidden_layer_sizes=(256, 256), random_state=1, n_iter_no_change = 30)

    
#x_scaled = preprocessing.scale(x_pose)
#y_scaled = preprocessing.scale(y_pose)
    
t0 = time.time()
#clf.fit(X, y)    
mlreg.fit(x_weighted, y_pose)    

t1 = time.time()
print("Training time was:")
print(t1 - t0)
# save the model to disk
filename = 'F:\ProyectosUnity\VanillaMotionMatching\VanillaMotionMatching\Assets\Scripts\Python\MotionData011NN256_WS001e10A.pkl'
pickle.dump(mlreg, open(filename, 'wb'))

'''
with open("MotionMatchingNN.sav", "rb") as fpick:
    with open("MotionMatchingNN.json", "w") as fjson:
        json.dump(pickle.load(fpick), fjson)
'''
   
A = []
B = []
B.append(1.)
B.append(2.)
A.append(B)
#print(np.shape(A))

#print clf.predict([X[1], X[2]])        
