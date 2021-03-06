# Albelli.Templates.Amazon

<a href="https://ci.appveyor.com/project/albumprinter/albelli-templates-amazon/branch/master"><img src="https://ci.appveyor.com/api/projects/status/bunen2a3k2rlt7dp?svg=true" /></a>
<a href="https://www.nuget.org/packages/Albelli.Templates.Amazon/"><img src="https://img.shields.io/nuget/vpre/Albelli.Templates.Amazon.svg" /></a>

## Overview

This solution helps to handle SNS/SQS messages (and any generic input) through the ASP.NET Core pipeline. It allows using the Startup file, Middlewares, and other features that you have in the real WebAPI application. This solution also provides several dotnet templates with some useful boilerplate code.

## Core

The templates are based on core libraries. The main mechanics and the explanation of how it works can be found in [the article](https://github.com/albumprinter/Albelli.Templates.Amazon/wiki/How-it-works).

## How to install templates

Type the following command to install the templates on your machine:
```
dotnet new -u Albelli.Templates.Amazon && dotnet new -i Albelli.Templates.Amazon
```

If you type `dotnet new` without any additional params again, you'll see the following new templates available:
```
albelli-amazon-sns
albelli-amazon-sqs
```

Then, for example, you can just simply execute `dotnet new albelli-amazon-sns` command to create a solution to handle SNS messages.
