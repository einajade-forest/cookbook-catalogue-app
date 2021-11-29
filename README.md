# My Cookbook Catalogue App
A Windows Presentation Foundation application to locate recipes within physical cookbooks.

_**Note.** This is a reference application - recipes are not digitalised for use._

## Purpose of this project
This is a personal project born from the desire to practice coding and the realisation that my physically cookbooks were frequently overlooked in favour of searching for recipes on the web. Use by a wider audience was not the original intention.

## How does it work?
#### Catalogue cookbooks and recipes
1. Add cookbooks to database
2. Include a list of recipes and the corresponding page references for the recipes
3. Create and apply tags to recipes to assist in refining future searches

#### Search for recipe:
1. Search for desired recipe by **keyword** or **tag**
2. Make note of the recipe name, page reference, cookbook title and shelf location
3. Locate physical cookbook on shelf and turn the page to the desired recipe

## Included files
This repository contains:
- XAML & C# files
- Transact-SQL script file to create the core database incl stored procedures and test data

The above should be sufficient to recreate the application by using the Microsoft Visual Studio WPF App (.NET Framework) project template and Microsoft SQL Server.

## Notable package resources used
- [CsvHelper](https://joshclose.github.io/CsvHelper/) from _Josh Close_ - a .NET library for reading and writing CSV files

## Licence
[MIT License](https://github.com/einajade-forest/cookbook-indexes-app/blob/4b745f7912d06926ab724c7e9ac3ba69542fb7e4/LICENSE)
