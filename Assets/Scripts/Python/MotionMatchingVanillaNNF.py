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


# Returns true if phase 'p' is in the 'i'th interval out of L intervals
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
    X = []
    j = 0
    for line in file:
        line_array = line.split(",")[1:]
        #print(len(line_array))
        n = len(line_array)
        if(n == 198):
        #if(n == 174):
            #print(len(line_array))
            line_floats = []
            phase = (float(line_array[n-1]))
            standing = (float(line_array[n-2]))
            if(interval(phase,5,6) and (standing == 0)):
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
        if i < 18:
            w_pose.append(0.005*data[i])  # leg joints
        elif i < 57:
            w_pose.append(0.005*data[i]) # other joints
        elif i >= 57 and i < 67:
            w_pose.append(data[i]) # past trajectory points
        elif i >= 87 and i < 97:
            w_pose.append(data[i]) # future trajectory points
        elif i >= 97 and i < 102:
            w_pose.append(data[i])    # past trajectory directions
        elif i >= 112 and i < 117:
            w_pose.append(data[i])    # future trajectory direction
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
#x_pose = [X[i][0:57] + X[i][133:] for i in range(0, len(X)-1)]
x_pose = [X[i][0:57] + X[i][133:194] + [X[i][(len(X[i]) - 1)]] for i in range(0, len(X)-1)]
#x_pose = [X[i][0:57] + X[i][133:170] + [X[i][(len(X[i]) - 1)]] for i in range(0, len(X)-1)]
#y_pose = [X[i][0:133] for i in range(1, len(X))]
y_pose = [X[i][0:133] + [X[i][(len(X[i]) - 5)] , X[i][(len(X[i]) - 4)], X[i][(len(X[i]) - 3)]] for i in range(1, len(X))]

print("Scaling data...")

scaler = preprocessing.StandardScaler()
#scaler = preprocessing.RobustScaler()
#RobustScaler(copy=True, quantile_range=(25.0, 75.0))
scaler.fit(x_pose)


scaler_output = preprocessing.StandardScaler()
StandardScaler(copy=True, with_mean = True, with_std = True)
scaler_output.fit(y_pose)


'''
scalerSS = preprocessing.StandardScaler()
StandardScaler(copy=True, with_mean = True, with_std = True)
scalerSS.fit(x_pose)
'''

#x_scaledSS = scalerSS.transform(x_pose)
#x_scaled = preprocessing.normalize(x_pose)
x_scaled = scaler.transform(x_pose)
x_weighted = weight_dataset(x_scaled)
y_scaled = scaler_output.transform(y_pose)

print("Data scaled")

#print(y_pose[100])
#print(y_scaled[100])

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
mlreg = MLPRegressor(solver='adam', alpha=1e-5, max_iter = 1000, verbose = 10, tol = 1e-8,
                     hidden_layer_sizes=(512, 512, 512), random_state=1, n_iter_no_change = 30)

    
#x_scaled = preprocessing.scale(x_pose)
#y_scaled = preprocessing.scale(y_pose)
    
t0 = time.time()
#clf.fit(X, y)    
mlreg.fit(x_weighted, y_scaled)    

t1 = time.time()
print("Training time was:")
print(t1 - t0)
# save the model to disk
filename = 'F:\ProyectosUnity\VanillaMotionMatching\VanillaMotionMatching\Assets\Scripts\Python\MotionDataExperiment1908NN06.pkl'
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
