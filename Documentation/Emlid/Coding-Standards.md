## Coding Standards

This section, once complete, will detail the coding standards used to produce all code, tests and stable releases aswell as the release process itself. It is under construction...


### Coding Style

* Clean code practice = easy to maintain = less bugs = robust.
* Long descriptive names, some common abbreviations okay.
* Should "read like a book". The compiler works for us with optimizations. It's not a crossword puzzle! Never i,x,y,z,a,b,c variables. That is dumb practice, not smart because you may be good at puzzles! Less stress with clean logical code.
* Whitespace, brackets and indentation is good! Your brain sees patterns and is slower looking through a mash of text.


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

