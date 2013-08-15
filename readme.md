##Project discontinued because since 2013 I have discovered Node.js and decided, that I will develop this project on it. I believe Node.js is much better suited for development of single page web aps than .Net.

Single page web application powered by Javascript(KnockoutJS, Twitter bootstrap), .NET(Fleck and RavenDB) on server.
Server-client communication is done only via webscokets.
Contains 3 VS2012 projects - Scraper, Websocket server and javascript fat client.
Usecase should be like this: 
Server scrapes a document(new law, law reform, international treaty or other parliamentary document which is in hands of every parliamentary member) and then it presents the voting on this document to the user of the web application. He then can vote on it. Voting should end before the last voting on it occurs in the parliament. That's basically it for now. For the futurre, I would like to implement parliamentary parties and a lot of social aspects.

Scraper is hand tailored for the purpose of getting vital information from the domain psp.cz. Scraping is done simply by calling constructor with the URL as the first parameter.

Clientside language is Czech, I don't plan on supporting any other languages just yet. All code and comments are written in english.

There are a few client side mocks for this app available for viewing here: http://plnkr.co/users/capaj
©GNU General Public License
