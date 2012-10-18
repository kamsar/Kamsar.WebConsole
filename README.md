# Kamsar.WebConsole

This library is designed to provide a framework to execute long-running web requests in, such as search index rebuilds, database backups, etc.

There are three primary components:

* A progress bar that displays task progress
* A "progress status" text that displays the current state of the progress bar in text
* A virtual console window where you can write detailed information about the task. The console auto-scrolls.

## Quick Installation/Usage

* Build Kamsar.WebConsole or add the project to your solution
* Create a new Web Form and change its codebehind to inherit from WebConsolePage
* Implement required abstract methods
* During execution of your Process() method, utilize the status updating methods (SetProgress, SetProgressStatus, and WriteConsole/WriteConsoleLine) to follow what you're doing.
* The page will update status in real-time as your processing runs
* Note: any controls, markup, or content in the .aspx page will be ignored; the console takes over the rendering process. Only the @Page line is needed.

See also the Kamsar.WebConsole.Samples project for an example of implementation.

## Licensing

This library is licensed under the MIT license. Go nuts.