Apple Store Apps Crawler
======================

Simple scalable crawler for Apps data from the iTunes Apple Store.

All the data you can see about a certain app once you open its page on the browser, is the data available from this
project (minus the description and text reviews).

You don't have to input any of your Apple Account credentials since this Crawler acts like a "Logged Out" user.

# Setting up your environment
If you want to host your own database, SQS queues and virtual machines, you can. All you have to do
is change the config files and the "Consts" class on the SharedLibrary to the values of your own preference (QueueNames, MongoDB Credentials/Address) 
and Amazon Web Services Keys (that you will need in order to access your queues from code).

For more detailed information, please, refer to this project's Wiki (W.I.P)

# Exporting the Database
As people kept requesting me, i decided to export the database on it's current state.
Email me if you need it.

Have in mind that downloading the database costs me money, since i pay for the outbound traffic provided by AWS when one queries the database
So, consider making a donation (via paypal) to marcello.grechi@gmail.com (the value is up to you).

If you need any specific extraction, let me know so we can figure out whats the best way to do it.

# About me
My name is Marcello Lins, i am a 23 y/o developer from Brazil who works with BigData and DataMining techniques at the moment.

http://about.me/marcellolins

# What is this project about ? 

The main idea of this project is to gather/mine data about apps of the Google Play Store and build a rich database so that developers, android fans and anyone else can use to generate statistics about the current play store situation

There are many questions we have no answer at the moment and we should be able to answer than with this database.

# What do i need before i start?

* I highly recommend you read all the pages of this wiki, which won`t take long.

* Know C#

# How about the database?

* I have made my MongoDB database public, including a user with read/write permissions so we can all use and populate the same database.

* If you feel like, you can make your own MongoDB Database and change the code Consts to point the output to your own MongoDB Database. No Biggie

**Refer to the Pages section of this wiki for individual information about each aspect of the project.**
