# Moovya Integration
[![openupm](https://img.shields.io/npm/v/com.evomo.gamehub?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.evomo.gamehub/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)


Plugin used to integrate multiple games inside the Moovya app

This package requires the [MotionAI Package](https://github.com/Evomo/unityMotionAIPlugin) to make use of it. 

## Install 
We recommend using the OpenUPM to install it by running:
```
openupm add com.evomo.gamehub
```


But if you wish to use it without  installing an external dependency you can add the scope and dependencies using: 


```

{
  "scopedRegistries": [
    {
      "name": "package.openupm.com",
      "url": "https://package.openupm.com",
      "scopes": [
        "com.solidalloy.type.references",
        "com.evomo.motionai",
        "com.evomo.gamehub",
        "com.openupm"
      ]
    }
  ],
  "dependencies": {
    "com.evomo.motionai": "[NewestVersion found on Github]"
  }
}
```

# How does it work ? 
The Moovya app has a CI/CD manager which will build and install games that are added as packages. This plugin ensures the framework needed for it is correctly implemented and presents some useful functions to communicate game state variables to the overlay found in the application.

# How to integrate the games? 

## Requirements

-   Game code needs to be in its own [assembly definition](https://docs.unity3d.com/Manual/ScriptCompilationAssemblyDefinitionFiles.html) and namespace.
- The game needs to be converted into a [package](https://docs.unity3d.com/Manual/CustomPackages.html)
- A [GameHubGame](https://github.com/Evomo/GameHubPlugin/blob/master/Assets/GamehubPlugin/Core/DataObjects/GameHubGame.cs) Scriptable Object needs to be created. 
- The package.json needs to have an "EvomoGame" type. 
- A [URP](https://docs.unity3d.com/Packages/com.unity.render-pipelines.universal@7.1/manual/universalrp-asset.html) should be specified, if not the default one is used.
# Example package.json 

```

{
  "name": "games.developer.gamename",
  "version": "1.0.0",
  "displayName": "Example Game",
  "description": "Example game ",
  "type": "EvomoGame",
  "dependencies": {
    "com.evomo.motionai": "1.2.12"
  }
}


```


# [Public API](https://github.com/Evomo/GameHubPlugin/blob/master/Assets/GamehubPlugin/Core/GameHubManager.cs#L267)


The public API can be accessed via the GameHubManager Singleton, it's used to control the lifecycle of the game session(Start, Pause and End) and send variables (such as lives, score, coins) to the overlay. 


