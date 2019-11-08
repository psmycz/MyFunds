# MyFunds

App for keeping records of funds.

For university purposes.

# How to make it work:
Need both projects to run simultaneously and it should be self-hosted, not on IIS Express
- MyFunds.IdentityServer - for authorization,
- MyFunds - for api

Identity Server runs on localhost on port 5005:
- http://localhost:5005

Api project run on localhost on port 5001:
- https://localhost:5001

Projects are set up for using those addresses - to get access api send request for token to identity server (is4)

There is example project how to connect and call api on branch "dotnet_client_example"

# doc:

- http://docs.identityserver.io/en/aspnetcore2/

- http://docs.identityserver.io/en/aspnetcore2/quickstarts/2_resource_owner_passwords.html

# btw 

There is swagger running on https://localhost:5001/swagger/index.html in api project :)
