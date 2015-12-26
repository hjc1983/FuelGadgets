
## About
  This [project](http://fgapp.azurewebsites.net) implements Azure Search and Azure DocumentDB which enable both consumers and operators to keep track of fuel prices in Australia.

## Technology stack of this site
  I make use of Azure Search indexer for DocumentDB - NoSQL, JSON database on storing all store locations and fuel prices. 
	- Azure DocumentDB
	- Azure Search - https://rcapp.search.windows.net
	- Azure Web app - http://fgapp.azurewebsites.net 
	- ASP.NET MVC 4 
	- Google Maps / JQuery
	- OAuth 2 - Social media login

## API
Here are two important methods that I call to fetch station results in JSON format.
 - Keyword Search
	[http://fgapp.azurewebsites.net/site/search/<keyword>](http://fgapp.azurewebsites.net/site/search/wat)

 - Location Search
	[http://fgapp.azurewebsites.net/site/search/location/<lat>/<lng>/](http://fgapp.azurewebsites.net/site/search/location/-33.890/151.274/)

## Build 
	Fork this repository is easy. however, you would need to have an Azure account and configure Azure Search and Azure DocumentDB. 
	Feel free to contact me if you would like to contribute or like to have more information on setting up a replica.
	Remember to update web.config to point Azure services once configured.
