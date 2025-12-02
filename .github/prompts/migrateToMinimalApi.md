---
agent: agent
---
migrate controller component to minimal api. Check @AuthEndpoints.cs to see how its define in this project. Use .NET 10 minimal api patterns. migrate all permission check and authorization. Register endpoints in Module register class. After migration remove old controller class file