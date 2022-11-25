import sys
import requests
from requests.auth import HTTPBasicAuth
import getpass

if (len(sys.argv) == 1):
    exit('Base URL argument missing.')
elif (len(sys.argv) > 2):
    exit('Too many arguments.')

baseUrl = str(sys.argv[1])

url = f'http://{baseUrl}/gamification'

basic = HTTPBasicAuth(input('Enter the username: '),
                      getpass.getpass('Enter the password: '))

# Set up the game

gameDefinition = {
    "id": 'gamovr',
    "name": "GaMoVR",
    "actions": ['unlockElement', 'executedWrongMove', 'finishGame', 'startNewLevel'],
    "pointConcept": [
        {
            "id": "1",
            "name": "xp",
            "score": 0,
            "periods": {}
        },
        {
            "id": "1",
            "name": "lives",
            "score": 0,
            "periods": {}
        }
    ]
}

response = requests.post(f'{url}/model/game', json=gameDefinition, auth=basic)

# Set up rules

ruleDefinitions = [{
    "id": "startNewLevel",
    "name": "startNewLevel",
            "content": """
package eu.trentorise.game.model
import java.util.ArrayList;
declare AlreadyLevelUp
end

declare game
   level : Double
   won: Boolean
   lives: Double
end


rule "startNewLevel"
when
      Action( id == 'startNewLevel')
       InputData(
    	$initialLives : data['initialLives'],
        $currentLevelId: data['levelID']
    )

    $lives : PointConcept(name == "lives")
    $customData: CustomData()

    not AlreadyLevelUp()
then
double lives = (double) $initialLives;
double level = (double) $currentLevelId;

modify($lives) { setScore(lives); } // update the counter   

 game g = new game(level,false,lives);
 if((ArrayList<game>)$customData.get("playedGames") == null){
    //add first element
  ArrayList<game> games  = new ArrayList<game>();
  games.add(g); 
  $customData.put("playedGames", games);
 }
else {
// some games already played
ArrayList<game> games = (ArrayList<game>)$customData.get("playedGames");
ArrayList<game> newGames = new ArrayList<game>();
System.out.println("giochi prima: "+games);
Boolean found = false;
for(int i=0;i<games.size();i++)
{
Map currentGame = (Map)games.get(i);
Double currentLevel = (Double)currentGame.get("level");
Boolean currentWon = (Boolean)currentGame.get("won");
Double currentLives = (Double)currentGame.get("lives");

if(currentLevel == level)
{
//update the found game
 game g1 = new game(level,false,lives);
 newGames.add(g1);
 found = true;
}
else {
game g1 = new game(currentLevel,currentWon,currentLives); 
 newGames.add(g1);
}
} // end for cycle

if(!found){
 System.out.println("non trovato devo aggiungerlo");
 game g1 = new game(level,false,lives);
 newGames.add(g1);

}
System.out.println("giochi dopo: "+newGames);
$customData.put("playedGames", newGames);


}

update($customData);
insert(new AlreadyLevelUp());
end
"""
},
    {
    "id": "finishGame",
    "name": "finishGame",
    "content": """
package eu.trentorise.game.model
import java.util.ArrayList;
declare AlreadyLevelUp
end

declare game
   level : Double
   won: Boolean
   lives: Double
end

rule "finishGame"
when
      Action( id == 'finishGame')
 InputData(
    	$newXP : data['xp'],
        $currentLevelId: data['levelID'],
        $won: data['won'],
        $lives: data['remainingLivesUponCompletion']
    )
    $xp: PointConcept(name=="xp")
    $customData: CustomData()
    not AlreadyLevelUp()

then
double newXP = (double)$newXP;
$xp.setScore($xp.getScore()+newXP);

double level = (double)$currentLevelId;
Boolean won = (Boolean)$won;
double lives = (double)$lives;

System.out.println("VITE: "+lives);


 game g = new game(level,won,lives);
System.out.println("GIOCO: "+g);
 if((ArrayList<game>)$customData.get("playedGames") == null){
    //add first element
  ArrayList<game> games  = new ArrayList<game>();
  games.add(g); 
  $customData.put("playedGames", games);
 }
else {
// some games already played
ArrayList<game> games = (ArrayList<game>)$customData.get("playedGames");
ArrayList<game> newGames = new ArrayList<game>();
System.out.println("giochi prima: "+games);
Boolean found = false;
for(int i=0;i<games.size();i++)
{
Map currentGame = (Map)games.get(i);
double currentLevel = (double)currentGame.get("level");
Boolean currentWon = (Boolean)currentGame.get("won");
double currentLives = (double)currentGame.get("lives");
System.out.println("current Level: "+currentLevel);
System.out.println("level in memoria: "+level);

if(currentLevel == level)
{
//update the found game
game g1 = new game(level,won,lives);
newGames.add(g1);
found = true;
}
else {
game g1 = new game(currentLevel,currentWon,currentLives); 
newGames.add(g1);
}
} // end for cycle

if(!found){
 System.out.println("non trovato devo aggiungerlo");
game g1 = new game(level,won,lives);
newGames.add(g1);

}
System.out.println("giochi dopo: "+newGames);
$customData.put("playedGames", newGames);


}

update($customData);


insert(new AlreadyLevelUp());
end
"""
},
    {
    "id": "reduceLives",
    "name": "reduceLives",
    "content": """
package eu.trentorise.game.model
declare AlreadyLevelUp
end

rule "reduceLives"
when
      Action( id == 'executedWrongMove')

    $lives : PointConcept(name == "lives")

    not AlreadyLevelUp()
then
$lives.setScore($lives.getScore()-1.0);

insert(new AlreadyLevelUp());
end
    """
},
    {
    "id": "unlockElement",
    "name": "unlockElement",
    "content": """
package eu.trentorise.game.model
import java.util.ArrayList;
declare AlreadyLevelUp
end

declare UnlockedElement
   key : String
   name: String
end


rule "unlockElement"
when
      Action( id == 'unlockElement')
       InputData(
         	$key : data['key'],
                $name: data['name']
    )


    $customData: CustomData()

    not AlreadyLevelUp()
then
String key = (String)$key;
String name = (String)$name;

UnlockedElement ue = new UnlockedElement(key,name);

 if((ArrayList<UnlockedElement>)$customData.get("unlockedElements") == null){
    // no unlocked elements - add the first element
  ArrayList<UnlockedElement> ues  = new ArrayList<UnlockedElement>();
  ues.add(ue); 
  $customData.put("unlockedElements", ues);
 }
else {
// some elements already unlocked
ArrayList<UnlockedElement> elements = (ArrayList<UnlockedElement>)$customData.get("unlockedElements");
ArrayList<UnlockedElement> newElements = new ArrayList<UnlockedElement>();
System.out.println("elementi prima: "+elements);
Boolean found = false;
for(int i=0;i<elements.size();i++)
{
Map currentElement = (Map)elements.get(i);
String currentKey = (String)currentElement.get("key");
String currentName = (String)currentElement.get("name");
System.out.println("TOTO: "+currentKey);
System.out.println("KEY: "+key);


if(currentKey.equalsIgnoreCase(key))
{
//update the found element
System.out.println("EEEEEEEEEEEEE");
 UnlockedElement e1 = new UnlockedElement(key,name);
 newElements.add(e1);
 found = true;
}
else {
UnlockedElement e1 = new UnlockedElement(currentKey,currentName); 
 newElements.add(e1);
}
} // end for cycle

if(!found){
 System.out.println("non trovato devo aggiungerlo");
 UnlockedElement e1 = new UnlockedElement(key,name);
 newElements.add(e1);

}
System.out.println("elementi dopo: "+newElements);
$customData.put("unlockedElements", newElements);

}

update($customData);
insert(new AlreadyLevelUp());
end
"""
}
]

for ruleDefinition in ruleDefinitions:
    response = requests.post(
        f'{url}/model/game/gamovr/rule', json=ruleDefinition, auth=basic)
