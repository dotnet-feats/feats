# feats

Yet another application to manage feature toggles!

This project requires the following:

- .net 5
- Docker
- your favorite IDE (I love vscode, but eh, Rider is still awesome)

## What is this

This project is split in 2 services :

- the management service
- the evaluation service

### Management service

The management service is where all feature toggle creation & manipulation are done:

- create a feature
- assign strategies
- publish
- archive

Still to do : an beauuutifull UI for this ;)

### Evaluation service

The evaluation service is where you need to call to know if a toggle is on or not. 
The current version only enables you to evaluate a single feature at a time, but one day i'll add 
the possibility to evaluate more than one at a time ;)

## Feature strategies

A strategy is what drives the evaluation of a feature toggle.

Here is the list of currently supported strategies:

- simple IsOn strategy : only a basic true/false flag;
- IsInList strategy:: given a list of items during strategy assignation, the evaluation will validate if the given value is in the list;

# Feature Management

Features can be organised by "Path": you can see paths like a folder or group structure. 
A feature can have the same name as another one as long as they are not in the same path.

## Create a feature

- Relative Path : `/features`
- Verb: `PUT`
- Content-Type: `application/json`
- Body: 
```json
{
    "name": "dimsum",
    "path" : "cats",
    "createdBy" : "mommy"
}
```

## Update a feature

Through this endpoint you can only update the name and path if your feature has not been published yet.

- Relative Path : `/features`
- Verb: `POST`
- Content-Type: `application/json`
- Body: 
```json
{
    "name": "dimsum",
    "path" : "cats",
    "newName": "chowmein",
    "newPath" : "cats",
    "updatedBy" : "mommy"
}
```

## Assign a strategy

Through these endpoints you can assign your strategy if your feature has not been published yet.
You can assign more than one strategy to a feature, they are currently all linked by `AND` logic.

### IsOn

- Relative Path : `/features/strategies/ison`
- Verb: `POST`
- Content-Type: `application/json`
- Body: 
```json
{
    "name": "dimsum",
    "path" : "cats",
    "assignedBy" : "mommy",
    "isOn": true
}
```

### IsInLIst

- Relative Path : `/features/strategies/isinlist`
- Verb: `POST`
- Content-Type: `application/json`
- Body: 
```json
{
    "name": "dimsum",
    "path" : "cats",
    "assignedBy" : "mommy",
    "items": ["cat", "fat"]
}
```

## Publish a feature

Publishing a feature maes it available in the evaluation service and blocks modifications to its definition.

- Relative Path : `/features/publish`
- Verb: `PUT`
- Content-Type: `application/json`
- Body: 
```json
{
    "name": "dimsum",
    "path" : "cats",
    "publishedBy" : "mommy"
}
```

## Un-publishing a feature

TODO

## Archiving a feature

TODO

## Listing Paths

Keeping in mind that one day a UI will be hooked on the management service, we need to be able to list our paths to organise
our features:

- Relative Path : `/paths`
- Verb: `GET`
- Content-Type: `application/json`
- Response: a list of paths with their feature counter
```json
[
    {
        "path": "one",
        "totalFeatures": 1
    },
    {
        "path": "one/level",
        "totalFeatures": 1
    }
]
```


## Listing Paths features

We can return all features at the root of the path and sub-paths when calling this endpoint:

- Relative Path : `/paths/features?path=one`
- Verb: `GET`
- Content-Type: `application/json`
- Response: a list of features
```json
[
    {
        "feature": "dimsum",
        "path": "one/level",
        "strategyNames": [
            "IsInList"
        ],
        "state": "Published",
        "createdOn": "2020-12-06T23:31:33.155409+00:00",
        "updatedOn": "2020-12-06T23:33:52.0215134+00:00",
        "createdBy": "mommy"
    }
]
```

# Strategy Evaluation

There is currently only one endpoint for feature evaluation on the evaluation service:

- Relative Path : `/features/?path={{UrlEncodedPath}}&name={{UrlEncodedName}}`
- Verb: `GET`
- Content-Type: `application/json`
- Response: 
```json
true | false
```


# Running locally

- Navigate to the `deploy/local` folder 
- in a terminal, run `docker-compose up`
