# GaMoVR - Gamification-based Modeling Learning in Virtual Reality

GaMoVR is a gamified learning environment to practice modelling UML class diagrams in VR. The game contains two ways to explore UML modelling.

A short demo video can be found [here](https://www.youtube.com/watch?v=NOQ4m_r61kA&t=1s) and a full playthrough of the complete game [here](https://drive.google.com/file/d/1_Q5-eBVNxlS7LaIrv3mDJa8Y7pvtf6fp/view?usp=sharing).

This project was presented at the MODELS2022 conference and published [here](https://doi.org/10.1145/3550356.3559088).

Since then, the project was reworked to be made open-source. This included reworking the different 3D models, changing sounds, sprites, etc. which is why the project provided in this repository differs in looks and sounds from the videos or the screenshots of the MODELS2022 publication.

A more detailed description of the game and project structure can be found in the [Wiki](https://github.com/maxischm/GaMoVR/wiki/Game-and-Project-Structure).

## Hangman

The main activity for practicing modeling is by playing the Hangman game based on the [word guessing game](<https://en.wikipedia.org/wiki/Hangman_(game)>). There, the player has to solve different modeling tasks, e.g. creating models given a description without making more mistakes than they have lives or within a certain time frame while the Hangman is built.

## Multi-Viewpoint Modeling

The learning environment also provides an environment where players can explore the relationship between a UML class diagram of a spaceship and a 3D representation of the same spaceship (an instance of the class diagram). These two views (class diagram and 3D representation) are synchronized by the application whenever the player makes changes to either view.

## Setup

To run the game, follow the instruction in the [Wiki](https://github.com/maxischm/GaMoVR/wiki/Setup).

# License

This project is licensed under the MIT License. See the [LICENSE](./LICENSE) file for the full license text.

Third-parts components used within this project can be found in designated folders with the respective licenses located there as well.
