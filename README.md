# BatchStarter
Runs command and arranges windows properly (Windows only).

# How to
To compile this you need .Net 4.7. Precompiled binaries are available [here](https://github.com/b0wter/BatchStarter/releases).
You need to specify which commands to run in a json file. Here is an example:
```
[
  {
    "command": "notepad.exe",
    "arguments": "",
    "posX": 1,
    "posY": 1,
    "width": 400,
    "height": 300
  },
  {
    "command": "notepad.exe",
    "arguments": "",
    "posX": 401,
    "posY": 1,
    "width": 400,
    "height": 300
  }
]
```
This will open notepad two times and place the windows (next to each other) in the top left corner of the screen. The only field required is _command_.

You need to supply the file to read from as a command line argument.
```
> BatchStarter.exe myConfig.json
```
You can supply as many files as you want.

In case a file cannot be read or a process cannot be started error output will be written to std out. A failing process does not cause a premature exit, all remaining processes will be started.
