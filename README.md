# feats

major WIP :) for fun

## wishfull thinking
managment sends events on streams in the event store, it also listens to stream to build the projections in postgresql

evaluations uses the postgresql projections as read-only and holds the basic evaluation strategies.... oh pimp points to add emits backs events of usage (nbr invoked + ratio on / off)


## cheat sheet for mimi

postgres `docker run it `
eventstore `docker run --name esdb-node -it -p 2113:2113 -p 1113:1113 eventstore/eventstore:20.6.1 --insecure --run-projections=All`

or

docker compose stuff eheh
in the deploy folder:

`docker-compose up`