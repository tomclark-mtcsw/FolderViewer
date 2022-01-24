# FolderViewer

Author:  Tom Clark
Date:    January 30, 2020

I wrote this class for a web application to allow the display of files and folders much like what you would see in Windows File Explorer.  This repository contains a basic .NET6 MVC web application to allow it to be tested, but FolderViewer.cs is the actual class file which you will find in the models folder.  You will need to provide two directory values (see appsettings.json): a test path and a root path.  The root is the path that will not be disclosed to the outside world.  The test path is the relative path to the directory you want to view.  So if your test directory was C:\temp\something\right-here, your root might be "C:\temp\something" and your test path "right-here".  When I was actually using this, there was a base directory which housed all the documents for the web application.  That was my root and was never exposed.

Class Description:
This class takes a path as an input and displays the directory structure of where the path points.  Each directory can be expanded. Once expanded it appears in the expandedList string that gets passed in via the client as a pipe-delimited list.  The paths for all items are from the DocumentRootPath and down from there. The full path is never used for security purposes.  Each file can be downloaded or other action taken once customized.

The file structure is only filled as the user requests it.  So if a given directory has dozens of nested directories, they are only exposed and retrieved as the user requests.  This keeps the performance reasonable.  The structure is stored in a recursive class.
