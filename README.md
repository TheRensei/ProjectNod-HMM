# ProjectNod-HMM
Project Nod was a Uni project for which I created a head gesture recognition system using Hidden Markov Models in Unity.

There are 2 Scenes in the Assets/Scenes folder. 
'Gesture Capture' scene contains a scene for capturing and recording the head movement.
Press a button in play mode to trigger the gesture capture.
The gesture data will be saved into the 'HeadGestureData', outside of Assets folder.
The folder is already populated with the captured data.

The 'Training+RealTime' scene contains a scene where training and testing of HMMs can be done.
There is an empty object in the scene that has attached:
- Training Performer
- Testing Performer
- Realtime Performer

The training order is:
1. Vector Quantization
2. Simple training
3. Complex training

Testing can be done in any order.

Real-time testing requires launching a PlayMode, so pressing the 'Run Real-Time Testing' button launches it.

![](https://raw.githubusercontent.com/TheRensei/ProjectNod-HMM/master/Images/ProjectNod0.gif)
![](https://raw.githubusercontent.com/TheRensei/ProjectNod-HMM/master/Images/ProjectNod1.gif)
![](https://raw.githubusercontent.com/TheRensei/ProjectNod-HMM/master/Images/real-time.PNG)
![](https://raw.githubusercontent.com/TheRensei/ProjectNod-HMM/master/Images/gesture%20capture.PNG)


The implementation was based on this: https://arxiv.org/abs/1707.06691
