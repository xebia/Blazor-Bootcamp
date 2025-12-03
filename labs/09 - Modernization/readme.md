# Modernization Lab

This lab is intended to bring together the concepts learned in previous labs and apply them to modernize a legacy application. You will work through a series of steps to refactor, containerize, and deploy an application using contemporary tools and practices.

At a high level, you will start with a working WFP application and migrate (rewrite) it to a Blazor web application.

The existing WPF application uses a LocalDB database to store data, and the code uses ADO.NET for data access.

The WPF app doesn't use dependency injection or any modern design patterns. It is very similar to a lot of WPF apps written in the 2005-2010 timeframe.
