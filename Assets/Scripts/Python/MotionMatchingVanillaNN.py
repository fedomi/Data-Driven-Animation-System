from sklearn.neural_network import MLPClassifier
from sklearn.neural_network import MLPRegressor
from sklearn import preprocessing
from sklearn.preprocessing import Normalizer
import numpy as np
import pickle
import time
import json
import sklearn

print(sklearn.__version__)

def readFile():
    N = 0
    file = open("F:\ProyectosUnity\VanillaMotionMatching\VanillaMotionMatching\Assets\Scripts\Python\MotionData_008.txt", "r")
    X = []
    j = 0
    for line in file:
        line_array = line.split(",")[1:]
        if(len(line_array) == 117):
            #print(len(line_array))
            line_floats = []
            for i in line_array:
                line_floats.append(float(i))
            N = len(line_floats)
            X.append(line_floats)
    return X, N


#X = [[0., 0., 0.], [1., 1., 1.], [2., 2., 2.]]
#y = [0, 1, 2]

X, N = readFile();
print(N)
#y = list(map(lambda x: x+1, np.arange(len(X))))

y = [x + 1 for x in range(0, len(X))]
x_pose = [X[i] for i in range(0, len(X)-1)]
y_pose = [X[i] for i in range(1, len(X))]


#x_scaled = preprocessing.scale(x_pose)
x_scaled = preprocessing.normalize(x_pose)
#print y
#X = np.reshape(X,(len(X), N))
#y = np.reshape(y, (-1, 1))
print (np.shape(x_pose))
print (np.shape(y_pose))
print (x_scaled)
#print (y_pose)

#print(X)
#print (y)
'''
clf = MLPClassifier(solver='adam', alpha=1e-5, max_iter = 1000, verbose = 10,
                     hidden_layer_sizes=(512, 512), random_state=1)
'''
mlreg = MLPRegressor(solver='adam', alpha=1e-5, max_iter = 10000, verbose = 10, tol = 1e-7,
                     hidden_layer_sizes=(128, 128), random_state=1, n_iter_no_change = 20)

    
#x_scaled = preprocessing.scale(x_pose)
#y_scaled = preprocessing.scale(y_pose)
    
t0 = time.time()
#clf.fit(X, y)    
mlreg.fit(x_scaled, y_pose)    

t1 = time.time()
print("Training time was:")
print(t1 - t0)
# save the model to disk
filename = 'F:\ProyectosUnity\VanillaMotionMatching\VanillaMotionMatching\Assets\Scripts\Python\MotionData008NN.pkl'
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
