## Coding Standards

This section, once complete, will detail the coding standards used to produce all code, tests and stable releases aswell as the release process itself. It is under construction...


### Coding Style

* Clean code practice = easy to maintain = less bugs = robust.
* Long descriptive names, some common abbreviations okay.
* Should "read like a book". The compiler works for us optimizing the names into registers and memory locations. Never single letters (e.g. i,x,y,z,a,b,c) or self-invented abbreviations variables. That is bad practice because it takes longer to read and understand than meaningful words, leading to misunderstanding or delays and stress working with the code. Complete abbreviations are an exception, e.g. "hex" is okay instead of hexadecimal, but not "ins" instead of "insert". So no need for a crossword puzzles, then you're confusing others and maybe yourself a while later.
* Whitespace, brackets and indentation is good! Your brain sees patterns and is slower looking through a mash of text.
* Naming standards for object models/frameworks follow the Microsoft naming standards. Pascal case for visible (public or protected) objects, types and fields with additional restrictions of 2 letter acronyms being uppercase (e.g. IO = IO but SSD = Ssd), compound words cased (Toolbar = ToolBar, Filename = FileName) and abbreviations not confused with acronyms (e.g. ID = Id as short for identifier).
* Naming standards for private and local fields follow the coding standards of the language, e.g. Microsoft's C# naming standard specifies private fields in "camel-case" (i.e. lower first work then Pascal rest, e.g. camelsHump) and prefixed with an underscore, e.g. "_myFieldName". Language specific standards are not used for public types and members which make-up part of a cross-platform or cross-language object model/framework.



### Compiler

* Compiler warning level = max.
* Warning = error in compiler settings.
* Full code analysis profile with warnings = error.
* No blanked ignorance of code analysis warnings.
* Only known errors are caught. Errors are never ignored. Catching all exceptions is not allowed. All other errors must be thrown / unhandled.
* Exceptions should never occur during normal operation, that is very bad for performance (e.g. stack walk, JIT debugger evaluation).


### Multi-Threading

* Thread safety is only implemented at the highest level. No locking all over the place = bad performance!


### Devices

* Devices are low-level so not thread-safe (they must be fast). For this reason they must always be contained within an "owning" class before being exposed to end usage.
* Devices do not fire events or rely on a event system. It is not possible to predict the sequence of events, but devices commonly have a fixed sequence of operation. Stale data could be reset or stack overflows occur if the device were to fire events.


### Release Process

* Code analysis suppressions must be removed and re-evaluated before each public release.
* Never release without unit test success and integration test plan pass in a controlled test environment.
* All supported devices must be tested before release. If you don't have all the devices, you can't release.

