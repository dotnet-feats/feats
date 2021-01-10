![Azure DevOps coverage](https://img.shields.io/azure-devops/coverage/dotnet-feats/feats/1)
[![Build Status](https://dev.azure.com/dotnet-feats/feats/_apis/build/status/dotnet-feats.feats?branchName=main)](https://dev.azure.com/dotnet-feats/feats/_build/latest?definitionId=1&branchName=main)
[![Stage](https://img.shields.io/badge/Stage-alpha-blue)]()

# feats

Yet another application to manage feature toggles!

If you want to play around the code for this project, you will require the following:

- .net 5
- Docker
- your favorite IDE (I love vscode, but eh, Rider is still awesome)

## What is this

This project is split in 2 services :

- the management service
- the evaluation service

As for database/persistence, it uses the awesome [EventStore](https://www.eventstore.com/).

### Management service

The management service is where all feature toggle creation & manipulation are done:

- create a feature
- assign strategies
- publish
- archive

Still to do : a beauuutifull UI for this, coming up, one day ;)

### Evaluation service

The evaluation service is where you need to call to know if a toggle is on or not. Everytime a call is made to this endpoint we increase a Counter Metric for Prometheus. We don't push our metrics, you have to scrape them.

The current version only enables you to evaluate a single feature at a time: but you can send all the strategy values at once.

## Feature strategies

A strategy is what drives the evaluation of a feature toggle.

Here is the list of currently supported strategies:

- simple IsOn strategy : only a basic true/false flag;
- IsInList strategy: given a list of items during strategy assignation, the evaluation will validate if the given value is in the list;
- IsBefore: date-time related evaluation, evaluates if the given date is before the one set in the strategy (all checks are exclusive)
- IsAfter: date-time related evaluation, evaluates if the given date is after the one set in the strategy (all checks are exclusive)
- IsGreaterThan: given a number (can be a decimal, double, who cares, ah eh, no exponential format though, give me a little break here....), evaluates if the given value is greater (exclusive values as well: > only)
- IsLowerThan: given a number (same as greater rule), evaluates if the given value is lower (exclusive values as well: < only)

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

> You can assign more than one strategy to a feature, they are currently all linked by `AND` logic.

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

### IsGreaterThan

- Relative Path : `/features/strategies/isgreaterthan`
- Verb: `POST`
- Content-Type: `application/json`
- Body: 
```json
{
    "name": "dimsum",
    "path" : "cats",
    "assignedBy" : "mommy",
    "value": 45678.45678
}
```

> `value` is a double

### IsLowerThan

- Relative Path : `/features/strategies/islowerthan`
- Verb: `POST`
- Content-Type: `application/json`
- Body: 
```json
{
    "name": "dimsum",
    "path" : "cats",
    "assignedBy" : "mommy",
    "value": 45678.45678
}
```

> `value` is a double

### IsBefore

- Relative Path : `/features/strategies/isbefore`
- Verb: `POST`
- Content-Type: `application/json`
- Body: 
```json
{
    "name": "dimsum",
    "path" : "cats",
    "assignedBy" : "mommy",
    "value": "2020-12-21T21:38:46Z"
}
```

> `value` is a iso 8601 date format, the date is **EXCLUSIVE** 

### IsAfter

- Relative Path : `/features/strategies/isafter`
- Verb: `POST`
- Content-Type: `application/json`
- Body: 
```json
{
    "name": "dimsum",
    "path" : "cats",
    "assignedBy" : "mommy",
    "value": "2020-12-21T21:38:46Z"
}
```

> `value` is a iso 8601 date format, the date is **EXCLUSIVE**

## Publish a feature

Publishing a feature makes it available in the evaluation service and blocks modifications to its definition.

- Relative Path : `/features/publish`
- Verb: `POST`
- Content-Type: `application/json`
- Body: 
```json
{
    "name": "dimsum",
    "path" : "cats",
    "publishedBy" : "mommy"
}
```

## Archiving a feature

Archiving a feature removes it from the evaluation service.

- Relative Path : `/features/archive`
- Verb: `POST`
- Content-Type: `application/json`
- Body: 
```json
{
    "name": "dimsum",
    "path" : "cats",
    "archivedBy" : "mommy"
}
```

## Listing Paths

Keeping in mind that one day a UI will be hooked on the management service, we need to be able to list our paths to organise
our features:

- Relative Path : `/paths`
- Verb: `GET`
- Content-Type: `application/json`
- Response: a list of paths with their feature counter (currently no filtering on status)
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

We can return all features at the root of the path and sub-paths when calling this endpoint (currently no filtering on status):

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

## Feature details

- Relative Path : `/features?path={{UrlEncodedPath}}&name={{UrlEncodedName}}`
- Verb: `GET`
- Content-Type: `application/json`
- Response: a list of features
```json
{
    "feature": "dimsum",
    "path": "one/level",
    "strategies": [
        {
            "name": "IsOn",
            "value": "{\"IsOn\":true}"
        },
        {
            "name": "IsInList",
            "value": "{\"ListName\":null,\"Items\":[\"cat\",\"fat\"]}"
        }
    ],
    "state": "Published",
    "createdOn": "2020-12-06T23:31:33.155409+00:00",
    "updatedOn": "2020-12-06T23:33:52.0215134+00:00",
    "createdBy": "moua"
}
```

# Strategy Evaluation

There is currently only one endpoint for feature evaluation on the evaluation service:

- Relative Path : `/features?path={{UrlEncodedPath}}&name={{UrlEncodedName}}`
- Verb: `GET`
- Content-Type: `application/json`
- Headers:
    - IsOn : none
    - IsInList: 
        - key: add a header per list name specified in your feature. The default is `feats.list`
        - value: your value
    - IsBefore: 
        - key: `feats.before`
        - value: your date, iso format
    - IsAfter: 
        - key: `feats.after`
        - value: your date, iso format
    - IsGreaterThan: 
        - key: `feats.greater`
        - value: your number (int, negative & floats accepted)
    - IsLowerThan: 
        - key: `feats.lower`
        - value: your number (int, negative & floats accepted)
- Response: 
```json
true | false
```

If your feature has a multitude of strategies : add all the headers you need.


# Running locally

## Building docker images

Since we have 2 docker files, there's alittle trik to build them:

```
docker build -t "feats.evaluations:1.0.0-SNAPSHOT" -f Dockerfile.evaluations .
```

```
docker build -t "feats.management:1.0.0-SNAPSHOT" -f Dockerfile.management .
```

## Running containers

- Navigate to the `deploy/local` folder 
- in a terminal, run `docker-compose up`
