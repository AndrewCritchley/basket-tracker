# Basket Tracker

## To do

- Missing async/await
- Needs actual logging
- Codebase is generally a mess

## Known Problems & Things to Work On

- I have no idea how to handle dependencies in the event handlers that are not already included in the consumer (Such as EF - specifically nuget dependencies. for now this reference has just been added to the consumer but this isn't ideal)
- Move the event processor out to it's own solution (separate from the services) to enforce the separation of the service and event processor

## Notes
- I had to add <CopyLocalLockFileAssemblies>true</CopyLocalLockFileAssemblies> to the EventHandlers csproj 