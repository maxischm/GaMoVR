# GaMoVR

Gamification-based Modeling Learning in Virtual Reality

# Gamification Engine Setup and Configuration

1. Start the Gamification Engine following the instructions in their [Wiki](https://github.com/smartcommunitylab/smartcampus.gamification/wiki/Setup).

Note: You need to add the following line to the `gamification.env` file:

```
SPRING_DATA_MONGODB_URL=mongodb://gamification-mongo:27017/gamification
```

2. Run the configuration script found under `./Gamification-Engine-Settings/config.py` with the base URL of the gamification engine as argument. E.g. if the engine is running on localhost, run the script with the following command.

```
python3 ./Gamification-Engine-Settings/config.py localhost:8010
```

    The script will prompt you to enter the username and password entered during the gamification engine setup.

3. In the Unity project open the file `Assets/ConfigurationFiles/GamificationEngineConfiguration.asset` in the Inspector. Enter the base URL, username, password, and GameId (if changed from the default value in the configuration script run in Step 2).
