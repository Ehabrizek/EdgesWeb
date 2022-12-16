# EdgesWeb
Production ready website found at https://combinationedges.com

# Prerequisites
Install npm <br>
https://nodejs.org/en/download/

Install Angular CLI <br>
	npm install -g @angular/cli

Install .NET 6 SDK <br>
https://dotnet.microsoft.com/en-us/download/dotnet/6.0

If hosting from IIS, install the hosting bundle as well!

# How to run
## Front end
Start from EdgesWeb directory <br>
1. Run command: <br>
		npm start <br>
2. Then go to https://localhost:4200

## Back end
Start from EdgesWebAPI directory <br>
1. Run command: <br>
		dotnet run

Note the front end is configured to use the production backend. <br>
If using your own local backend, update the app.component.ts to post to /Edges from proxy.conf.js