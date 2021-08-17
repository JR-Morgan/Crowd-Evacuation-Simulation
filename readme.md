# About This Project

This project was created as part of my Final Year Project towards my Undergraduate degree at Aston University supervised by [Dr Maria Chli](https://research.aston.ac.uk/en/persons/maria-chli).

The project delivers a complete application that allows Architects and AEC professionals to perform simulations of emergency evacuation scenarios.
The application was created using the [Unity Engine](https://unity.com/) and integrates with [Speckle](https://speckle.systems/) to provide interoperability with Autodesk Revit, Rhinoceros 3D, and more.


See [Notice.md](https://github.com/JR-Morgan/Crowd-Evacuation-Simulation/blob/master/notice.md) for attribution of third-party assets.

## Running The Software
The latest version of the software can be downloaded from the product [Releases](https://github.com/JR-Morgan/Crowd-Evacuation-Simulation/releases).

Currently, builds only target Windows. The software can be run on any system that meets [Unity's system requirements for Unity 2020 LTS](https://docs.unity3d.com/Manual/system-requirements.html).

The Unity application can be edited and compiled from source using Unity 2020.3.15f. The Results Viewer application can be compiled using Jetbrains Rider or Visual Studio.


## Simulating Evacuations

Building models can be imported through Speckle streams. Instructions for how to install and login to the Speckle manager can be found [here](https://speckle.guide/user/manager.html).
Once logged-in, Speckle streams can be imported from within the evacuation application. Instructions on how to create streams from within Autodesk Revit can be found [here](https://speckle.guide/user/revit.html).

The project provides three example buildings of varying complexity to experiment with, without the need for setting up the Speckle Manager.

Currently, the project has two different agent behavioral models, a model that follows the Social Force Model (SFM) [(Helbing et al 1995)](https://journals.aps.org/pre/abstract/10.1103/PhysRevE.51.4282) and [Reciprocal Velocity Obstacles](https://gamma.cs.unc.edu/RVO/) (RVO).
The parameters used for SFM have been calibrated using the values given by [Moussaïd et al (2009)](http://dx.doi.org/10.1098/rspb.2009.0405).
Upon completion of the simulation, a heat-map of agent density will be displayed and a graph showing a frequency plot of agent evacuation times. The total evacuation time and mean evacuation time shall also be displayed, and a JSON file containing the agent states and every time step in the simulation will be outputted to `%appdata%/../LocalLow/Jedd Morgan/Crowd Evacuation Simulation/`. 

© Aston University 2021
